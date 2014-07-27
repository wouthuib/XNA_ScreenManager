using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using XNA_ScreenManager.GameWorldClasses.Effects;

namespace XNA_ScreenManager.MapClasses
{
    public class DamageBaloon : GameEffect
    {
        int damage = 0;
        float transperancy = 1;
        public Vector2 Velocity = new Vector2(0, 1);
        int previousDmgTimeMSec = 0, previousDmgTimeSec = 0;

        public DamageBaloon(Texture2D getsprite, Vector2 getposition, int getdamage)
            : base()
        {
            sprite = getsprite;
            position = getposition;
            damage = getdamage;
            spriteSize = new Vector2(40, 40);
            spriteFrame = new Rectangle((int)position.X, (int)position.Y, (int)spriteSize.X, (int)spriteSize.Y);
        }

        public override void Update(GameTime gameTime)
        {
            if (previousDmgTimeMSec <= (int)gameTime.TotalGameTime.Milliseconds
                || previousDmgTimeSec < (int)gameTime.TotalGameTime.Seconds - 1)
            {
                if (!settimer)
                {
                    settimer = true;
                    Velocity = new Vector2(0, -1.6f);
                    transperancy = 1;

                    previousDmgTimeMSec = (int)gameTime.TotalGameTime.Milliseconds + 700;
                    previousDmgTimeSec = (int)gameTime.TotalGameTime.Seconds;
                }
                else
                {
                    // will remove the object
                    KeepAliveTimer = 0;
                }
            }

            // Update Color transperancy
            transperancy -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Reduce velocity
            Velocity.Y += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply jumping
            if (Velocity.Y < -1.2f)
                Position += Velocity * 150 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply Gravity 
            Position += new Vector2(0, 1) * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int[] dmg = new int[damage.ToString().Length];

            if (damage > 0)
            {
                for (int i = dmg.Length; i >= 1 ; i--)
                {
                    Rectangle destin = new Rectangle(
                                          (int)((position.X - (dmg.Length * 8)) + (i * 20)),
                                          (int)(position.Y + spriteFrame.Height * 0.2f),
                                          (int)(spriteFrame.Width * 0.75f),
                                          (int)(spriteFrame.Height * 0.75f));

                    Rectangle source = new Rectangle(
                                          (int)(getDamageCount(damage, i) * spriteFrame.Width), 0,
                                          (int)spriteFrame.Width,
                                          (int)spriteFrame.Height);

                    spriteBatch.Draw(sprite, destin, source,
                        Color.White * transperancy, 0f, Vector2.Zero, SpriteEffects.None, 0);
                }
            }
            else
                spriteBatch.Draw(sprite, new Rectangle((int)(position.X - spriteFrame.Width * 0.8f),
                                                       (int)(position.Y + spriteFrame.Height * 0.2f),
                                                       (int)(spriteFrame.Width * 2f),
                                                       (int)(spriteFrame.Height * 0.75f)),
                                         new Rectangle((int) 0, (int) 40, 
                                                       (int)(spriteFrame.Width * 3), 
                                                       (int)spriteFrame.Height),
                        Color.White * transperancy, 0f, Vector2.Zero, SpriteEffects.None, 0);

            base.Draw(spriteBatch);
        }

        private int getDamageCount(int dmgvalue, int charcount)
        {
            if (charcount <= dmgvalue.ToString().Length)
            {
                char[] arr = dmgvalue.ToString().ToCharArray(charcount - 1, 1);
                return Convert.ToInt32(new string(arr));
            }
            else
                return 0;
        }
    }
}
