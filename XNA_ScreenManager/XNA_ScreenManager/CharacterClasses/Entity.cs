using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace XNA_ScreenManager.CharacterClasses
{
    public enum EntityState
    {
        Stand,
        Walk,
        Jump,
        Falling,
        Swing,
        Shoot,
        Sit,
        Rope,
        Ladder,
        Hit,
        Died, 
        Frozen, 
        Animate
    };
    public enum EntityType { Player, Friend, Monster, NPC, Warp, Wall, Slope, Bullet, Arrow };

    public abstract class Entity
    {
        #region Vital Field and Property Region

        protected EntityType entityType;
        protected Texture2D entityFace;
        protected string entityScript;
        protected EntityState state;

        public Texture2D EntityFace
        {
            get { return entityFace; }
            set { entityFace = value; }
        }

        public string EntityScript
        {
            get { return entityScript; }
            set { entityScript = value; }
        }

        public EntityState State
        {
            get { return state; }
            set { state = value; }
        }

        public EntityType EntityType
        {
            get { return entityType; }
            protected set { entityType = value; }
        }

        #endregion

        #region Drawable properties

        protected Vector2 position;
        protected Vector2 oldposition;
        protected Vector2 tileLocation;
        protected Rectangle spritesize;
        protected Texture2D sprite;
        protected Rectangle spriteFrame;
        protected bool active;
        protected bool collideLadder = false;
        protected bool collideRope = false;
        protected bool collideWarp = false;
        protected bool collideNPC = false;
        protected bool collideSlope = false;
        protected float keepAliveTime = -1;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public float PositionX
        {
            get { return position.X; }
            set { position.X = value; }
        }
        public float PositionY
        {
            get { return position.Y; }
            set { position.Y = value; }
        }
        public Vector2 OldPosition
        {
            get { return oldposition; }
            set { oldposition = value; }
        }
        public float OldPositionX
        {
            get { return oldposition.X; }
            set { oldposition.X = value; }
        }
        public float OldPositionY
        {
            get { return oldposition.Y; }
            set { oldposition.Y = value; }
        }
        public Rectangle SpriteSize
        {
            get { return spritesize; }
            protected set { spritesize = value; }
        }
        public Vector2 TileLocation
        {
            get { return tileLocation; }
            protected set { tileLocation = value; }
        }
        public Texture2D Sprite
        {
            get { return sprite; }
            protected set { sprite = value; }
        }
        public Rectangle SpriteFrame
        {
            get { return spriteFrame; }
            protected set { spriteFrame = value; }
        }
        public bool Active
        {
            get { return active; }
            protected set { active = value; }
        }
        public bool CollideLadder
        {
            get { return collideLadder; }
            set { collideLadder = value; }
        }
        public bool CollideRope
        {
            get { return collideRope; }
            set { collideRope = value; }
        }
        public bool CollideWarp
        {
            get { return collideWarp; }
            set { collideWarp = value; }
        }
        public bool CollideNPC
        {
            get { return collideNPC; }
            set { collideNPC = value; }
        }
        public bool CollideSlope
        {
            get { return collideSlope; }
            set { collideSlope = value; }
        }
        public float KeepAliveTime
        {
            get { return keepAliveTime; }
            set { keepAliveTime = value; }
        }

        #endregion

        #region Constructor Region

        public Entity()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        #endregion
    }
}
