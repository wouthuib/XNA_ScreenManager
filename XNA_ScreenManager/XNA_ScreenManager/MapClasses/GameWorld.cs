using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Squared.Tiled;
using System.IO;
using XNA_ScreenManager.CharacterClasses;
using XNA_ScreenManager.ItemClasses;
using XNA_ScreenManager.PlayerClasses;
using XNA_ScreenManager.ScreenClasses;
using System.Threading;


namespace XNA_ScreenManager.MapClasses
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GameWorld// : DrawableGameComponent
    {
        #region properties
        // Game Services
        SpriteBatch spriteBatch = null;
        ContentManager Content;
        GraphicsDevice gfxdevice;

        // Keyboard- and Mousestate
        KeyboardState keyboardStateCurrent, keyboardStatePrevious;

        // Class Instances
        public Map map;
        public PlayerSprite playerSprite;
        Camera2d cam;

        // Static Classes
        PlayerInfo playerInfo = PlayerInfo.Instance;
        ItemStore itemStore = ItemStore.Instance;
        ScreenManager screenManager = ScreenManager.Instance;

        // Map entities
        Texture2D Background;
        List<Entity> listEntity = new List<Entity>();
        List<Effect> listEffect = new List<Effect>();

        // dynamic items
        ArrowProperties arrowprop = new ArrowProperties(false, null, new Vector2(0,0), 0 , new Vector2(0,0));

        public bool Active { get; set; }
        public bool Paused { get; set; }
        #endregion

        #region contructor

        private static GameWorld mInstance;
        private static System.Object _mutex = new System.Object();

        private GameWorld(Game game, Camera2d camref)
        {
            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            Content = (ContentManager)game.Services.GetService(typeof(ContentManager));
            gfxdevice = (GraphicsDevice)game.Services.GetService(typeof(GraphicsDevice));

            cam = camref;
            LoadObjects();
        }

        public static GameWorld CreateInstance(Game game, Camera2d camref)
        {
            lock (_mutex) // now I can claim some form of thread safety...
            {
                if (mInstance == null)
                {
                    mInstance = new GameWorld(game, camref);
                }
            }
            return mInstance;
        }

        public static GameWorld GetInstance
        {
            get
            {
                if (mInstance == null)
                {
                    throw new Exception("The GameWorld is called, but has not yet been created!!");
                }
                return mInstance;
            }
        }

        protected void LoadObjects()
        {
            map = Map.Load(Path.Combine(Content.RootDirectory, @"maps\victoria01.tmx"), Content);

            foreach (var property in map.Properties)
            {
                if (property.Key == "Background")
                    Background = Content.Load<Texture2D>(@"gfx\background\" + property.Value);
            }

            playerSprite = new PlayerSprite(Content.Load<Texture2D>(@"gfx\player\player_basic"),
                (int)map.ObjectGroups["Hero"].Objects["hero"].X,
                (int)map.ObjectGroups["Hero"].Objects["hero"].Y,
                new Vector2(map.TileWidth, map.TileHeight));
            listEntity.Add(playerSprite);

            LoadEntities();
            itemStore.loadItems("itemtable.bin");

            playerInfo.InitNewGame();

            //base.Initialize();
        }
        #endregion

        #region update
        public void Update(GameTime gameTime)
        {
            if (Active && !Paused)
            {
                // Update Player and other Entities
                foreach (Entity obj in listEntity)
                    obj.Update(gameTime);
                
                // Update Warp objects
                foreach (Effect obj in listEffect)
                    obj.Update(gameTime);

                // Update all enitities map collisions
                UpdateMapEntities(gameTime);

                // remove timeout entities
                for (int i = 0; i < listEntity.Count; i++)
                {
                    var obj = listEntity[i];

                    if (obj.KeepAliveTime != -1 && obj.KeepAliveTime <= (int)gameTime.TotalGameTime.Seconds)
                        listEntity.Remove(obj);
                }

                // remove timeout effects
                for (int i = 0; i < listEffect.Count; i++)
                {
                    var obj = listEffect[i];

                    if (obj.KeepAliveTime != -1 && obj.KeepAliveTime <= (int)gameTime.TotalGameTime.Seconds)
                        listEffect.Remove(obj);
                }
                
                // create new entities
                createEntities();
            }
        }

        public void UpdateMapEntities(GameTime gameTime)
        {
            string newmap = null;
            Vector2 newpos = Vector2.Zero;
            Vector2 campos = Vector2.Zero;

            // Reset player variables
            playerSprite.CollideWarp = false;
            playerSprite.CollideNPC = false;

            if (listEntity.Count > 0)
            {
                foreach (Entity entity in listEntity)
                {
                    // start with the collision check
                    if (entity.Position != entity.OldPosition)
                    {
                        entity.CollideLadder = false;
                        entity.CollideRope = false;
                        entity.CollideSlope = false;

                        Rectangle EntityRec = new Rectangle(
                            (int)entity.Position.X + (int)(playerSprite.SpriteSize.Width * 0.25f),
                            (int)entity.Position.Y,
                            (int)entity.SpriteFrame.Width - (int)(playerSprite.SpriteSize.Width * 0.25f),
                            (int)entity.SpriteFrame.Height);

                        #region walls collision
                        // Check wall collision (player and NPC!)
                        foreach (var obj in map.ObjectGroups["Walls"].Objects)
                        {
                            char[] chars = obj.Value.Name.ToCharArray();
                            string objname = new string(chars).Substring(0, 4); // max 4 chars to skip numbers

                            if (objname == "wall")
                            {
                                Rectangle Wall = new Rectangle((int)obj.Value.X, (int)obj.Value.Y, (int)obj.Value.Width, (int)obj.Value.Height);

                                if (EntityRec.Intersects(Wall) &&
                                    entity.Position.X + entity.SpriteFrame.Width * 0.65f > Wall.Left &&
                                    entity.Position.X + entity.SpriteFrame.Width * 0.45f < Wall.Right)
                                {

                                    // reading XML - wall properties
                                    int Block = 0;
                                    foreach (var property in obj.Value.Properties)
                                        if (property.Key == "Block")
                                            Block = Convert.ToInt32(property.Value);

                                    if (Block == 1)
                                    {
                                        entity.PositionX = entity.OldPositionX;
                                    }
                                    else
                                    {
                                        switch(entity.State)
                                        {
                                            case EntityState.Rope:
                                            case EntityState.Ladder:
                                                if(entity.Position.Y + EntityRec.Height * 0.75f < Wall.Top)
                                                {
                                                    entity.PositionY -= 10;
                                                    entity.State = EntityState.Stand;
                                                }
                                                break;
                                            case EntityState.Jump:
                                                break;
                                            default:
                                                if (entity.EntityType != EntityType.NPC)
                                                {
                                                    if (entity.PositionY > entity.OldPositionY &&
                                                        entity.PositionY + (entity.SpriteFrame.Height * 0.90f) < Wall.Top)
                                                        entity.PositionY = Wall.Top - entity.SpriteFrame.Height;
                                                }
                                                else
                                                {
                                                    if (entity.PositionY > entity.OldPositionY &&
                                                        entity.PositionY + (entity.SpriteFrame.Height * 0.50f) < Wall.Top)
                                                        entity.PositionY = Wall.Top - entity.SpriteFrame.Height;
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        #region ladder and rope collision
                        // Check ladder + Robe collision (player and NPC!)
                        foreach (var obj in map.ObjectGroups["Climbing"].Objects)
                        {
                            char[] chars = obj.Value.Name.ToCharArray();
                            string objname = new string(chars).Substring(0, 4); // max 4 chars to skip numbers

                            if (objname == "rope")
                            {
                                Rectangle Rope = new Rectangle((int)obj.Value.X, (int)obj.Value.Y, (int)obj.Value.Width, (int)obj.Value.Height);

                                if (EntityRec.Intersects(Rope) &&
                                    entity.Position.X + entity.SpriteFrame.Width * 0.65f > Rope.Left &&
                                    entity.Position.X + entity.SpriteFrame.Width * 0.45f < Rope.Right &&
                                    entity.Position.Y + entity.SpriteFrame.Height * 0.50f < Rope.Bottom)
                                {
                                    entity.CollideRope = true;
                                }
                            }
                            else if (objname == "ladd")
                            {
                                Rectangle Ladder = new Rectangle((int)obj.Value.X, (int)obj.Value.Y, (int)obj.Value.Width, (int)obj.Value.Height);

                                if (EntityRec.Intersects(Ladder) &&
                                    entity.Position.X + entity.SpriteFrame.Width * 0.65f > Ladder.Left &&
                                    entity.Position.X + entity.SpriteFrame.Width * 0.45f < Ladder.Right &&
                                    entity.Position.Y + entity.SpriteFrame.Height * 0.50f < Ladder.Bottom)
                                {
                                    entity.CollideLadder = true;
                                }
                            }
                        }
                        #endregion
                        #region NPC collision
                        // Check NPC collision (player only!)
                        if (entity.EntityType == EntityType.NPC)
                        {
                            Rectangle NPC = new Rectangle((int)entity.Position.X, (int)entity.Position.Y, 
                                                            entity.SpriteFrame.Width, entity.SpriteFrame.Height);

                            if (NPC.Intersects(new Rectangle(
                                    (int)playerSprite.Position.X + (int)(playerSprite.SpriteFrame.Width * 0.25f),
                                    (int)playerSprite.Position.Y,
                                    (int)playerSprite.SpriteFrame.Width - (int)(playerSprite.SpriteFrame.Width * 0.25f),
                                    (int)playerSprite.SpriteFrame.Height)
                                    ))
                            {
                                playerSprite.CollideNPC = true;

                                // Check for Keyboard input
                                if (keyboardStateCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Space) == true &&
                                    keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space) == true &&
                                    playerSprite.State == EntityState.Stand)
                                {
                                    // call NPC script
                                    screenManager.messageScreen(true, entity.EntityFace, entity.EntityScript);
                                }
                            }
                        }
                        #endregion
                        #region slope collision
                        // Check Slope collision (player and NPC!)
                        foreach (var obj in map.ObjectGroups["Slopes"].Objects)
                        {
                            char[] chars = obj.Value.Name.ToCharArray();
                            string objname = new string(chars).Substring(0, 5); // max 4 chars to skip numbers

                            if (objname == "slope")
                            {
                                Rectangle Slope = new Rectangle((int)obj.Value.X, (int)obj.Value.Y, (int)obj.Value.Width, (int)obj.Value.Height);

                                if (EntityRec.Intersects(Slope) &&
                                    entity.Position.X + entity.SpriteFrame.Width * 0.65f > Slope.Left &&
                                    entity.Position.X + entity.SpriteFrame.Width * 0.45f < Slope.Right &&
                                    entity.Position.Y + entity.SpriteFrame.Height < Slope.Bottom + 2)
                                {
                                    // set collision slope
                                    entity.CollideSlope = true;

                                    // collision detection
                                    Vector2 tileSlope = new Vector2(0, 0);

                                    EntityRec = new Rectangle((int)entity.Position.X, (int)entity.Position.Y,
                                        (int)entity.SpriteFrame.Width, (int)entity.SpriteFrame.Height);

                                    // reading XML - wall properties
                                    foreach (var property in obj.Value.Properties)
                                    {
                                        if (property.Key == "SlopeX")
                                            tileSlope.X = Convert.ToInt32(property.Value);
                                        if (property.Key == "SlopeY")
                                            tileSlope.Y = Convert.ToInt32(property.Value);
                                    }

                                    float SlopeRad = (float)Slope.Height / (float)Slope.Width;

                                    if (tileSlope.X < tileSlope.Y) // move down
                                    {
                                        //float inSlopePosition = (EntityRec.Center.X * SlopeRad) - Slope.Left;
                                        float inSlopePosition = (EntityRec.Center.X - Slope.Left) * SlopeRad;
                                        if (inSlopePosition > 0)
                                            if (EntityRec.Bottom > Slope.Top + inSlopePosition)
                                                entity.PositionY = (Slope.Top + inSlopePosition) - entity.SpriteFrame.Height;
                                    }

                                    if (tileSlope.X > tileSlope.Y) // move up
                                    {
                                        float inSlopePosition = (EntityRec.Center.X - Slope.Left) * SlopeRad;

                                        if (inSlopePosition < Slope.Height)
                                            if (EntityRec.Bottom > Slope.Bottom - inSlopePosition)
                                                entity.PositionY = (Slope.Bottom - inSlopePosition) - entity.SpriteFrame.Height;
                                    }
                                }
                            }
                        }
                        #endregion
                        #region warp collision
                        // Check for warp collision (player only!)
                        if (listEffect.FindAll(delegate(Effect obj) { return obj.GetType().IsSubclassOf(typeof(Effect)); }).Count > 0)
                        {
                            foreach (Effect effect in listEffect)
                            {
                                if (effect is Warp)
                                {
                                    Warp warp = (Warp)effect;

                                    Rectangle Warp = new Rectangle((int)warp.Position.X, (int)warp.Position.Y,
                                        (int)warp.SpriteFrame.Width, (int)warp.SpriteFrame.Height);

                                    if (Warp.Intersects(
                                        new Rectangle(
                                            (int)playerSprite.Position.X + (int)(playerSprite.SpriteSize.Width * 0.25f),
                                            (int)playerSprite.Position.Y,
                                            (int)playerSprite.SpriteFrame.Width - (int)(playerSprite.SpriteSize.Width * 0.25f),
                                            ((int)playerSprite.SpriteFrame.Height)
                                            )))
                                    {
                                        entity.CollideWarp = true;
                                        newmap = warp.newMap;
                                        newpos = warp.newPosition;
                                        campos = warp.camPosition;
                                    }
                                }
                            }
                        }
                        #endregion
                        #region Arrow collision
                        // Check for Arrow collision (monsters only!)
                        if (entity.EntityType == EntityType.Monster)
                        {
                            foreach (Entity arrow in listEntity)
                            {
                                if (arrow.EntityType == EntityType.Arrow)
                                {
                                    if (new Rectangle((int)entity.Position.X + (int)(entity.SpriteFrame.Width * 0.30f),
                                                      (int)entity.Position.Y + (int)(entity.SpriteFrame.Height * 0.40f),
                                                      (int)entity.SpriteFrame.Width - (int)(entity.SpriteFrame.Width * 0.30f),
                                                      (int)entity.SpriteFrame.Height).                                       
                                        Intersects(
                                            new Rectangle((int)arrow.Position.X + (int)(entity.SpriteFrame.Width * 0.30f), 
                                                (int)arrow.Position.Y,
                                                (int)arrow.SpriteFrame.Width - (int)(entity.SpriteFrame.Width * 0.30f), 
                                                (int)arrow.SpriteFrame.Height)))
                                    {
                                        entity.State = EntityState.Hit;
                                        arrow.KeepAliveTime = 0;        // remove arrow
                                    }
                                }
                            }
                        }
                        #endregion
                        #region mapbound collision
                        // check maps bounds (player and NPC!)
                        if (EntityRec.Right > map.Width * map.TileWidth || EntityRec.Bottom > map.Height * map.TileHeight ||
                            EntityRec.Left < 0 || EntityRec.Top < 0)
                            entity.Position = entity.OldPosition;
                        #endregion

                    }
                }
            }

            // First finish the foreach loop before cleaning 
            // the list index and opening a new maps
            if (newmap != null)
            {
                if (playerSprite.Effect(gameTime))
                {
                    loadnewmap(newmap, newpos, campos);
                }
            }
        }
        #endregion

        #region draw
        public void Draw(GameTime gameTime)
        {
            //base.Draw(gameTime);

            if (Active)
            {
                if (Background != null)
                    spriteBatch.Draw(Background, new Rectangle((int)cam._pos.X - (gfxdevice.Viewport.Width / 2),
                                                    (int)cam._pos.Y - (gfxdevice.Viewport.Height / 2),
                                                    gfxdevice.Viewport.Width,
                                                    gfxdevice.Viewport.Height), Color.White);

                //Draw map Layer 1
                map.Draw(spriteBatch, new Rectangle((int)cam._pos.X - (gfxdevice.Viewport.Width / 2),
                                                    (int)cam._pos.Y - (gfxdevice.Viewport.Height / 2),
                                                    gfxdevice.Viewport.Width,
                                                    gfxdevice.Viewport.Height),
                                                    new Vector2((int)cam._pos.X - (gfxdevice.Viewport.Width / 2),
                                                                (int)cam._pos.Y - (gfxdevice.Viewport.Height / 2)),
                                                                "Tile Layer 1");


                // Sort all entities based on their Y position
                listEntity.Sort(delegate(Entity a, Entity b)
                {
                    int xdiff = a.Position.Y.CompareTo(b.Position.Y);
                    return xdiff;
                });

                // Draw all Entities (incl Player)
                foreach (Entity obj in listEntity)
                {
                    if(obj != playerSprite)
                        obj.Draw(spriteBatch);
                }
                // Draw the player
                playerSprite.Draw(spriteBatch);

                // Draw all Warp Effects
                foreach (Effect obj in listEffect)
                {
                    obj.Draw(spriteBatch);
                }

                //Draw map Layer 2
                map.Draw(spriteBatch, new Rectangle((int)cam._pos.X - (gfxdevice.Viewport.Width / 2),
                                                    (int)cam._pos.Y - (gfxdevice.Viewport.Height / 2),
                                                    gfxdevice.Viewport.Width,
                                                    gfxdevice.Viewport.Height),
                                                    new Vector2((int)cam._pos.X - (gfxdevice.Viewport.Width / 2),
                                                                (int)cam._pos.Y - (gfxdevice.Viewport.Height / 2)),
                                                                "Tile Layer 2");
            }
        }
        #endregion

        #region functions load / save

        public void LoadEntities()
        {
            foreach (var group in map.ObjectGroups)
            {
                if (group.Key == "NPCS")
                {
                    foreach (var obj in group.Value.Objects)
                    {
                        char[] chars = obj.Value.Name.ToCharArray();
                        string objname = new string(chars).Substring(0, 3); // max 3 chars to skip numbers

                        int offsetX = 0, offsetY = 0, spritesizeX = 0, spritesizeY = 0;
                        string texture = null, face = null, script = null;

                        if (objname == "npc")
                        {
                            foreach (var objprop in obj.Value.Properties)
                            {
                                string objkey = objprop.Key.ToString();
                                string objvalue = objprop.Value.ToString();

                                switch (objkey)
                                {
                                    case "OffsetX":
                                        offsetX = Convert.ToInt32(objvalue);
                                        break;
                                    case "OffsetY":
                                        offsetY = Convert.ToInt32(objvalue);
                                        break;
                                    case "Texture":
                                        texture = objvalue;
                                        break;
                                    case "spriteSizeX":
                                        spritesizeX = Convert.ToInt32(objvalue);
                                        break;
                                    case "spriteSizeY":
                                        spritesizeY = Convert.ToInt32(objvalue);
                                        break;
                                    case "Face":
                                        face = objvalue;
                                        break;
                                    case "Script":
                                        script = objvalue;
                                        break;
                                }
                            }
                            try
                            {
                                // properties are filled now check the state
                                listEntity.Add(new NPCharacter(
                                            Content.Load<Texture2D>(@"gfx\NPCs\" + texture),
                                            new Vector2(offsetX, offsetY),
                                            new Vector2(spritesizeX, spritesizeY),
                                            new Vector2(obj.Value.X, obj.Value.Y),
                                            Content.Load<Texture2D>(@"gfx\faces\" + face), 
                                            script));
                            }
                            catch (Exception ee)
                            {
                                // bug handler for NPC properties
                                string aa = ee.ToString();
                            }
                        }
                    }
                }
                else if (group.Key == "Monsters")
                {
                    foreach (var obj in group.Value.Objects)
                    {
                        char[] chars = obj.Value.Name.ToCharArray();
                        string objname = new string(chars).Substring(0, 3); // max 3 chars to skip numbers

                        int borderX = 0, borderY = 0;
                        string texture = null;

                        if (objname == "mob")
                        {
                            foreach (var objprop in obj.Value.Properties)
                            {
                                string objkey = objprop.Key.ToString();
                                string objvalue = objprop.Value.ToString();

                                switch (objkey)
                                {
                                    case "Texture":
                                        texture = objvalue.ToString();
                                        break;
                                    case "BorderL":
                                        borderX = Convert.ToInt32(objvalue);
                                        break;
                                    case "BorderR":
                                        borderY = Convert.ToInt32(objvalue);
                                        break;
                                }
                            }
                            try
                            {
                                // properties are filled now check the state
                                listEntity.Add(new Monster(
                                            Content.Load<Texture2D>(@"gfx\Mobs\" + texture),
                                            new Vector2(obj.Value.X, obj.Value.Y),
                                            new Vector2(borderX, borderY)));
                            }
                            catch (Exception ee)
                            {
                                // bug handler for Monster properties
                                string aa = ee.ToString();
                            }
                        }
                    }
                }
                else if (group.Key == "Warps")
                {
                    foreach (var obj in group.Value.Objects)
                    {
                        char[] chars = obj.Value.Name.ToCharArray();
                        string objname = new string(chars).Substring(0, 4); // max 4 chars to skip numbers

                        if (objname == "warp")
                        {
                            string newmap = null;
                            int newposx = 0, newposy = 0, camposx = 0, camposy = 0;

                            foreach (var objprop in obj.Value.Properties)
                            {
                                string objkey = objprop.Key.ToString();
                                string objvalue = objprop.Value.ToString();

                                if (objkey == "newmap")
                                    newmap = objvalue;
                                else if (objkey == "newposx")
                                    newposx = Convert.ToInt32(objvalue);
                                else if (objkey == "newposy")
                                    newposy = Convert.ToInt32(objvalue);
                                else if (objkey == "camposx")
                                    camposx = Convert.ToInt32(objvalue);
                                else if (objkey == "camposy")
                                    camposy = Convert.ToInt32(objvalue);

                            }
                            listEffect.Add(new Warp(Content.Load<Texture2D>(@"gfx\gameobjects\warp"),
                                                        new Vector2(obj.Value.X - 20, obj.Value.Y - 225),
                                                        newmap, new Vector2(newposx, newposy),
                                                        new Vector2(camposx, camposy)));
                        }
                    }
                }
            }
        }

        public void loadnewmap(string newmap, Vector2 newpos, Vector2 campos)
        {
            map = Map.Load(Path.Combine(Content.RootDirectory, @newmap), Content);

            screenManager.setScreen("loadingScreen");

            playerSprite.Position = new Vector2(newpos.X * map.TileWidth, newpos.Y * map.TileHeight);

            if (campos.X != 0 && campos.Y != 0)
                screenManager.actionScreen.cam.Pos = new Vector2(campos.X * map.TileWidth, campos.Y * map.TileHeight);

            foreach(var property in map.Properties)
            {
                if (property.Key == "Background")
                    Background = Content.Load<Texture2D>(@"gfx\background\" + property.Value);
            }

            for (int i = 0; i < listEntity.Count; i++)
                listEntity.Remove(listEntity[i]);

            for (int i = 0; i < listEffect.Count; i++)
                listEffect.Remove(listEffect[i]);

            listEntity.Clear();
            listEffect.Clear();

            // do not forget to put the player back in the entity list
            listEntity.Add(playerSprite);

            LoadEntities();
        }

        public void createEntities()
        {
            if (arrowprop.active)
            {
                arrowprop.active = false;
                listEntity.Add(new Arrow(Content.Load<Texture2D>(arrowprop.sprite), arrowprop.position, arrowprop.speed, arrowprop.direction));
            }
        }

        public void createEffects(int getdamage, Vector2 getposition)
        {
            listEffect.Add(new DamageBaloon(Content.Load<Texture2D>(@"gfx\effects\arrow"),
                                            Content.Load<SpriteFont>(@"font\gamefont"), 
                                            getposition, getdamage));
        }

        public void createArrow(Vector2 arg0, float arg1, Vector2 arg2)
        {
            arrowprop.active = true;
            arrowprop.sprite = @"gfx\gameobjects\arrow";
            arrowprop.position = arg0;
            arrowprop.speed = arg1;
            arrowprop.direction = arg2;
        }

        private struct ArrowProperties
        {
            public bool active;
            public string sprite;
            public Vector2 position;
            public float speed;
            public Vector2 direction;

            public ArrowProperties(bool arg0, string arg1, Vector2 arg2, float arg3, Vector2 arg4)
            {
                active = arg0;
                sprite = arg1;
                position = arg2;
                speed = arg3;
                direction = arg4;
            }
        }
        #endregion
    }
}
