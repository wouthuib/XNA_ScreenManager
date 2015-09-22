using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.ScreenClasses.MainClasses;
using XNA_ScreenManager.ItemClasses;
using XNA_ScreenManager.PlayerClasses;
using System.Text;

namespace XNA_ScreenManager.GameAssets.InGame
{
    class ShopMenu : Menu
    {
        Texture2D spr_active_tab, spr_tab_names;
        Vector2 active_tab_pos = Vector2.Zero;
        SpriteFont smallFont, normalFont;
        private int tabid = 0;

        // Local Shop items, will be filled by the server when opening shop! 
        // to avoid local hacking, compare the shop transaction on the server
        public List<Item> item_list { get; set; }   

        public ShopMenu(Game game, float locktime)
            : base(game, locktime)
        {
            sprite = content.Load<Texture2D>(@"gfx\hud\ingame_menus\shopmenu\shop");
            spr_active_tab = content.Load<Texture2D>(@"gfx\hud\ingame_menus\shopmenu\tab_highlight");
            spr_tab_names = content.Load<Texture2D>(@"gfx\hud\ingame_menus\shopmenu\tab_names");

            smallFont = content.Load<SpriteFont>(@"font\Arial_10px");
            normalFont = content.Load<SpriteFont>(@"font\Arial_12px");

            // create tabs
            for (int i = 0; i < 5; i++)
            {
                Button tab = create_button("shopmenu_tab_" + i.ToString());
                tab.Position = new Vector2(Position.X + 242 + (31.25f * i), Position.Y + 105);
                tab.Width = 28;
                tab.Height = 18;
            }

            // create close button
            Button closebutton = create_button("shopmenu_button_close");
            closebutton.Position = new Vector2(Position.X + 161, Position.Y + 44);
            closebutton.Width = 63;
            closebutton.Height = 15;

            // create buy button
            Button buybutton = create_button("shopmenu_button_buy");
            buybutton.Position = new Vector2(Position.X + 161, Position.Y + 64);
            buybutton.Width = 63;
            buybutton.Height = 15;

            // create sell button
            Button sellbutton = create_button("shopmenu_button_sell");
            sellbutton.Position = new Vector2(Position.X + 392, Position.Y + 57);
            sellbutton.Width = 63;
            sellbutton.Height = 15;

            // initize item list instance
            item_list = new List<Item>();
        }

        #region update

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
            }
            
            this.DrawShopPrices(gameTime);
            this.DrawItems(gameTime);
        }

        public void DrawItems(GameTime gameTime)
        {
            Vector2 DrawPosition = Viewport + Position;

            for (int i = 0; i < listdraggables.FindAll(x=>x.storetype == "shop").Count; i++)
            {
                if (listdraggables[i].itemsprite != null)
                    spritebatch.Draw(listdraggables[i].itemsprite,
                        new Rectangle(
                        (int)(Viewport + listdraggables[i].Position).X,
                        (int)(Viewport + listdraggables[i].Position).Y,
                        35, 35), listdraggables[i].sprite_source_rec,
                        Color.White);
            }
                
        }

        public void DrawShopPrices(GameTime gameTime)
        {
            Vector2 DrawPosition = Viewport + Position;
            
            int y = 98;

            lock (item_list)
            {
                foreach (var item in item_list) // <= draw shop item descriptions
                {
                    StringBuilder[] lines = new StringBuilder[2];
                    lines[0] = new StringBuilder();
                    lines[1] = new StringBuilder();

                    // item name
                    for (int i = 0; i < item.itemName.Length; i++)
                        lines[0].Append(item.itemName[i]);

                    // item price
                    for (int i = 1; i <= item.Price.ToString().Length; i++)
                        lines[1].Append(item.Price.ToString()[i - 1]);

                    // draw lines
                    for (int i = 0; i < lines.Length; i++)
                    {
                        Color color = Color.Black;
                        if (i == 1) color = Color.Red;

                        // white item decription
                        spritebatch.DrawString(smallFont, lines[i].ToString(),
                            DrawPosition + new Vector2(50, y + (i * 15)), Color.White);

                        // black back color counter
                        spritebatch.DrawString(smallFont, lines[i].ToString(),
                            DrawPosition + new Vector2(50, y + (i * 15)) + Vector2.One, color);
                    }

                    y += 42;
                }
            }
        }

        #endregion

        #region functions

        public void SortShopItems()
        {
            this.listdraggables.Clear();

            // place shop items
            int y = 45;

            foreach (var item in item_list)
            {
                this.placeShopDraggable(item, new Vector2(5, y), "shop");
                y += 42;
            }

            //place inventory items
            y = 45;

            foreach (var item in PlayerStore.Instance.activePlayer.inventory.item_list)
            {
                this.placeShopDraggable(item, new Vector2(55, y), "inventory");
                y += 42;
            }
        }

        public void placeShopDraggable(Item item, Vector2 position, string store)
        {
            if (listdraggables.FindAll(x => x.item_id == item.itemID).Count == 0)
            {
                // create new draggable item object
                lock (listdraggables)
                {
                    Vector2 menu_offset = new Vector2(10, 51);

                    // create the draggable item object
                    DraggableObject draggable = create_draggable("shopmenu_drag_" +
                            Math.Round((float)position.X / 36).ToString() + "_" +
                            Math.Round((float)position.Y / 36).ToString(), item as Item, "shopmenu") as DraggableObject;

                    // add the item to the list
                    listdraggables.Add(draggable);

                    // update the item properties
                    draggable.Position = this.Position + position + menu_offset;
                    draggable.Width = 36;
                    draggable.Height = 36;
                    draggable.itemsprite = content.Load<Texture2D>(@item.itemSpritePath);
                    draggable.amount = 1;

                    if (store == "shop")
                        draggable.storetype = "shop";
                    else if (store == "inventory")
                        draggable.storetype = "inventory";
                }
            }
        }

        #endregion

        #region events

        protected override void event_buttonOnClick(object btn)
        {
            Button button = btn as Button;

            if (button.Name.StartsWith("shopmenu_tab_"))
            {
                string[] value = button.Name.Split('_');
                tabid = Convert.ToInt32(value[value.Length - 1]);
                active_tab_pos = new Vector2((31.25f * tabid), 0);
                //SortMenuItems();
            }
            if (button.Name.StartsWith("shopmenu_button_"))
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

            if (item.Name.StartsWith("shopmenu_drag_"))
                item.Position = new Vector2(MouseManager.Instance.MousePosition.X,
                                            MouseManager.Instance.MousePosition.Y) - item.DragPoint;
        }

        #endregion
    }
}
