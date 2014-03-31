using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.ScreenClasses;

namespace XNA_ScreenManager.CharacterClasses
{
    class NPCharacter : Entity
    {
        #region properties
        // Global staic classes
        GameWorld world;
        ScreenManager screenmanager;

        // Keyboard State
        KeyboardState keyboardStateCurrent, keyboardStatePrevious;

        // Sprite properties
        public int spriteWidth = 32;
        public int spriteHeight = 48;
        public Vector2 spriteOffset = Vector2.Zero;

        // Sprite Animation Properties
        public Vector2 Direction = Vector2.Zero;                                                    // Sprite Move direction
        const int NPC_SPEED = 80;                                                                   // The actual speed of the player
        const int NPC_WALK_TIME = 1;                                                                // The actual speed of the player
        const int ANIMATION_SPEED = 120;                                                            // Animation speed, 120 = default 
        #endregion

        public NPCharacter(Texture2D _Sprite, Vector2 _spriteoffset, Vector2 _spriteSize, 
            Vector2 _position, Texture2D face, string name, string script)
            : base()
        {
            // Derived properties
            Active = true;
            Sprite = _Sprite;
            SpriteSize = new Rectangle(0, 0, (int)_spriteSize.X, (int)_spriteSize.Y);
            Position = _position;
            entityType = EntityType.NPC;
            entityFace = face;
            entityName = name;
            entityScript = script;

            // Local properties
            spriteOffset = _spriteoffset;
            SpriteFrame = new Rectangle((int)_spriteoffset.X, (int)_spriteoffset.Y, (int)_spriteSize.X, (int)_spriteSize.Y);
            spriteWidth = (int)_spriteSize.X;
            spriteHeight = (int)_spriteSize.Y;
        }

        public override void Update(GameTime gameTime)
        {
            // Connect Global instances
            if (screenmanager == null)
                screenmanager = ScreenManager.Instance;

            if (world == null)
                world = GameWorld.GetInstance;

            // Apply Gravity 
            Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // create NPC rectangle
            Rectangle NPC = new Rectangle((int)this.Position.X, (int)this.Position.Y,
                                               this.SpriteFrame.Width, this.SpriteFrame.Height);

            // check intersection with player
            if (NPC.Intersects
                (new Rectangle((int)world.playerSprite.Position.X + (int)(world.playerSprite.SpriteFrame.Width * 0.25f),
                    (int)world.playerSprite.Position.Y,
                    (int)world.playerSprite.SpriteFrame.Width - (int)(world.playerSprite.SpriteFrame.Width * 0.25f),
                    (int)world.playerSprite.SpriteFrame.Height)
                ))
            {
                // set player state
                world.playerSprite.CollideNPC = true;

                // Update keyboard states
                keyboardStateCurrent = Keyboard.GetState();

                // Check for Keyboard input
                if (keyboardStateCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Space) == true &&
                    keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space) == true &&
                    world.playerSprite.State == EntityState.Stand)
                {
                    screenmanager.messageScreen(true, new Rectangle((int)Position.X, (int)Position.Y, SpriteFrame.Width, SpriteFrame.Height), this.entityName, this.entityScript);
                }

                // Save keyboard states
                keyboardStatePrevious = keyboardStateCurrent;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
                spriteBatch.Draw(sprite, new Rectangle((int)Position.X, (int)Position.Y, SpriteFrame.Width, SpriteFrame.Height),
                    SpriteFrame, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }
}
