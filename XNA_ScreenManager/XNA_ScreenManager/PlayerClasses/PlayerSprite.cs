using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNA_ScreenManager.CharacterClasses;
using XNA_ScreenManager.ItemClasses;
using XNA_ScreenManager.ScriptClasses;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.PlayerClasses;
using Microsoft.Xna.Framework.Content;

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
        Equipment equipment = Equipment.Instance;
        ScriptInterpreter scriptManager = ScriptInterpreter.Instance;
        PlayerInfo playerinfo = PlayerInfo.Instance;

        // link to world content manager
        ContentManager Content;

        // Player properties
        public Rectangle spriteScale;
        public int spriteWidth = 80;
        public int spriteHeight = 80;
        public Vector2 spriteOfset = new Vector2(80,0);
        private SpriteEffects spriteEffect = SpriteEffects.None;
        private float transperancy = 1;

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
        float previousGameTimeMsec,                                                                 // GameTime in Miliseconds
              previousEffectTimeSec;                                                                // GameTime in Miliseconds
        private bool landed;                                                                        // land switch, arrow switch

        //map properties
        private Vector2 TileSize = Vector2.Zero;
        #endregion

        public PlayerSprite(GameWorld getworld,
            int _X, int _Y, Vector2 _tileSize)
            : base()
        {
            // Derived properties
            Active = true;
            SpriteFrame = new Rectangle((int)spriteOfset.X, (int)spriteOfset.Y, spriteWidth, spriteHeight);
            SpriteSize = new Rectangle(0, 0, spriteWidth, spriteHeight);
            Position = new Vector2(_X, _Y);
            OldPosition = new Vector2(_X, _Y);
            
            // Local properties
            TileSize = _tileSize;
            spriteScale = new Rectangle(                                                            // Obsolete but can be used for events
                Convert.ToInt32(Position.X),
                Convert.ToInt32(Position.Y),
                spriteWidth, spriteHeight);
            Direction = new Vector2();                                                              // Move direction
            state = EntityState.Stand;                                                              // Player state

            // Link world get Content Manager
            world = getworld;
            Content = world.Content;
        }

        public override void Update(GameTime gameTime)
        {
            keyboardStateCurrent = Keyboard.GetState();

            // reset effect state
            if (!collideWarp)
            {
                this.color = Color.White;
                this.effectCounter = 0;
            }

            if (Active)
            {
                switch (state)
                {
                    #region state cooldown
                    case EntityState.Cooldown:

                        // reduce timer
                        previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (previousGameTimeMsec < 0)
                        {
                            previousGameTimeMsec = 0;

                            spriteFrame.X = 0;
                            state = EntityState.Stand;
                        }

                        // Apply Gravity 
                        Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        break;

                    #endregion
                    #region state swinging
                    case EntityState.Swing:

                        Speed = 0;
                        Direction = Vector2.Zero;
                        Velocity = Vector2.Zero;

                        // Move the Character
                        OldPosition = Position;

                        // player animation
                        spriteOfset = new Vector2(spriteFrame.Width * 0, spriteFrame.Height * 7);
                        spriteFrame.Y = (int)spriteOfset.Y;

                        // reduce timer
                        previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (previousGameTimeMsec < 0)
                        {
                            //previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.20f;
                            previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + (float)((350 - playerinfo.ASPD * 12) * 0.0006f) + 0.05f;

                            spriteFrame.X += spriteWidth * 2;

                            if (spriteFrame.X > spriteOfset.X + (spriteWidth * 2))
                            {
                                previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + (float)((350 - playerinfo.ASPD * 12) * 0.0006f) + 0.05f;

                                // make sure the world is connected
                                if (world == null)
                                    world = GameWorld.GetInstance;

                                // create swing effect
                                world.createEffects(EffectType.WeaponSwing, new Vector2(this.Position.X + this.spriteFrame.Width * 0.6f, this.Position.Y + this.spriteFrame.Height * 0.7f), spriteEffect, 1);

                                // start cooldown
                                state = EntityState.Cooldown;
                            }
                        }

                        // Apply Gravity 
                        Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        break;
                    #endregion
                    #region state stabbing
                    case EntityState.Stab:

                        Speed = 0;
                        Direction = Vector2.Zero;
                        Velocity = Vector2.Zero;

                        // Move the Character
                        OldPosition = Position;

                        // player animation
                        spriteOfset = new Vector2(spriteFrame.Width * 0, spriteFrame.Height * 6);
                        spriteFrame.Y = (int)spriteOfset.Y;

                        // reduce timer
                        previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (previousGameTimeMsec < 0)
                        {
                            previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + (float)((350 - playerinfo.ASPD * 12) * 0.0006f) + 0.05f;

                            spriteFrame.X += spriteWidth * 2;

                            if (spriteFrame.X > spriteWidth * 1)
                            {
                                previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + (float)((350 - playerinfo.ASPD * 12) * 0.0006f) + 0.05f;

                                // make sure the world is connected
                                if (world == null)
                                    world = GameWorld.GetInstance;

                                // create swing effect
                                world.createEffects(EffectType.WeaponSwing, new Vector2(this.Position.X, this.Position.Y + this.spriteFrame.Height * 0.7f), spriteEffect, 0);

                                // reset sprite frame and change state

                                state = EntityState.Cooldown;
                            }
                        }

                        // Apply Gravity 
                        Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        break;
                    #endregion
                    #region state shooting
                    case EntityState.Shoot:

                        Speed = 0;
                        Direction = Vector2.Zero;
                        Velocity = Vector2.Zero;

                        // Move the Character
                        OldPosition = Position;

                        // player animation
                        spriteOfset = new Vector2(spriteFrame.Width * 0, spriteFrame.Height * 4);
                        spriteFrame.Y = (int)spriteOfset.Y;

                        // reduce timer
                        previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (previousGameTimeMsec < 0)
                        {
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
                                    // make sure the world is connected
                                    if (world == null)
                                        world = GameWorld.GetInstance;

                                    // create and release an arrow
                                    if(spriteEffect == SpriteEffects.FlipHorizontally)
                                        world.createArrow(new Vector2(this.Position.X, this.Position.Y + this.spriteFrame.Height * 0.6f), 800, new Vector2(1, 0));
                                    else
                                        world.createArrow(new Vector2(this.Position.X, this.Position.Y + this.spriteFrame.Height * 0.6f), 800, new Vector2(-1, 0));

                                    // Set the timer for cooldown
                                    previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + (float)((350 - playerinfo.ASPD * 12) * 0.0006f) + 0.05f;

                                    // reset sprite frame and change state
                                    // start cooldown
                                    spriteFrame.X = 0;
                                    state = EntityState.Cooldown;
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
                        spriteOfset = new Vector2(spriteFrame.Width * 0, spriteFrame.Height * 4);
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

                        // double check collision
                        if (this.collideRope == false)
                            this.state = EntityState.Falling;

                        if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.Y = MOVE_DOWN;
                            this.Speed = PLAYER_SPEED * 0.75f;

                            //player animation
                            spriteOfset = new Vector2(spriteFrame.Width * 2, spriteFrame.Height * 3);
                            spriteFrame.Y = (int)spriteOfset.Y;

                            // double check frame if previous state has higher X
                            if (spriteFrame.X > spriteWidth * 3)
                                spriteFrame.X = (int)spriteOfset.X;

                            // reduce timer
                            previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (previousGameTimeMsec < 0)
                            {
                                previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.12f;
                                spriteFrame.X += spriteWidth;
                                if (spriteFrame.X > spriteWidth * 3)
                                    spriteFrame.X = (int)spriteOfset.X;
                            }
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.Y = MOVE_UP;
                            this.Speed = PLAYER_SPEED * 0.75f;

                            //player animation
                            spriteOfset = new Vector2(spriteFrame.Width * 2, spriteFrame.Height * 3);
                            spriteFrame.Y = (int)spriteOfset.Y;

                            // double check frame if previous state has higher X
                            if (spriteFrame.X > spriteWidth * 3)
                                spriteFrame.X = (int)spriteOfset.X;

                            // reduce timer
                            previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (previousGameTimeMsec < 0)
                            {
                                previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.12f;
                                spriteFrame.X += spriteWidth;
                                if (spriteFrame.X > spriteWidth * 3)
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

                        // double check collision
                        if (this.collideLadder == false)
                            this.state = EntityState.Falling;

                        if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.Y = MOVE_DOWN;
                            this.Speed = PLAYER_SPEED * 0.75f;

                            //player animation
                            spriteOfset = new Vector2(spriteFrame.Width * 0, spriteFrame.Height * 3);
                            spriteFrame.Y = (int)spriteOfset.Y;

                            // double check frame if previous state has higher X
                            if (spriteFrame.X > spriteWidth * 1)
                                spriteFrame.X = (int)spriteOfset.X;

                            // reduce timer
                            previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (previousGameTimeMsec < 0)
                            {
                                previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
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
                            spriteOfset = new Vector2(spriteFrame.Width * 0, spriteFrame.Height * 3);
                            spriteFrame.Y = (int)spriteOfset.Y;

                            // double check frame if previous state has higher X
                            if (spriteFrame.X > spriteWidth * 1)
                                spriteFrame.X = (int)spriteOfset.X;

                            // reduce timer
                            previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (previousGameTimeMsec < 0)
                            {
                                previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
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
                            // check if weapon is equiped
                            if (equipment.item_list.FindAll(delegate(Item item) { return item.Type == ItemType.Weapon; }).Count > 0)
                            {
                                WeaponType weapontype = equipment.item_list.Find(delegate(Item item) { return item.Type == ItemType.Weapon; }).WeaponType;

                                // check the weapon type
                                if (weapontype == WeaponType.Bow)
                                {
                                    previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + (float)((350 - playerinfo.ASPD * 12) * 0.0006f) + 0.05f;

                                    spriteFrame.X = 0;
                                    state = EntityState.Shoot;
                                }
                                else if (weapontype == WeaponType.Dagger || weapontype == WeaponType.One_handed_Sword)
                                {
                                    previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + (float)((350 - playerinfo.ASPD * 12) * 0.0006f) + 0.05f;

                                    spriteFrame.X = 0;

                                    if(randomizer.Instance.generateRandom(0,2) == 1)
                                        state = EntityState.Stab;
                                    else
                                        state = EntityState.Swing;
                                }
                            }
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
                        spriteOfset = new Vector2(spriteFrame.Width * 0, spriteFrame.Height * 1);
                        spriteFrame.Y = (int)spriteOfset.Y;

                        // reduce timer
                        previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (previousGameTimeMsec < 0)
                        {
                            previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
                            spriteFrame.X += spriteWidth;
                            if (spriteFrame.X > spriteWidth * 3)
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
                            spriteOfset = new Vector2(spriteFrame.Width * 4, spriteFrame.Height * 2);
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
                        spriteOfset = new Vector2(spriteFrame.Width * 5, spriteFrame.Height * 2);
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
                            // reduce timer
                            previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (previousGameTimeMsec < 0)
                            {
                                previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.08f;

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
                    #region state hit
                    case EntityState.Hit:

                        // Add an upward impulse
                        Velocity = new Vector2(0, -1.5f);

                        // Add an sideward pulse
                        if (spriteEffect == SpriteEffects.None)
                            Direction = new Vector2(1.6f, 0);
                        else
                            Direction = new Vector2(-1.6f, 0);

                        // Damage controll and balloon is triggered in monster sprite

                        // Move the Character
                        OldPosition = Position;

                        // player sprite hit
                        spriteOfset = new Vector2(spriteFrame.Width * 0, spriteFrame.Height * 0);
                        spriteFrame = new Rectangle((int)spriteOfset.X, (int)spriteOfset.Y, spriteFrame.Width, spriteFrame.Height);

                        // Set new state
                        state = EntityState.Frozen;

                        break;
                    #endregion
                    #region state recover hit
                    case EntityState.Frozen:

                        // Upward Position
                        Velocity.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * 2;

                        // Make player transperant
                        if (transperancy >= 0 )
                            this.transperancy -= (float)gameTime.ElapsedGameTime.TotalSeconds * 10;

                        // turn red
                        this.color = Color.Red;

                        // Move the Character
                        OldPosition = Position;

                        // player sprite hit
                        spriteOfset = new Vector2(spriteFrame.Width * 0, spriteFrame.Height * 0);
                        spriteFrame = new Rectangle((int)spriteOfset.X, (int)spriteOfset.Y, spriteFrame.Width, spriteFrame.Height);

                        // Apply Gravity + jumping
                        if (Velocity.Y < -1.2f)
                        {
                            // Apply jumping
                            Position += Velocity * 350 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                            // Apply Gravity 
                            Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                            // Walk / Jump speed
                            Position += Direction * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        else
                        {
                            landed = false;
                            state = EntityState.Falling;
                            Direction = Vector2.Zero;
                            Velocity = Vector2.Zero;
                            //this.color = Color.White;
                            this.transperancy = 1;
                        }

                        break;
                    #endregion
                }
            }

            #region temporary quickbuttons
            // temporary global function buttons 
            // should be handles by singleton class keyboard manager
            if (keyboardStateCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.F1) == true &&
                     keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F1) == true)
            {
                inventory.addItem(itemStore.getItem(new Random().Next(1100, 1114)));
            } 
            else if (keyboardStateCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.F2) == true &&
                      keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F2) == true)
            {
                inventory.addItem(itemStore.getItem(2300));
            }
            else if (keyboardStateCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.F3) == true &&
                      keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F3) == true)
            {
                inventory.addItem(itemStore.getItem(1300));
            }
            else if (keyboardStateCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.F4) == true &&
                     keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F4) == true)
            {
                inventory.saveItem("inventory.bin");
            }
            else if (keyboardStateCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.F5) == true &&
                     keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F5) == true)
            {
                inventory.loadItems("inventory.bin");
            }
            // temporary
            #endregion

            keyboardStatePrevious = keyboardStateCurrent;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                // because in some states the hair sprite is out of bound (80x80)
                Rectangle hairFrame = spriteFrame, 
                          clothFrame = spriteFrame,
                          weaponFrame = spriteFrame,
                          faceFrame = spriteFrame,
                          bodyFrame = spriteFrame;

                switch(state)
                {
                    case EntityState.Jump:
                    case EntityState.Falling:
                        hairFrame = new Rectangle(spriteFrame.X, spriteFrame.Y - 10, spriteFrame.Width, SpriteFrame.Height + 10);
                        clothFrame = new Rectangle(spriteFrame.X, spriteFrame.Y + 10, spriteFrame.Width, SpriteFrame.Height + 10);
                        bodyFrame = new Rectangle(spriteFrame.X, spriteFrame.Y + 10, spriteFrame.Width, SpriteFrame.Height + 10);
                        weaponFrame = new Rectangle(spriteFrame.X, spriteFrame.Y + 10, spriteFrame.Width, SpriteFrame.Height + 10);
                        faceFrame = new Rectangle(spriteFrame.X, spriteFrame.Y, spriteFrame.Width, SpriteFrame.Height);
                        break;
                    case EntityState.Cooldown:
                        if (equipment.item_list.Find(delegate(Item item) { return item.Type == ItemType.Weapon; }).WeaponType != WeaponType.Bow)
                            weaponFrame = new Rectangle(spriteFrame.X - 20, spriteFrame.Y, spriteFrame.Width + 20, SpriteFrame.Height);
                        break;
                    default:
                        break;
                }


                if (playerinfo.body_sprite != null)
                    spriteBatch.Draw(Content.Load<Texture2D>(playerinfo.body_sprite), new Rectangle((int)Position.X, (int)(Position.Y + (bodyFrame.Height - spriteFrame.Height)), spriteFrame.Width, bodyFrame.Height),
                    bodyFrame, this.color * this.transperancy, 0f, Vector2.Zero, spriteEffect, 0f);

                if (playerinfo.faceset_sprite != null)
                spriteBatch.Draw(Content.Load<Texture2D>(playerinfo.faceset_sprite), new Rectangle((int)Position.X, (int)Position.Y, spriteFrame.Width, spriteFrame.Height),
                    faceFrame, this.color * this.transperancy, 0f, Vector2.Zero, spriteEffect, 0f);

                if (playerinfo.hair_sprite != null)
                    spriteBatch.Draw(Content.Load<Texture2D>(playerinfo.hair_sprite), new Rectangle((int)Position.X, (int)(Position.Y - (hairFrame.Height - spriteFrame.Height)), spriteFrame.Width, hairFrame.Height),
                    hairFrame, this.color * this.transperancy, 0f, Vector2.Zero, spriteEffect, 0f);

                if (playerinfo.costume_sprite != null)
                    spriteBatch.Draw(Content.Load<Texture2D>(playerinfo.costume_sprite), new Rectangle((int)Position.X, (int)(Position.Y + (clothFrame.Height - spriteFrame.Height)), spriteFrame.Width, clothFrame.Height),
                    clothFrame, this.color * this.transperancy, 0f, Vector2.Zero, spriteEffect, 0f);

                if (playerinfo.weapon_sprite != null)
                {
                    if (spriteEffect == SpriteEffects.None)
                        spriteBatch.Draw(Content.Load<Texture2D>(playerinfo.weapon_sprite), new Rectangle((int)(Position.X - (weaponFrame.Width - spriteFrame.Width)), (int)(Position.Y + (weaponFrame.Height - spriteFrame.Height)), weaponFrame.Width, weaponFrame.Height),
                        weaponFrame, this.color * this.transperancy, 0f, Vector2.Zero, spriteEffect, 0f);
                    else
                        spriteBatch.Draw(Content.Load<Texture2D>(playerinfo.weapon_sprite), new Rectangle((int)Position.X, (int)(Position.Y + (weaponFrame.Height - spriteFrame.Height)), weaponFrame.Width, weaponFrame.Height),
                        weaponFrame, this.color * this.transperancy, 0f, Vector2.Zero, spriteEffect, 0f);
                }
            }
        }

        public bool Effect(GameTime gameTime)
        {
            // press up will instant warp
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                this.effectCounter = 0;
                this.color = Color.White;
                return true;
            } 

            // standing in warp portal effect
            if (this.effectCounter == 2)
            {
                this.effectCounter = 0;
                this.color = Color.White;       // reset color
                return true;
            }
            else
            {
                previousEffectTimeSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (previousEffectTimeSec <= 0)
                {
                    previousEffectTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds + 3;
                    this.effectCounter++;
                }

                this.color.R = (byte)((previousEffectTimeSec * 250) - gameTime.ElapsedGameTime.TotalSeconds / 4);

                return false;
            }
        }
    }
}
