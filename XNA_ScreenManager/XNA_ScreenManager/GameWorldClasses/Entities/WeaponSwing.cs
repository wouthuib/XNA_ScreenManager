using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.CharacterClasses;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.ScreenClasses.MainClasses;

namespace XNA_ScreenManager.GameWorldClasses.Entities
{

    public enum WeaponSwingType { Stab01, Swing01, Swing02, Swing03 };

    class WeaponSwing : XNA_ScreenManager.MapClasses.Effect
    {
        #region properties
        GameWorld world;

        WeaponSwingType swingtype;
        Vector2 spritesize = new Vector2(48, 48);

        #endregion

        public WeaponSwing(Vector2 position, WeaponSwingType gettype, SpriteEffects spreffect) :
            base()
        {
            // Link properties to instance
            this.world = GameWorld.GetInstance;

            // general properties
            this.swingtype = gettype;
            this.position = position;
            this.sprite_effect = spreffect;
            this.angle = -3;

            switch (swingtype)
            {
                case WeaponSwingType.Stab01:
                    this.sprite = world.Content.Load<Texture2D>(@"gfx\effects\weapon\0.stabO2.1_0");
                    if (sprite_effect == SpriteEffects.FlipHorizontally)
                        this.position.X += 80;
                    keepAliveTimer = (float)ResourceManager.GetInstance.
                                gameTime.TotalGameTime.Seconds + 0.12f;
                    break;
                case WeaponSwingType.Swing01:
                    this.sprite = world.Content.Load<Texture2D>(@"gfx\effects\weapon\0.swingT2.2_0");
                    if (sprite_effect == SpriteEffects.FlipHorizontally)
                        this.angle = -11.5f;
                    else
                        this.angle = -13.6f;
                    keepAliveTimer = (float)ResourceManager.GetInstance.
                                gameTime.TotalGameTime.Seconds + 0.06f;
                    break;
                case WeaponSwingType.Swing02:
                    this.sprite = world.Content.Load<Texture2D>(@"gfx\effects\weapon\0.swingT3.2_0");
                    break;
                case WeaponSwingType.Swing03:
                    this.sprite = world.Content.Load<Texture2D>(@"gfx\effects\weapon\0.swingTF.3_0");
                    break;
            }

            this.spriteFrame = new Rectangle(0, 0, (int)sprite.Width, (int)sprite.Height);
            this.origin = new Vector2(SpriteFrame.Width * 0.5f, SpriteFrame.Height * 0.5f);
        }

        public override void Update(GameTime gameTime)
        {
            // Make the item slowly disapear
            if (transperant > 0)
                transperant -= (float)gameTime.ElapsedGameTime.TotalSeconds * 3;

            // Make it slowly rotate
            if (swingtype == WeaponSwingType.Swing01)
            {
                if (sprite_effect == SpriteEffects.None)
                {
                    angle -= (float)gameTime.ElapsedGameTime.TotalSeconds * 15f;
                    position.X += (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.35f;
                }
                else
                {
                    angle += (float)gameTime.ElapsedGameTime.TotalSeconds * 15f;
                    position.X -= (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.35f;
                }
            }

            // stabbing will not rotate
            if (swingtype == WeaponSwingType.Stab01)
                angle = 0;

            // base Effect Update
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
           spriteBatch.Draw(sprite, new Rectangle((int)Position.X, (int)Position.Y,
                    (int)(SpriteFrame.Width * 0.7f), (int)(SpriteFrame.Height* 0.7f)),
                    SpriteFrame, Color.White * transperant, angle, origin, sprite_effect, 0f);
        }
    }
}
