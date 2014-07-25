using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.CharacterClasses;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.GameWorldClasses.Entities;
using XNA_ScreenManager.ItemClasses;

namespace XNA_ScreenManager.PlayerClasses
{
    public class NetworkPlayerSprite : PlayerSprite
    {
        #region properties and constructor
        public string Name, MapName;
        private Vector2 previousPosition;
        private float previousGameTimeMsec;

        private const int PLAYER_SPEED = 200;                                                     // The actual speed of the player
        private const int ANIMATION_SPEED = 120;                                                  // Animation speed, 120 = default 
        private const int MOVE_UP = -1;                                                           // player moving directions
        private const int MOVE_DOWN = 1;                                                          // player moving directions
        private const int MOVE_LEFT = -1;                                                         // player moving directions
        private const int MOVE_RIGHT = 1;                                                         // player moving directions

        public NetworkPlayerSprite(
            string name,
            int positionX,
            int positionY,
            string _spritename,
            string _spritestate,
            int _prevspriteframe,
            int _maxspriteframe,
            string _attackSprite,
            string _spriteEffect,
            string mapName,
            string skincolor,
            string facesprite,
            string hairsprite,
            string haircolor
            ) 
            : base(positionX, positionX)
        {
            Name = name;
            Position = new Vector2(positionX, positionY);
            spritename = _spritename;
            state = (EntityState)Enum.Parse(typeof(EntityState), _spritestate);
            prevspriteframe = _prevspriteframe;
            maxspriteframe = _maxspriteframe;
            attackSprite = _attackSprite;
            spriteEffect = (SpriteEffects)Enum.Parse(typeof(SpriteEffects), _spriteEffect);
            MapName = mapName;
            
            this.Player = new PlayerInfo();

            try
            {
                //this.Player.skin_color = (Color)Enum.Parse(typeof(Color), skincolor);
                this.Player.faceset_sprite = facesprite;
                this.Player.hair_sprite = hairsprite;
                //this.Player.hair_color = (Color)Enum.Parse(typeof(Color), facesprite);
            }
            catch
            {
                throw new Exception("something is wrong");
            }

        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            previousPosition = this.position;   // save previous postion
            previousState = this.state;         // save previous state before

            #region update from server
            for (int i = 0; i < NetworkPlayerStore.Instance.playersprites.Length; i++)
            {
                NetworkPlayerSprite entry = NetworkPlayerStore.Instance.playersprites[i];

                if (entry != null)
                {
                    if (entry.Name == this.Name)
                    {
                        if (state != (EntityState)Enum.Parse(typeof(EntityState), NetworkPlayerStore.Instance.playerlist[i].spritestate) ||
                            spriteEffect != (SpriteEffects)Enum.Parse(typeof(SpriteEffects), NetworkPlayerStore.Instance.playerlist[i].spriteEffect))
                        {
                            Position = new Vector2(NetworkPlayerStore.Instance.playerlist[i].PositionX, NetworkPlayerStore.Instance.playerlist[i].PositionY);
                            spritename = NetworkPlayerStore.Instance.playerlist[i].spritename;                            
                            prevspriteframe = NetworkPlayerStore.Instance.playerlist[i].prevspriteframe;
                            maxspriteframe = NetworkPlayerStore.Instance.playerlist[i].maxspriteframe;
                            attackSprite = NetworkPlayerStore.Instance.playerlist[i].attackSprite;
                            spriteEffect = (SpriteEffects)Enum.Parse(typeof(SpriteEffects), NetworkPlayerStore.Instance.playerlist[i].spriteEffect);
                            MapName = NetworkPlayerStore.Instance.playerlist[i].mapName;
                            state = (EntityState)Enum.Parse(typeof(EntityState), NetworkPlayerStore.Instance.playerlist[i].spritestate);

                            //this.Player.skin_color = (Color)Enum.Parse(typeof(Color), NetworkPlayerStore.Instance.playerlist[i].skincol);
                            this.Player.faceset_sprite = NetworkPlayerStore.Instance.playerlist[i].facespr;
                            this.Player.hair_sprite = NetworkPlayerStore.Instance.playerlist[i].hairspr;
                            //this.Player.hair_color = (Color)Enum.Parse(typeof(Color), NetworkPlayerStore.Instance.playerlist[i].hailcol);

                            spriteframe = 0;
                        }
                    }
                }
            }
            #endregion

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
                                
                                world.newEffect.Add(new WeaponSwing(pos, WeaponSwingType.Swing01, spriteEffect));
                            }
                            else
                            {
                                Vector2 pos = new Vector2(this.Position.X - this.SpriteFrame.Width * 0.6f, this.Position.Y + this.SpriteFrame.Height * 0.7f);
                                
                                world.newEffect.Add(new WeaponSwing(pos, WeaponSwingType.Swing01, spriteEffect));
                            }

                            state = EntityState.Cooldown;
                        }
                    }

                    break;
                #endregion
                #region state stabbing
                case EntityState.Stab:

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

                            // create stab effect
                            if (spriteEffect == SpriteEffects.FlipHorizontally)
                            {
                                Vector2 pos = new Vector2(this.Position.X + this.SpriteFrame.Width * 0.3f, this.Position.Y + this.SpriteFrame.Height * 0.7f);
                                
                                world.newEffect.Add(new WeaponSwing(pos, WeaponSwingType.Stab01, spriteEffect));
                            }
                            else
                            {
                                Vector2 pos = new Vector2(this.Position.X - this.SpriteFrame.Width * 0.7f, this.Position.Y + this.SpriteFrame.Height * 0.7f);
                                
                                world.newEffect.Add(new WeaponSwing(pos, WeaponSwingType.Stab01, spriteEffect));
                            }

                            state = EntityState.Cooldown;
                        }
                    }

                    // Apply Gravity 
                    // Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

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

                        if (spriteframe > 2)
                        {
                            // make sure the world is connected
                            if (world == null)
                                world = GameWorld.GetInstance;

                            // create and release an arrow
                            //if (spriteEffect == SpriteEffects.FlipHorizontally)
                            //    world.newEffect.Add(new Arrow(Content.Load<Texture2D>(@"gfx\gameobjects\arrow"),
                            //        new Vector2(this.Position.X, this.Position.Y + this.SpriteFrame.Height * 0.6f),
                            //        800, new Vector2(1, 0), Vector2.Zero));
                            //else
                            //    world.newEffect.Add(new Arrow(Content.Load<Texture2D>(@"gfx\gameobjects\arrow"),
                            //        new Vector2(this.Position.X, this.Position.Y + this.SpriteFrame.Height * 0.6f),
                            //        800, new Vector2(-1, 0), Vector2.Zero));

                            // Set the timer for cooldown
                            previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;

                            // reset sprite frame and change state
                            // start cooldown
                            spriteFrame.X = 0;
                            state = EntityState.Cooldown;
                        }
                    }

                    // Apply Gravity 
                    // Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    break;
                #endregion
                #region state sit
                case EntityState.Sit:

                    Speed = 0;
                    Direction = Vector2.Zero;
                    Velocity = Vector2.Zero;
                                        
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
                    // Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    break;
                #endregion
                #region state Rope
                case EntityState.Rope:

                    Speed = 0;
                    Direction = Vector2.Zero;
                    Velocity = Vector2.Zero;
                    spriteEffect = SpriteEffects.None;

                    if (previousPosition.Y < position.Y)
                    {
                        // move player location (make ActiveMap tile check here in the future)
                        this.Direction.Y = MOVE_DOWN;
                        this.Speed = PLAYER_SPEED * 0.75f;

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
                    else if (previousPosition.Y > position.Y)
                    {
                        // move player location (make ActiveMap tile check here in the future)
                        this.Direction.Y = MOVE_UP;
                        this.Speed = PLAYER_SPEED * 0.75f;

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
                    // Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    break;
                #endregion
                #region state Ladder
                case EntityState.Ladder:

                    Speed = 0;
                    Direction = Vector2.Zero;
                    Velocity = Vector2.Zero;
                    spriteEffect = SpriteEffects.None;

                    if (previousPosition.Y < position.Y)
                    {
                        // move player location (make ActiveMap tile check here in the future)
                        this.Direction.Y = MOVE_DOWN;
                        this.Speed = PLAYER_SPEED * 0.75f;

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
                    else if (previousPosition.Y > position.Y)
                    {
                        // move player location (make ActiveMap tile check here in the future)
                        this.Direction.Y = MOVE_UP;
                        this.Speed = PLAYER_SPEED * 0.75f;

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
                            spritename = "ladder_" + spriteframe.ToString();
                            playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                        }
                    }

                    // Move the Character
                    OldPosition = Position;

                    // Climb speed
                    // Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    break;
                #endregion
                #region state stand
                case EntityState.Stand:

                    Speed = 0;
                    Direction = Vector2.Zero;
                    Velocity = Vector2.Zero;
                    
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

                            if (weapon != null)
                            {
                                if (weapon.WeaponType == WeaponType.Two_handed_Axe ||
                                   weapon.WeaponType == WeaponType.Two_handed_Spear ||
                                   weapon.WeaponType == WeaponType.Two_handed_Sword)
                                    spritename = "stand2_" + spriteframe.ToString();
                                else
                                    spritename = "stand1_" + spriteframe.ToString();
                            }
                            else
                                spritename = "stand1_" + spriteframe.ToString();

                            playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                        }
                    }

                    break;
                #endregion
                #region state walk
                case EntityState.Walk:

                    Speed = 0;
                    Direction = Vector2.Zero;
                    Velocity = Vector2.Zero;

                    if (spriteEffect == SpriteEffects.FlipHorizontally)
                    {
                        // move player location (make ActiveMap tile check here in the future)
                        this.Direction.X = MOVE_RIGHT;
                        this.Speed = PLAYER_SPEED;
                    }
                    else if (spriteEffect == SpriteEffects.None)
                    {
                        // move player location (make ActiveMap tile check here in the future)
                        this.Direction.X = MOVE_LEFT;
                        this.Speed = PLAYER_SPEED;
                    }

                    // reduce timer
                    previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // set sprite frames
                    if (previousGameTimeMsec < 0)
                    {
                        previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
                        spriteframe++;
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

                    // Move the Character
                    OldPosition = Position;

                    // Walk speed
                    Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    break;
                #endregion
                #region state jump
                case EntityState.Jump:

                    if(previousState != this.state)
                        Velocity = new Vector2(0, -1.5f);
                    else
                        Velocity.Y += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (previousPosition.X > Position.X)
                    {
                        // move player location (make ActiveMap tile check here in the future)
                        this.Direction.X += MOVE_LEFT * 0.1f * ((float)gameTime.ElapsedGameTime.TotalSeconds * 10f);
                        this.Speed = PLAYER_SPEED;

                        if (this.Direction.X < -1)
                            this.Direction.X = -1;
                        else if (this.Direction.X < 0)
                            this.Direction.X = 0;
                    }
                    else if (previousPosition.X < Position.X)
                    {
                        // move player location (make ActiveMap tile check here in the future)
                        this.Direction.X += MOVE_RIGHT * 0.1f * ((float)gameTime.ElapsedGameTime.TotalSeconds * 10f);
                        this.Speed = PLAYER_SPEED;

                        if (this.Direction.X > 1)
                            this.Direction.X = 1;
                        else if (this.Direction.X > 0)
                            this.Direction.X = 0;
                    }

                    // Move the Character
                    OldPosition = Position;

                    // Player animation
                    for (int i = 0; i < spritepath.Length; i++)
                    {
                        spritename = "jump_0";
                        playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                    }

                    // Apply jumping
                    Position += Velocity * 350 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Apply Gravity 
                    Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Walk / Jump speed
                    Position += Direction * (Speed / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    break;
                #endregion
                #region state falling
                case EntityState.Falling:

                    if (spriteEffect == SpriteEffects.None)
                    {
                        // move player location (make ActiveMap tile check here in the future)
                        this.Direction.X += MOVE_LEFT * 0.1f * ((float)gameTime.ElapsedGameTime.TotalSeconds * 10f);
                        this.Speed = PLAYER_SPEED;

                        if (this.Direction.X < -1)
                            this.Direction.X = -1;
                        else if (this.Direction.X < 0)
                            this.Direction.X = 0;
                    }
                    else if (spriteEffect == SpriteEffects.FlipHorizontally)
                    {
                        // move player location (make ActiveMap tile check here in the future)
                        this.Direction.X += MOVE_RIGHT * 0.1f * ((float)gameTime.ElapsedGameTime.TotalSeconds * 10f);
                        this.Speed = PLAYER_SPEED;

                        if (this.Direction.X > 1)
                            this.Direction.X = 1;
                        else if (this.Direction.X > 0)
                            this.Direction.X = 0;
                    }

                    // Move the Character
                    OldPosition = Position;

                    // Apply Gravity (slightly lower than usual due to server delay)
                    Position += new Vector2(0, 1) * 200 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Walk / Jump speed
                    Position += Direction * (Speed / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds;

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

                    break;
                #endregion
                #region state frozen
                case EntityState.Frozen:

                    // Upward Position
                    Velocity.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * 2;

                    // Make player transperant
                    if (transperancy >= 0)
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

            previousState = this.state;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
