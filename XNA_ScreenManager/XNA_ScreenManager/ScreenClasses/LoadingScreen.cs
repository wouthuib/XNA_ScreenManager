using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XNA_ScreenManager.ScreenClasses
{
    public class LoadingScreen : GameScreen
    {
        ContentManager Content;
        GraphicsDevice gfxdevice;
        SpriteBatch spriteBatch;
        ScreenManager screenmanager = ScreenManager.Instance;

        Texture2D LoadingPicture;
        int previousEffectTimeSec, previousEffectTimeMin;
        bool start = true;

        public LoadingScreen(Game game)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));
            gfxdevice = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));

            LoadingPicture = Content.Load<Texture2D>(@"gfx\loadings\0_" + new Random().Next(0, 1).ToString());
        }

        public override void Update(GameTime gameTime)
        {
            if (previousEffectTimeSec <= (int)gameTime.TotalGameTime.Seconds
                || previousEffectTimeMin != (int)gameTime.TotalGameTime.Minutes)
            {
                if (start)
                {
                    previousEffectTimeSec = (int)gameTime.TotalGameTime.Seconds + 5;
                    previousEffectTimeMin = (int)gameTime.TotalGameTime.Minutes;
                    start = false;
                }
                else
                {
                    start = true;
                    screenmanager.setScreen("actionScreen");
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw the base first
            base.Draw(gameTime);

            spriteBatch.Draw(LoadingPicture, new Rectangle(0, 0, gfxdevice.Viewport.Width, gfxdevice.Viewport.Height), Color.White);
        }
    }
}
