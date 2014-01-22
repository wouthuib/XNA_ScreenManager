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
        //Vector2 spriteSize = new Vector2(50, 50);
        int previousDmgTimeMSec, previousDmgTimeSec;
        bool settimer = false;

        public DamageBaloon(Texture2D getsprite, SpriteFont getspriteFont, Vector2 getposition, int getdamage)
            : base()
        {
            damagefont = getspriteFont;
            sprite = getsprite;
            damagefont = getspriteFont;
            position = getposition;
            damage = getdamage;
            spriteFrame = new Rectangle((int)position.X, (int)position.Y, (int)spriteSize.X, (int)spriteSize.Y);
        }

        public override void Update(GameTime gameTime)
        {
            if (previousDmgTimeMSec <= (int)gameTime.TotalGameTime.Milliseconds
                || previousDmgTimeSec != (int)gameTime.TotalGameTime.Seconds)
            {
                if (!settimer)
                {
                    settimer = true;
                    previousDmgTimeMSec = (int)gameTime.TotalGameTime.Milliseconds + 700;
                    previousDmgTimeSec = (int)gameTime.TotalGameTime.Seconds;
                }
                else
                {
                    keepAliveTime = (int)gameTime.TotalGameTime.Seconds;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int[] dmg = new int[damage.ToString().Length];
            string dmgtxt = null;

            for (int i = 1; i <= dmg.Length; i++)
            {
                if (dmgtxt != null || getDamageCount(damage, i) >= 1)
                    dmgtxt = dmgtxt + getDamageCount(damage, i).ToString();
            }

            if (damage > 0)
                spriteBatch.DrawString(damagefont, dmgtxt,
                    new Vector2(position.X + spriteFrame.Width * 0.45f - dmgtxt.Length, position.Y + spriteFrame.Height * 0.2f),
                        Color.White);
            else
                spriteBatch.DrawString(damagefont, "MISS",
                    new Vector2(position.X + spriteFrame.Width * 0.35f, position.Y + spriteFrame.Height * 0.2f),
                         Color.White);

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
