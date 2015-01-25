using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using XNA_ScreenManager.ScreenClasses.MainClasses;

namespace XNA_ScreenManager.GameAssets.InGame
{
    class EquipMenu : Menu
    {
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

            base.Draw(gameTime);
        }

        protected override void event_buttonOnClick(object btn)
        {
            Button button = btn as Button;

            if (button.Name.StartsWith("equipmenu_button_"))
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
