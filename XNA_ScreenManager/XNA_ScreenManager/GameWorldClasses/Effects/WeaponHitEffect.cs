using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA_ScreenManager.ScreenClasses.MainClasses;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.MapClasses;
using Microsoft.Xna.Framework;

namespace XNA_ScreenManager.GameWorldClasses.Effects
{
    class WeaponHitEffect : GameEffect
    {
        private string spritepath;
        private int frames = 0, frame = 0;
        private float previousGameTimeSec = 0;

        public WeaponHitEffect(string getspritepath, Vector2 getpos, int getframes)
            : base()
        {
            this.spritepath = getspritepath;
            this.Position = getpos;
            this.frames = getframes;
            this.keepAliveTimer = 5;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            // reduce timer
            previousGameTimeSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (previousGameTimeSec < 0)
            {
                previousGameTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;

                if (frame < frames)
                {
                    this.sprite = ResourceManager.GetInstance.Content.Load<Texture2D>(spritepath + frame.ToString());
                    this.SpriteFrame = new Rectangle(0, 0, sprite.Width, sprite.Height);
                }
                else
                    keepAliveTimer = 0;
                
                frame++;
            }
        }
    }
}
