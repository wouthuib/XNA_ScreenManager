using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.ScreenClasses.SubComponents;
using Microsoft.Xna.Framework.Content;
using XNA_ScreenManager.PlayerClasses;
using Microsoft.Xna.Framework.Input;
using System.Reflection;
using System.Collections.Specialized;
using XNA_ScreenManager.ItemClasses;

namespace XNA_ScreenManager.ScreenClasses.Menus
{
    public enum Phase
    {
        Name,
        Properties,
        Continue
    }

    public class CharacterCreationScreen : GameScreen
    {
        SpriteBatch spriteBatch;
        ContentManager Content;

        public KeyboardInput keyboardiput;
        BackgroundComponent[] bgcomp = new BackgroundComponent[8];
        SpriteFont spriteFont, smallFont;
        PlayerSprite playersprite;
        //PlayerStore playerInfo = PlayerStore.Instance;
        public PlayerInfo newPlayer = null;
        public Phase phase = new Phase();

        Texture2D nameboard, propertyboard;
        KeyboardState newState, oldState;

        public MenuComponent[] properties = new MenuComponent[6];

        string[] menuItems = {
            "Face", 
            "Hair Style", 
            "Hair Color",
            "Skin Color",
            "Job Class",
            "Gender"};

        int selectedIndex = 0;

        string[] item0 = { "Face 01", "Face 02", "Face 03", "Face 04", "Face 05", "Face 06" };
        string[] item1 = { "Style 01", "Style 02", "Style 03", "Style 04", "Style 05", "Style 06" };
        string[] item2 = { "Red", "Green", "Blue", "Yellow", 
                           "Orange", "Purple", "Pink", "Brown", "Black", "Gray", "White" };
        string[] item3 = { "Pale", "Light", "Tanned", "Dark", "Blue", "Green" };
        string[] item4 = { "Warrior", "Magician", "Bowman", "Thief", "Pirate" };
        string[] item5 = { "Male", "Female"};

        public CharacterCreationScreen(Game game)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));

            smallFont = Content.Load<SpriteFont>(@"font\Arial_10px");
            spriteFont = Content.Load<SpriteFont>(@"font\Arial_12px");

            keyboardiput = new KeyboardInput(game, spriteFont, new Vector2(580, 175));
            bgcomp[1] = new BackgroundComponent(game, Content.Load<Texture2D>(@"gfx\background\background2"));
            bgcomp[2] = new BackgroundComponent(game, Content.Load<Texture2D>(@"gfx\background\character_create"));
            bgcomp[3] = new BackgroundComponent(game, Content.Load<Texture2D>(@"gfx\background\frame2"));

            Components.Add(bgcomp[1]);
            Components.Add(bgcomp[2]);
            Components.Add(bgcomp[3]);

            for (int i = 0; i <= properties.Length -1; i++)
            {
                properties[i] = new MenuComponent(game, Content.Load<SpriteFont>(@"font\Arial_12px"));
                properties[i].StartIndex = 0;
                properties[i].Position = new Vector2(180, 140 + (i * 24));
                properties[i].HiliteColor = Color.Black;
                properties[i].Shade = false;
                properties[i].DisplaySingle = true;
                properties[i].ListDown = false;
            }

            properties[0].SetMenuItems(item0);
            properties[1].SetMenuItems(item1);
            properties[2].SetMenuItems(item2);
            properties[3].SetMenuItems(item3);
            properties[4].SetMenuItems(item4);
            properties[5].SetMenuItems(item5);

            nameboard = Content.Load<Texture2D>(@"gfx\screens\screenobjects\character_nameboard");
            propertyboard = Content.Load<Texture2D>(@"gfx\screens\screenobjects\character_properties");

            // Create player info instance
            // newPlayer = new PlayerInfo();

            // Activate Text Input Class (for PlayerName)
            // keyboardiput.Activate(newPlayer.Name.ToString());

            // Set Phase to Name
            phase = Phase.Name;

            // player sprite
            playersprite = new PlayerSprite(352, 188, new Vector2(32, 32));

        }

        public override void Update(GameTime gameTime)
        {
            newState = Keyboard.GetState();

            switch (phase)
            {
                case Phase.Name:

                    keyboardiput.Update(gameTime);

                    // Equip Beginner Clothes + Knife
                    if (Equipment.Instance.item_list.FindAll(delegate(Item item)
                    { return item.Slot == ItemSlot.Bodygear; }).Count == 0)
                        Equipment.Instance.addItem(ItemStore.Instance.getItem(2301));
                    if (Equipment.Instance.item_list.FindAll(delegate(Item item)
                    { return item.Slot == ItemSlot.Weapon; }).Count == 0)
                        Equipment.Instance.addItem(ItemStore.Instance.getItem(1200));

                    break;
                case Phase.Properties:
                    for (int i = 0; i <= properties.Length - 1; i++)
                    {
                        float strlength = 0;

                        foreach (char ii in properties[i].MenuItems[properties[i].SelectedIndex])
                            strlength += properties[i].SpriteFont.MeasureString(ii.ToString()).X;

                        properties[i].Position =
                            new Vector2(210 - strlength / 2, 
                            140 + (i * 24));

                        properties[i].HiliteColor = Color.Black;
                        properties[selectedIndex].Shade = false;
                    }

                    if (CheckKey(Keys.Down))
                    {
                        selectedIndex++;
                        if (selectedIndex == properties.Length) //menuItems.Count)
                            selectedIndex = 0;
                    }

                    if (CheckKey(Keys.Up))
                    {
                        selectedIndex--;
                        if (selectedIndex == - 1)
                            selectedIndex = properties.Length - 1; //menuItems.Count - 1;
                    }

                    properties[selectedIndex].Update(gameTime);
                    properties[selectedIndex].HiliteColor = Color.Yellow;
                    properties[selectedIndex].Shade = true;

                    updatePlayerInfo();

                    break;
            }

            oldState = newState;
        }

        private void updatePlayerInfo()
        {
            this.newPlayer.faceset_sprite = @"gfx\player\faceset\faceset0" + properties[0].SelectedIndex.ToString();
            this.newPlayer.hair_sprite = @"gfx\player\hairset\hairset0" + properties[1].SelectedIndex.ToString();

            PropertyInfo propinfo = properties[2].GetType().GetProperty("MenuItems");
            StringCollection value = new StringCollection();

            // get hair color
            #region get hair color
            if (propinfo.GetIndexParameters().Length == 0)
                value = (StringCollection)propinfo.GetValue(properties[2], null);

            string colorname = value[properties[2].SelectedIndex].ToString();

            var colorprops = typeof(Color).GetProperty(colorname);
            this.newPlayer.hair_color = (Color)colorprops.GetValue(null, null);
            #endregion

            // get skin color
            #region get hair color
            if (propinfo.GetIndexParameters().Length == 0)
                value = (StringCollection)propinfo.GetValue(properties[3], null);

            colorname = value[properties[3].SelectedIndex].ToString();

            switch (colorname.ToString())
            {
                case "Pale":
                    newPlayer.skin_color = new Color(255, 229, 200);
                    break;
                case "Light":
                    newPlayer.skin_color = new Color(255, 206, 180);
                    break;
                case "Tanned":
                    newPlayer.skin_color = new Color(240, 184, 160);
                    break;
                case "Dark":
                    newPlayer.skin_color = new Color(180, 138, 120);
                    break;
                case "Blue":
                    newPlayer.skin_color = Color.LightSkyBlue;
                    break;
                case "Green":
                    newPlayer.skin_color = Color.LightGreen;
                    break;
                default:
                    // all other colors are OK
                    break;
            }
            #endregion
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // draw nameboard
            spriteBatch.Draw(nameboard, new Vector2(480, 75), Color.White);
            keyboardiput.Draw(gameTime);

            if (phase != Phase.Name)
            {
                // draw propertyboard
                spriteBatch.Draw(propertyboard, new Vector2(80, 100), Color.White);

                // Draw property options
                for (int i = 0; i <= properties.Length - 1; i++)
                    properties[i].Draw(gameTime);

                // Draw property names
                for (int i = 0; i <= menuItems.Length - 1; i++)
                {
                    spriteBatch.DrawString(smallFont, menuItems[i], new Vector2(100, 140 + (i * 24)) + Vector2.One, Color.Black);
                    spriteBatch.DrawString(smallFont, menuItems[i], new Vector2(100, 140 + (i * 24)), Color.Red);
                }
            }

            // Draw Player
            playersprite.Draw(spriteBatch, this.newPlayer);
        }

        private bool CheckKey(Keys theKey)
        {
            return oldState.IsKeyDown(theKey) && newState.IsKeyUp(theKey);
        }
    }
}
