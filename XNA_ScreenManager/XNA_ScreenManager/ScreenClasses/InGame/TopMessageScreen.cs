using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace XNA_ScreenManager.ScreenClasses.InGame
{
    public class TopMessageScreen : DrawableGameComponent
    {
        ContentManager Content;
        GraphicsDevice gfxdevice;
        SpriteBatch spriteBatch;
        ScreenManager screenmanager = ScreenManager.Instance;
        GameTime gameTime;

        ItemClasses.Item iteminfo;
        SpriteFont spriteFont;

        float previousTimeSec, transperancy;
        Vector2 position = new Vector2();
        string midstring = null;

        public TopMessageScreen(Game game)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));
            gfxdevice = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));
            gameTime = (GameTime)Game.Services.GetService(typeof(GameTime));

            spriteFont = Content.Load<SpriteFont>(@"font\Arial_12px");
        }

        public bool Active { get; set; }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                // reduce timer
                previousTimeSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                transperancy = 0.2f + previousTimeSec;

                if (transperancy > 1)
                    transperancy = 1;

                if (previousTimeSec <= 0)
                {
                    this.iteminfo = null;
                    this.Active = false;
                }
            }
        }

        public void Display(ItemClasses.Item item, string mtext)
        {
            this.iteminfo = item;
            this.midstring = mtext;
            previousTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds + 2.0f;
            this.Active = true;
        }

        public override void Draw(GameTime gameTime)
        {
            if (Active)
            {
                string message = this.iteminfo.itemName + " has been " + midstring + " to your inventory.";

                Vector2 size = spriteFont.MeasureString(message);

                Texture2D rect = new Texture2D(gfxdevice, (int)size.X, (int)size.Y);

                Color[] data = new Color[(int)size.X * (int)size.Y];
                for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
                rect.SetData(data);

                spriteBatch.Draw(rect, new Vector2((position.X + gfxdevice.Viewport.Width * 0.5f) - size.X * 0.5f, position.Y), (Color.White * 0.5f) * transperancy);

                spriteBatch.DrawString(spriteFont, message, new Vector2((position.X + gfxdevice.Viewport.Width * 0.5f) - size.X * 0.5f, position.Y), Color.White * transperancy);

            }
        }
    }
}
