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
    public class CastEffect : GameEffect
    {
        string Path;
        int FrameCount;
        float previousGameTimeSec = 0, frame = 0;
        Vector2 Offset;

        public CastEffect(string path, int framecount, Vector2 offset)
            : base()
        {
            this.Offset = offset;
            this.Path = path;
            this.FrameCount = framecount;
            this.sprite = ResourceManager.GetInstance.Content.Load<Texture2D>(this.Path + frame.ToString());
            this.SpriteFrame = new Rectangle((int)0, (int)0, (int)sprite.Width, (int)sprite.Height);
            this.keepAliveTimer = 21 * 0.10f; // 21 frames
            this.size = new Vector2(0.75f, 0.75f);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            this.Position = new Vector2(GameWorld.GetInstance.playerSprite.Position.X + Offset.X,
                                        GameWorld.GetInstance.playerSprite.Position.Y + Offset.Y);

            // reduce timer
            previousGameTimeSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (previousGameTimeSec < 0)
            {
                previousGameTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
                frame++;

                if (frame <= FrameCount)
                    this.sprite = ResourceManager.GetInstance.Content.Load<Texture2D>(this.Path + frame.ToString());
            }
        }
    }
}
