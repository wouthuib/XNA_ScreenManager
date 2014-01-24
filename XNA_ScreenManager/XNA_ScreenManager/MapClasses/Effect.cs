using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA_ScreenManager.MapClasses
{
    public enum EffectType { DamageBaloon, ItemSprite };

    public abstract class Effect
    {
        #region Drawable properties
        protected Texture2D sprite;
        protected Vector2 spriteSize;
        protected Rectangle spriteFrame;
        protected Vector2 position;
        protected EffectType type;

        public Texture2D Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }
        public Vector2 SpriteSize
        {
            get { return spriteSize; }
            set { spriteSize = value; }
        }
        public Rectangle SpriteFrame
        {
            get { return spriteFrame; }
            set { spriteFrame = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public EffectType Type
        {
            get { return type; }
            set { type = value; }
        }
        #endregion

        #region Timer properties
        protected float keepAliveTime = -1;

        public float KeepAliveTime
        {
            get { return keepAliveTime; }
            set { keepAliveTime = value; }
        }
        #endregion

        #region Constructor Region
        public Effect()
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
