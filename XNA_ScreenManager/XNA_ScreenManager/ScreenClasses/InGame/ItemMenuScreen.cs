using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Specialized;

using XNA_ScreenManager.ItemClasses;
using System.Collections.Generic;
using XNA_ScreenManager.ScreenClasses.SubComponents;

namespace XNA_ScreenManager.ScreenClasses
{
    public class ItemMenuScreen : GameScreen
    {
        #region properties
        ItemlistComponent itemlist;
        MenuComponent options;
        Inventory inventory = Inventory.Instance;
        Equipment equipment = Equipment.Instance;
        ScreenManager manager = ScreenManager.Instance;

        List<Item> itemobjects = new List<Item>();

        SpriteFont spriteFont;
        SpriteBatch spriteBatch;
        GraphicsDevice graphics;

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
        int selectedCategory = 0;
        bool itemOptions = false;

        #endregion

        #region constructor
        public ItemMenuScreen(Game game, SpriteFont spriteFont, Texture2D background)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            graphics = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));
            this.spriteFont = spriteFont;
            itemlist = new ItemlistComponent(game, spriteFont);
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
                    return inventory.item_list;
                case 1: // Collectables
                    return inventory.item_list.FindAll(delegate(Item item) { return item.itemType == ItemType.Collectable; });
                case 2: // Weapons
                    return inventory.item_list.FindAll(delegate(Item item) { return item.itemType == ItemType.Weapon; });
                case 3: // Armor
                    return inventory.item_list.FindAll(delegate(Item item) { return item.itemType == ItemType.Armor; });
                case 4: // Accessory
                    return inventory.item_list.FindAll(delegate(Item item) { return item.itemType == ItemType.Accessory; });
                case 5: // KeyItem
                    return inventory.item_list.FindAll(delegate(Item item) { return item.itemType == ItemType.KeyItem; });
                default:
                    return inventory.item_list;
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

            if (itemOptions == false)
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
                        itemOptions = true;
                        options.SetMenuItems(new string[]{
                            "Equip", "Use", "Remove", "Cancel"});
                    }
                }

                if (CheckKey(Keys.Escape) || CheckKey(Keys.Back))
                {
                    manager.setScreen("InGameMainMenuScreen");
                }

                // update item components
                base.Update(gameTime);
            }
            else
            {
                // Check item options
                if (CheckKey(Keys.Enter))
                {
                    switch (options.SelectedIndex)
                    {
                        case 0:
                            itemEquip();
                            break;
                        case 1:
                            itemConsume();
                            break;
                        case 2:
                            itemRemove();
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

            if (filterItemList().Count > 0)
            {
                // item description
                spriteBatch.DrawString(spriteFont, itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemDescription, new Vector2(80, 450), normalColor);

                // item options
                if (itemOptions)
                {
                    Texture2D rect = new Texture2D(graphics, 100, 80);

                    Color[] data = new Color[100 * 80];
                    for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
                    rect.SetData(data);

                    spriteBatch.Draw(rect, new Vector2(itemlist.selectPos.X + 150, itemlist.selectPos.Y + 0),
                        Color.White * 0.8f);


                    Vector2 optionPos = new Vector2();
                    optionPos.X = itemlist.selectPos.X + 150;
                    optionPos.Y = itemlist.selectPos.Y;

                    options.Position = optionPos;
                    options.Draw(gameTime);
                }
            }
        }

        #endregion

        #region item functions

        private void itemEquip()
        {
            if (equipment.getEquip(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemSlot) == null)
            {
                equipment.addItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex]);
                inventory.removeItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemID);
            }
            else
            {
                Item getequip = equipment.getEquip(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemSlot);
                Item getinvent = itemlist.menuItemsnoDupes[itemlist.SelectedIndex];

                equipment.removeItem(getinvent.itemSlot);
                equipment.addItem(getinvent);

                inventory.removeItem(getinvent.itemID);
                inventory.addItem(getequip);
            }

            updateItemList();       // update item menu
            itemOptions = false;    // close options

            // Update selected index
            if (itemlist.SelectedIndex > itemlist.menuItemsnoDupes.Count - 1)
                itemlist.SelectedIndex = itemlist.menuItemsnoDupes.Count - 1;
        }

        private void itemConsume()
        {
            //filterItemList()[menu.SelectedIndex]
            itemOptions = false;
        }

        private void itemRemove()
        {
            inventory.removeItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemID);

            // Update selected index
            if (itemlist.SelectedIndex > itemlist.menuItemsnoDupes.Count - 1)
                itemlist.SelectedIndex = itemlist.menuItemsnoDupes.Count - 1;

            updateItemList();       // update item menu
            itemOptions = false;    // close options
        }

        #endregion
    }
}
