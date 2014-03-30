using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace XNA_ScreenManager.ScreenClasses.InGame
{
    public class TopMessageScreen : GameScreen
    {
        ContentManager Content;
        GraphicsDevice gfxdevice;
        SpriteBatch spriteBatch;
        ScreenManager screenmanager = ScreenManager.Instance;
        GameTime gameTime;

        ItemClasses.Item iteminfo;
        SpriteFont spriteFont;

        Boolean Active = false;
        float previousAnimateTimeSec;

        public TopMessageScreen(Game game)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));
            gfxdevice = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));
            gameTime = (GameTime)Game.Services.GetService(typeof(GameTime));

            spriteFont = Content.Load<SpriteFont>(@"font\Arial_12px");
        }

        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                // reduce timer
                previousAnimateTimeSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (previousAnimateTimeSec <= 0)
                {
                    this.iteminfo = null;
                    this.Active = false;
                    this.Hide();
                }
            }
        }

        public void Display(ItemClasses.Item item)
        {
            previousAnimateTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds + 1.5f;
            this.Active = true;
        }

        public override void Draw(GameTime gameTime)
        {
            if (Active)
            {
                string message = this.iteminfo.itemName + " has been added to your inventory.";

                Vector2 size = spriteFont.MeasureString(message);

                Texture2D rect = new Texture2D(gfxdevice, (int)size.X, (int)size.Y);

                Color[] data = new Color[(int)size.X * (int)size.Y];
                for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
                rect.SetData(data);

                spriteBatch.Draw(rect, new Vector2((gfxdevice.Viewport.Width * 0.5f) - (int)(size.X * 0.5f), 10), Color.White * 0.5f);

                spriteBatch.DrawString(spriteFont, message, new Vector2((gfxdevice.Viewport.Width * 0.5f) - (int)(size.X * 0.5f), 10), Color.White);

            }
        }
    }
}
