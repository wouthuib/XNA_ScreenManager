using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA_ScreenManager.ScreenClasses.MainClasses;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.MapClasses;
using Microsoft.Xna.Framework;
using XNA_ScreenManager.PlayerClasses;
using XNA_ScreenManager.MonsterClasses;

namespace XNA_ScreenManager.GameWorldClasses.Effects
{
    public class CastEffect : GameEffect
    {
        string Path, lockPlayer, lockInstance;
        int FrameCount;
        float previousGameTimeSec = 0, frame = 0;
        Vector2 Offset;

        public CastEffect(
            string path, 
            int framecount, 
            Vector2 offset, 
            Vector2 position, 
            string lockplayer = null,
            string lockinstance = null)
            : base()
        {
            this.Offset = offset;
            this.Path = path;
            this.FrameCount = framecount;
            this.Position = position;
            this.lockPlayer = lockplayer;
            this.lockInstance = lockinstance;
            this.sprite = ResourceManager.GetInstance.Content.Load<Texture2D>(this.Path + frame.ToString());
            this.SpriteFrame = new Rectangle((int)0, (int)0, (int)sprite.Width, (int)sprite.Height);
            this.keepAliveTimer = (framecount + 1) * 0.10f; // 21 frames
            this.size = new Vector2(0.75f, 0.75f);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            // lock effect on player position
            if (lockPlayer != null)
            {
                if (lockPlayer == GameWorld.GetInstance.playerSprite.PlayerName)
                    this.Position = new Vector2(GameWorld.GetInstance.playerSprite.Position.X + Offset.X,
                                                GameWorld.GetInstance.playerSprite.Position.Y + Offset.Y);
                else
                {
                    foreach (NetworkPlayerSprite player in GameWorld.GetInstance.listEntity)
                    {
                        if (player.Name == lockPlayer)
                        {
                            this.Position = new Vector2(player.Position.X + Offset.X,
                                                        player.Position.Y + Offset.Y);
                            break;
                        }
                    }
                }
            }

            // lock effect on monster position
            if (lockInstance != null)
            {
                foreach (NetworkMonsterSprite monster in GameWorld.GetInstance.listEntity)
                {
                    if (monster.InstanceID == Guid.Parse(lockInstance))
                    {
                        this.Position = new Vector2(monster.Position.X + Offset.X,
                                                    monster.Position.Y + Offset.Y);
                        break;
                    }
                }
            }

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
