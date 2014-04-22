using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using XNA_ScreenManager.ItemClasses;
using System.Collections.Generic;
using XNA_ScreenManager.ScreenClasses.SubComponents;
using System;
using XNA_ScreenManager.PlayerClasses;
using System.Reflection;
using Microsoft.Xna.Framework.Content;

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
        ContentManager Content;

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
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));

            this.spriteFont = spriteFont;
            itemlist = new ItemlistComponent(game);

            //SetmenuCategories(categories);
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
            switch (SelectedSlot)
            {
                case 0: // Weapon
                    return inventory.item_list.FindAll(delegate(Item item) { return item.Slot == ItemSlot.Weapon; });
                case 1: // Shield
                    return inventory.item_list.FindAll(delegate(Item item) { return item.Slot == ItemSlot.Shield; });
                case 2: // Headgear
                    return inventory.item_list.FindAll(delegate(Item item) { return item.Slot == ItemSlot.Headgear; });
                case 3: // Neck
                    return inventory.item_list.FindAll(delegate(Item item) { return item.Slot == ItemSlot.Neck; });
                case 4: // Bodygear
                    return inventory.item_list.FindAll(delegate(Item item) { return item.Slot == ItemSlot.Bodygear; });
                case 5: // Accessory
                    return inventory.item_list.FindAll(delegate(Item item) { return item.Slot == ItemSlot.Accessory; });
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

                    if (selectedSlot == Enum.GetNames(typeof(ItemSlot)).Length -1) // last slot is "None" we skip this one
                        selectedSlot = 0;
                }
                else if (CheckKey(Keys.Up))
                {
                    selectedSlot--;

                    if (selectedSlot == -1)
                        selectedSlot = Enum.GetNames(typeof(ItemSlot)).Length - 2; // last slot is "None" we skip this one
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
                            // make sure items are available
                            if (itemlist.menuItemsnoDupes.Count > 0)
                            {
                                slotOptions = false;
                                itemOptions = true;
                            }
                            break;
                        case 1:
                            if (slotEquiped(SelectedSlot))
                            {
                                itemUnEquip();
                                slotOptions = false;
                                itemOptions = false;
                            }
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
                itemlist.Update(gameTime);
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
            #region item list
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

            base.Draw(gameTime);
            itemlist.MaxDisplay = 4;
            itemlist.Draw(gameTime); // draw items

            // make sure items are available
            if (itemlist.menuItemsnoDupes.Count == 0)
                spriteBatch.DrawString(spriteFont, "None", new Vector2(itemlist.Position.X - 40, itemlist.Position.Y), Color.DarkGray);
            #endregion

            #region slot types
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

                // Draw Slot Name
                if (Enum.GetNames(typeof(ItemSlot))[i] != "None")
                {
                    spriteBatch.DrawString(spriteFont,
                    Enum.GetNames(typeof(ItemSlot))[i],
                    position,
                    myColor);

                    if (slotEquiped(i))
                    {
                        Texture2D sprite = manager.game.Content.Load<Texture2D>(@"" + getslotItem(i).itemSpritePath);
                        Rectangle srcframe = new Rectangle(getslotItem(i).SpriteFrameX * 48,
                                                           getslotItem(i).SpriteFrameY * 48,
                                                           48, 48);
                        Rectangle tarframe = new Rectangle((int)position.X + 170, (int)position.Y - 8, 30, 30);
                        spriteBatch.Draw(sprite, tarframe, srcframe, Color.White);

                        // Draw Item Name
                        spriteBatch.DrawString(spriteFont,
                        getslotItem(i).itemName,
                        new Vector2(position.X + 200, position.Y),
                        myColor);
                    }
                    else
                        spriteBatch.DrawString(spriteFont,
                        "None",
                        new Vector2(position.X + 200, position.Y),
                        Color.DarkGray);
                }

                position.Y += spriteFont.LineSpacing;
            }
            #endregion

            #region menu options
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
            #endregion
            
            #region player stats
            // Draw Player Name
            spriteBatch.DrawString(spriteFont, PlayerStore.Instance.activePlayer.Name.ToString(),
                new Vector2(20, 130), NormalColor);

            // Draw Player Stat Values
            position = new Vector2(20, 170);

            for (int i = 0; i < Enum.GetNames(typeof(PlayerStats)).Length; i++)
            {
                // Draw Player Stat Name
                spriteBatch.DrawString(spriteFont,
                Enum.GetNames(typeof(PlayerStats))[i],
                position, Color.DarkGray);

                // Get Stat Value
                Object player = PlayerStore.Instance.activePlayer;
                PropertyInfo info = player.GetType().GetProperty(Enum.GetNames(typeof(PlayerStats))[i]);

                // Draw Player Stat Value
                spriteBatch.DrawString(spriteFont,
                info.GetValue(player, null).ToString(),
                new Vector2(120, position.Y), NormalColor);

                /*
                if (itemOptions)
                {
                    spriteBatch.DrawString(spriteFont,
                    info.GetValue(player, null).ToString(),
                    new Vector2(140, position.Y), NormalColor);
                }
                */

                position.Y += spriteFont.LineSpacing;
            }
            #endregion

            // item description
            if(itemOptions)
                spriteBatch.DrawString(spriteFont, itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemDescription, new Vector2(80, 60), normalColor);

        }
        #endregion

        #region equipment functions

        private void itemEquip()
        {
            if (ItemStore.Instance.getItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemID).Type == ItemType.Weapon ||
                ItemStore.Instance.getItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemID).Type == ItemType.Armor ||
                ItemStore.Instance.getItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemID).Type == ItemType.Accessory)
            {
                if (equipment.getEquip(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].Slot) == null)
                {
                    // equip item from inventory
                    equipment.addItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex]);
                    inventory.removeItem(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].itemID);
                }
                else
                {
                    // swap inventory and equipment
                    Item getequip = equipment.getEquip(itemlist.menuItemsnoDupes[itemlist.SelectedIndex].Slot);
                    Item getinvent = itemlist.menuItemsnoDupes[itemlist.SelectedIndex];

                    equipment.removeItem(getinvent.Slot);
                    equipment.addItem(getinvent);

                    inventory.removeItem(getinvent.itemID);
                    inventory.addItem(getequip);
                }
            }

            updateItemList();       // update item menu
            itemOptions = false;    // close options

            // Update selected index
            if (itemlist.SelectedIndex > itemlist.menuItemsnoDupes.Count - 1)
                itemlist.SelectedIndex = itemlist.menuItemsnoDupes.Count - 1;
        }

        private void itemUnEquip()
        {
            if (equipment.getEquip(equipment.item_list.Find(delegate(Item item) 
                { 
                    return item.Slot == (ItemSlot)Enum.Parse(typeof(ItemSlot),
                       Enum.GetNames(typeof(ItemSlot))[SelectedSlot]);
                }
            ).Slot) != null)
            {
                // remove equipment
                Item getequip = getslotItem(SelectedSlot);
                //Item getinvent = itemlist.menuItemsnoDupes[itemlist.SelectedIndex];

                equipment.removeItem(getequip.Slot);
                inventory.addItem(getequip);
            }

            updateItemList();       // update item menu

            // Update selected index
            if (itemlist.SelectedIndex > itemlist.menuItemsnoDupes.Count - 1)
                itemlist.SelectedIndex = itemlist.menuItemsnoDupes.Count - 1;
        }

        private bool slotEquiped(int i)
        {
            if (equipment.item_list.FindAll(delegate(Item item)
                            {
                                return item.Slot == (ItemSlot)Enum.Parse(typeof(ItemSlot),
                                   Enum.GetNames(typeof(ItemSlot))[i]);
                            }
                            ).Count > 0)
            return true;
            else
            return false;
        }

        private Item getslotItem(int i)
        {
            return equipment.item_list.Find(delegate(Item item) { return item.Slot == (ItemSlot)Enum.Parse(typeof(ItemSlot), Enum.GetNames(typeof(ItemSlot))[i]); });
        }
        #endregion
    }
}
