using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XNA_ScreenManager.CharacterClasses;
using XNA_ScreenManager.MapClasses;

namespace XNA_ScreenManager.PlayerClasses
{
    class Arrow : Entity
    {
        // Drawing properties
        private Vector2 spriteOfset = new Vector2(90, 0);
        private SpriteEffects spriteEffect;
        private float Speed;
        private Vector2 Direction;

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

            if (Direction.X >= 1)
                spriteEffect = SpriteEffects.FlipHorizontally;
            else
                spriteEffect = SpriteEffects.None;
        }

        public override void Update(GameTime gameTime)
        {
            // Move the Monster
            OldPosition = Position;

            // Walk speed
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
