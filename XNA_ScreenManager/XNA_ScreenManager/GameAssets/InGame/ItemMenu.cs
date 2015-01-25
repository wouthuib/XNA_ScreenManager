using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using XNA_ScreenManager.ScreenClasses.MainClasses;
using XNA_ScreenManager.PlayerClasses;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.ItemClasses;
using System.Collections.Generic;
using XNA_ScreenManager.Networking;
using XNA_ScreenManager.ScreenClasses;

namespace XNA_ScreenManager.GameAssets.InGame
{
    class ItemMenu : Menu
    {
        #region properties

        Texture2D spr_active_tab, spr_tab_names;
        Vector2 active_tab_pos = Vector2.Zero;

        SpriteFont smallFont, normalFont;

        private int tabid = 0;
        private bool serverRequest = false, sorting = false; //, initialRequest = false;

        public bool Loading
        {
            get 
            {
                if (serverRequest || sorting)
                    return true;
                else
                    return false;
            }
        }

        #endregion

        public ItemMenu(Game game, float locktime)
            : base(game, locktime)
        {
            sprite = content.Load<Texture2D>(@"gfx\hud\ingame_menus\inventory\menu");
            spr_active_tab = content.Load<Texture2D>(@"gfx\hud\ingame_menus\inventory\active tab");
            spr_tab_names = content.Load<Texture2D>(@"gfx\hud\ingame_menus\inventory\tab names");

            smallFont = content.Load<SpriteFont>(@"font\Arial_10px");
            normalFont = content.Load<SpriteFont>(@"font\Arial_12px");

            // create tabs
            for(int i = 0; i < 5; i++)
            {
                Button tab = create_button("itemmenu_tab_" + i.ToString());
                tab.Position = new Vector2(Position.X + 9 + (31.25f * i), Position.Y + 26);
                tab.Width = 28;
                tab.Height = 18;
            }

            // create close button
            Button closebutton = create_button("itemmenu_button_close");
            closebutton.Position = new Vector2(Position.X + 151, Position.Y + 7);
            closebutton.Width = 9;
            closebutton.Height = 9;
        }

        #region update

        public override void Update(GameTime gameTime)
        {
            //if (ScreenManager.Instance.activeScreen == ScreenManager.Instance.actionScreen &&
            //    !initialRequest)
            //{
            //    initialRequest = true;
            //    ServerReqInventory();
            //}

            foreach (var button in listbutton)
            {
                if (button.onclick)
                {
                    LockTime = (float)gameTime.TotalGameTime.TotalSeconds;
                    DragLock = false;
                }
            }

            // foreach doesn't work as the TCP thread adds and deletes items from the list
            // for loop is more flexible and simply 
            for (int i = 0; i < listdraggables.Count; i++)
            {
                if (listdraggables[i] != null) // check if item is not removed by TCP thread
                    if (listdraggables[i].ondrag)
                    {
                        LockTime = (float)gameTime.TotalGameTime.TotalSeconds;
                        DragLock = false;
                    }
            }
                        
            base.Update(gameTime);
        }

        #endregion

        #region draw

        public override void Draw(GameTime gameTime)
        {
            Vector2 DrawPosition = Viewport + Position;

            spritebatch.Draw(sprite, DrawPosition, Color.White);
            spritebatch.Draw(spr_active_tab, DrawPosition + active_tab_pos, Color.White);
            spritebatch.Draw(spr_tab_names, DrawPosition, Color.White);

            foreach (var button in listbutton)
            {
                if (button.sprite != null)
                    spritebatch.Draw(button.sprite, DrawPosition + button.Position, Color.White);
                //else
                //{
                //    Texture2D rect = new Texture2D(ResourceManager.GetInstance.gfxdevice, (int)Math.Abs(button.Width), (int)button.Height);

                //    Color[] data = new Color[(int)Math.Abs(button.Width) * (int)button.Height];
                //    for (int i = 0; i < data.Length; ++i) data[i] = Color.Red;
                //    rect.SetData(data);

                //    spritebatch.Draw(rect, Viewport + button.Position, Color.White * 0.5f);
                //}
            }

            Drawitems(gameTime);

            base.Draw(gameTime);
        }

        private void Drawitems(GameTime gameTime)
        {
            Vector2 DrawPosition = Viewport + Position;

            if (!serverRequest)
                for (int i = 0; i < listdraggables.Count; i ++ )
                {
                    if (listdraggables[i].itemsprite != null)
                        spritebatch.Draw(listdraggables[i].itemsprite,
                            new Rectangle(
                            (int)(Viewport + listdraggables[i].Position).X,
                            (int)(Viewport + listdraggables[i].Position).Y,
                            35, 35), listdraggables[i].sprite_source_rec,
                            Color.White);

                    // black back color counter
                    spritebatch.DrawString(smallFont,
                        listdraggables[i].amount.ToString() + "x",
                        Viewport + listdraggables[i].Position + Vector2.One, Color.Black);
                }
        }

        #endregion

        #region functions

        // Fetch Invetory and place this in Menu Item list
        private List<Item> filterItemList()
        {
            // created sorted list based on the item ID
            var sort = PlayerStore.Instance.activePlayer.inventory.item_list;
            sort.Sort((item1, item2) => item1.itemID.CompareTo(item2.itemID));

            switch (tabid)
            {
                case 0: // Equipables
                    return sort.FindAll(x => x.Type == ItemType.Weapon || x.Type == ItemType.Armor || x.Type == ItemType.Accessory);
                case 1: // Usables
                    return sort.FindAll(x => x.Type == ItemType.Consumable);
                case 2: // Monster drops
                    return sort.FindAll(x => x.Type == ItemType.Collectable);
                case 3: // Key Items
                    return sort.FindAll(x => x.Type == ItemType.KeyItem);
                case 4: // Cash Items
                    return sort.FindAll(x => x.Type == ItemType.Cash);
                default:
                    return sort;
            }
        }

        // Fetch Item Store and place this in Menu Item list
        private List<Item> filterItemTypes()
        {
            ItemStore store = ItemStore.Instance;

            switch (tabid)
            {
                case 0: // Equipables
                    return store.item_list.FindAll(
                        x => x.Type == ItemType.Weapon || x.Type == ItemType.Armor || x.Type == ItemType.Accessory);
                case 1: // Usables
                    return store.item_list.FindAll(
                        x => x.Type == ItemType.Consumable);
                case 2: // Monster drops
                    return store.item_list.FindAll(
                        x => x.Type == ItemType.Collectable);
                case 3: // Key Items
                    return store.item_list.FindAll(
                        x => x.Type == ItemType.KeyItem);
                case 4: // Cash Items
                    return store.item_list.FindAll(
                        x => x.Type == ItemType.Cash);
                default:
                    return store.item_list;
            }
        }

        public void ServerReqInventory()
        {
            serverRequest = true;
            NetworkGameData.Instance.sendItemData(0, "ReqInventory");
        }

        public void ServerReqFinish()
        {
            SortMenuItems();
            serverRequest = false;
        }

        private void SortMenuItems()
        {
            sorting = true;

            listdraggables.Clear(); // clear existing draggables

            //List<Item> ItemListNoDup = new List<Item>();
            //foreach (var item in filterItemList())
            //    if (ItemListNoDup.FindAll(x => x.itemID == item.itemID).Count == 0)
            //        ItemListNoDup.Add(item);

            foreach (var item in filterItemList())
                placeDraggable(item);

            sorting = false;
        }

        public void placeDraggable(Item item)
        {
            // testje
            // var list_item filterItemList().FirstOrDefault(x => x.itemID == item.itemID)
            // if (list_item.Equals(default(Item)))

            if (filterItemTypes().FindAll(x => x.itemID == item.itemID).Count > 0)            
            {
                if (listdraggables.FindAll(x => x.item_id == item.itemID).Count == 0)
                {
                    // create new draggable item object
                    lock (listdraggables)
                    {
                        Vector2 menu_offset = new Vector2(10, 51);
                        Vector2 item_position = new Vector2(0, 0);

                        // if the menu already have draggable items
                        // update this item position 
                        if (listdraggables.Count > 0)
                        {
                            if (listdraggables[listdraggables.Count - 1].Position.X - menu_offset.X - this.Position.X < 108)
                                item_position = new Vector2((listdraggables[listdraggables.Count - 1].Position.X - menu_offset.X - this.Position.X) + 36,
                                    listdraggables[listdraggables.Count - 1].Position.Y - menu_offset.Y - this.Position.Y);
                            else
                                item_position = new Vector2(0, (listdraggables[listdraggables.Count - 1].Position.Y - menu_offset.Y - this.Position.Y) + 36);
                        }

                        // create the draggable item object
                        DraggableObject draggable = create_draggable("itemmenu_drag_" +
                                Math.Round((float)item_position.X / 36).ToString() + "_" +
                                Math.Round((float)item_position.Y / 36).ToString(), item as Item, "itemmenu") as DraggableObject;

                        // add the item to the list
                        listdraggables.Add(draggable);

                        // update the item properties
                        draggable.Position = this.Position + item_position + menu_offset;
                        draggable.Width = 36;
                        draggable.Height = 36;
                        draggable.itemsprite = content.Load<Texture2D>(@item.itemSpritePath);
                        draggable.amount = 1;
                    }
                }
                else
                {
                    // increase existing item amount
                    lock (listdraggables)
                    {
                        DraggableObject draggable = listdraggables.Find(x => x.item_id == item.itemID);
                        draggable.amount ++;
                    }
                }
            }
        }

        public void removeDraggable(Item item)
        {
            if (filterItemTypes().FindAll(x => x.itemID == item.itemID).Count > 0)
                lock (listdraggables)
                {
                    DraggableObject draggable = listdraggables.Find(x => x.item_id == item.itemID);

                    if (filterItemList().FindAll(x => x.itemID == item.itemID).Count == 0)
                    {
                        //listdraggables.Remove(draggable);
                        SortMenuItems();
                    }
                    else
                    {
                        draggable.amount --;
                    }
                }
        }

        #endregion

        #region events

        protected override void event_buttonOnClick(object btn)
        {
            Button button = btn as Button;

            if (button.Name.StartsWith("itemmenu_tab_"))
            {
                string[] value = button.Name.Split('_');
                tabid = Convert.ToInt32(value[value.Length -1]);
                active_tab_pos = new Vector2((31.25f * tabid), 0);
                SortMenuItems();
            }
            else if (button.Name.StartsWith("itemmenu_button_"))
            {
                string[] value = button.Name.Split('_');
                switch(value[value.Length - 1])
                {
                    case "close":
                        this.Hide();
                        break;
                    default:
                        break;
                }
            }

            base.event_buttonOnClick(btn);
        }

        protected override void event_buttonOnDrag(object btn)
        {
            DraggableObject item = btn as DraggableObject;

            if (item.Name.StartsWith("itemmenu_drag_"))
                item.Position = new Vector2(MouseManager.Instance.MousePosition.X,
                                            MouseManager.Instance.MousePosition.Y) - item.DragPoint;
        }

        #endregion
    }
}
