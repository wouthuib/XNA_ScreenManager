using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.ScreenClasses;
using Microsoft.Xna.Framework.Content;
using XNA_ScreenManager.ScreenClasses.MainClasses;

namespace XNA_ScreenManager.CharacterClasses
{
    class NPCharacter : Entity
    {
        #region properties
        // Global staic classes
        GameWorld world;
        ScreenManager screenmanager;
        SpriteFont spriteFont;

        // Keyboard State
        KeyboardState oldState , newState;

        // Sprite properties
        public int spriteWidth = 32;
        public int spriteHeight = 48;
        public int frames = 48;
        public Vector2 spriteOffset = Vector2.Zero;
        float previousTime = 0, transperancy = 0, previousAnimateTimeSec = 0;

        // Sprite Animation Properties
        public Vector2 Direction = Vector2.Zero;                                                    // Sprite Move direction
        const int NPC_SPEED = 80;                                                                   // The actual speed of the player
        const int NPC_WALK_TIME = 1;                                                                // The actual speed of the player
        const int ANIMATION_SPEED = 120;                                                            // Animation speed, 120 = default 
        #endregion

        public NPCharacter(Texture2D _Sprite, Vector2 _spriteoffset, Vector2 _spriteSize, 
            Vector2 _position, int _frames, Texture2D face, string name, string script)
            : base()
        {            
            spriteFont = ResourceManager.GetInstance.Content.Load<SpriteFont>(@"font\Arial_12px");

            // Derived properties
            Active = true;
            Sprite = _Sprite;
            SpriteSize = new Rectangle(0, 0, (int)_spriteSize.X, (int)_spriteSize.Y);
            Position = _position;
            frames = _frames;
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

        private void Animate(GameTime gameTime)
        {
            // reduce timer
            previousAnimateTimeSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (previousAnimateTimeSec <= 0 && frames > 1)
            {
                previousAnimateTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;

                spriteFrame.X += spriteWidth;
                if (spriteFrame.X >= (spriteWidth * frames))
                {
                    spriteFrame.X = 0; 
                    previousAnimateTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds + 3f;
                }  
            }
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

            // Animate NPC
            Animate(gameTime);

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

                previousTime = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.8f;
                transperancy = 0.8f;

                // Update keyboard states
                newState = Keyboard.GetState();

                // Check for Keyboard input
                if (CheckKey(Keys.Space) || CheckKey(Keys.Enter))
                {
                    screenmanager.messageScreen(true, new Rectangle((int)Position.X, (int)Position.Y, SpriteFrame.Width, SpriteFrame.Height), this.entityName, this.entityScript);
                }

                // Save keyboard states
                oldState = newState;
            }
            else
            {
                // no collision, remove NPC nametag
                if (previousTime > 0)
                {
                    previousTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    transperancy = previousTime;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                // draw NPC
                spriteBatch.Draw(sprite, new Rectangle((int)Position.X, (int)Position.Y, SpriteFrame.Width, SpriteFrame.Height),
                    SpriteFrame, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

                // draw NPC rectangle
                Texture2D rect = new Texture2D(ResourceManager.GetInstance.gfxdevice, 
                    (int)(spriteFont.MeasureString(this.entityName).X), (int)(spriteFont.MeasureString(this.entityName).Y));

                Color[] data = new Color[(int)(spriteFont.MeasureString(this.entityName).X) * (int)(spriteFont.MeasureString(this.entityName).Y)];
                for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
                rect.SetData(data);

                spriteBatch.Draw(rect, new Vector2((this.position.X + SpriteFrame.Width * 0.5f) - (spriteFont.MeasureString(this.entityName).X * 0.5f), position.Y - 10),
                    Color.White * transperancy);

                // NPC name
                spriteBatch.DrawString(spriteFont, this.entityName.ToString(),
                    new Vector2((this.position.X + SpriteFrame.Width * 0.5f) - (spriteFont.MeasureString(this.entityName).X * 0.5f), this.position.Y - 10),
                    Color.White * transperancy);
            }
        }

        private bool CheckKey(Keys theKey)
        {
            return oldState.IsKeyDown(theKey) && newState.IsKeyUp(theKey);
        }
    }
}
