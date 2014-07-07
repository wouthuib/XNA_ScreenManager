using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Specialized;

using XNA_ScreenManager.ItemClasses;
using System.Collections.Generic;
using XNA_ScreenManager.ScreenClasses.SubComponents;
using Microsoft.Xna.Framework.Content;
using XNA_ScreenManager.PlayerClasses;
using System.Text.RegularExpressions;
using XNA_ScreenManager.ScriptClasses;
using XNA_ScreenManager.CharacterClasses;
using System;

namespace XNA_ScreenManager.ScreenClasses
{
    public class ItemMenuScreen : GameScreen
    {
        #region properties
        ItemlistComponent itemlist;
        MenuComponent options;
        //Inventory inventory = Inventory.Instance;
        //Equipment equipment = Equipment.Instance;
        ScreenManager manager = ScreenManager.Instance;
        PlayerStore playerStore = PlayerStore.Instance;

        List<Item> itemobjects = new List<Item>();

        SpriteFont spriteFont;
        SpriteBatch spriteBatch;
        GraphicsDevice graphics;
        ContentManager Content;

        Color normalColor = Color.Yellow;
        Color hiliteColor = Color.Red;

        KeyboardState oldState;

        private StringCollection menuItems = new StringCollection();
        private StringCollection menuCategories = new StringCollection();

        string[] categories = {
                "| All |", 
                "| Collectable |", 
                "| Weapon |",
                "| Armor |",
                "| Accessory |",
                "| KeyItem |" };

        int width, height;
        int selectedCategory = 0, SetItemSlot = 1;
        bool itemOptions = false, itemSelection = true, itemQuickSlotOption = false;

        #endregion

        #region constructor
        public ItemMenuScreen(Game game, SpriteFont spriteFont, Texture2D background)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            graphics = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));

            this.spriteFont = spriteFont;
            itemlist = new ItemlistComponent(game);
            options = new MenuComponent(game, spriteFont);

            SetmenuCategories(categories);
            updateItemList();

            Components.Add(new BackgroundComponent(game, background));
            Components.Add(itemlist);
        }
        #endregion

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
            switch(selectedCategory)
            {
                case 0: //All
                    return playerStore.activePlayer.inventory.item_list;
                case 1: // Collectables
                    return playerStore.activePlayer.inventory.item_list.FindAll(delegate(Item item) { return item.Type == ItemType.Collectable; });
                case 2: // Weapons
                    return playerStore.activePlayer.inventory.item_list.FindAll(delegate(Item item) { return item.Type == ItemType.Weapon; });
                case 3: // Armor
                    return playerStore.activePlayer.inventory.item_list.FindAll(delegate(Item item) { return item.Type == ItemType.Armor; });
                case 4: // Accessory
                    return playerStore.activePlayer.inventory.item_list.FindAll(delegate(Item item) { return item.Type == ItemType.Accessory; });
                case 5: // KeyItem
                    return playerStore.activePlayer.inventory.item_list.FindAll(delegate(Item item) { return item.Type == ItemType.KeyItem; });
                default:
                    return playerStore.activePlayer.inventory.item_list;
            }
        }

        #endregion

        #region menu update categories + options

        // Menu category functions
        public int SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = (int)MathHelper.Clamp(
                value,
                0,
                menuCategories.Count - 1);
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

        public void SetmenuCategories(string[] cats)
        {
            menuCategories.Clear();
            menuCategories.AddRange(cats);
            CalculateBounds();
        }

        private void CalculateBounds()
        {
            width = 0;
            height = 0;
            foreach (string cats in menuCategories)
            {
                Vector2 size = spriteFont.MeasureString(cats);
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
            KeyboardState newState = Keyboard.GetState();

            if (itemSelection)
            {
                if (CheckKey(Keys.Right))
                {
                    selectedCategory++;
                    updateItemList();

                    if (selectedCategory == menuCategories.Count)
                        selectedCategory = 0;

                    if (itemlist.SelectedIndex > filterItemList().Count)
                        itemlist.SelectedIndex = filterItemList().Count;
                }

                if (CheckKey(Keys.Left))
                {
                    selectedCategory--;
                    updateItemList();

                    if (selectedCategory == -1)
                        selectedCategory = menuCategories.Count - 1;

                    if (itemlist.SelectedIndex > filterItemList().Count)
                        itemlist.SelectedIndex = filterItemList().Count;
                }

                if (CheckKey(Keys.Enter))
                {
                    if (filterItemList().Count > 0)
                    {
                        itemSelection = false;
                        itemOptions = true;
                        options.Style = OrderStyle.Central;
                        options.SetMenuItems(new string[]{ itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemName + " - Choose an Option.", "",
                            "Equip", "Use", "Add QuickSlot", "Remove", "Cancel"});
                        options.StartIndex = 2;
                        options.SelectedIndex = 2;
                    }
                }

                // update item components
                base.Update(gameTime);
            }
            else if (itemOptions)
            {
                // Check item options
                if (CheckKey(Keys.Enter))
                {
                    switch (options.SelectedIndex)
                    {
                        case 0:
                        case 1:
                            break;
                        case 2:
                            itemEquip();
                            break;
                        case 3:
                            itemConsume(playerStore.activePlayer);
                            break;
                        case 4:
                            itemQuickSlot();
                            break;
                        case 5:
                            itemRemove();
                            break;
                        case 6:
                            itemOptions = false;
                            itemSelection = true;
                            break;
                    }
                }
                else if (CheckKey(Keys.Escape))
                {
                    itemOptions = false;
                    itemSelection = true;
                }

                // update item components
                options.Update(gameTime);
            }
            else if (itemQuickSlotOption)
            {
                // Check item options
                if (CheckKey(Keys.Enter) || CheckKey(Keys.Space))
                {
                    if (options.SelectedIndex == 3) // itemcount
                    {
                        //SellShopItems(SetItemSlot); // <-- place item in skillbar
                        itemQuickSlotOption = false;
                        itemSelection = true;
                    }
                    else if (options.MenuItems[options.SelectedIndex].ToString() == "Cancel")
                    {
                        itemQuickSlotOption = false;
                        itemSelection = true;
                    }
                }
                else if (CheckKey(Keys.Right) && options.SelectedIndex == 3)
                {
                    SetItemSlot++;
                    if (SetItemSlot > playerStore.activePlayer.skillbar.skillslot.Length)
                        SetItemSlot = 1;
                    options.MenuItems[3] = "<- " + SetItemSlot.ToString() + " ->";
                }
                else if (CheckKey(Keys.Left) && options.SelectedIndex == 3)
                {
                    SetItemSlot--;
                    if (SetItemSlot == 0)
                        SetItemSlot = playerStore.activePlayer.skillbar.skillslot.Length;
                    options.MenuItems[3] = "<- " + SetItemSlot.ToString() + " ->";
                }
                else if (CheckKey(Keys.Escape) || CheckKey(Keys.Back))
                {
                    itemQuickSlotOption = false;
                    itemSelection = true;
                }
            }

            // save keyboard state
            oldState = newState;
        }

        #endregion

        #region draw methods
        // BasicEffect Class functions
        public override void Show()
        {
            itemlist.Position = new Vector2(150, 130);
            base.Show();
        }

        public override void Draw(GameTime gameTime)
        {
            if (itemOptions)
                itemlist.NormalColor = Color.DarkGray;
            else
                itemlist.NormalColor = Color.Yellow;

            // Draw the base first
            base.Draw(gameTime);

            // the menu items second
            Vector2 position = new Vector2();
            Color myColor;

            // Draw Categroies
            position = new Vector2(80, 70);

            for (int i = 0; i < menuCategories.Count; i++)
            {
                if (i == SelectedCategory)
                    myColor = HiliteColor;
                else
                    myColor = NormalColor;

                spriteBatch.DrawString(spriteFont,
                menuCategories[i],
                position,
                myColor);

                if (i < menuCategories.Count - 1)
                    position.X += 50 + (menuCategories[i].Length * 6);
            }

            if (itemlist.menuItemsnoDupes.Count > 0)
            {
                // item description
                spriteBatch.DrawString(spriteFont, itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemDescription, new Vector2(80, 450), normalColor);
                                
                #region itemoption popup
                // item options
                if (itemOptions || itemQuickSlotOption)
                {
                    Texture2D rect = new Texture2D(graphics, (int)options.getBounds().X, options.MenuItems.Count * 20),
                              rect2 = new Texture2D(graphics, (int)options.getBounds().X, options.MenuItems.Count * 20);

                    Color[] data = new Color[(int)options.getBounds().X * options.MenuItems.Count * 20];

                    Vector2 PopupPosition = new Vector2((graphics.Viewport.Width * 0.5f) - (options.getBounds().X * 0.5f),
                                                        (graphics.Viewport.Height * 0.5f) - (options.MenuItems.Count * 10));

                    // set colors for menu borders and fill
                    for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
                    rect.SetData(data);
                    for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
                    rect2.SetData(data);

                    // draw menu fill 20% transperancy
                    spriteBatch.Draw(rect,
                        new Rectangle((int)(PopupPosition.X - 5),(int)(PopupPosition.Y - 5), rect.Width + 10, rect.Height + 10), Color.White * 0.8f);

                    // draw borders
                    spriteBatch.Draw(rect2,
                        new Rectangle((int)(PopupPosition.X - 5), (int)(PopupPosition.Y - 5), 5, (int)options.MenuItems.Count * 20 + 10),
                        new Rectangle(0, 0, 5, 5), Color.White);
                    spriteBatch.Draw(rect2,
                        new Rectangle((int)(PopupPosition.X - 5), (int)(PopupPosition.Y - 5), (int)rect.Width + 10, 5),
                        new Rectangle(0, 0, 5, 5), Color.White);
                    spriteBatch.Draw(rect2,
                        new Rectangle((int)(PopupPosition.X - 5), (int)(PopupPosition.Y + (options.MenuItems.Count * 20) + 5), (int)rect.Width + 15, 5),
                        new Rectangle(0, 0, 5, 5), Color.White);
                    spriteBatch.Draw(rect2,
                        new Rectangle((int)(PopupPosition.X + rect.Width + 5), (int)(PopupPosition.Y - 5), 5, (int)options.MenuItems.Count * 20 + 15),
                        new Rectangle(0, 0, 5, 5), Color.White);


                    Vector2 optionPos = new Vector2(PopupPosition.X, PopupPosition.Y);

                    options.Position = optionPos;
                    options.Draw(gameTime);
                }
                #endregion
            }
        }

        #endregion

        #region item functions

        private void itemEquip()
        {
            if (ItemStore.Instance.getItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemID).Type == ItemType.Weapon ||
                ItemStore.Instance.getItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemID).Type == ItemType.Armor ||
                ItemStore.Instance.getItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemID).Type == ItemType.Accessory)
            {
                if (playerStore.activePlayer.equipment.getEquip(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].Slot) == null)
                {
                    playerStore.activePlayer.equipment.addItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex]);
                    playerStore.activePlayer.inventory.removeItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemID);
                }
                else
                {
                    Item getequip = playerStore.activePlayer.equipment.getEquip(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].Slot);
                    Item getinvent = itemlist.menuItemsnoDupes[itemlist.SelectedIndex];

                    playerStore.activePlayer.equipment.removeItem(getinvent.Slot);
                    playerStore.activePlayer.equipment.addItem(getinvent);

                    playerStore.activePlayer.inventory.removeItem(getinvent.itemID);
                    playerStore.activePlayer.inventory.addItem(getequip);
                }
            }

            updateItemList();       // update item menu
            itemOptions = false;    // close options

            // Update selected index
            if (itemlist.SelectedIndex > itemlist.menuItemsnoDupes.Count - 1)
                itemlist.SelectedIndex = itemlist.menuItemsnoDupes.Count - 1;
        }

        private void itemConsume(PlayerInfo Consumer)
        {
            Item selectedItem = itemlist.menuItemsnoDupes[itemlist.SelectedIndex];

            if (selectedItem.Type == ItemType.Consumable)
            {
                string script = selectedItem.Script;

                // remove beginning and ending spaces

                script = Regex.Replace(script, "{", "");
                script = Regex.Replace(script, "}", "");
                script = Regex.Replace(script, " ", "");

                // call static class that handles item scripts
                // use the script interpretter to read the content

                ScriptInterpreter.Instance.loadScript(script);
                ScriptInterpreter.Instance.StartReading = true;
                ScriptInterpreter.Instance.Property = null;
                ScriptInterpreter.Instance.Values.Clear();

                ScriptInterpreter.Instance.readScript();

                // clear script, reset to begin
                ScriptInterpreter.Instance.clearInstance();
            }

            itemRemove();
            itemOptions = false;
        }

        private void itemRemove()
        {
            playerStore.activePlayer.inventory.removeItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemID);

            // Update selected index
            if (itemlist.SelectedIndex > itemlist.menuItemsnoDupes.Count - 1)
                itemlist.SelectedIndex = itemlist.menuItemsnoDupes.Count - 1;

            updateItemList();       // update item menu
            itemOptions = false;    // close options
        }

        private void itemQuickSlot()
        {
            Item selectedItem = itemlist.menuItemsnoDupes[itemlist.SelectedIndex];

            if (selectedItem.Type == ItemType.Consumable)
            {
                itemOptions = false;
                itemQuickSlotOption = true;

                options.Style = OrderStyle.Central;
                options.SetMenuItems(new string[] 
                { 
                    "In which Quickslot do you", 
                    "want to place " + selectedItem.itemName +"?", 
                    "",
                    "<- " + SetItemSlot.ToString() + " ->",
                    "Cancel" 
                });
                options.StartIndex = 3;
                options.SelectedIndex = 3;
            }
        }

        #endregion
    }
}
