using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Specialized;

namespace XNA_ScreenManager.ScreenClasses.SubComponents
{
    public class ScreenPopup : MenuComponent
    {
        GraphicsDevice graphics;
        
        public ScreenPopup(Game game, SpriteFont spritefont)
            : base(game, spritefont)
        {
            spriteFont = spritefont;

            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            graphics = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));
        }

        public override void Draw(GameTime gameTime)
        {
            if (show)
            {
                Texture2D rect = new Texture2D(graphics, (int)getBounds().X, MenuItems.Count * 20),
                          rect2 = new Texture2D(graphics, (int)getBounds().X, MenuItems.Count * 20);

                Color[] data = new Color[(int)getBounds().X * MenuItems.Count * 20];

                // set colors for menu borders and fill
                for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
                rect.SetData(data);
                for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
                rect2.SetData(data);

                // draw menu fill 10% transperancy
                spriteBatch.Draw(rect,
                    new Rectangle((int)(position.X - 5), (int)(position.Y - 5), rect.Width + 10, (MenuItems.Count * 20) + 15),
                    Color.White * 0.9f);

                // draw borders
                spriteBatch.Draw(rect2,
                    new Rectangle((int)(position.X - 10), (int)(position.Y - 5), 5, (MenuItems.Count * 20) + 15),
                    new Rectangle(0, 0, 5, 5), Color.White);
                spriteBatch.Draw(rect2,
                    new Rectangle((int)(position.X - 10), (int)(position.Y - 10), (int)rect.Width + 20, 5),
                    new Rectangle(0, 0, 5, 5), Color.White);
                spriteBatch.Draw(rect2,
                    new Rectangle((int)(position.X - 10), (int)position.Y + (MenuItems.Count * 20) + 10, (int)rect.Width + 20, 5),
                    new Rectangle(0, 0, 5, 5), Color.White);
                spriteBatch.Draw(rect2,
                    new Rectangle((int)(position.X + 5 + rect.Width), (int)(position.Y - 5), 5, (int)MenuItems.Count * 20 + 15),
                    new Rectangle(0, 0, 5, 5), Color.White);
            }

            base.Draw(gameTime);
        }
    }
}
