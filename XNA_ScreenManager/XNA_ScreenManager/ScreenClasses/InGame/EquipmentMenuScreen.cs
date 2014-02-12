using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Specialized;

using XNA_ScreenManager.ItemClasses;
using System.Collections.Generic;
using XNA_ScreenManager.ScreenClasses.SubComponents;
using System;

namespace XNA_ScreenManager.ScreenClasses.InGame
{
    public class EquipmentMenuScreen : GameScreen
    {
        #region properties
        ItemlistComponent itemlist;
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

        //private StringCollection menuItems = new StringCollection();
        private string[] menuOptions = new string[] { "Change", "UnEquip", "Cancel" };

        int width, height;
        int selectedSlot = 0, selectedOption = 0;
        private bool itemOptions = false, slotOptions = false;

        #endregion
                
        public EquipmentMenuScreen(Game game, SpriteFont spriteFont, Texture2D background)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            graphics = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));
            this.spriteFont = spriteFont;
            itemlist = new ItemlistComponent(game, spriteFont);

            //SetmenuCategories(categories);
            updateItemList();

            Components.Add(new BackgroundComponent(game, background));
            Components.Add(itemlist);
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
            switch (SelectedSlot)
            {
                case 0: // Weapon
                    return inventory.item_list.FindAll(delegate(Item item) { return item.itemSlot == ItemSlot.Weapon; });
                case 1: // Shield
                    return inventory.item_list.FindAll(delegate(Item item) { return item.itemSlot == ItemSlot.Shield; });
                case 2: // Headgear
                    return inventory.item_list.FindAll(delegate(Item item) { return item.itemSlot == ItemSlot.Headgear; });
                case 3: // Neck
                    return inventory.item_list.FindAll(delegate(Item item) { return item.itemSlot == ItemSlot.Neck; });
                case 4: // Bodygear
                    return inventory.item_list.FindAll(delegate(Item item) { return item.itemSlot == ItemSlot.Bodygear; });
                case 5: // Accessory
                    return inventory.item_list.FindAll(delegate(Item item) { return item.itemSlot == ItemSlot.Accessory; });
                default:
                    return inventory.item_list;
            }
        }

        #endregion

        #region optionmenu update methods

        // Menu category functions
        public int SelectedSlot
        {
            get { return selectedSlot; }
            set
            {
                selectedSlot = (int)MathHelper.Clamp(
                value,
                0,
                Enum.GetNames(typeof(ItemSlot)).Length - 1);
            }
        }

        public int SelectedOption
        {
            get { return selectedOption; }
            set
            {
                selectedOption = (int)MathHelper.Clamp(
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

            if (!slotOptions && !itemOptions)
            {
                if (CheckKey(Keys.Down))
                {
                    selectedSlot++;

                    if (selectedSlot == Enum.GetNames(typeof(ItemSlot)).Length)
                        selectedSlot = 0;
                }
                else if (CheckKey(Keys.Up))
                {
                    selectedSlot--;

                    if (selectedSlot == -1)
                        selectedSlot = Enum.GetNames(typeof(ItemSlot)).Length - 1;
                }
                else if (CheckKey(Keys.Enter))
                {
                    itemOptions = false;
                    slotOptions = true;
                }
            }
            else if (slotOptions && !itemOptions)
            {
                if (CheckKey(Keys.Right))
                {
                    selectedOption++;

                    if (selectedOption == menuOptions.Length)
                        selectedOption = 0;
                }
                else if (CheckKey(Keys.Left))
                {
                    selectedOption--;

                    if (selectedOption == -1)
                        selectedOption = menuOptions.Length - 1;
                }
                else if (CheckKey(Keys.Enter))
                {
                    switch (selectedOption)
                    {
                        case 0:
                            slotOptions = false;
                            itemOptions = true;
                            break;
                        case 1:
                            itemUnEquip();
                            slotOptions = false;
                            itemOptions = false;
                            break;
                        case 2:
                            slotOptions = false;
                            itemOptions = false;
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (itemOptions)
            {
                // update item components
                base.Update(gameTime);

                if (CheckKey(Keys.Enter))
                {
                    itemEquip();
                    slotOptions = false;
                    itemOptions = false;
                }
            }

            // always update the itemlist
            updateItemList();

            // save keyboard state
            oldState = newState;
        }

        #endregion

        #region draw methods
        public override void Show()
        {
            itemlist.Position = new Vector2(150, 130);
            base.Show();
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw the base first
            itemlist.Position = new Vector2(360, 350);
            if (!itemOptions)
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

            base.Draw(gameTime); // draw items

            // Draw the menu items second
            Vector2 position = new Vector2();
            Color myColor;

            // Draw Slot Types
            position = new Vector2(320, 178);

            for (int i = 0; i < Enum.GetNames(typeof(ItemSlot)).Length; i++)
            {
                if (i == SelectedSlot)
                    myColor = HiliteColor;
                else
                {
                    if (!slotOptions && !itemOptions)
                        myColor = NormalColor;
                    else
                        myColor = Color.DarkGray;
                }

                spriteBatch.DrawString(spriteFont,
                Enum.GetNames(typeof(ItemSlot))[i],
                position,
                myColor);

                if (equipment.item_list.FindAll(delegate(Item item) { return item.itemSlot == (ItemSlot)Enum.Parse(typeof(ItemSlot), Enum.GetNames(typeof(ItemSlot))[i]); }).Count > 0)
                    spriteBatch.DrawString(spriteFont,
                    equipment.item_list.Find(delegate(Item item) { return item.itemSlot == (ItemSlot)Enum.Parse(typeof(ItemSlot), Enum.GetNames(typeof(ItemSlot))[i]); }).itemName,
                    new Vector2(position.X + 200, position.Y),
                    myColor);

                position.Y += spriteFont.LineSpacing;
            }

            // Draw Menu Option Types
            position = new Vector2(320, 130);

            for (int i = 0; i < menuOptions.Length; i++)
            {
                if (slotOptions)
                {
                    if(i == SelectedOption)
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
                
            // item description
            if(itemOptions)
                spriteBatch.DrawString(spriteFont, itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemDescription, new Vector2(80, 70), normalColor);

        }
        #endregion

        #region equipment functions

        private void itemEquip()
        {
            if (equipment.getEquip(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemSlot) == null)
            {
                // equip item from inventory
                equipment.addItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex]);
                inventory.removeItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemID);
            }
            else
            {
                // swap inventory and equipment
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

        private void itemUnEquip()
        {
            if (equipment.getEquip(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemSlot) == null)
            {
                // remove equipment
                Item getequip = equipment.getEquip(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemSlot);
                Item getinvent = itemlist.menuItemsnoDupes[itemlist.SelectedIndex];

                equipment.removeItem(getinvent.itemSlot);
                inventory.addItem(getequip);
            }

            updateItemList();       // update item menu

            // Update selected index
            if (itemlist.SelectedIndex > itemlist.menuItemsnoDupes.Count - 1)
                itemlist.SelectedIndex = itemlist.menuItemsnoDupes.Count - 1;
        }
        #endregion
    }
}
