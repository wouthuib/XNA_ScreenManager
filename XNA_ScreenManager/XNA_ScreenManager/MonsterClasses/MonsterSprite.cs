using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.PlayerClasses;

namespace XNA_ScreenManager.CharacterClasses
{
    public class MonsterSprite : Entity
    {
        #region properties

        // static randomizer
        randomizer Randomizer = randomizer.Instance;                                                // generate unique random ID
        PlayerInfo PlayerInfo = PlayerInfo.Instance;                                                // get battle information of player
        GameWorld world;

        // Drawing properties
        private int spriteWidth = 90;
        private int spriteHeight = 90;
        private Vector2 spriteOfset = new Vector2(90, 0);
        private SpriteEffects spriteEffect = SpriteEffects.None;
        private int damage = 0;
        private float transperancy = 0;

        // Respawn properties
        private Vector2 resp_pos = Vector2.Zero,                                                    // Respawn Position
                        resp_bord = Vector2.Zero;                                                   // Walking Border
        private bool spawn = false;                                                                 // Spawn Activator
        private int RESPAWN_TIME = 8;                                                               // 8 seconds respawn

        // Sprite Animation Properties
        Color color = Color.White;                                                                  // Sprite color
        private Vector2 Direction = Vector2.Zero;                                                   // Sprite Move direction
        private float Speed;                                                                        // Speed used in functions
        private bool ani_forward = true;                                                            // if we play for or backward

        // Movement properties
        const int WALK_SPEED = 100;                                                                 // The actual speed of the entity
        const int ANIMATION_SPEED = 120;                                                            // Animation speed, 120 = default
        const int IDLE_TIME = 10;                                                                   // idle time until next movement
        Border Borders = new Border(0, 0);                                                          // max tiles to walk from center (avoid falling)

        // Clocks and Timers
        float previousAnimateTimeSec,                                                               // Animation in Miliseconds
              previousWalkTimeSec,                                                                  // WalkTime in Seconds
              previousIdleTimeSec,                                                                  // IdleTime in Seconds
              previousHitTimeSec,                                                                   // IdleTime in Seconds
              previousDiedTimeSec,                                                                  // IdleTime in Seconds
              previousSpawnTimeSec,                                                                 // IdleTime in Seconds
              previousAttackTimeSec = 0,                                                            // IdleTime in Seconds
              currentAttackTimeSec = 0;                                                             // IdleTime in Seconds

        #endregion

        public MonsterSprite(Texture2D texture, Vector2 position, Vector2 borders)
            : base()
        {
            // Derived properties
            Active = true;
            sprite = texture;
            SpriteFrame = new Rectangle((int)spriteOfset.X, (int)spriteOfset.Y, spriteWidth, spriteHeight);
            Position = position;
            OldPosition = position;
            entityType = EntityType.Monster;

            // Save for respawning
            resp_pos = position;
            resp_bord = borders;

            // temporary parameters these should eventually be imported from the Monster Database
            HP = 1500; MP = 0; ATK = 60; DEF = 30; LVL = 1; HIT = 60; FLEE = 5;

            // Local properties
            Direction = new Vector2();                                                              // Move direction
            state = EntityState.Spawn;                                                              // Player state
            Borders = new Border(borders.X, borders.Y);                                             // Max Tiles from center
        }

        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                update_movement(gameTime);
                update_animation(gameTime);
                update_collision(gameTime);
            }
        }

        private void update_movement(GameTime gameTime)
        {
            switch (state)
            {
                case EntityState.Stand:

                    // reduce timer
                    previousIdleTimeSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (previousIdleTimeSec <= 0)
                    {
                        previousWalkTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds + Randomizer.generateRandom(6, 12);

                        // temporary random generator
                        if (Randomizer.generateRandom(0, 2) == 1)
                            spriteEffect = SpriteEffects.None;
                        else
                            spriteEffect = SpriteEffects.FlipHorizontally;

                        // reset sprite frame and change state
                        spriteFrame.X = 0;
                        state = EntityState.Walk;
                    }

                    break;

                case EntityState.Walk:

                    // reduce timer
                    previousWalkTimeSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (previousWalkTimeSec <= 0)
                    {
                        previousIdleTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds + Randomizer.generateRandom(6, 12);

                        // reset sprite frame and change state
                        spriteFrame.X = 0;
                        state = EntityState.Stand;
                    }

                    break;
            }
        }

        private void update_animation(GameTime gameTime)
        {
            switch (state)
            {
                #region stand
                case EntityState.Stand:

                    Speed = 0;
                    Direction = Vector2.Zero;

                    // Check if Monster is steady standing
                    if (Position.Y > OldPosition.Y)
                        state = EntityState.Falling;

                    // Move the Monster
                    OldPosition = Position;

                    // monster animation
                    spriteOfset = new Vector2(0, 0);
                    spriteFrame.Y = Convert.ToInt32(spriteOfset.Y);

                    // reduce timer
                    previousAnimateTimeSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (previousAnimateTimeSec <= 0)
                    {
                        previousAnimateTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;

                        if (ani_forward)
                        {
                            spriteFrame.X += spriteWidth;
                            if (spriteFrame.X > spriteOfset.X + (spriteWidth * 2))
                            {
                                spriteFrame.X = (int)spriteOfset.X + (spriteWidth * 2);
                                ani_forward = false;
                            }
                        }
                        else
                        {
                            spriteFrame.X -= spriteWidth;
                            if (spriteFrame.X < spriteOfset.X)
                            {
                                spriteFrame.X = (int)spriteOfset.X;
                                ani_forward = true;
                            }
                        }
                        
                    }

                    // Apply Gravity 
                    Position += new Vector2(0, 1) * 200 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    break;
                #endregion
                #region walk
                case EntityState.Walk:

                    Speed = 0;
                    Direction = Vector2.Zero;

                    if (spriteEffect == SpriteEffects.FlipHorizontally)
                    {
                        // walk right
                        this.Direction.X = 1;
                        this.Speed = WALK_SPEED;
                    }
                    else if (spriteEffect == SpriteEffects.None)
                    {
                        // walk left
                        this.Direction.X = -1;
                        this.Speed = WALK_SPEED;
                    }

                    // monster animation
                    spriteOfset = new Vector2(0, 90);
                    spriteFrame.Y = Convert.ToInt32(spriteOfset.Y);

                    // reduce timer
                    previousAnimateTimeSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (previousAnimateTimeSec <= 0)
                    {
                        previousAnimateTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;

                        if (ani_forward)
                        {
                            spriteFrame.X += spriteWidth;
                            if (spriteFrame.X > spriteOfset.X + (spriteWidth * 3))
                            {
                                spriteFrame.X = (int)spriteOfset.X + (spriteWidth * 3);
                                ani_forward = false;
                            }
                        }
                        else
                        {
                            spriteFrame.X -= spriteWidth;
                            if (spriteFrame.X < spriteOfset.X)
                            {
                                spriteFrame.X = (int)spriteOfset.X;
                                ani_forward = true;
                            }
                        }
                    }

                    // Check if monster is steady standing
                    if (Position.Y > OldPosition.Y && collideSlope == false)
                        state = EntityState.Falling;

                    // Update the Position Monster
                    OldPosition = Position;

                    // Walk speed
                    Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Apply Gravity 
                    Position += new Vector2(0, 1) * 200 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Walking Border for monster
                    if (Position.X <= Borders.Min)
                    {
                        Position = OldPosition;
                        spriteEffect = SpriteEffects.FlipHorizontally;
                    }
                    else if(Position.X >= Borders.Max)
                    {
                        Position = OldPosition;
                        spriteEffect = SpriteEffects.None;
                    }

                    break;
                #endregion
                #region falling
                case EntityState.Falling:

                    if (OldPosition.Y < position.Y)
                    {
                        // Move the Character
                        OldPosition = Position;

                        // Apply Gravity 
                        Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else
                       state = EntityState.Stand;

                    break;
                #endregion
                #region hit
                case EntityState.Hit:
                    
                    // Start freeze timer 
                    previousHitTimeSec = (int)gameTime.ElapsedGameTime.TotalSeconds + 0.7f;

                    // Check of world instance is created
                    if (world == null)
                        world = GameWorld.GetInstance;

                    // Start damage controll
                    damage = Battle.battle_calc_damage(PlayerInfo, this);
                    this.HP -= damage;

                    // create a damage baloon
                    world.createEffects(EffectType.DamageBaloon, new Vector2((this.position.X + this.SpriteFrame.Width * 0.45f) - damage.ToString().Length * 5,
                                                             this.position.Y + this.SpriteFrame.Height * 0.20f), SpriteEffects.None, damage);

                    // change state (freeze or kill)
                    if (this.HP <= 0)
                    {
                        // Monster respawn timer
                        previousDiedTimeSec = (int)gameTime.ElapsedGameTime.TotalSeconds + RESPAWN_TIME;

                        // Monster Item Drops
                        world.createEffects(EffectType.ItemSprite, 
                        new Vector2(Randomizer.generateRandom((int)this.position.X + 20, (int)this.position.X + this.spriteFrame.Width - 20),
                                    (int)(this.position.Y + this.spriteFrame.Height * 0.70f)), SpriteEffects.None, Randomizer.generateRandom(1200, 1210));

                        // Change state monster
                        state = EntityState.Died;
                    }
                    else
                        state = EntityState.Frozen;

                    break;
                #endregion
                #region frozen
                case EntityState.Frozen:

                    // Apply Gravity 
                    Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // monster animation
                    spriteOfset = new Vector2(0, 180);
                    spriteFrame.X = Convert.ToInt32(spriteOfset.X);
                    spriteFrame.Y = Convert.ToInt32(spriteOfset.Y);

                    // reduce timer
                    previousHitTimeSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (previousHitTimeSec <= 0)
                    {
                        // reset sprite frame
                        spriteFrame.X = 0;
                        state = EntityState.Stand;
                    }
                    break;
                #endregion
                #region died
                case EntityState.Died:

                    // Apply Gravity 
                    Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // monster animation
                    spriteOfset = new Vector2(0, 270);
                    spriteFrame.Y = Convert.ToInt32(spriteOfset.Y);

                    // Monster fades away
                    transperancy -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // reduce timer
                    previousAnimateTimeSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (previousAnimateTimeSec <= 0)
                    {
                        previousAnimateTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;

                        spriteFrame.X += spriteWidth;

                        if (spriteFrame.X > spriteOfset.X + (spriteWidth * 3))
                        {
                            spriteFrame.X = (int)spriteOfset.X + (spriteWidth * 3);
                        }
                    }

                    // reduce timer
                    previousDiedTimeSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // removing counter
                    if (previousDiedTimeSec <= 0)
                    {
                        // link to world
                        if (world == null)
                            world = GameWorld.GetInstance;

                        // respawn a new monster
                        world.createMonster(sprite, resp_pos, (int)resp_bord.X, (int)resp_bord.Y);

                        // remove monster from map
                        this.keepAliveTime = 0;
                    }
                    break;
                #endregion
                #region spawn
                case EntityState.Spawn:

                    // Apply Gravity 
                    Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Monster fadesin
                    if (transperancy < 1)
                        transperancy += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // reduce timer
                    previousSpawnTimeSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Monster Spawn Timer
                    if (previousSpawnTimeSec <= 0)
                    {
                        if (spawn)
                        {
                            transperancy = 1;
                            state = EntityState.Stand;
                        }
                        else
                        {
                            spawn = true;
                            previousSpawnTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds + 1.1f;
                        }
                    }

                    break;
                #endregion
            }
        }

        private void update_collision(GameTime gameTime)
        {
            // Check of world instance is created
            if (world == null)
                world = GameWorld.GetInstance;

            previousAttackTimeSec = currentAttackTimeSec;

            // check for monster collisions
            foreach (Entity player in world.listEntity)
            {
                if (player.EntityType == EntityType.Player)
                {
                    if (new Rectangle(
                            (int)(player.Position.X + player.SpriteFrame.Width * 0.40f),
                            (int)player.Position.Y,
                            (int)(player.SpriteFrame.Width * 0.30f),
                            (int)player.SpriteFrame.Height).
                        Intersects(new Rectangle(
                            (int)(this.Position.X + this.SpriteFrame.Width * 0.40f), 
                            (int)this.Position.Y,
                            (int)(this.SpriteFrame.Width * 0.30f), 
                            (int)this.SpriteFrame.Height)) == true)
                    {
                        // player + monster state not equal to hit or frozen
                        if (this.State != EntityState.Hit &&
                            this.State != EntityState.Died &&
                            this.State != EntityState.Spawn &&
                            player.State != EntityState.Hit &&
                            player.State != EntityState.Frozen)
                        {
                            // activate timer
                            currentAttackTimeSec += (float)gameTime.ElapsedGameTime.TotalSeconds;

                            // we now use 800 msec, but this should get a ASPD timer
                            if (currentAttackTimeSec >= 0.8f)
                            {
                                // reset the attach timer
                                currentAttackTimeSec = 0;

                                // Start damage controll
                                int damage = (int)Battle.battle_calc_damage_mob(this, PlayerInfo.Instance);
                                player.HP -= damage;

                                // Hit the player
                                if (damage > 0)
                                    player.State = EntityState.Hit;

                                // create a damage baloon
                                world.createEffects(EffectType.DamageBaloon, new Vector2((player.Position.X + player.SpriteFrame.Width * 0.45f) - damage.ToString().Length * 5,
                                                                         player.Position.Y + player.SpriteFrame.Height * 0.20f), SpriteEffects.None, damage, 2);
                            }
                        }
                    }
                }
            }

            // reset timer when no player collision
            if (currentAttackTimeSec == previousAttackTimeSec)
            {
                // monster gets hit will not reset the timer
                if(this.state != EntityState.Hit)
                    currentAttackTimeSec = 0;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
                spriteBatch.Draw(sprite, new Rectangle((int)Position.X, (int)Position.Y, SpriteFrame.Width, SpriteFrame.Height),
                    SpriteFrame, Color.White * transperancy, 0f, Vector2.Zero, spriteEffect, 0f);
        }

        private struct Border
        {
            // Structure for monster walking bounds
            public float Min, Max;

            public Border(float min, float max)
            {
                Min = min * 32;
                Max = max * 32;
            }
        }
    }

    // Singleton randomizer to provide unique values
    public class randomizer
    {
        private static randomizer instance;
        private Random rand;

        private randomizer()
        {
            rand = new Random();
        }

        public int generateRandom(int min, int max)
        {
            return rand.Next(min, max);
        }

        public static randomizer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new randomizer();
                }
                return instance;
            }
        }
    }
}