using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.CharacterClasses;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.ItemClasses;

namespace XNA_ScreenManager.PlayerClasses
{
    public class NetworkPlayerSprite : PlayerSprite
    {
        #region properties and constructor
        public string Name, MapName;
        public Vector2 previousPosition;
        private float previousGameTimeMsec;

        private const int MOVE_UP = -1;                                                           // player moving directions
        private const int MOVE_DOWN = 1;                                                          // player moving directions
        private const int MOVE_LEFT = -1;                                                         // player moving directions
        private const int MOVE_RIGHT = 1;                                                         // player moving directions

        private string
            armor_name,                 // Armor and Costume Sprite (4)
            //accessorry_top_name,        // Accessory top Sprite (Sunglasses, Ear rings) (5)
            //accessorry_bottom_name,     // Accessory bottom Sprite (mouth items, capes) (6)
            headgear_name,              // Headgear Sprite (Hats, Helmets) (7)
            weapon_name;                // Weapon Sprite (8)
            //hands_name;                 // Hands Sprite (9)

        public NetworkPlayerSprite(
            string name,
            string ip,
            float positionX,
            float positionY,
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
            string haircolor,
            string armor,
            string headgear,
            string weapon
            ) 
            : base(positionX, positionX)
        {
            Name = name;
            entityName = name; // used by removing instance in worldmap!!
            Position = new Vector2(positionX, positionY);
            if (_spritename != null)
                spritename = _spritename;
            if (_spritestate != null)
                state = (EntityState)Enum.Parse(typeof(EntityState), _spritestate);
            prevspriteframe = _prevspriteframe;
            maxspriteframe = _maxspriteframe;
            attackSprite = _attackSprite;
            if (_spriteEffect != null)
                spriteEffect = (SpriteEffects)Enum.Parse(typeof(SpriteEffects), _spriteEffect);
            else
                spriteEffect = SpriteEffects.FlipHorizontally;
            MapName = mapName;
            
            this.Player = new PlayerInfo(true); // make a networkplayer

            this.Player.skin_color = getColor(skincolor);
            this.Player.faceset_sprite = facesprite;
            this.Player.hair_sprite = hairsprite;
            this.Player.hair_color = getColor(haircolor);
            this.armor_name = armor;
            this.headgear_name = headgear;
            this.weapon_name = weapon;
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            //#region update from server
            //foreach (var entity in GameWorld.GetInstance.listEntity)
            //{
            //    if (entity is NetworkPlayerSprite)
            //    {
            //        NetworkPlayerSprite player = (NetworkPlayerSprite)entity;
            //        if (player.Name == this.Name)
            //        {
            //            int i = NetworkStoreID(this.Name); // get player index

            //            if (i >= 0 && // player index found
            //                (state != (EntityState)Enum.Parse(typeof(EntityState), NetworkPlayerStore.Instance.playerlist[i].spritestate) ||
            //                spriteEffect != (SpriteEffects)Enum.Parse(typeof(SpriteEffects), NetworkPlayerStore.Instance.playerlist[i].spriteEffect) ||
            //                ((state == EntityState.Ladder || state == EntityState.Rope) && Direction != getVector(NetworkPlayerStore.Instance.playerlist[i].direction))))
            //            {
            //                Position = new Vector2(NetworkPlayerStore.Instance.playerlist[i].PositionX, NetworkPlayerStore.Instance.playerlist[i].PositionY);
            //                //spritename = NetworkPlayerStore.Instance.playerlist[i].spritename;                            
            //                //prevspriteframe = NetworkPlayerStore.Instance.playerlist[i].prevspriteframe;
            //                //maxspriteframe = NetworkPlayerStore.Instance.playerlist[i].maxspriteframe;
            //                //attackSprite = NetworkPlayerStore.Instance.playerlist[i].attackSprite;
            //                //spriteEffect = (SpriteEffects)Enum.Parse(typeof(SpriteEffects), NetworkPlayerStore.Instance.playerlist[i].spriteEffect);
            //                //MapName = NetworkPlayerStore.Instance.playerlist[i].mapName;
            //                state = (EntityState)Enum.Parse(typeof(EntityState), NetworkPlayerStore.Instance.playerlist[i].spritestate);
            //                //Direction = getVector(NetworkPlayerStore.Instance.playerlist[i].direction);

            //                //this.Player.skin_color = getColor(NetworkPlayerStore.Instance.playerlist[i].skincol);
            //                //this.Player.faceset_sprite = NetworkPlayerStore.Instance.playerlist[i].facespr;
            //                //this.Player.hair_sprite = NetworkPlayerStore.Instance.playerlist[i].hairspr;
            //                //this.Player.hair_color = getColor(NetworkPlayerStore.Instance.playerlist[i].hailcol);
                            
            //                //this.armor_name = NetworkPlayerStore.Instance.playerlist[i].armor;
            //                //this.headgear_name = NetworkPlayerStore.Instance.playerlist[i].headgear;
            //                //this.weapon_name = NetworkPlayerStore.Instance.playerlist[i].weapon;

            //                //spriteframe = 0;

            //                //if(previousState == EntityState.Shoot && state != previousState)
            //                //{
            //                //    state = EntityState.Shoot;
            //                //    spriteframe = 2;
            //                //}
            //            }
            //        }
            //    }
            //}
            //#endregion

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
                                
                                //world.newEffect.Add(new WeaponSwing(pos, WeaponSwingType.Swing01, spriteEffect));
                            }
                            else
                            {
                                Vector2 pos = new Vector2(this.Position.X - this.SpriteFrame.Width * 0.6f, this.Position.Y + this.SpriteFrame.Height * 0.7f);
                                
                                //world.newEffect.Add(new WeaponSwing(pos, WeaponSwingType.Swing01, spriteEffect));
                            }

                            //state = EntityState.Cooldown;
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
                                //world.newEffect.Add(new WeaponSwing(pos, WeaponSwingType.Stab01, spriteEffect));
                            }
                            else
                            {
                                Vector2 pos = new Vector2(this.Position.X - this.SpriteFrame.Width * 0.7f, this.Position.Y + this.SpriteFrame.Height * 0.7f);                                
                                //world.newEffect.Add(new WeaponSwing(pos, WeaponSwingType.Stab01, spriteEffect));
                            }

                            //state = EntityState.Cooldown;
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

                        if (spriteframe > 1)
                        {
                            // Set the timer for cooldown
                            previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
                            spriteframe = 1;

                            //// make sure the world is connected
                            //if (world == null)
                            //    world = GameWorld.GetInstance;
                            //// create and release an arrow
                            //if (spriteEffect == SpriteEffects.FlipHorizontally)
                            //    world.newEffect.Add(new Arrow(Content.Load<Texture2D>(@"gfx\gameobjects\arrow"),
                            //        new Vector2(this.Position.X, this.Position.Y + this.SpriteFrame.Height * 0.6f),
                            //        800, new Vector2(1, 0), Vector2.Zero));
                            //else
                            //    world.newEffect.Add(new Arrow(Content.Load<Texture2D>(@"gfx\gameobjects\arrow"),
                            //        new Vector2(this.Position.X, this.Position.Y + this.SpriteFrame.Height * 0.6f),
                            //        800, new Vector2(-1, 0), Vector2.Zero));  
                            //state = EntityState.Cooldown;
                        }
                    }

                    //if(spriteframe == 2) // set by server update, see above
                    //    state = (EntityState)Enum.Parse(typeof(EntityState), Array.Find(NetworkPlayerStore.Instance.playerlist, p => p.Name == this.Name).spritestate);

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
                    Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    break;
                #endregion
                #region state Ladder
                case EntityState.Ladder:
                    
                    Velocity = Vector2.Zero;
                    spriteEffect = SpriteEffects.None;

                    if (Position.Y != previousPosition.Y)
                    {
                        spriteframe++;

                        // double check frame if previous state has higher X

                        if (spriteframe > 1)
                            spriteframe = 0;
                    }

                    // Player animation
                    if (prevspriteframe != spriteframe || !spritename.StartsWith("ladder_"))
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
                    Position += Direction * (PLAYER_SPEED * 0.75f) * (float)gameTime.ElapsedGameTime.TotalSeconds;

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
                            Item weapon = null;

                            if(weapon_name != null)
                                weapon = itemStore.item_list.Find(x=>x.itemName == weapon_name);

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

                    // Apply Gravity 
                    Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

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

                    // Apply Gravity 
                    Position += new Vector2(0, 1) * 250 * (float)gameTime.ElapsedGameTime.TotalSeconds;

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
            if (Active)
            {
                DrawSpriteFrame(spriteBatch); // display spriteframe

                for (int i = 0; i < spritepath.Length; i++)
                {
                    Color drawcolor = Color.White;
                    Texture2D drawsprite = null;
                    Vector2 drawPosition = Vector2.Zero;
                    Vector2 sprCorrect = Vector2.Zero;

                    string headgear2 = this.Player.hair_sprite;

                    try
                    {
                        if (getspritepath(i) != null &&
                            getoffset(i) != Vector2.Zero && // getoffset(i) = (does sprite exist, if not then skip)
                            (i != 3 || headgear_name == null)) // when headgear equiped, skip hairsprite
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
                                drawcolor = this.Player.skin_color;

                            // give hair color to hairset sprite
                            if (i == 3)
                                drawcolor = this.Player.hair_color;

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
                    catch (Exception ee)
                    {
                        string exception = ee.ToString();
                        string error = "Cannot find " + getspritepath(i) + spritename + "!";
                        throw new Exception(error);
                    }
                }
            }
        }

        public override Vector2 getoffset(int spriteID)
        {
            if (spriteID < 4 || spriteID == 9)
            {
                if (this.Player.list_offsets.FindAll(x => x.ID == spriteID).Count == 0)
                    loadoffsetfromXML(spriteID); // load from XML

                if (this.Player.list_offsets.FindAll(x => x.ID == spriteID && x.Name == spritename + ".png").Count > 0)
                {
                    if (spriteID == 3)
                    {
                        int xx = this.Player.list_offsets.Find(x => x.ID == spriteID && x.Name == spritename + ".png").X;
                        int yy = this.Player.list_offsets.Find(x => x.ID == spriteID && x.Name == spritename + ".png").Y;
                    }

                    return new Vector2(this.Player.list_offsets.Find(x => x.ID == spriteID && x.Name == spritename + ".png").X,
                                       this.Player.list_offsets.Find(x => x.ID == spriteID && x.Name == spritename + ".png").Y);
                }
                else
                    return Vector2.Zero; // the sprite simply does not exist (e.g. hands for ladder and rope are disabled)
            }
            else if (spriteID == 4) // get the Armor Sprite information
            {
                if (armor_name != null)
                    return getOffsetbyItemName(armor_name);
            }
            else if (spriteID == 7) // get the Headgear Sprite information
            {
                if (headgear_name != null)
                    return getOffsetbyItemName(headgear_name);
            }
            else if (spriteID == 8) // get the Weapon Sprite information
            {
                if (weapon_name != null)
                    return getOffsetbyItemName(weapon_name);
            }

            return Vector2.Zero;
        }

        protected override string getspritepath(int spriteID)
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
                    if (armor_name != null)
                        return itemStore.item_list.Find(x => x.itemName == armor_name).equipSpritePath;
                    else
                        return null;
                // accessory 5 and 6 comes later...
                case 7:
                    if (headgear_name != null)
                        return itemStore.item_list.Find(x => x.itemName == headgear_name).equipSpritePath;
                    else
                        return null;
                case 8:
                    if (weapon_name != null)
                        return itemStore.item_list.Find(x => x.itemName == weapon_name).equipSpritePath;
                    else
                        return null;
                case 9:
                    return player.hands_sprite;
            }
            return null;
        }

        private Vector2 getOffsetbyItemName(string name)
        {
            Item item = null;

            for (int i = 0; i < itemStore.item_list.Count; i++)
            {
                if (itemStore.item_list[i] != null)
                {
                    if (itemStore.item_list[i].itemName == name)
                    {
                        item = itemStore.item_list[i];
                        break;
                    }
                }
            }

            if (item != null && spritename != null)
            {
                for (int i = 0; i < item.list_offsets.Count; i++)
                {
                    if (item.list_offsets[i].Name == spritename.ToString() + ".png")
                        return new Vector2(item.list_offsets[i].X, item.list_offsets[i].Y);
                }
            }
            else
            {
                throw new Exception("item without name!!!");
            }

            return Vector2.Zero;
        }

        public static Color getColor(string colorcode)
        {
            string[] values = colorcode.Split(':');

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = values[i].Trim(new char[]{' ','R','G','B','A','{','}'});
            }

            return new Color(
                Convert.ToInt32(values[1]),
                Convert.ToInt32(values[2]), 
                Convert.ToInt32(values[3]));
        }

        public static Vector2 getVector(string vectorstr)
        {
            string[] values = vectorstr.Split(':');

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = values[i].Trim(new char[] { ' ', 'X' , 'Y' ,'{', '}' });
                values[i] = values[i].Replace('.', '&')
                                     .Replace(',', '.')
                                     .Replace('&', ',');
            }

            return new Vector2(
                float.Parse(values[1]),
                float.Parse(values[2]));
        }                
    }
}
