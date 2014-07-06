using System;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using XNA_ScreenManager.ItemClasses;
using XNA_ScreenManager.ScreenClasses.SubComponents;
using XNA_ScreenManager.PlayerClasses;
using Microsoft.Xna.Framework.Content;
using System.Text.RegularExpressions;

namespace XNA_ScreenManager.ScreenClasses.InGame
{
    public class ShopMenuScreen : GameScreen
    {
        #region properties
        ItemlistComponent itemlist;
        MenuComponent options;
        ScreenManager manager = ScreenManager.Instance;
        PlayerStore PlayerStore = PlayerStore.Instance;

        List<Item> shopobjects = new List<Item>();

        SpriteFont spriteFont;
        SpriteBatch spriteBatch;
        GraphicsDevice graphics;
        ContentManager Content;

        Color normalColor = Color.Yellow;
        Color hiliteColor = Color.Red;

        KeyboardState oldState;

        private string[] menuOptions = new string[] { "Buy", "Sell", "Equip", "Cancel" };

        private string[] shopItems = new string[] { "1200", "1201", "1202", "1203", "1300", "2300" };

        int width, height;
        int selectedMenuOption = 0;
        int itembuysellcount = 1;

        private bool ScreenMenuOptions = true,
                     ScreenItemSelection = false,
                     ScreenItemOptions = false,
                     ScreenSellOptions = false;

        #endregion

        public ShopMenuScreen(Game game, SpriteFont spriteFont, Texture2D background)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            graphics = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));

            this.spriteFont = Content.Load<SpriteFont>(@"font\Comic_Sans_15px");

            itemlist = new ItemlistComponent(game);
            options = new ScreenPopup(game, Content.Load<SpriteFont>(@"font\Comic_Sans_18px"));

            updateItemList();
            SetShopItems(this.shopItems);

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
                    return PlayerStore.activePlayer.inventory.item_list;
                case 2: // equip
                    itemlist.Price = ShopPrice.None;
                    return PlayerStore.activePlayer.inventory.item_list;
                default: // cancel
                    itemlist.Price = ShopPrice.None;
                    return PlayerStore.activePlayer.inventory.item_list;
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

            if (ScreenMenuOptions) // --> Menu Option Selection
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
                else if (CheckKey(Keys.Enter) || CheckKey(Keys.Space))
                {
                    // Selected Menu Option Check
                    switch (selectedMenuOption)
                    {
                        case 0: // buy
                            if (shopobjects.Count > 0)
                            {
                                ScreenMenuOptions = false;
                                ScreenItemSelection = true;
                            }
                            break;
                        case 1: // sell
                            if (itemlist.menuItemsnoDupes.Count > 0)
                            {
                                ScreenMenuOptions = false;
                                ScreenItemSelection = true;
                            }
                            break;
                        case 2: // equip
                            break;
                        case 3: // cancel
                            ScreenClasses.ScreenManager.Instance.setScreen("actionScreen");
                            break;
                    }
                }
            }

            else if (ScreenItemSelection)
            {
                // Update item components
                itemlist.Update(gameTime);
                base.Update(gameTime);

                // Check Item Count, if empty return to Menu Selection
                if (selectedMenuOption == 0 && shopobjects.Count <= 0 ||
                    selectedMenuOption == 1 && itemlist.menuItemsnoDupes.Count <= 0)
                {
                    ScreenItemOptions = false;
                    ScreenItemSelection = false;
                    ScreenMenuOptions = true;
                }

                // Key Check 
                if (CheckKey(Keys.Enter) || CheckKey(Keys.Space))
                {
                    string question = null;

                    switch (selectedMenuOption)
                    {
                        case 0:
                            if (shopobjects.Count > 0)
                                question = "Buy " + itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemName + " for " +
                                itemlist.menuItemsnoDupes[itemlist.SelectedIndex].Price.ToString() + " $ ?";
                            break;
                        case 1:
                            if (itemlist.menuItemsnoDupes.Count > 0)
                                question = "Sell " + itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemName + " for " +
                                ((int)(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].Price / 2)).ToString() + " $ ?";
                            break;
                    }

                    if (selectedMenuOption <= 1 && question != null) // buy and sell
                    {
                        ScreenItemSelection = false;
                        ScreenItemOptions = true;

                        options.Style = OrderStyle.Central;
                        options.SetMenuItems(new string[] { question, "", "Confirm", "Cancel" });
                        options.StartIndex = 2;
                        options.SelectedIndex = 2;
                    }
                }
                else if (CheckKey(Keys.Escape) || CheckKey(Keys.Back))
                {
                    ScreenItemSelection = false;
                    ScreenMenuOptions = true;
                }
            }

            else if (ScreenItemOptions) // --> Item Option Popup
            {
                // Check item options
                if (CheckKey(Keys.Enter) || CheckKey(Keys.Space))
                {
                    if (options.MenuItems[options.SelectedIndex].ToString() == "Confirm")
                    {
                        // read the menu option: buy, sell, equip, cancel
                        switch (selectedMenuOption)
                        {
                            case 0:
                                BuyShopItems();
                                break;
                            case 1:
                                if (PlayerStore.activePlayer.inventory.item_list.
                                    FindAll(x => x.itemName == itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemName)
                                    .Count > 1)
                                {
                                    ScreenItemOptions = false;
                                    ScreenSellOptions = true;
                                    itembuysellcount = 1;

                                    options.Style = OrderStyle.Central;
                                    options.SetMenuItems(new string[] 
                                    { 
                                        "How many " + itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemName + "'s", 
                                        "do you want to sell?", 
                                        "",
                                        "<- " + itembuysellcount.ToString() + " ->",
                                        "Cancel" 
                                    });
                                    options.StartIndex = 3;
                                    options.SelectedIndex = 3;
                                }
                                else
                                {
                                    SellShopItems();
                                }
                                break;
                            case 2:
                                break;
                            default:
                                break;
                        }
                    }
                    else if (options.MenuItems[options.SelectedIndex].ToString() == "Cancel")
                    {
                        ScreenItemOptions = false;
                        ScreenItemSelection = true;
                    }
                }
                else if (CheckKey(Keys.Escape) || CheckKey(Keys.Back))
                {
                    ScreenItemOptions = false;
                    ScreenItemSelection = true;
                }
            }

            else if (ScreenSellOptions) // --> Item Option Popup
            {
                // Check item options
                if (CheckKey(Keys.Enter) || CheckKey(Keys.Space))
                {
                    if (options.SelectedIndex == 3) // itemcount
                    {
                        SellShopItems(itembuysellcount);
                        ScreenSellOptions = false;
                        ScreenItemSelection = true;
                    }
                    else if (options.MenuItems[options.SelectedIndex].ToString() == "Cancel")
                    {
                        ScreenSellOptions = false;
                        ScreenItemSelection = true;
                    }
                }
                else if (CheckKey(Keys.Right) && options.SelectedIndex == 3)
                {
                    itembuysellcount++;
                    if (itembuysellcount > PlayerStore.activePlayer.inventory.item_list.
                                    FindAll(x => x.itemName == itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemName)
                                    .Count)
                        itembuysellcount = 1;
                    options.MenuItems[3] = "<- " + itembuysellcount.ToString() + " ->";
                }
                else if (CheckKey(Keys.Left) && options.SelectedIndex == 3)
                {
                    itembuysellcount--;
                    if (itembuysellcount == 0)
                        itembuysellcount = PlayerStore.activePlayer.inventory.item_list.
                                    FindAll(x => x.itemName == itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemName)
                                    .Count;
                    options.MenuItems[3] = "<- " + itembuysellcount.ToString() + " ->";
                }
                else if (CheckKey(Keys.Escape) || CheckKey(Keys.Back))
                {
                    ScreenSellOptions = false;
                    ScreenItemSelection = true;
                }
            }

            // update item components
            options.Update(gameTime);

            // update itemlist
            updateItemList();

            // save keyboard state
            oldState = newState;
        }

        #endregion

        #region draw methods
        public override void Show()
        {
            itemlist.Position = new Vector2(100, 200);
            options.Position = new Vector2(300, 200);
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
            if (ScreenItemSelection)
            {
                itemlist.NormalColor = Color.Yellow;
                itemlist.HiliteColor = Color.Red;
                itemlist.Transperancy = 1f;
            }
            else
            {
                itemlist.NormalColor = Color.DarkGray;
                itemlist.HiliteColor = Color.DarkGray;
                itemlist.Transperancy = 0.60f;
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
                if (ScreenMenuOptions)
                {
                    if(i == SelectedMenuOption)
                        myColor = HiliteColor;
                    else
                        myColor = NormalColor;
                }
                else
                    myColor = Color.DarkGray;

                position.X = 100 - menuOptions[i].Length * 3;

                spriteBatch.DrawString(Content.Load<SpriteFont>(@"font\Comic_Sans_18px"),
                menuOptions[i],
                position,
                myColor);

                if (i < menuOptions.Length - 1)
                    position.Y += spriteFont.LineSpacing + 5;
            }
            #endregion

            #region itemoption popup
            // item options
            if (ScreenItemOptions || ScreenSellOptions)
            {
                //Texture2D rect = new Texture2D(graphics, (int)options.getBounds().X, options.MenuItems.Count * 20),
                //          rect2 = new Texture2D(graphics, (int)options.getBounds().X, options.MenuItems.Count * 20);
  
                //Color[] data = new Color[(int)options.getBounds().X * options.MenuItems.Count * 20];

                //// set colors for menu borders and fill
                //for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
                //rect.SetData(data);
                //for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
                //rect2.SetData(data);

                //// draw menu fill 10% transperancy
                //spriteBatch.Draw(rect, 
                //    new Rectangle((int)(itemlist.selectPos.X + 145), (int)(itemlist.selectPos.Y - (options.MenuItems.Count * 20) - 5), rect.Width + 10, rect.Height + 15),
                //    Color.White * 0.9f);                              

                //// draw borders
                //spriteBatch.Draw(rect2, 
                //    new Rectangle((int)itemlist.selectPos.X + 140, (int)itemlist.selectPos.Y - (options.MenuItems.Count * 20) - 10, (int)5, (int)options.MenuItems.Count * 20 + 15),
                //    new Rectangle(0, 0, 5, 5), Color.White);
                //spriteBatch.Draw(rect2,
                //    new Rectangle((int)itemlist.selectPos.X + 140, (int)itemlist.selectPos.Y - (options.MenuItems.Count * 20) - 10, (int)rect.Width + 15, 5),
                //    new Rectangle(0, 0, 5, 5), Color.White);
                //spriteBatch.Draw(rect2,
                //    new Rectangle((int)itemlist.selectPos.X + 140, (int)itemlist.selectPos.Y + 5, (int)rect.Width + 15, 5),
                //    new Rectangle(0, 0, 5, 5), Color.White);
                //spriteBatch.Draw(rect2,
                //    new Rectangle((int)(itemlist.selectPos.X + 155 + rect.Width), (int)itemlist.selectPos.Y - (options.MenuItems.Count * 20) - 10, 5, (int)options.MenuItems.Count * 20 + 20),
                //    new Rectangle(0, 0, 5, 5), Color.White);


                //Vector2 optionPos = new Vector2();
                //optionPos.X = itemlist.selectPos.X + 150;
                //optionPos.Y = itemlist.selectPos.Y - (options.MenuItems.Count * 20);

                //options.Position = optionPos;
                options.Draw(gameTime);
            }
            #endregion

            #region other shop info
            // item information
            if (ScreenItemSelection)
            {
                if (SelectedMenuOption != 0 && itemlist.menuItemsnoDupes.Count > 0) // other than shop items  
                {
                    // item description
                    spriteBatch.DrawString(spriteFont, itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemDescription, new Vector2(80, 60), normalColor);

                    // item stat info + equipment comparison
                    DrawInventoryinfo(gameTime);

                }
                else if (shopobjects.Count > 0) // shop items
                {
                    spriteBatch.DrawString(spriteFont, shopobjects[SelectedItem].itemDescription, new Vector2(80, 60), normalColor);

                    // item stat info + equipment comparison
                    DrawInventoryinfo(gameTime);
                }
            }

            // player gold
            spriteBatch.DrawString(spriteFont, "Gold: " + PlayerClasses.PlayerStore.Instance.activePlayer.Gold.ToString() + " $",
                new Vector2(650 - (PlayerClasses.PlayerStore.Instance.activePlayer.Gold.ToString().Length * 5), 280), Color.LightBlue);
            #endregion

        }

        private void DrawInventoryinfo(GameTime gameTime)
        {
            // item stat info + equipment comparison
            Color myColor = Color.LightBlue;
            Vector2 position = new Vector2(270, 130);
            int row = 1, space = 0;

            // Both shop and inventory items
            ItemSlot slot = ItemSlot.Weapon;
            Item shopItem = null, invenItem = null, getItem = null;
            PropertyInfo propertyEquipment;
            int equipval = 0, shopval = 0, invenval = 0;

            // Get Slot Type of selected item
            if (SelectedMenuOption == 0) // <- Buy
            {
                slot = shopobjects[SelectedItem].Slot;
                shopItem = shopobjects[SelectedItem];
                getItem = shopItem;
            }
            else if (SelectedMenuOption == 1) // <- Sell
            {
                slot = itemlist.menuItemsnoDupes[itemlist.SelectedIndex].Slot;
                invenItem = itemlist.menuItemsnoDupes[itemlist.SelectedIndex];
                getItem = invenItem;
            }

            // Get item details of equipment, shop and inventory lists based on selected index
            Item equipItem = PlayerStore.activePlayer.equipment.item_list.Find(delegate(Item item) { return item.Slot == slot; });
                        
            // nothing equiped on slot, use item 1000 which is a dummy for nothing
            if (equipItem == null)
                equipItem = ItemStore.Instance.getItem(1000);

            for (int i = 7; i < equipItem.GetType().GetProperties().Length; i++)
            {
                switch (i)
                {
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 12:
                    case 13:
                    case 15:
                    case 16:

                        // Comparison only for weapons and armor
                        if (getItem.Type == ItemType.Weapon || getItem.Type == ItemType.Armor)
                        {
                            // Get Property Information
                            propertyEquipment = getItem.GetType().GetProperties()[i];

                            // Draw the Property Names
                            spriteBatch.DrawString(spriteFont,
                            Regex.Replace(propertyEquipment.Name, "_", " "),
                            position,
                            Color.DarkGray);

                            // Draw shop buy item comparison
                            if (i <= 10)
                            {
                                // Draw the equiped values
                                spriteBatch.DrawString(spriteFont,
                                Regex.Replace(propertyEquipment.GetValue(equipItem, null).ToString(), "_", " "),
                                new Vector2(position.X + 80 + space, position.Y),
                                Color.LightBlue);

                                // Get Property Information
                                propertyEquipment = equipItem.GetType().GetProperties()[i];

                                // Get Property values
                                equipval = (int)(propertyEquipment.GetValue(equipItem, null));

                                if (selectedMenuOption == 0)
                                    shopval = (int)(shopItem.GetType().GetProperties()[i].GetValue(shopItem, null));
                                else if (selectedMenuOption == 1)
                                    invenval = (int)(invenItem.GetType().GetProperties()[i].GetValue(invenItem, null));

                                if ((SelectedMenuOption == 0 && equipval == shopval) ||
                                    (SelectedMenuOption == 1 && equipval == invenval))
                                {
                                    myColor = Color.LightBlue;

                                    spriteBatch.DrawString(spriteFont,
                                        "=", new Vector2(position.X + 125, position.Y), myColor);
                                }
                                else if ((SelectedMenuOption == 0 && equipval > shopval) ||
                                         (SelectedMenuOption == 1 && equipval > invenval))
                                {
                                    myColor = Color.IndianRed;

                                    spriteBatch.DrawString(spriteFont,
                                        "<", new Vector2(position.X + 125, position.Y), myColor);
                                }
                                else if ((SelectedMenuOption == 0 && equipval < shopval) ||
                                         (SelectedMenuOption == 1 && equipval < invenval))
                                {
                                    myColor = Color.LightGreen;

                                    spriteBatch.DrawString(spriteFont,
                                        ">", new Vector2(position.X + 125, position.Y), myColor);
                                }

                                if (SelectedMenuOption == 0)
                                    spriteBatch.DrawString(spriteFont,
                                    shopval.ToString(),
                                    new Vector2(position.X + 155, position.Y),
                                    myColor);
                                else if (SelectedMenuOption == 1)
                                    spriteBatch.DrawString(spriteFont,
                                    invenval.ToString(),
                                    new Vector2(position.X + 155, position.Y),
                                    myColor);
                            }
                            else
                            {
                                myColor = Color.LightBlue;

                                if (propertyEquipment.Name.ToString() == "Class")
                                {
                                    if (getItem.Class.ToString() != PlayerStore.activePlayer.Jobclass && getItem.Class.ToString() != "All")
                                        myColor = Color.IndianRed;
                                    else
                                        myColor = Color.LightGreen;
                                }

                                // Draw the selected item values for type, slot etc
                                spriteBatch.DrawString(spriteFont,
                                Regex.Replace(propertyEquipment.GetValue(getItem, null).ToString(), "_", " "),
                                new Vector2(position.X + 80 + space, position.Y),
                                myColor);
                            }

                            row++;

                            if (row <= 4)
                                position.Y += spriteFont.LineSpacing + 5;
                            else
                            {
                                position.Y = 130;
                                position.X += 220;
                                row = 1;
                                space = 70;
                            }
                        }

                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region shop functions

        public void SetShopItems(string[] shopItems)
        {
            shopobjects.Clear();

            foreach (var item in shopItems)
            {
                // use try to skip invalid item ID errors
                try
                {
                    Item getitem = ItemStore.Instance.getItem(Convert.ToInt32(item));
                    shopobjects.Add(getitem);
                }
                catch (Exception ee)
                {
                    // do nothing
                    string aa = ee.ToString();
                }
            }
        }

        private void SellShopItems(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                int SellValue = (int)(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].Price / 2);

                PlayerStore.activePlayer.inventory.removeItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemID);

                // Update selected index
                if (itemlist.SelectedIndex > itemlist.menuItemsnoDupes.Count - 1)
                    itemlist.SelectedIndex = itemlist.menuItemsnoDupes.Count - 1;

                PlayerStore.Instance.activePlayer.Gold += SellValue;
            }
            updateItemList();               // update item menu
            ScreenItemOptions = false;      // close options
            ScreenItemSelection = true;     // back to item selection

            // when inventory is empty return to menu
            if (itemlist.menuItemsnoDupes.Count <= 0)
            {
                ScreenItemSelection = false;
                ScreenMenuOptions = true;
            }
        }

        private void BuyShopItems()
        {
            int BuyValue = (int)(shopobjects[SelectedItem].Price);

            // Check if PLayer posses enough Gold
            if (PlayerStore.Instance.activePlayer.Gold >= BuyValue)
            {

                PlayerStore.activePlayer.inventory.addItem(shopobjects[SelectedItem]);

                PlayerStore.activePlayer.Gold -= BuyValue;

                updateItemList();                   // update item menu
                ScreenItemOptions = false;          // close options
                ScreenItemSelection = true;        // return to menu
            }
        }
        #endregion
    }
}
