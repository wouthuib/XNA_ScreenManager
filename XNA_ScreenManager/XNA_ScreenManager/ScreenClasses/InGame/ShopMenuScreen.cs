using System;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using XNA_ScreenManager.ItemClasses;
using XNA_ScreenManager.ScreenClasses.SubComponents;
using XNA_ScreenManager.PlayerClasses;

namespace XNA_ScreenManager.ScreenClasses.InGame
{
    public class ShopMenuScreen : GameScreen
    {
        #region properties
        ItemlistComponent itemlist;
        MenuComponent options;
        Inventory inventory = Inventory.Instance;
        Equipment equipment = Equipment.Instance;
        ScreenManager manager = ScreenManager.Instance;

        List<Item> shopobjects = new List<Item>();

        SpriteFont spriteFont;
        SpriteBatch spriteBatch;
        GraphicsDevice graphics;

        Color normalColor = Color.Yellow;
        Color hiliteColor = Color.Red;

        KeyboardState oldState;

        private string[] menuOptions = new string[] { "Buy", "Sell", "Equip", "Cancel" };

        private string[] shopItems = new string[] { "1200", "1201", "1202", "1203" };

        int width, height;
        int selectedMenuOption = 0;
        private bool itemSelection = false , itemOptions = false;

        #endregion

        public ShopMenuScreen(Game game, SpriteFont spriteFont, Texture2D background)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            graphics = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));
            this.spriteFont = spriteFont;
            itemlist = new ItemlistComponent(game, spriteFont);
            options = new MenuComponent(game, spriteFont);
            updateItemList();

            Components.Add(new BackgroundComponent(game, background));
        }

        #region item list

        public int SelectedItem
        {
            get { return itemlist.SelectedIndex; }
        }

        public void updateItemList()
        {
            /* In here we pick the categorized item list
             * The itemcomponent class will remove the duplicates
             * The selected index in the itemcomponent class
             * will only display the unique items and counts
             */
            itemlist.SetMenuItems(filterItemList());
        }

        // Fetch Invetory and place this in Menu Item list
        public List<Item> filterItemList()
        {
            // display all
            switch (SelectedMenuOption)
            {
                case 0: // buy
                    itemlist.Price = ShopPrice.Buy;
                    return shopobjects;
                case 1: // sell
                    itemlist.Price = ShopPrice.Sell;
                    return inventory.item_list;
                case 2: // equip
                    itemlist.Price = ShopPrice.None;
                    return inventory.item_list;
                default: // cancel
                    itemlist.Price = ShopPrice.None;
                    return inventory.item_list;
            }
        }

        #endregion

        #region optionmenu update methods
                
        public int SelectedMenuOption
        {
            get { return selectedMenuOption; }
            set
            {
                selectedMenuOption = (int)MathHelper.Clamp(
                value,
                0,
                menuOptions.Length - 1);
            }
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

        public override void Update(GameTime gameTime)
        {
            //record new keyboard state
            KeyboardState newState = Keyboard.GetState();

            if (!itemSelection)
            {
                if (CheckKey(Keys.Down))
                {
                    selectedMenuOption++;

                    if (selectedMenuOption == menuOptions.Length)
                        selectedMenuOption = 0;
                }
                else if (CheckKey(Keys.Up))
                {
                    selectedMenuOption--;

                    if (selectedMenuOption == -1)
                        selectedMenuOption = menuOptions.Length - 1;
                }
                else if (CheckKey(Keys.Enter))
                {
                    itemSelection = true;
                }
            }
            else
            {
                if (!itemOptions)
                {
                    // update item components
                    itemlist.Update(gameTime);
                    base.Update(gameTime);

                    if (CheckKey(Keys.Enter))
                    {
                        itemOptions = true;
                        options.SelectedIndex = 0;
                        options.SetMenuItems(new string[]{"Confirm", "Cancel"});
                    }
                }
                else
                {
                    // Check item options
                    if (CheckKey(Keys.Enter))
                    {
                        switch (options.SelectedIndex)
                        {
                            case 0:
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                itemOptions = false;
                                break;
                        }
                    }
                    else if (CheckKey(Keys.Escape))
                    {
                        itemOptions = false;
                    }

                    // update item components
                    options.Update(gameTime);
                }
            }

            // update itemlist
            updateItemList();

            // save keyboard state
            oldState = newState;
        }

        #endregion

        #region draw methods
        public override void Show()
        {
            itemlist.Position = new Vector2(100, 280);
            base.Show();
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw the menu items second
            Vector2 position = new Vector2();
            Color myColor;

            #region item list
            // Draw the base first
            itemlist.Position = new Vector2(100, 280);
            if (!itemSelection)
            {
                itemlist.NormalColor = Color.DarkGray;
                itemlist.HiliteColor = Color.DarkGray;
                itemlist.Transperancy = 0.60f;
            }
            else
            {
                itemlist.NormalColor = Color.Yellow;
                itemlist.HiliteColor = Color.Red;
                itemlist.Transperancy = 1f;
            }

            base.Draw(gameTime);
            itemlist.MaxDisplay = 6;
            itemlist.Draw(gameTime); // draw items

            // make sure items are available
            if (itemlist.menuItemsnoDupes.Count == 0)
                spriteBatch.DrawString(spriteFont, "None", new Vector2(itemlist.Position.X - 40, itemlist.Position.Y), Color.DarkGray);
            #endregion

            #region menu options
            // Draw Menu Option Types
            position = new Vector2(100, 130);

            for (int i = 0; i < menuOptions.Length; i++)
            {
                if (!itemSelection)
                {
                    if(i == SelectedMenuOption)
                        myColor = HiliteColor;
                    else
                        myColor = NormalColor;
                }
                else
                    myColor = Color.DarkGray;

                position.X = 100 - menuOptions[i].Length * 3;

                spriteBatch.DrawString(spriteFont,
                menuOptions[i],
                position,
                myColor);

                if (i < menuOptions.Length - 1)
                    position.Y += spriteFont.LineSpacing + 5;
            }
            #endregion

            #region itemoption popup
            // item options
            if (itemOptions)
            {
                Texture2D rect = new Texture2D(graphics, 100, options.MenuItems.Count * 20);

                Color[] data = new Color[100 * options.MenuItems.Count * 20];
                for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
                rect.SetData(data);

                spriteBatch.Draw(rect, new Vector2(itemlist.selectPos.X + 150, itemlist.selectPos.Y - (options.MenuItems.Count * 20)),
                    Color.White * 0.8f);

                Vector2 optionPos = new Vector2();
                optionPos.X = itemlist.selectPos.X + 150;
                optionPos.Y = itemlist.selectPos.Y - (options.MenuItems.Count * 20);

                options.Position = optionPos;
                options.Draw(gameTime);
            }
            #endregion

            #region other shop info
            // item description
            if (itemSelection)
            {
                if (SelectedMenuOption != 0) // not shop items
                {
                    if (itemlist.menuItemsnoDupes.Count > 0)
                        spriteBatch.DrawString(spriteFont, itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemDescription, new Vector2(80, 60), normalColor);
                }
                else
                {
                    if (shopobjects.Count > 0)
                        spriteBatch.DrawString(spriteFont, shopobjects[SelectedItem].itemDescription, new Vector2(80, 60), normalColor);
                }
            }

            // player gold
            spriteBatch.DrawString(spriteFont, "Gold: " + PlayerClasses.PlayerInfo.Instance.Gold.ToString() + " $",
                new Vector2(650 - (PlayerClasses.PlayerInfo.Instance.Gold.ToString().Length * 5), 280), Color.DarkGray);
            #endregion

        }
        #endregion

        #region shop functions
        #endregion
    }
}
