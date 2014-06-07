using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.PlayerClasses;

namespace XNA_ScreenManager
{
    public class StartScreen : GameScreen
    {
        MenuComponent menu;

        public StartScreen(Game game, SpriteFont spriteFont, Texture2D background)
            : base(game)
        {
            Components.Add(new BackgroundComponent(game, background));
            string[] items = { "New Adventure", "Load Adventure", "Help", "Quit" };
            menu = new MenuComponent(game, spriteFont);
            menu.SetMenuItems(items);
            menu.StartIndex = 0;
            Components.Add(menu);
        }

        public int SelectedIndex
        {
            get { return menu.SelectedIndex; }
        }

        public override void Update(GameTime gameTime)
        {
            if (PlayerStore.Instance.Count > 0)
                menu.MenuItems[0] = "Continue Adventure";
            else
                menu.MenuItems[0] = "New Adventure";

            base.Update(gameTime);
        }

        public override void Show()
        {
            menu.Position = new Vector2(
            (Game.Window.ClientBounds.Width - menu.Width) / 2, 360);
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}

















