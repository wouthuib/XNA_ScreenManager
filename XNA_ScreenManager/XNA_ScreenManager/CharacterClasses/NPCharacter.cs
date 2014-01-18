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

namespace XNA_ScreenManager.CharacterClasses
{
    class NPCharacter : Entity
    {
        // Sprite properties
        public int spriteWidth = 32;
        public int spriteHeight = 48;
        public Vector2 spriteOffset = Vector2.Zero;

        // Sprite Animation Properties
        public Vector2 Direction = Vector2.Zero;                                                    // Sprite Move direction
        const int NPC_SPEED = 80;                                                                   // The actual speed of the player
        const int NPC_WALK_TIME = 1;                                                                // The actual speed of the player
        const int ANIMATION_SPEED = 120;                                                            // Animation speed, 120 = default 


        public NPCharacter(Texture2D _Sprite, Vector2 _spriteoffset, Vector2 _spriteSize, 
            Vector2 _position, Texture2D face, string script)
            : base()
        {
            // Derived properties
            Active = true;
            Sprite = _Sprite;
            SpriteSize = new Rectangle(0, 0, (int)_spriteSize.X, (int)_spriteSize.Y);
            Position = _position;
            entityType = EntityType.NPC;
            entityFace = face;
            entityScript = script;

            // Local properties
            spriteOffset = _spriteoffset;
            SpriteFrame = new Rectangle((int)_spriteoffset.X, (int)_spriteoffset.Y, (int)_spriteSize.X, (int)_spriteSize.Y);
            spriteWidth = (int)_spriteSize.X;
            spriteHeight = (int)_spriteSize.Y;
        }

        public override void Update(GameTime gameTime)
        {
            // Apply Gravity 
            Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
                spriteBatch.Draw(sprite, new Rectangle((int)Position.X, (int)Position.Y, SpriteFrame.Width, SpriteFrame.Height),
                    SpriteFrame, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }
}
