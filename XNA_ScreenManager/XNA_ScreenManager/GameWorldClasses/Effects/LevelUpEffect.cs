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
    public class LevelUpEffect : XNA_ScreenManager.MapClasses.Effect
    {
        float previousGameTimeSec = 0, frame = 0;

        public LevelUpEffect()
            : base()
        {
            this.sprite = ResourceManager.GetInstance.Content.Load<Texture2D>(@"gfx\effects\LevelUp\LevelUp_" + frame.ToString());
            this.SpriteFrame = new Rectangle((int)0, (int)0, (int)sprite.Width, (int)sprite.Height);
            this.keepAliveTimer = 21 * 0.10f; // 21 frames
            this.size = new Vector2(0.75f, 0.75f);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            this.Position = new Vector2(GameWorld.GetInstance.playerSprite.Position.X - 76,
                                        GameWorld.GetInstance.playerSprite.Position.Y - 170);

            // reduce timer
            previousGameTimeSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (previousGameTimeSec < 0)
            {
                previousGameTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
                frame++;

                if (frame < 21)
                    this.sprite = ResourceManager.GetInstance.Content.Load<Texture2D>(@"gfx\effects\LevelUp\LevelUp_" + frame.ToString());
            }
        }
    }
}
