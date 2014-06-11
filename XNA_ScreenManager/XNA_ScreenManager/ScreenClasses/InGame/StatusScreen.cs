using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using XNA_ScreenManager.ScreenClasses.SubComponents;
using XNA_ScreenManager.ItemClasses;
using Microsoft.Xna.Framework.Input;
using System.Reflection;
using XNA_ScreenManager.PlayerClasses;

namespace XNA_ScreenManager.ScreenClasses.InGame
{
    public class StatusScreen : GameScreen
    {
        #region properties
        //Inventory inventory = Inventory.Instance;
        //Equipment equipment = Equipment.Instance;
        ScreenManager manager = ScreenManager.Instance;

        SpriteFont spriteFont;
        SpriteBatch spriteBatch;
        GraphicsDevice graphics;
        ContentManager Content;

        Color normalColor = Color.Yellow;
        Color hiliteColor = Color.Red;

        KeyboardState oldState;

        string[] menuOptions = { 
             "Change",
             "Cancel" };

        string[] menuItems;

        private bool OptionsActive = true;
        private int selectedOption = 0;
        private int width, height;

        #endregion

        public StatusScreen(Game game, Texture2D background)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            graphics = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));

            this.spriteFont = Content.Load<SpriteFont>(@"font\Comic_Sans_18px");

            Components.Add(new BackgroundComponent(game, background));
        }

        public Color NormalColor
        {
            get { return normalColor; }
            set { normalColor = value; }
        }

        public Color HiliteColor
        {
            get { return hiliteColor; }
            set { hiliteColor = value; }
        }

        private void CalculateBounds()
        {
            width = 0;
            height = 0;
            foreach (string slot in Enum.GetNames(typeof(ItemSlot)))
            {
                Vector2 size = spriteFont.MeasureString(slot);
                if (size.X > width)
                    width = (int)size.X;
                height += spriteFont.LineSpacing;
            }
        }

        public bool CheckKey(Keys theKey)
        {
            KeyboardState newState = Keyboard.GetState();
            return oldState.IsKeyDown(theKey) && newState.IsKeyUp(theKey);
        }

        public int SelectedOption
        {
            get { return selectedOption; }
            set
            {
                selectedOption = (int)MathHelper.Clamp(
                value,
                -1,
                menuOptions.Length);
            }
        }

        public string[] MenuItems
        {
            get { return menuItems; }
            set 
            {
                menuItems = value;
            }
        }

        public override void Update(GameTime gameTime)
        {
            //record new keyboard state
            KeyboardState newState = Keyboard.GetState();

            if (OptionsActive)
            {

                if (CheckKey(Keys.Right))
                {
                    SelectedOption++;

                    if (SelectedOption >= menuOptions.Length) // last slot is "None" we skip this one
                        SelectedOption = 0;
                }
                else if (CheckKey(Keys.Left))
                {
                    SelectedOption--;

                    if (SelectedOption <= -1)
                        SelectedOption = menuOptions.Length - 1; // last slot is "None" we skip this one
                }
            }
            else
            {
            }

            base.Update(gameTime);

            // save keyboard state
            oldState = newState;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Vector2 position = new Vector2();
            Color myColor;

            #region menu options
            // Draw Menu Option Types
            position = new Vector2(80, 60);

            for (int i = 0; i < menuOptions.Length; i++)
            {
                if (OptionsActive)
                {
                    if (i == SelectedOption)
                        myColor = HiliteColor;
                    else
                        myColor = NormalColor;
                }
                else
                    myColor = Color.DarkGray;

                spriteBatch.DrawString(spriteFont,
                menuOptions[i],
                position,
                myColor);

                if (i < menuOptions.Length - 1)
                    position.X += 50 + (menuOptions[i].Length * 6);
            }
            #endregion

            #region player statuspoints
            // Draw Player Skillpoints
            spriteBatch.DrawString(spriteFont,
                "Available Status points: ",
                new Vector2(490, 60),
                Color.Yellow);

            spriteBatch.DrawString(spriteFont,
                PlayerStore.Instance.activePlayer.Statpoints.ToString(),
                new Vector2(675, 60),
                Color.White);
            #endregion

            #region player Battle Info
            // Draw Player Name
            spriteBatch.DrawString(spriteFont, "Battle information", new Vector2(500, 150), NormalColor);

            // Draw Player Battle Info Values
            position = new Vector2(500, 190);

            for (int i = 0; i < Enum.GetNames(typeof(PlayerBattleInfo)).Length; i++)
            {
                // Draw Player Stat Name
                spriteBatch.DrawString(spriteFont,
                Enum.GetNames(typeof(PlayerBattleInfo))[i],
                position, Color.DarkGray);

                // Get Stat Value
                Object player = PlayerStore.Instance.activePlayer;
                PropertyInfo info = player.GetType().GetProperty(Enum.GetNames(typeof(PlayerBattleInfo))[i]);

                // Draw Player Battle Values
                spriteBatch.DrawString(spriteFont,
                info.GetValue(player, null).ToString(),
                new Vector2(600, position.Y), Color.White);

                position.Y += spriteFont.LineSpacing;
            }
            #endregion

            #region player Stat Info

            // Draw Player Status Details Values
            spriteBatch.DrawString(spriteFont, "Status details", new Vector2(80, 150), NormalColor);

            position = new Vector2(80, 190);

            for (int i = 0; i < Enum.GetNames(typeof(PlayerStats)).Length; i++)
            {
                // Draw Player Stat Name
                spriteBatch.DrawString(spriteFont,
                Enum.GetNames(typeof(PlayerStats))[i],
                position, Color.DarkGray);

                // Get Stat Value
                Object player = PlayerStore.Instance.activePlayer;
                PropertyInfo info = player.GetType().GetProperty(Enum.GetNames(typeof(PlayerStats))[i]);

                int columnspacing = 200;

                if (spriteFont.MeasureString(Enum.GetNames(typeof(PlayerStats))[i]).X > 120)
                    columnspacing = (int)(spriteFont.MeasureString(PlayerStore.Instance.activePlayer.Name).X + 50);

                // Draw Player Stat Value
                spriteBatch.DrawString(spriteFont,
                info.GetValue(player, null).ToString(),
                new Vector2(columnspacing, position.Y), NormalColor);

                position.Y += spriteFont.LineSpacing;
            }
            #endregion
        }
    }
}
