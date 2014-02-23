using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.CharacterClasses;

namespace XNA_ScreenManager.PlayerClasses
{
    class Arrow : Entity
    {
        // Drawing properties
        private Vector2 spriteOfset = new Vector2(90, 0);
        private SpriteEffects spriteEffect;
        private float Speed;
        private Vector2 Direction;
        private bool Activate = false;

        public Arrow(Texture2D texture, Vector2 position, float speed, Vector2 direction)
            : base()
        {
            // Derived properties
            Active = true;
            sprite = texture;
            SpriteFrame = new Rectangle(0, 0, sprite.Width, sprite.Height);
            Position = position;
            OldPosition = position;
            Speed = speed;
            Direction = direction;
            entityType = EntityType.Arrow;

            if (Direction.X >= 1)
                spriteEffect = SpriteEffects.FlipHorizontally;
            else
                spriteEffect = SpriteEffects.None;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Activate)
            {
                KeepAliveTime = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.48f;
                Activate = true;
            }

            // Remove Arrow Timer
            if (KeepAliveTime > 0)
                KeepAliveTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
                KeepAliveTime = 0;

            // Move the Arrow
            OldPosition = Position;

            // Arrow speed
            Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
                spriteBatch.Draw(sprite, new Rectangle((int)Position.X, (int)Position.Y, 
                    (int)(SpriteFrame.Width * 0.5f), (int)(SpriteFrame.Height * 0.5f)),
                    SpriteFrame, Color.White, 0f, Vector2.Zero, spriteEffect, 0f);
        }
    }
}
