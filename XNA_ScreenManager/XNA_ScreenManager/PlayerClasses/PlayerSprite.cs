using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNA_ScreenManager.CharacterClasses;
using XNA_ScreenManager.ItemClasses;
using XNA_ScreenManager.ScriptClasses;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.PlayerClasses;

namespace XNA_ScreenManager
{
    public class PlayerSprite : Entity
    {
        #region properties
        // The Gameworld
        GameWorld world;

        // Keyboard- and Mousestate
        KeyboardState keyboardStateCurrent, keyboardStatePrevious;

        // Player inventory
        Inventory inventory = Inventory.Instance;
        ItemStore itemStore = ItemStore.Instance;
        ScriptInterpreter scriptManager = ScriptInterpreter.Instance;
        PlayerInfo playerinfo = PlayerInfo.Instance;

        // Player properties
        public Rectangle spriteScale;
        public int spriteWidth = 90;
        public int spriteHeight = 90;
        public Vector2 spriteOfset = new Vector2(90,0);
        private SpriteEffects spriteEffect = SpriteEffects.None;

        // Sprite Animation Properties
        public int effectCounter = 0;                                                               // for the warp effect
        Color color = Color.White;                                                                  // sprite color
        public Vector2 Direction = Vector2.Zero;                                                    // Sprite Move direction
        public float Speed;                                                                         // Speed used in functions
        public Vector2 Velocity = new Vector2(0,1);                                                 // speed used in jump
        const int PLAYER_SPEED = 200;                                                               // The actual speed of the player
        const int ANIMATION_SPEED = 120;                                                            // Animation speed, 120 = default 
        const int MOVE_UP = -1;                                                                     // player moving directions
        const int MOVE_DOWN = 1;                                                                    // player moving directions
        const int MOVE_LEFT = -1;                                                                   // player moving directions
        const int MOVE_RIGHT = 1;                                                                   // player moving directions
        int previousGameTimeMsec,                                                                   // GameTime in Miliseconds
            previousGameTimeSec;                                                                    // GameTime in Seconds
        int previousEffectTimeSec,                                                                  // GameTime in Miliseconds
            previousEffectTimeMin;                                                                  // GameTime in Seconds
        bool landed;                                                                                // land switch, arrow switch

        //map properties
        private Vector2 TileSize = Vector2.Zero;
        #endregion

        public PlayerSprite(Texture2D _Sprite, int _X, int _Y, Vector2 _tileSize)
            : base()
        {
            // Derived properties
            Active = true;
            Sprite = _Sprite;
            SpriteFrame = new Rectangle((int)spriteOfset.X, (int)spriteOfset.Y, spriteWidth, spriteHeight);
            SpriteSize = new Rectangle(0, 0, spriteWidth, spriteHeight);
            Position = new Vector2(_X, _Y);
            OldPosition = new Vector2(_X, _Y);

            // temporary parameters these should eventually be imported from the Monster Database
            playerinfo.Health = 150;
            playerinfo.Strength = 15;
            playerinfo.Dexterity = 15;
            playerinfo.Luck = 10;
            playerinfo.Agility = 15;
            playerinfo.Level = 10;
            
            // Local properties
            TileSize = _tileSize;
            spriteScale = new Rectangle(                                                            // Obsolete but can be used for events
                Convert.ToInt32(Position.X),
                Convert.ToInt32(Position.Y),
                spriteWidth, spriteHeight);
            Direction = new Vector2();                                                              // Move direction
            state = EntityState.Stand;                                                              // Player state
        }

        public override void Update(GameTime gameTime)
        {
            keyboardStateCurrent = Keyboard.GetState();

            if (Active)
            {
                switch (state)
                {
                    #region state shooting
                    case EntityState.Shoot:

                        Speed = 0;
                        Direction = Vector2.Zero;
                        Velocity = Vector2.Zero;

                        // Move the Character
                        OldPosition = Position;

                        // player animation
                        spriteOfset = new Vector2(spriteFrame.Width * 0, spriteFrame.Height * 3);
                        spriteFrame.Y = (int)spriteOfset.Y;

                        if (previousGameTimeMsec <= (int)gameTime.TotalGameTime.Milliseconds
                            || previousGameTimeSec != (int)gameTime.TotalGameTime.Seconds)
                        {

                            previousGameTimeMsec = (int)gameTime.TotalGameTime.Milliseconds + 5;
                            previousGameTimeSec = (int)gameTime.TotalGameTime.Seconds;

                            spriteFrame.X += spriteWidth;

                            if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
                            {
                                // Later = charge arrow skill
                                if (spriteFrame.X > spriteOfset.X + (spriteWidth * 1))
                                    spriteFrame.X = (int)spriteOfset.X + spriteWidth;
                            }
                            else
                            {
                                if (spriteFrame.X > spriteOfset.X + (spriteWidth * 2))
                                {
                                    previousGameTimeMsec = (int)gameTime.TotalGameTime.Milliseconds;
                                    previousGameTimeSec = (int)gameTime.TotalGameTime.Seconds;

                                    // make sure the world is connected
                                    if (world == null)
                                        world = GameWorld.GetInstance;

                                    // create and release an arrow
                                    if(spriteEffect == SpriteEffects.FlipHorizontally)
                                        world.createArrow(new Vector2(this.Position.X, this.Position.Y + this.spriteFrame.Height * 0.6f), 800, new Vector2(1, 0));
                                    else
                                        world.createArrow(new Vector2(this.Position.X, this.Position.Y + this.spriteFrame.Height * 0.6f), 800, new Vector2(-1, 0));

                                    // reset sprite frame and change state
                                    spriteFrame.X = 0;
                                    state = EntityState.Stand;
                                }
                            }
                        }

                        // Apply Gravity 
                        Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        break;
                    #endregion
                    #region state sit
                    case EntityState.Sit:

                        Speed = 0;
                        Direction = Vector2.Zero;
                        Velocity = Vector2.Zero;
                        
                        if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
                        {
                            state = EntityState.Stand;
                        }
                        else if (keyboardStateCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Up) &&
                                 keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                        {
                            state = EntityState.Stand;
                        }
                        else if (keyboardStateCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Insert) &&
                                 keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Insert))
                        {
                            state = EntityState.Stand;
                        }

                        // Move the Character
                        OldPosition = Position;

                        // player sprite jump
                        spriteOfset = new Vector2(spriteFrame.Width * 4, spriteFrame.Height * 2);
                        spriteFrame = new Rectangle((int)spriteOfset.X, (int)spriteOfset.Y, spriteFrame.Width, spriteFrame.Height);

                        // Apply Gravity 
                        Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        break;
                    #endregion
                    #region state Rope
                    case EntityState.Rope:

                        Speed = 0;
                        Direction = Vector2.Zero;
                        Velocity = Vector2.Zero;
                        spriteEffect = SpriteEffects.None;

                        if(this.collideRope == false)
                            this.state = EntityState.Falling;

                        if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.Y = MOVE_DOWN;
                            this.Speed = PLAYER_SPEED * 0.75f;

                            //player animation
                            spriteOfset = new Vector2(spriteFrame.Width * 0, spriteFrame.Height * 2);
                            spriteFrame.Y = (int)spriteOfset.Y;

                            if (previousGameTimeMsec <= (int)gameTime.TotalGameTime.Milliseconds
                            || previousGameTimeSec != (int)gameTime.TotalGameTime.Seconds)
                            {
                                previousGameTimeMsec = (int)gameTime.TotalGameTime.Milliseconds + ANIMATION_SPEED;
                                previousGameTimeSec = (int)gameTime.TotalGameTime.Seconds;
                                spriteFrame.X += spriteWidth;
                                if (spriteFrame.X > spriteWidth * 1)
                                    spriteFrame.X = (int)spriteOfset.X;
                            }
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.Y = MOVE_UP;
                            this.Speed = PLAYER_SPEED * 0.75f;

                            //player animation
                            spriteOfset = new Vector2(spriteFrame.Width * 0, spriteFrame.Height * 2);
                            spriteFrame.Y = (int)spriteOfset.Y;

                            if (previousGameTimeMsec <= (int)gameTime.TotalGameTime.Milliseconds
                            || previousGameTimeSec != (int)gameTime.TotalGameTime.Seconds)
                            {
                                previousGameTimeMsec = (int)gameTime.TotalGameTime.Milliseconds + ANIMATION_SPEED;
                                previousGameTimeSec = (int)gameTime.TotalGameTime.Seconds;
                                spriteFrame.X += spriteWidth;
                                if (spriteFrame.X > spriteWidth * 1)
                                    spriteFrame.X = (int)spriteOfset.X;
                            }
                        }

                        // Move the Character
                        OldPosition = Position;

                        // Climb speed
                        Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        break;
                    #endregion
                    #region state Ladder
                    case EntityState.Ladder:

                        Speed = 0;
                        Direction = Vector2.Zero;
                        Velocity = Vector2.Zero;
                        spriteEffect = SpriteEffects.None;

                        if (this.collideLadder == false)
                            this.state = EntityState.Falling;

                        if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.Y = MOVE_DOWN;
                            this.Speed = PLAYER_SPEED * 0.75f;

                            //player animation
                            spriteOfset = new Vector2(spriteFrame.Width * 2, spriteFrame.Height * 2);
                            spriteFrame.Y = (int)spriteOfset.Y;

                            if (previousGameTimeMsec <= (int)gameTime.TotalGameTime.Milliseconds
                            || previousGameTimeSec != (int)gameTime.TotalGameTime.Seconds)
                            {
                                previousGameTimeMsec = (int)gameTime.TotalGameTime.Milliseconds + ANIMATION_SPEED;
                                previousGameTimeSec = (int)gameTime.TotalGameTime.Seconds;
                                spriteFrame.X += spriteWidth;
                                if (spriteFrame.X > spriteOfset.X + (spriteWidth * 1))
                                    spriteFrame.X = (int)spriteOfset.X;
                            }
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.Y = MOVE_UP;
                            this.Speed = PLAYER_SPEED * 0.75f;

                            //player animation
                            spriteOfset = new Vector2(spriteFrame.Width * 2, spriteFrame.Height * 2);
                            spriteFrame.Y = (int)spriteOfset.Y;

                            if (previousGameTimeMsec <= (int)gameTime.TotalGameTime.Milliseconds
                            || previousGameTimeSec != (int)gameTime.TotalGameTime.Seconds)
                            {
                                previousGameTimeMsec = (int)gameTime.TotalGameTime.Milliseconds + ANIMATION_SPEED;
                                previousGameTimeSec = (int)gameTime.TotalGameTime.Seconds;
                                spriteFrame.X += spriteWidth;
                                if (spriteFrame.X > spriteOfset.X + (spriteWidth * 1))
                                    spriteFrame.X = (int)spriteOfset.X;
                            }
                        }

                        // Move the Character
                        OldPosition = Position;

                        // Climb speed
                        Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        break;
                    #endregion
                    #region state stand
                    case EntityState.Stand:

                        Speed = 0;
                        Direction = Vector2.Zero;
                        Velocity = Vector2.Zero;

                        if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                        {
                            state = EntityState.Walk;
                            spriteEffect = SpriteEffects.FlipHorizontally;
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                        {
                            state = EntityState.Walk;
                            spriteEffect = SpriteEffects.None;
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
                        {
                            if (!collideNPC)
                            {
                                Velocity += new Vector2(0, -1.6f); // Add an upward impulse
                                state = EntityState.Jump;
                            }
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Insert))
                        {
                                state = EntityState.Sit;
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                        {
                            if (this.collideLadder)
                                state = EntityState.Ladder;
                            else if (this.collideRope)
                                state = EntityState.Rope;
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
                        {
                            // check player jobclass (if archer or wizard)
                            previousGameTimeMsec = (int)gameTime.TotalGameTime.Milliseconds + (350 - playerinfo.BaseASPD * 12);
                            previousGameTimeSec = (int)gameTime.TotalGameTime.Seconds;
                            spriteFrame.X = 0;
                            state = EntityState.Shoot;
                        }

                        // Check if player is steady standing
                        if (Position.Y > OldPosition.Y)
                            state = EntityState.Falling;

                        // Move the Character
                        OldPosition = Position;

                        // player sprite jump
                        spriteOfset = new Vector2(spriteFrame.Width * 0, spriteFrame.Height * 0);
                        spriteFrame = new Rectangle((int)spriteOfset.X, (int)spriteOfset.Y, spriteFrame.Width, spriteFrame.Height);

                        // Apply Gravity 
                        Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        break;
                    #endregion
                    #region state walk
                    case EntityState.Walk:

                        Speed = 0;
                        Direction = Vector2.Zero;
                        Velocity = Vector2.Zero;

                        if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.X = MOVE_RIGHT;
                            this.Speed = PLAYER_SPEED;
                            spriteEffect = SpriteEffects.FlipHorizontally;
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.X = MOVE_LEFT;
                            this.Speed = PLAYER_SPEED;
                            spriteEffect = SpriteEffects.None;
                        }
                        else
                        {
                            state = EntityState.Stand;
                        }
                        if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
                        {
                            Velocity += new Vector2(0, -1.6f); // Add an upward impulse
                            state = EntityState.Jump;
                        }

                        // Player animation
                        spriteOfset = new Vector2(180, 0);

                        if (previousGameTimeMsec <= (int)gameTime.TotalGameTime.Milliseconds
                            || previousGameTimeSec != (int)gameTime.TotalGameTime.Seconds)
                        {
                            previousGameTimeMsec = (int)gameTime.TotalGameTime.Milliseconds + ANIMATION_SPEED;
                            previousGameTimeSec = (int)gameTime.TotalGameTime.Seconds;
                            spriteFrame.X += spriteWidth;
                            if (spriteFrame.X > spriteOfset.X + (spriteWidth * 3))
                                spriteFrame.X = (int)spriteOfset.X;
                        }

                        // Check if player is steady standing
                        if (Position.Y > OldPosition.Y && collideSlope == false)
                            state = EntityState.Falling;
                                                
                        // Move the Character
                        OldPosition = Position;

                        // Walk speed
                        Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        // Apply Gravity 
                        Position += new Vector2(0,1) * 200 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        break;
                    #endregion
                    #region state jump
                    case EntityState.Jump:

                        Velocity.Y += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        
                        if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.X += MOVE_LEFT * 0.1f * ((float)gameTime.ElapsedGameTime.TotalSeconds * 10f);
                            this.Speed = PLAYER_SPEED;

                            if (this.Direction.X < -1)
                                this.Direction.X = -1;
                            else if (this.Direction.X < 0)
                                this.Direction.X = 0;
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.X += MOVE_RIGHT * 0.1f * ((float)gameTime.ElapsedGameTime.TotalSeconds * 10f);
                            this.Speed = PLAYER_SPEED;

                            if (this.Direction.X > 1)
                                this.Direction.X = 1;
                            else if (this.Direction.X > 0)
                                this.Direction.X = 0;
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                        {
                            if (this.collideLadder)
                                state = EntityState.Ladder;
                            else if (this.collideRope)
                                state = EntityState.Rope;
                            else
                                state = EntityState.Sit;
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                        {
                            if (this.collideLadder)
                                state = EntityState.Ladder;
                            else if (this.collideRope)
                                state = EntityState.Rope;
                        }

                            // Move the Character
                            OldPosition = Position;

                            // player sprite jump
                            spriteOfset = new Vector2(spriteFrame.Width * 0, spriteFrame.Height * 1);
                            spriteFrame = new Rectangle((int)spriteOfset.X, (int)spriteOfset.Y, spriteFrame.Width, spriteFrame.Height);

                            // Apply Gravity + jumping
                            if (Velocity.Y < -1.2f)
                            {
                                // Apply jumping
                                Position += Velocity * 350 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                                // Apply Gravity 
                                Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                                // Walk / Jump speed
                                Position += Direction * (Speed / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                            else
                            {
                                landed = false;
                                state = EntityState.Falling;
                            }

                        break;
                    #endregion
                    #region state falling
                    case EntityState.Falling:
                    
                        // player sprite falling
                        spriteOfset = new Vector2(spriteFrame.Width * 1, spriteFrame.Height * 1);
                        spriteFrame = new Rectangle((int)spriteOfset.X, (int)spriteOfset.Y, spriteFrame.Width, spriteFrame.Height);

                        if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.X += MOVE_LEFT * 0.1f * ((float)gameTime.ElapsedGameTime.TotalSeconds * 10f);
                            this.Speed = PLAYER_SPEED;

                            if (this.Direction.X < -1)
                                this.Direction.X = -1;
                            else if (this.Direction.X < 0)
                                this.Direction.X = 0;
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.X += MOVE_RIGHT * 0.1f * ((float)gameTime.ElapsedGameTime.TotalSeconds * 10f);
                            this.Speed = PLAYER_SPEED;

                            if (this.Direction.X > 1)
                                this.Direction.X = 1;
                            else if (this.Direction.X > 0)
                                this.Direction.X = 0;
                        }

                        if (OldPosition.Y < position.Y)
                        {
                            // Move the Character
                            OldPosition = Position;

                            Velocity.Y += (float)gameTime.ElapsedGameTime.TotalSeconds;
                            
                            // Apply jumping
                            // Position += Velocity * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                            // Apply Gravity 
                            Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            
                            // Walk / Jump speed
                            Position += Direction * (Speed / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        else
                        {
                            if (previousGameTimeMsec <= (int)gameTime.TotalGameTime.Milliseconds
                            || previousGameTimeSec != (int)gameTime.TotalGameTime.Seconds)
                            {
                                previousGameTimeMsec = (int)gameTime.TotalGameTime.Milliseconds + 250;
                                previousGameTimeSec = (int)gameTime.TotalGameTime.Seconds;

                                if (landed == true)
                                    state = EntityState.Stand;
                                else
                                    landed = true;
                            }

                            // Move the Character
                            OldPosition = Position;

                            // Apply Gravity 
                            Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                            // Walk / Jump speed
                            Position += Direction * (Speed / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        break;
                    #endregion
                }
            }

            // temporary global function buttons 
            // should be handles by singleton class keyboard manager
            if (keyboardStateCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.F1) == true &&
                keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F1) == true)
            {
                inventory.addItem(itemStore.getItem(new Random().Next(1200, 1210)));
            }
            else if (keyboardStateCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.F3) == true &&
                     keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F3) == true)
            {
                inventory.loadItems("inventory.bin");
            }
            else if (keyboardStateCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.F4) == true &&
                     keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F4) == true)
            {
                inventory.saveItem("inventory.bin");
            }
            // temporary

            if (!collideWarp)
            {
                this.color = Color.White;
                this.effectCounter = 0;
            }

            keyboardStatePrevious = keyboardStateCurrent;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
                spriteBatch.Draw(sprite, new Rectangle((int)Position.X, (int)Position.Y, spriteFrame.Width, spriteFrame.Height), 
                    spriteFrame, this.color, 0f, Vector2.Zero, spriteEffect, 0f);
        }

        public bool Effect(GameTime gameTime)
        {
            if (this.effectCounter == 3)
            {
                this.effectCounter = 0;
                this.color = Color.White;       // reset color
                return true;
            }
            else
            {
                if (previousEffectTimeSec <= (int)gameTime.TotalGameTime.Seconds
                    || previousEffectTimeMin != (int)gameTime.TotalGameTime.Minutes)
                {
                    previousEffectTimeSec = (int)gameTime.TotalGameTime.Seconds + 3;
                    previousEffectTimeMin = (int)gameTime.TotalGameTime.Minutes;
                    this.effectCounter++;
                }

                this.color.R = (byte)((previousEffectTimeSec * 250) - gameTime.TotalGameTime.Milliseconds / 4);

                return false;
            }
        }
    }
}
