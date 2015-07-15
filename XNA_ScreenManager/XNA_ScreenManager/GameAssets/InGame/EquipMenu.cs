using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using XNA_ScreenManager.ScreenClasses.MainClasses;
using XNA_ScreenManager.ItemClasses;
using System.Collections.Generic;
using XNA_ScreenManager.PlayerClasses;

namespace XNA_ScreenManager.GameAssets.InGame
{
    class EquipMenu : Menu
    {
        private bool serverRequest = false, sorting = false; //, initialRequest = false;

        public EquipMenu(Game game, float locktime)
            : base(game, locktime)
        {
            sprite = content.Load<Texture2D>(@"gfx\hud\ingame_menus\equipment\menu");

            // create close button
            Button closebutton = create_button("equipmenu_button_close");
            closebutton.Position = new Vector2(Position.X + 164, Position.Y + 7);
            closebutton.Width = 9;
            closebutton.Height = 9;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var button in listbutton)
            {
                if (button.onclick)
                {
                    LockTime = (float)gameTime.TotalGameTime.TotalSeconds;
                    DragLock = false;
                }
            }

            try
            {
                foreach (var button in listdraggables)
                {
                    if (button.ondrag)
                    {
                        LockTime = (float)gameTime.TotalGameTime.TotalSeconds;
                        DragLock = false;
                    }
                }
            }
            catch
            { } // sometimes exception that button was changed, to be fixed!!
                

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 DrawPosition = Viewport + Position;

            spritebatch.Draw(sprite, DrawPosition, Color.White);

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

            // new 06-07-2015
            if (!serverRequest)
                for (int i = 0; i < listdraggables.Count; i++)
                {
                    if (listdraggables[i].itemsprite != null)
                        spritebatch.Draw(listdraggables[i].itemsprite,
                            new Rectangle(
                            (int)(Viewport + listdraggables[i].Position).X,
                            (int)(Viewport + listdraggables[i].Position).Y,
                            35, 35), listdraggables[i].sprite_source_rec,
                            Color.White);
                }

            base.Draw(gameTime);
        }

        #region functions

        // new 06-07-2015
        public void placeDraggable(Item item)
        {
            bool equiped = false;

            // remove objects with same slot
            foreach (DraggableObject drag in listdraggables)
            {
                string slot1 = ItemStore.Instance.item_list.Find(x => x.itemID == drag.item_id).Slot.ToString();
                string slot2 = ItemStore.Instance.item_list.Find(x => x.itemID == item.itemID).Slot.ToString();

                if (slot1 == slot2)
                {
                    drag.itemsprite = content.Load<Texture2D>(@item.itemSpritePath);
                    drag.sprite_source_rec = new Rectangle(item.SpriteFrameX * 48, item.SpriteFrameY * 48, 48, 48);
                    equiped = true;
                    break;
                }
            }

            if (ItemStore.Instance.item_list.FindAll(x => x.itemID == item.itemID).Count > 0
                && !equiped)
            {

                // create new draggable item object
                lock (listdraggables)
                {
                    Vector2 menu_offset = new Vector2(5, 12);
                    Vector2 item_position = Vector2.Zero;
                    Vector2 slot_position = Vector2.Zero;

                    switch(ItemStore.Instance.item_list.Find(x => x.itemID == item.itemID).Slot.ToString())
                    {
                        case "Weapon":         // both hands i.e. two-handed sword and bows
                            slot_position = new Vector2(97, 100);
                            break;
                        case "Shield":         // shoulds i.e. cape
                            slot_position = new Vector2(0, 0);
                            break;
                        case "Headgear":       // complete head i.e. helmet
                            slot_position = new Vector2(33, 0);
                            break;
                        case "Neck":           // necklace and scarf
                            slot_position = new Vector2(0, 0);
                            break;
                        case "Bodygear":       // complete body i.e. cloak
                            slot_position = new Vector2(33, 100);
                            break;
                        case "Feet":           // Feet i.e. boots
                            slot_position = new Vector2(0, 0);
                            break;
                        case "Accessory":      // rings etc..
                            slot_position = new Vector2(0, 0);
                            break;
                    }

                    item_position = new Vector2((menu_offset.X + slot_position.X), (menu_offset.Y + slot_position.Y));

                    // create the draggable item object
                    DraggableObject draggable = create_draggable("equipmenu_drag_" +
                            Math.Round((float)item_position.X / 36).ToString() + "_" +
                            Math.Round((float)item_position.Y / 36).ToString(), item as Item, "equipmenu") as DraggableObject;

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
        }

        public void sortDraggable()
        {
            sorting = true;

            lock (listdraggables)
            {
                List<Item> templist = new List<Item>();
                foreach (var item in PlayerStore.Instance.activePlayer.equipment.item_list)
                    if (PlayerStore.Instance.activePlayer.equipment.item_list.FindAll(x => x.itemID == item.itemID).Count > 0)
                        templist.Add(item);

                listdraggables.Clear(); // clear existing draggables

                foreach (var newdrag in templist)
                    placeDraggable(newdrag);
            }

            sorting = false;
        }
        
        #endregion

        #region events
        
        protected override void event_buttonOnClick(object btn)
        {
            Button button = btn as Button;

            if (button.Name.StartsWith("equipmenu_button_"))
            {
                string[] value = button.Name.Split('_');
                switch (value[value.Length - 1])
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

            if (item.Name.StartsWith("equipmenu_drag_"))
                item.Position = new Vector2(MouseManager.Instance.MousePosition.X,
                                            MouseManager.Instance.MousePosition.Y) - item.DragPoint;
        }
        
        #endregion
    }
}
