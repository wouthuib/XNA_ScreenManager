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

        Texture2D LoadingPicture; //, LoadingCircle;
        Rectangle spriteFrame = new Rectangle(0, 0, 102, 102);
        int FramePositionX = 0;
        float previousEffectTimeSec;

        public LoadingScreen(Game game)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));
            gfxdevice = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));

            //LoadingCircle = Content.Load<Texture2D>(@"gfx\screens\screenobjects\LoadingCircle");
            LoadingPicture = Content.Load<Texture2D>(@"gfx\loadings\0_" + new Random().Next(0, 1).ToString());
        }

        public override void Update(GameTime gameTime)
        {
            // reduce timer
            previousEffectTimeSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // set sprite frames
            if (previousEffectTimeSec < 0)
            {
                previousEffectTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
                
                // actions??
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw the base first
            base.Draw(gameTime);

            spriteBatch.Draw(LoadingPicture, new Rectangle(
                0, 0, 
                gfxdevice.Viewport.Width, 
                gfxdevice.Viewport.Height), 
                Color.White);
        }

        public int SelectedFrame
        {
            get { return FramePositionX; }
            set
            {
                FramePositionX = value;

                if (FramePositionX > 816)
                    FramePositionX = 0;
            }
        }
    }
}
