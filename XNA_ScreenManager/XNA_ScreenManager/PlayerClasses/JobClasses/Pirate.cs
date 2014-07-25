using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNA_ScreenManager.PlayerClasses.JobClasses
{
    public class Pirate : PlayerSprite
    {
        public Pirate(int _X, int _Y, Vector2 _tileSize) 
            : base(_X, _Y)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
