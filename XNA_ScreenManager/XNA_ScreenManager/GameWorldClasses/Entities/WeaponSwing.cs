using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.CharacterClasses;
using XNA_ScreenManager.MapClasses;

namespace XNA_ScreenManager.GameWorldClasses.Entities
{

    public enum WeaponSwingType { Stab01, Swing01, Swing02, Swing03 };

    class WeaponSwing : XNA_ScreenManager.MapClasses.Effect
    {
        #region properties
        GameWorld world;

        WeaponSwingType swingtype;
        SpriteEffects sprite_effect;
        Vector2 spritesize = new Vector2(48, 48);
        Vector2 circleOrigin = Vector2.Zero;

        private float transperant = 0;
        private float angle = -3;
        bool settimer = false, hit = false;
        float keepAliveTimer = 0;

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

            switch (swingtype)
            {
                case WeaponSwingType.Stab01:
                    this.sprite = world.Content.Load<Texture2D>(@"gfx\effects\weapon\0.stabO2.1_0");
                    if (sprite_effect == SpriteEffects.FlipHorizontally)
                        this.position.X += 80;
                    break;
                case WeaponSwingType.Swing01:
                    this.sprite = world.Content.Load<Texture2D>(@"gfx\effects\weapon\0.swingT2.2_0");
                    if (sprite_effect == SpriteEffects.FlipHorizontally)
                    {
                        this.position.X -= 20;
                        this.angle = 3;
                    }
                    break;
                case WeaponSwingType.Swing02:
                    this.sprite = world.Content.Load<Texture2D>(@"gfx\effects\weapon\0.swingT3.2_0");
                    break;
                case WeaponSwingType.Swing03:
                    this.sprite = world.Content.Load<Texture2D>(@"gfx\effects\weapon\0.swingTF.3_0");
                    break;
            }

            this.spriteFrame = new Rectangle(0, 0, (int)sprite.Width, (int)sprite.Height);
            circleOrigin = new Vector2(SpriteFrame.Width * 0.5f, SpriteFrame.Height * 0.5f);
            this.KeepAliveTime = -1;
        }

        public override void Update(GameTime gameTime)
        {
            if (keepAliveTimer <= 0)
            {
                if (!settimer)
                {
                    settimer = true;
                    this.transperant = 1;
                    this.keepAliveTimer = (int)gameTime.TotalGameTime.TotalSeconds + 0.6f;
                }
                else
                {
                    // will remove the object
                    this.keepAliveTime = 0;
                }
            }

            // Remove ItemSprite Timer
            keepAliveTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Make the item slowly disapear
            if (transperant > 0)
                transperant -= (float)gameTime.ElapsedGameTime.TotalSeconds * 2;

            // Make it slowly rotate
            if (swingtype != WeaponSwingType.Stab01)
            {
                if (sprite_effect == SpriteEffects.None)
                    angle -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                else
                    angle += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // stabbing will not rotate
            if (swingtype == WeaponSwingType.Stab01)
                angle = 0;

            // check for monster collisions
            foreach (Entity monster in world.listEntity)
            {
                if (monster.EntityType == EntityType.Monster)
                {
                    if (new Rectangle((int)(monster.Position.X + monster.SpriteFrame.Width * 0.60f),
                        (int)monster.Position.Y,
                        (int)(monster.SpriteFrame.Width * 0.30f),
                        (int)monster.SpriteFrame.Height).
                    Intersects(new Rectangle(
                        (int)(Position.X - SpriteFrame.Width * 0.2f), (int)Position.Y,
                        (int)(SpriteFrame.Width * 1.2f), (int)SpriteFrame.Height)) == true && transperant > 0.8f && this.hit == false)
                    {
                        if (monster.State != EntityState.Hit && monster.State != EntityState.Died && monster.State != EntityState.Spawn)
                        {
                            monster.State = EntityState.Hit;
                            hit = true;
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
           spriteBatch.Draw(sprite, new Rectangle((int)Position.X, (int)Position.Y,
                    (int)(SpriteFrame.Width * 0.7f), (int)(SpriteFrame.Height* 0.7f)),
                    SpriteFrame, Color.White * transperant, angle, circleOrigin, sprite_effect, 0f);
        }
    }
}
