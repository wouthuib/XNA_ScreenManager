using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNA_ScreenManager.MapClasses
{
    public class DamageBaloon : Effect
    {
        SpriteFont damagefont;
        int damage = 0;
        float transperancy = 1;
        public Vector2 Velocity = new Vector2(0, 1);
        int previousDmgTimeMSec = 0, previousDmgTimeSec = 0;
        bool settimer = false;

        public DamageBaloon(Texture2D getsprite, SpriteFont getspriteFont, Vector2 getposition, int getdamage)
            : base()
        {
            damagefont = getspriteFont;
            sprite = getsprite;
            damagefont = getspriteFont;
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
                    keepAliveTime = 0;
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

            base.Update(gameTime);
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
                spriteBatch.DrawString(damagefont, "MISS",
                    new Vector2(position.X + spriteFrame.Width * 0.10f, position.Y + spriteFrame.Height * 0.2f),
                         Color.White * transperancy);

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
