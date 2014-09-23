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
using XNA_ScreenManager.ScreenClasses.MainClasses;
using System.Collections.Generic;
using XNA_ScreenManager.GameWorldClasses.Entities;
using System.Xml;
using System.IO;
using XNA_ScreenManager.Networking;

namespace XNA_ScreenManager
{
    public class PlayerSprite : PlayerRoutines
    {
        #region properties

        // The Gameworld
        protected GameWorld world;
        protected ResourceManager resourcemanager = ResourceManager.GetInstance;

        // Keyboard- and Mousestate
        protected KeyboardState keyboardStateCurrent, keyboardStatePrevious;

        // Player inventory
        protected ItemStore itemStore = ItemStore.Instance;
        protected ScriptInterpreter scriptManager = ScriptInterpreter.Instance;

        // link to world content manager
        protected ContentManager Content;

        // Player properties
        public SpriteEffects spriteEffect = SpriteEffects.None, 
                             previousSpriteEffect;
        protected float transperancy = 1;
        protected bool debug = false;

        // Sprite Animation Properties
        protected int effectCounter = 0;                                                          // for the warp effect
        protected Color color = Color.White;                                                      // sprite color
        public Vector2 Direction, previousDirection;                                              // Sprite Move direction
        protected float Speed;                                                                    // Speed used in functions
        protected Vector2 Velocity = new Vector2(0,1);                                            // speed used in jump
        public int PLAYER_SPEED = 200;                                                            // The actual speed of the player
        private const int ANIMATION_SPEED = 120;                                                  // Animation speed, 120 = default 
        private const int MOVE_UP = -1;                                                           // player moving directions
        private const int MOVE_DOWN = 1;                                                          // player moving directions
        private const int MOVE_LEFT = -1;                                                         // player moving directions
        private const int MOVE_RIGHT = 1;                                                         // player moving directions
        private float previousGameTimeMsec;                                                       // GameTime in Miliseconds
        protected float previousNetworkMsec;                                                      // Network update time
        private bool holdPosition;                                                                // player hold still but animate

        // new Texture properties
        public int spriteframe = 0, prevspriteframe = 0, maxspriteframe = 0;
        public string spritename = "stand1_0", attackSprite;
        public string[] spritepath = new string[] 
        { 
            @"gfx\player\body\head\",                                                               // Head Sprite  (0)
            @"gfx\player\body\torso\",                                                              // Body Sprite (1)
            @"gfx\player\faceset\face1\",                                                           // Faceset Sprite (2)
            @"gfx\player\hairset\hair1\",                                                           // Hairset Sprite (3)
            "",                                                                                     // Armor and Costume Sprite (4)
            "",                                                                                     // Accessory top Sprite (Sunglasses, Ear rings) (5)
            "",                                                                                     // Accessory bottom Sprite (mouth items, capes) (6)
            "",                                                                                     // Headgear Sprite (Hats, Helmets) (7)
            "",                                                                                     // Weapon Sprite (8)
            @"gfx\player\body\hands\",                                                              // Hands Sprite (9)
        };

        #endregion

        public PlayerSprite(float _X, float _Y)
            : base()
        {
            // Derived properties
            Active = true;
            SpriteSize = new Rectangle(0, 0, 50, 70);
            Position = new Vector2(_X, _Y);
            OldPosition = new Vector2(_X, _Y);
            
            // Local properties
            Direction = new Vector2();                                                              // Move direction
            state = EntityState.Stand;                                                              // Player state

            Content = resourcemanager.Content;
            spriteEffect = SpriteEffects.FlipHorizontally;
        }

        public EntityState SetState 
        { 
            get{return this.state;} 
            set{this.state = value;} 
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // Player Update Routines

            keyboardStateCurrent = Keyboard.GetState();
            previousState = this.state;
            previousSpriteEffect = this.spriteEffect;
            previousDirection = this.Direction;

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
                    #region state skillactive
                    case EntityState.Skill:

                            // Move the Character
                            OldPosition = Position;

                            // lock player at position
                            this.Direction.X = 0;

                            // Walk speed
                            Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        break;

                    #endregion
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

                        // reduce timer
                        previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        // Player animation
                        if (prevspriteframe != spriteframe)
                        {
                            prevspriteframe = spriteframe;
                            for (int i = 0; i < spritepath.Length; i++)
                            {
                                spritename = attackSprite + spriteframe.ToString();
                                playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                            }
                        }

                        if (previousGameTimeMsec < 0)
                        {
                            previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;

                            // set sprite frames
                            spriteframe++;

                            if (spriteframe > maxspriteframe)
                            {
                                spriteframe = maxspriteframe;
                                previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;

                                // make sure the world is connected
                                if (world == null)
                                    world = GameWorld.GetInstance;

                                // create swing effect
                                if (spriteEffect == SpriteEffects.FlipHorizontally)
                                {
                                    Vector2 pos = new Vector2(this.Position.X + this.SpriteFrame.Width * 1.6f, this.Position.Y + this.SpriteFrame.Height * 0.7f);
                                    
                                    world.newEffect.Add(new DamageArea(new Vector2(pos.X - 50, pos.Y), new Rectangle(0, 0, 80, 10), false, 1,
                                        (float)gameTime.ElapsedGameTime.TotalSeconds + 0.2f, 100));
                                    NetworkGameData.Instance.sendDmgArea(getPlayer().Name, new Vector2(pos.X - 50, pos.Y), new Vector2(80, 10), false.ToString(), 1, 0.20f, 100);

                                    world.newEffect.Add(new WeaponSwing(pos, WeaponSwingType.Swing01, spriteEffect));
                                }
                                else
                                {
                                    Vector2 pos = new Vector2(this.Position.X - this.SpriteFrame.Width * 0.6f, this.Position.Y + this.SpriteFrame.Height * 0.7f);
                                    
                                    world.newEffect.Add(new DamageArea(new Vector2(pos.X - 18, pos.Y), new Rectangle(0, 0, 80, 10), false, 1,
                                        (float)gameTime.ElapsedGameTime.TotalSeconds + 0.2f, 100));
                                    NetworkGameData.Instance.sendDmgArea(getPlayer().Name, new Vector2(pos.X - 18, pos.Y), new Vector2(80, 10), false.ToString(), 1, 0.20f, 100);
                                    
                                    world.newEffect.Add(new WeaponSwing(pos, WeaponSwingType.Swing01, spriteEffect));
                                }

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
                        Vector2 extend_position = Vector2.Zero;

                        // Move the Character
                        OldPosition = Position;

                        // reduce timer
                        previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        // long stab, extend weaponswing and DMG-area position
                        if (attackSprite.EndsWith("F_"))
                            extend_position = new Vector2(22, 0);

                        // Player animation
                        if (prevspriteframe != spriteframe)
                        {
                            prevspriteframe = spriteframe;
                            for (int i = 0; i < spritepath.Length; i++)
                            {
                                spritename = attackSprite + spriteframe.ToString();
                                playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                            }
                        }

                        if (previousGameTimeMsec < 0)
                        {
                            previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;

                            // set sprite frames
                            spriteframe++;

                            if (spriteframe > maxspriteframe)
                            {
                                spriteframe = maxspriteframe;
                                previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;

                                // make sure the world is connected
                                if (world == null)
                                    world = GameWorld.GetInstance;

                                // create stab effect and DMG-area
                                if (spriteEffect == SpriteEffects.FlipHorizontally)
                                {
                                    Vector2 pos = new Vector2(this.Position.X + this.SpriteFrame.Width * 0.3f, this.Position.Y + this.SpriteFrame.Height * 0.7f) + extend_position;
                                    
                                    world.newEffect.Add(new DamageArea(new Vector2(pos.X + 15, pos.Y), new Rectangle(0, 0, 80, 10), false, 1,
                                        (float)gameTime.ElapsedGameTime.TotalSeconds + 0.1f, 100));

                                    NetworkGameData.Instance.sendDmgArea(getPlayer().Name, new Vector2(pos.X + 15, pos.Y), new Vector2(80, 10), false.ToString(), 1, 0.10f, 100);

                                    world.newEffect.Add(new WeaponSwing(pos, WeaponSwingType.Stab01, spriteEffect));

                                }
                                else
                                {
                                    Vector2 pos = new Vector2(this.Position.X - this.SpriteFrame.Width * 0.7f, this.Position.Y + this.SpriteFrame.Height * 0.7f) - extend_position;
                                    
                                    world.newEffect.Add(new DamageArea(new Vector2(pos.X - 18, pos.Y), new Rectangle(0, 0, 80, 10), false, 1,
                                        (float)gameTime.ElapsedGameTime.TotalSeconds + 0.1f, 100));

                                    NetworkGameData.Instance.sendDmgArea(getPlayer().Name, new Vector2(pos.X - 18, pos.Y), new Vector2(80, 10), false.ToString(), 1, 0.10f, 100);

                                    world.newEffect.Add(new WeaponSwing(pos, WeaponSwingType.Stab01, spriteEffect));
                                }

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

                        // reduce timer
                        previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        
                        // Player animation
                        if (prevspriteframe != spriteframe)
                        {
                            prevspriteframe = spriteframe;
                            for (int i = 0; i < spritepath.Length; i++)
                            {
                                spritename = "shoot1_" + spriteframe.ToString();
                                playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                            }
                        }

                        if (previousGameTimeMsec < 0)
                        {
                            spriteframe++;

                            if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt)
                                || keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D1))
                            {
                                // Later = charge arrow skill
                                if (spriteframe > 1)
                                    spriteframe = 1;
                            }
                            else
                            {
                                if (spriteframe > 2)
                                {
                                    // make sure the world is connected
                                    if (world == null)
                                        world = GameWorld.GetInstance;

                                    // create and release an arrow
                                    if (spriteEffect == SpriteEffects.FlipHorizontally)
                                        world.newEffect.Add(new Arrow(Content.Load<Texture2D>(@"gfx\gameobjects\arrow"),
                                            new Vector2(this.Position.X, this.Position.Y + this.SpriteFrame.Height * 0.6f),
                                            800, new Vector2(1, 0), Vector2.Zero));
                                    else
                                        world.newEffect.Add(new Arrow(Content.Load<Texture2D>(@"gfx\gameobjects\arrow"),
                                            new Vector2(this.Position.X, this.Position.Y + this.SpriteFrame.Height * 0.6f),
                                            800, new Vector2(-1, 0), Vector2.Zero));

                                    // Set the timer for cooldown
                                    previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;

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
                            //state = EntityState.Stand;

                            NetworkGameData.Instance.sendPlayerData("Stand", getPlayer());
                        }
                        else if (keyboardStateCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Up) &&
                                 keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                        {
                            //state = EntityState.Stand;

                            NetworkGameData.Instance.sendPlayerData("Stand", getPlayer());
                        }
                        //else if (keyboardStateCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Insert) &&
                        //         keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Insert))
                        //{
                        //    state = EntityState.Stand;
                        //}

                        // Move the Character
                        OldPosition = Position;

                        // Player animation
                        if (prevspriteframe != spriteframe)
                        {
                            prevspriteframe = spriteframe;
                            for (int i = 0; i < spritepath.Length; i++)
                            {
                                spritename = "sit_0";
                                playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                            }
                        }                        

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
                        //if (this.collideRope == false)
                        //    this.state = EntityState.Falling;

                        if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down) && !HoldPosition)
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.Y = MOVE_DOWN;
                            this.Speed = PLAYER_SPEED * 0.75f;
                            NetworkGameData.Instance.sendPlayerData("Down", getPlayer());

                            // reduce timer
                            previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (previousGameTimeMsec < 0)
                            {
                                previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
                                spriteframe++;
                            }

                            // double check frame if previous state has higher X
                            if (spriteframe > 1)
                                spriteframe = 0;
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up) && !HoldPosition)
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.Y = MOVE_UP;
                            this.Speed = PLAYER_SPEED * 0.75f;
                            NetworkGameData.Instance.sendPlayerData("Up", getPlayer());

                            // reduce timer
                            previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (previousGameTimeMsec < 0)
                            {
                                previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
                                spriteframe++;
                            }

                            // double check frame if previous state has higher X
                            if (spriteframe > 1)
                                spriteframe = 0;
                        }

                        // Player animation
                        if (prevspriteframe != spriteframe)
                        {
                            prevspriteframe = spriteframe;
                            for (int i = 0; i < spritepath.Length; i++)
                            {
                                spritename = "rope_" + spriteframe.ToString();
                                playerStore.activePlayer.spriteOfset[i] = getoffset(i);
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
                        // if (this.collideLadder == false)
                        //    this.state = EntityState.Falling;

                        if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down) && !HoldPosition)
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.Y = MOVE_DOWN;
                            this.Speed = PLAYER_SPEED * 0.75f;
                            NetworkGameData.Instance.sendPlayerData("Down", getPlayer());

                            // reduce timer
                            previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (previousGameTimeMsec < 0)
                            {
                                previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
                                spriteframe++;
                            }

                            // double check frame if previous state has higher X
                            if (spriteframe > 1)
                                spriteframe = 0;
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up) && !HoldPosition)
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.Y = MOVE_UP;
                            this.Speed = PLAYER_SPEED * 0.75f;
                            NetworkGameData.Instance.sendPlayerData("Up", getPlayer());

                            // reduce timer
                            previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (previousGameTimeMsec < 0)
                            {
                                previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
                                spriteframe++;
                            }

                            // double check frame if previous state has higher X
                            if (spriteframe > 1)
                                spriteframe = 0;
                        }

                        // Player animation
                        //if (prevspriteframe != spriteframe)
                        //{
                        //    prevspriteframe = spriteframe;
                        //    for (int i = 0; i < spritepath.Length; i++)
                        //    {
                        //        spritename = "ladder_" + spriteframe.ToString();
                        //        playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                        //    }
                        //}

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

                        if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) && !HoldPosition)
                        {
                            spriteframe = 0;
                            //state = EntityState.Walk;
                            spriteEffect = SpriteEffects.FlipHorizontally;

                            NetworkGameData.Instance.sendPlayerData("Right", getPlayer());
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) && !HoldPosition)
                        {
                            spriteframe = 0;
                            //state = EntityState.Walk;
                            spriteEffect = SpriteEffects.None;
                            
                            NetworkGameData.Instance.sendPlayerData("Left", getPlayer());
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space) && !HoldPosition)
                        {
                            if (!collideNPC)
                            {
                                spriteframe = 0;
                                Velocity += new Vector2(0, -1.6f); // Add an upward impulse
                                //state = EntityState.Jump;
                                
                                NetworkGameData.Instance.sendPlayerData("Jump", getPlayer());
                            }
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Insert) && !HoldPosition)
                        {
                            spriteframe = 0;
                            // state = EntityState.Sit;

                            NetworkGameData.Instance.sendPlayerData("Sit", getPlayer());
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up) && !HoldPosition)
                        {
                            spriteframe = 0;
                            //if (this.collideLadder)
                            //    state = EntityState.Ladder;
                            //else if (this.collideRope)
                            //    state = EntityState.Rope;

                            NetworkGameData.Instance.sendPlayerData("Up", getPlayer());
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt) && !HoldPosition)
                        {
                            // check if weapon is equiped
                            if (getPlayer().equipment.item_list.FindAll(delegate(Item item) { return item.Type == ItemType.Weapon; }).Count > 0)
                            {
                                WeaponType weapontype = getPlayer().equipment.item_list.Find(delegate(Item item) { return item.Type == ItemType.Weapon; }).WeaponType;

                                // check the weapon type
                                if (weapontype == WeaponType.Bow)
                                {
                                    previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + (float)((350 - playerStore.activePlayer.ASPD * 12) * 0.0006f) + 0.05f;

                                    spriteframe = 0;
                                    state = EntityState.Shoot;
                                }
                                else
                                {
                                    previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + (float)((350 - playerStore.activePlayer.ASPD * 12) * 0.0006f) + 0.05f;

                                    spriteframe = 0;
                                    GetattackSprite(weapontype);
                                }
                            }
                        }

                        // Check if player is steady standing
                        //if (Position.Y > OldPosition.Y)
                        //    state = EntityState.Falling;

                        // Move the Character
                        OldPosition = Position;

                        // reduce timer
                        previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (previousGameTimeMsec < 0)
                        {
                            previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
                            spriteframe++;
                            if (spriteframe > 4)
                                spriteframe = 0;
                        }

                        // Player animation
                        if (prevspriteframe != spriteframe)
                        {
                            prevspriteframe = spriteframe;
                            for (int i = 0; i < spritepath.Length; i++)
                            {
                                Item weapon = getPlayer().equipment.item_list.Find(x => x.Slot == ItemSlot.Weapon);

                                if(weapon.WeaponType == WeaponType.Two_handed_Axe ||
                                   weapon.WeaponType == WeaponType.Two_handed_Spear ||
                                   weapon.WeaponType == WeaponType.Two_handed_Sword)
                                    spritename = "stand2_" + spriteframe.ToString();
                                else
                                    spritename = "stand1_" + spriteframe.ToString();

                                playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                            }
                        }

                        // Apply Gravity 
                        Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        break;
                    #endregion
                    #region state walk
                    case EntityState.Walk:

                        Speed = 0;
                        Direction = Vector2.Zero;
                        Velocity = Vector2.Zero;

                        if (!keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) &&
                            keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) && 
                            !HoldPosition)
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.X = MOVE_RIGHT;
                            this.Speed = PLAYER_SPEED;
                            spriteEffect = SpriteEffects.FlipHorizontally;
                        }
                        else if (!keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) &&
                                 keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) && 
                                 !HoldPosition)
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.X = MOVE_LEFT;
                            this.Speed = PLAYER_SPEED;
                            spriteEffect = SpriteEffects.None;
                        }
                        else
                        {
                            //state = EntityState.Stand;
                            NetworkGameData.Instance.sendPlayerData("Stop", getPlayer());
                        }
                        if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space) && !HoldPosition)
                        {
                            if (!collideNPC)
                            {
                                Velocity += new Vector2(0, -1.6f); // Add an upward impulse
                                //state = EntityState.Jump;

                                NetworkGameData.Instance.sendPlayerData("Jump", getPlayer());
                            }
                        }

                        // reduce timer
                        previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        // set sprite frames
                        if (previousGameTimeMsec < 0)
                        {
                            previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
                            spriteframe ++;
                        }
                        if (spriteframe > 3)
                            spriteframe = 0;

                        // Player animation
                        if (prevspriteframe != spriteframe)
                        {
                            prevspriteframe = spriteframe;
                            for (int i = 0; i < spritepath.Length; i++)
                            {
                                spritename = "walk1_" + spriteframe.ToString();
                                playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                            }
                        }

                        // Check if player is steady standing
                        //if (Position.Y > OldPosition.Y && collideSlope == false)
                        //    state = EntityState.Falling;
                                                
                        // Move the Character
                        OldPosition = Position;

                        // Walk speed
                        Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        // Apply Gravity 
                        Position += new Vector2(0,1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

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

                            NetworkGameData.Instance.sendPlayerData("Left", getPlayer());

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

                            NetworkGameData.Instance.sendPlayerData("Right", getPlayer());
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                        {
                            //if (this.collideLadder)
                            //    state = EntityState.Ladder;
                            //else if (this.collideRope)
                            //    state = EntityState.Rope;
                            //else
                            //    state = EntityState.Sit;

                            NetworkGameData.Instance.sendPlayerData("Down", getPlayer());
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                        {
                            //if (this.collideLadder)
                            //    state = EntityState.Ladder;
                            //else if (this.collideRope)
                            //    state = EntityState.Rope;

                            NetworkGameData.Instance.sendPlayerData("Up", getPlayer());
                        }

                        // Move the Character
                        OldPosition = Position;

                        // Player animation
                        for (int i = 0; i < spritepath.Length; i++)
                        {
                            spritename = "jump_0";
                            playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                        }
        
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
                            // landed = false;
                            // state = EntityState.Falling;
                        }

                        break;
                    #endregion
                    #region state falling
                    case EntityState.Falling:

                        if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                        {
                            // move player location (make ActiveMap tile check here in the future)
                            this.Direction.X += MOVE_LEFT * 0.1f * ((float)gameTime.ElapsedGameTime.TotalSeconds * 10f);
                            this.Speed = PLAYER_SPEED;

                            if (this.Direction.X < -1)
                                this.Direction.X = -1;
                            else if (this.Direction.X < 0)
                                this.Direction.X = 0;

                            NetworkGameData.Instance.sendPlayerData("Left", getPlayer());
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

                            NetworkGameData.Instance.sendPlayerData("Right", getPlayer());
                        }

                        if (OldPosition.Y < position.Y)
                        {
                            // Move the Character
                            OldPosition = Position;

                            Velocity.Y += (float)gameTime.ElapsedGameTime.TotalSeconds;

                            // Apply Gravity 
                            Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            
                            // Walk / Jump speed
                            Position += Direction * (Speed / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        else
                        {
                            // reduce timer
                            previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                            // Move the Character
                            OldPosition = Position;

                            // Apply Gravity 
                            Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                            // Walk / Jump speed
                            Position += Direction * (Speed / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }

                        // Player animation
                        for (int i = 0; i < spritepath.Length; i++)
                        {
                            spritename = "fly_0";
                            playerStore.activePlayer.spriteOfset[i] = getoffset(i);
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

                        // Damage controll and balloon is triggered in monster-sprite Class

                        // Move the Character
                        OldPosition = Position;

                        // Player animation
                        for (int i = 0; i < spritepath.Length; i++)
                        {
                            spritename = "fly_0";
                            playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                        }

                        // Set new state
                        // state = EntityState.Frozen;

                        break;
                    #endregion
                    #region state frozen
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

                        // Player animation
                        for (int i = 0; i < spritepath.Length; i++)
                        {
                            spritename = "fly_0";
                            playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                        }

                        // Apply Gravity + jumping
                        if (Velocity.Y < -1.2f && OldPosition.Y != Position.Y)
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
                            // landed = false;
                            // state = EntityState.Falling;
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
            if (CheckKey(Keys.Q) && !HoldPosition)
                getPlayer().inventory.addItem(itemStore.getItem(new Random().Next(1100, 1114)));
            else if (CheckKey(Keys.W) && !HoldPosition)
                getPlayer().inventory.addItem(itemStore.getItem(XNA_ScreenManager.ScreenClasses.MainClasses.ResourceManager.randomizer.Instance.generateRandom(1300, 1303)));
            else if (CheckKey(Keys.E) && !HoldPosition)
                getPlayer().inventory.addItem(itemStore.getItem(XNA_ScreenManager.ScreenClasses.MainClasses.ResourceManager.randomizer.Instance.generateRandom(2300, 2308)));
            // temporary
            #endregion
                        
            
            // if the client is connected to a server
            // these functions will trigger a player update            
            if ((state == EntityState.Ladder || state == EntityState.Rope))
            {
                if (keyboardStateCurrent.IsKeyDown(Keys.Up) || keyboardStateCurrent.IsKeyDown(Keys.Down) ||
                    keyboardStatePrevious.IsKeyDown(Keys.Up) || keyboardStatePrevious.IsKeyDown(Keys.Down))
                {
                    if (keyboardStateCurrent.IsKeyDown(Keys.Up))
                        this.Direction.Y = MOVE_UP;
                    else if (keyboardStateCurrent.IsKeyDown(Keys.Down))
                        this.Direction.Y = MOVE_DOWN;
                    else 
                        this.Direction = Vector2.Zero;

                    NetworkGameData.Instance.sendPlayerData();
                }
            } 
            else if (previousState != state || spriteEffect != previousSpriteEffect)
            {
                NetworkGameData.Instance.sendPlayerData();
            }

            keyboardStatePrevious = keyboardStateCurrent;
        }

        private void GetattackSprite(WeaponType weapontype)
        {
            if (XNA_ScreenManager.ScreenClasses.MainClasses.ResourceManager.randomizer.Instance.generateRandom(0, 2) == 1)
            {
                state = EntityState.Stab;

                switch (weapontype)
                {
                    case WeaponType.Dagger:
                    case WeaponType.One_handed_Axe:
                    case WeaponType.One_handed_Sword:
                        attackSprite = "stabO";
                        break;
                    case WeaponType.Two_handed_Sword:
                        attackSprite = "stabT";
                        break;
                    case WeaponType.Two_handed_Axe:
                    case WeaponType.Two_handed_Spear:
                        attackSprite = "stabP";
                        break;
                }

                switch (XNA_ScreenManager.ScreenClasses.MainClasses.ResourceManager.randomizer.Instance.generateRandom(0, 3))
                {
                    case 0:
                        attackSprite += "1_";
                        maxspriteframe = 1;
                        break;
                    case 1:
                        attackSprite += "2_";
                        maxspriteframe = 1;
                        break;
                    case 2:
                        attackSprite += "F_";
                        maxspriteframe = 2;
                        break;
                }
            }
            else
            {
                state = EntityState.Swing;

                switch (weapontype)
                {
                    case WeaponType.Dagger:
                    case WeaponType.One_handed_Axe:
                    case WeaponType.One_handed_Sword:
                        attackSprite = "swingO";
                        break;
                    case WeaponType.Two_handed_Sword:
                        attackSprite = "swingT";
                        break;
                    case WeaponType.Two_handed_Axe:
                    case WeaponType.Two_handed_Spear:
                        attackSprite = "swingP";
                        break;
                }

                switch (XNA_ScreenManager.ScreenClasses.MainClasses.ResourceManager.randomizer.Instance.generateRandom(0, 3))
                {
                    case 0:
                        attackSprite += "1_";
                        maxspriteframe = 2;
                        break;
                    case 1:
                        attackSprite += "2_";
                        maxspriteframe = 2;
                        break;
                    case 2:
                        attackSprite += "F_";
                        maxspriteframe = 3;
                        break;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                DrawSpriteFrame(spriteBatch); // display spriteframe

                for (int i = 0; i < spritepath.Length; i++)
                {
                    Color drawcolor = Color.White;
                    Texture2D drawsprite = null;
                    Vector2 drawPosition = Vector2.Zero;
                    Vector2 sprCorrect = Vector2.Zero;

                    try
                    {
                        if (getspritepath(i) != null && 
                            getoffset(i) != Vector2.Zero && // getoffset(i) = (does sprite exist, if not then skip)
                            (i != 3 || getPlayer().equipment.item_list.FindAll(x=> x.Slot == ItemSlot.Headgear).Count == 0)) // when headgear equiped, skip hairsprite
                        {
                            Vector2 debugvector = getoffset(i);

                            // load texture into sprite from Content Manager
                            drawsprite = Content.Load<Texture2D>(getspritepath(i) + spritename);

                            // Calculate position based on spriteEffect
                            if (spriteEffect == SpriteEffects.None)
                                drawPosition.X = (int)Position.X + (int)getoffset(i).X + 35;
                            else
                                drawPosition.X = (int)Position.X + (int)Math.Abs(getoffset(i).X) - drawsprite.Width + 25;

                            // give skin color to head, hands and torso sprite
                            if (i == 0 || i == 1 || i == 9)
                                drawcolor = getPlayer().skin_color;

                            // give hair color to hairset sprite
                            if (i == 3)
                                drawcolor = getPlayer().hair_color;

                            // draw player sprite
                            spriteBatch.Draw(drawsprite,
                                new Rectangle(
                                    (int)drawPosition.X, //+ (int)sprCorrect.X,
                                    (int)Position.Y + (int)getoffset(i).Y + 78, //+ (int)spriteCorrect(i, drawsprite).Y,
                                    drawsprite.Width, 
                                    drawsprite.Height),
                                new Rectangle(
                                    0,
                                    (int)spriteCorrect(i, drawsprite).Y, 
                                    drawsprite.Width, 
                                    drawsprite.Height),
                                drawcolor * this.transperancy, 0f, Vector2.Zero, spriteEffect, 0f);
                        }
                    }
                    catch(Exception ee)
                    {
                        string exception = ee.ToString();
                        string error = "Cannot find " + getspritepath(i) + spritename + "!" ;
                        throw new Exception(error);
                    }
                }
            }
        }

        protected void DrawSpriteFrame(SpriteBatch spriteBatch)
        {
            if (this.debug)
            {
                Texture2D rect = new Texture2D(ResourceManager.GetInstance.gfxdevice, (int)Math.Abs(SpriteFrame.Width), (int)SpriteFrame.Height);

                Color[] data = new Color[(int)Math.Abs(SpriteFrame.Width) * (int)SpriteFrame.Height];
                for (int i = 0; i < data.Length; ++i) data[i] = Color.Red;
                rect.SetData(data);

                spriteBatch.Draw(rect, new Rectangle((int)SpriteFrame.X, (int)SpriteFrame.Y,
                         (int)Math.Abs(SpriteFrame.Width), (int)SpriteFrame.Height),
                         SpriteFrame, Color.White * 0.5f, 0, Vector2.Zero, spriteEffect, 0f);
            }
        }

        public override Rectangle SpriteFrame
        {
            get 
            {
                return new Rectangle((int)Position.X + 5, (int)Position.Y + 5, 50, 75);
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
            return false;
        }
                
        public bool CheckKey(Microsoft.Xna.Framework.Input.Keys theKey)
        {
            KeyboardState keyboardStateCurrent = Keyboard.GetState();
            return keyboardStatePrevious.IsKeyDown(theKey) && keyboardStateCurrent.IsKeyUp(theKey);
        }

        public bool HoldPosition
        { 
            get 
            {
                if (XNA_ScreenManager.ScreenClasses.ScreenManager.Instance.actionScreen.hud.chatbarInput.Active)
                    return true;
                if (holdPosition)
                    return true;
                else
                    return false;
            }
            set { holdPosition = value; }
        }

        #region load spriteoffset
        protected Vector2 spriteCorrect(int spriteID, Texture2D spr)
        {
            Vector2 sprCorrect = Vector2.Zero;

            // if hatgear equiped, reduce hairsprite
            if (spriteID == 3 && getspritepath(7) != null)
            {
                if (spritename.StartsWith("ladder") || spritename.StartsWith("rope"))
                    sprCorrect.Y = spr.Height * 0.75f;
                else
                    sprCorrect.Y = spr.Height * 0.20f;
            }

            return sprCorrect;
        }              

        public virtual Vector2 getoffset(int spriteID)
        {
            PlayerInfo player = getPlayer();
            Equipment equipment = getPlayer().equipment;

            if (spriteID < 4 || spriteID == 9 )
            {
                if (getPlayer().list_offsets.FindAll(x => x.ID == spriteID).Count == 0)
                    loadoffsetfromXML(spriteID); // load from XML

                if (getPlayer().list_offsets.FindAll(x => x.ID == spriteID && x.Name == spritename + ".png").Count > 0)
                {
                    return new Vector2(getPlayer().list_offsets.Find(x => x.Name == spritename.ToString() + ".png" && x.ID == spriteID).X,
                                       getPlayer().list_offsets.Find(x => x.Name == spritename.ToString() + ".png" && x.ID == spriteID).Y);
                }
                else
                    return Vector2.Zero; // the sprite simply does not exist (e.g. hands for ladder and rope are disabled)
            }
            else if (spriteID == 4) // get the Armor Sprite information
            {
                if (equipment.item_list.FindAll(x => x.Slot == ItemSlot.Bodygear).Count > 0)
                {
                    if (getPlayer().equipment.item_list.Find(x => x.Slot == ItemSlot.Bodygear).list_offsets.FindAll(y => y.Name == spritename.ToString() + ".png").Count > 0)
                    {
                        Item item = getPlayer().equipment.item_list.Find(x => x.Slot == ItemSlot.Bodygear);
                        int X = item.list_offsets.Find(y => y.Name == spritename.ToString() + ".png").X;
                        int Y = item.list_offsets.Find(y => y.Name == spritename.ToString() + ".png").Y;
                        return new Vector2(X, Y);
                    }
                }
            }
            else if (spriteID == 7) // get the Headgear Sprite information
            {
                if (getPlayer().equipment.item_list.FindAll(x => x.Slot == ItemSlot.Headgear).Count > 0)
                {
                    if (getPlayer().equipment.item_list.Find(x => x.Slot == ItemSlot.Headgear).list_offsets.FindAll(y => y.Name == spritename.ToString() + ".png").Count > 0)
                    {
                        Item item = getPlayer().equipment.item_list.Find(x => x.Slot == ItemSlot.Headgear);
                        int X = item.list_offsets.Find(y => y.Name == spritename.ToString() + ".png").X;
                        int Y = item.list_offsets.Find(y => y.Name == spritename.ToString() + ".png").Y;
                        return new Vector2(X, Y);
                    }
                }
            }
            else if (spriteID == 8) // get the Weapon Sprite information
            {
                if (getPlayer().equipment.item_list.FindAll(x => x.Slot == ItemSlot.Weapon).Count > 0)
                {
                    if (getPlayer().equipment.item_list.Find(x => x.Slot == ItemSlot.Weapon).list_offsets.FindAll(y => y.Name == spritename.ToString() + ".png").Count > 0)
                    {
                        Item item = getPlayer().equipment.item_list.Find(x => x.Slot == ItemSlot.Weapon);
                        int X = item.list_offsets.Find(y => y.Name == spritename.ToString() + ".png").X;
                        int Y = item.list_offsets.Find(y => y.Name == spritename.ToString() + ".png").Y;
                        return new Vector2(X, Y);
                    }
                }
            }

            return Vector2.Zero;
        }

        public void clearoffset(int spriteID)
        {
            // fill list with XML structures
            getPlayer().list_offsets.RemoveAll(x => x.ID == spriteID);
        }

        protected void loadoffsetfromXML(int spriteID)
        {
            List<string> attribute = new List<string>();

            if (getspritepath(spriteID) != null)
            {
                using (var reader = new StreamReader(Path.Combine(Content.RootDirectory + "\\" + getspritepath(spriteID), "data.xml")))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(' ');

                        try
                        {
                            if (values[0] != "<i>")
                            {
                                for (int i = 0; i < values.Length; i++)
                                {
                                    if (values[i].StartsWith("image="))
                                    {
                                        char[] arrstart = new char[] { 'i', 'm', 'a', 'g', 'e', '=', '"' };
                                        char[] arrend = new char[] { '"' };
                                        string result = values[i].TrimStart(arrstart);
                                        result = result.TrimEnd(arrend);
                                        attribute.Add(result);
                                    }
                                    else if (values[i].StartsWith("x="))
                                    {
                                        char[] arrstart = new char[] { 'x', '=', '"' };
                                        char[] arrend = new char[] { '"' };
                                        string result = values[i].TrimStart(arrstart);
                                        result = result.TrimEnd(arrend);
                                        attribute.Add(result);
                                    }
                                    else if (values[i].StartsWith("y="))
                                    {
                                        char[] arrstart = new char[] { 'y', '=', '"' };
                                        char[] arrend = new char[] { '"', '\\', '"', '/', '>' };
                                        string result = values[i].TrimStart(arrstart);
                                        result = result.TrimEnd(arrend);
                                        attribute.Add(result);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string ee = ex.ToString();
                        }
                    }
                }

                // fill list with XML structures
                getPlayer().list_offsets.RemoveAll(x => x.ID == spriteID);

                for (int i = 0; i < attribute.Count; i++)
                {
                    if (attribute[i].EndsWith(".png"))
                    {
                        getPlayer().list_offsets.Add(new spriteOffset(
                                spriteID,
                                attribute[i].ToString(),
                                Convert.ToInt32(attribute[i + 1]),
                                Convert.ToInt32(attribute[i + 2])));
                    }
                }
            }
        }

        protected virtual string getspritepath(int spriteID)
        {
            PlayerInfo player = getPlayer();

            switch (spriteID)
            {
                case 0:
                    return player.head_sprite;
                case 1:
                    return player.body_sprite;
                case 2:
                    return player.faceset_sprite;
                case 3:
                    return player.hair_sprite;
                case 4:
                    return player.costume_sprite;
                // accessory 5 and 6 comes later...
                case 7:
                    return player.hatgear_sprite;
                case 8:
                    return player.weapon_sprite;
                case 9:
                    return player.hands_sprite;
            }
            return null;
        }
        #endregion
    }

    [Serializable]
    public struct spriteOffset
    {
        public string Name;
        public int ID, X, Y;

        public spriteOffset(int id, string name, int x, int y)
        {
            this.ID = id;
            this.Name = name;
            this.X = x;
            this.Y = y;
        }
    }
}
