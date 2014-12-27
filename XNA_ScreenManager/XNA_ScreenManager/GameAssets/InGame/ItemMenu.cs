using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using XNA_ScreenManager.ScreenClasses.MainClasses;

namespace XNA_ScreenManager.GameAssets.InGame
{
    class ItemMenu : Menu
    {
        Texture2D spr_active_tab, spr_tab_names;
        Vector2 active_tab_pos = Vector2.Zero;

        public ItemMenu(Game game)
            : base(game)
        {       
            sprite = content.Load<Texture2D>(@"gfx\hud\item inventory\menu");
            spr_active_tab = content.Load<Texture2D>(@"gfx\hud\item inventory\active tab");
            spr_tab_names = content.Load<Texture2D>(@"gfx\hud\item inventory\tab names");

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

        public override void Update(GameTime gameTime)
        {
            if (!isDragged)
                foreach (var button in listbutton)
                {
                    if (button.onclick)
                    {
                        DragLock = false;  
                    }
                }

            base.Update(gameTime);
        }

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

            base.Draw(gameTime);
        }

        protected override void event_buttonOnClick(object btn)
        {
            Button button = btn as Button;

            if (button.Name.StartsWith("itemmenu_tab_"))
            {
                string[] value = button.Name.Split('_');
                int tabid = Convert.ToInt32(value[value.Length -1]);
                active_tab_pos = new Vector2((31.25f * tabid), 0);
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
    }
}
