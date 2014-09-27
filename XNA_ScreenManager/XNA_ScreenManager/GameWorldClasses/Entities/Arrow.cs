using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.CharacterClasses;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.ScreenClasses.MainClasses;
using XNA_ScreenManager.MonsterClasses;
using XNA_ScreenManager.GameWorldClasses.Effects;

namespace XNA_ScreenManager.PlayerClasses
{
    class Arrow : GameEffect
    {
        // Drawing properties
        private Vector2 spriteOfset = new Vector2(90, 0);
        private float Speed;
        private Vector2 Direction, Curving;

        public Arrow(Texture2D texture, Vector2 position, float speed, Vector2 direction, Vector2 curving)
            : base()
        {
            // Derived properties
            sprite = texture;
            SpriteFrame = new Rectangle(0, 0, sprite.Width, sprite.Height);
            Position = position;
            Speed = speed;
            Direction = direction;
            this.size = new Vector2(0.5f, 0.5f);
            this.Curving = curving;

            keepAliveTimer = (float)ResourceManager.GetInstance.
                            gameTime.TotalGameTime.Seconds + 0.48f;

            if (Direction.X >= 1)
                sprite_effect = SpriteEffects.FlipHorizontally;
            else
                sprite_effect = SpriteEffects.None;
        }

        public override void Update(GameTime gameTime)
        {
            // Arrow speed
            Position += (Direction + Curving) * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            CollisionCheck();

            // base Effect Update
            base.Update(gameTime);
        }

        private void CollisionCheck()
        {
            foreach(var entity in GameWorld.GetInstance.listEntity)
            {
                if (entity is MonsterSprite_old)
                {
                    if (new Rectangle((int)entity.Position.X + (int)(entity.SpriteFrame.Width * 0.30f),
                                          (int)entity.Position.Y,
                                          (int)entity.SpriteFrame.Width - (int)(entity.SpriteFrame.Width * 0.30f),
                                          (int)entity.SpriteFrame.Height).
                            Intersects(
                                new Rectangle((int)this.Position.X + (int)(this.SpriteFrame.Width * 0.45f),
                                    (int)this.Position.Y + (int)(this.SpriteFrame.Height * 0.45f),
                                    (int)this.SpriteFrame.Width - (int)(this.SpriteFrame.Width * 0.45f),
                                    (int)this.SpriteFrame.Height - (int)(this.SpriteFrame.Height * 0.45f))))
                    {
                        // make the monster suffer :-)
                        // and remove the arrow
                        if (entity.State != EntityState.Died &&
                            entity.State != EntityState.Spawn)
                        {
                            // monster hit
                            entity.State = EntityState.Hit;
                            this.keepAliveTimer = 0;

                            // Start damage controll
                            //int damage = (int)Battle.battle_calc_damage(PlayerStore.Instance.activePlayer, (MonsterSprite)entity, 1);
                            //entity.HP -= damage;

                            // create damage balloon
                            //GameWorld.GetInstance.newEffect.Add(new DamageBaloon(
                            //        ResourceManager.GetInstance.Content.Load<Texture2D>(@"gfx\effects\damage_counter1"),
                            //        new Vector2((entity.Position.X + entity.SpriteFrame.Width * 0.45f) - damage.ToString().Length * 5,
                            //        entity.Position.Y + entity.SpriteFrame.Height * 0.20f), damage));
                        }
                    }
                }
            }
        }
    }
}
