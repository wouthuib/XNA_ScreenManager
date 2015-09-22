using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.ItemClasses;
using XNA_ScreenManager.GameAssets.InGame;
using XNA_ScreenManager.Networking;
using MapleLibrary;

namespace XNA_ScreenManager.GameAssets
{
    public delegate void ButtonOnDrag(object sender);
    public delegate void ButtonOnRelease(object sender, string menu);

    public class DraggableObject : Button
    {
        public event ButtonOnDrag ButtonOnDrag;
        public event ButtonOnRelease ButtonOnRelease;
        public bool DragLock = false;
        public Vector2 DragPoint;

        public Texture2D itemsprite;
        Vector2 OldPosition;

        public object attached_object;
        public string menuname;
        public string storetype; // shop, inventory or equip

        public Rectangle sprite_source_rec;
        public int amount;
        public int item_id
        {
            get 
            {
                if (attached_object is Item)
                {
                    Item item = attached_object as Item;
                    return item.itemID;
                }
                else
                    return -1;

            }
            set 
            {
                if (attached_object is Item)
                {
                    Item item = new Item();

                    item = attached_object as Item;
                    item.itemID = value;
                    attached_object = item;

                }
            } // new 06-07-2015
        }

        public static DraggableObject createDraggable(string name, object obj, string menu)
        {
            var results = new DraggableObject();
            results.Name = name;

            if (obj is Item)
            {
                Item item = obj as Item;
                results.attached_object = obj;
                results.menuname = menu;
                results.sprite_source_rec = new Rectangle(item.SpriteFrameX * 48, item.SpriteFrameY * 48, 48, 48);
            }

            return results;
        }

        public bool ondrag
        {
            get
            {
                if ((!MenuManager.Instance.menudragged && !MenuManager.Instance.itemdragged &&
                    MouseManager.Instance.MouseButtonIsDown(MouseButtons.Left) &&
                    MouseManager.Instance.MousePosition.Intersects(Bounderies)) ||
                    DragLock)
                {
                    if (!DragLock) // first time click
                    {
                        this.OldPosition = Position;
                        this.DragPoint = new Vector2(
                            MouseManager.Instance.MousePosition.X,
                            MouseManager.Instance.MousePosition.Y) - OldPosition;
                    }

                    if (MouseManager.Instance.MouseButtonIsUp(MouseButtons.Left))
                    {
                        MenuManager.Instance.itemdragged = false; // unfreeze other menus
                        this.Position = OldPosition; // reset Position
                        DragLock = false;
                        ButtonOnRelease(this, menuname); // new event (see MenuManager)
                        return false;
                    }

                    MenuManager.Instance.itemdragged = true;
                    DragLock = true;
                    sprite = spr_click;
                    ButtonOnDrag(this); // trigger event
                    return true;
                }
                else
                {
                    DragLock = false;
                    return false;
                }
            }
        }

        public static void event_releaseDraggable(object btn, string source_menu)
        {
            DraggableObject obj = btn as DraggableObject;
            EquipMenu equipmenu = MenuManager.Instance.Components.Find(x => x is EquipMenu) as EquipMenu;
            ItemMenu itemmenu = MenuManager.Instance.Components.Find(x => x is ItemMenu) as ItemMenu;

            if (source_menu == "itemmenu" && equipmenu.Enabled &&
                MouseManager.Instance.MousePosition.Intersects(equipmenu.Bounderies))
            {
                if (obj.attached_object is Item)
                {
                    Item item = obj.attached_object as Item;

                    // move item from item menu to equipment

                    NetworkGameData.Instance.sendItemData(item.itemID, "EquipItem");
                }
            }
            else if (source_menu == "equipmenu" && itemmenu.Enabled &&
                !MouseManager.Instance.MousePosition.Intersects(equipmenu.Bounderies))
            {
                // remove item from equipment

                if (obj.attached_object is Item)
                {
                    Item item = obj.attached_object as Item;

                    // move item from item menu to equipment

                    NetworkGameData.Instance.sendItemData(item.itemID, "UnEquipItem");
                }
            }
        }
    }
}
