using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XNA_ScreenManager.CharacterClasses;

namespace XNA_ScreenManager.MapClasses
{
    public class Warp : Effect
    {
        public bool active;
        public Vector2 newPosition;
        public Vector2 camPosition;
        public string newMap;

        const int ANIMATION_SPEED = 120;                                                            // Animation speed, 120 = default 
        int previousGameTimeMsec,                                                                   // GameTime in Miliseconds
            previousGameTimeSec;                                                                    // GameTime in Seconds

        public Warp (Texture2D Sprite, Vector2 Position, string newmap, Vector2 newposition, Vector2 camposition) :
            base()
        {
            position = Position;
            camPosition = camposition;
            newMap = newmap;
            newPosition = newposition;
            sprite = Sprite;
            spriteSize = new Vector2(96, 257);
            spriteFrame = new Rectangle(0, 0, 96, 257);
            active = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (previousGameTimeMsec <= (int)gameTime.TotalGameTime.Milliseconds
                || previousGameTimeSec != (int)gameTime.TotalGameTime.Seconds)
            {
                previousGameTimeMsec = (int)gameTime.TotalGameTime.Milliseconds + ANIMATION_SPEED;
                previousGameTimeSec = (int)gameTime.TotalGameTime.Seconds;
                spriteFrame.X += (int)spriteSize.X;
                if (spriteFrame.X > spriteSize.X * 6)
                    spriteFrame.X = 0;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (active)
                spriteBatch.Draw(sprite, new Rectangle((int)position.X, (int)position.Y, (int)spriteSize.X, (int)spriteSize.Y),
                    new Rectangle((int)spriteFrame.X, (int)spriteFrame.Y, (int)spriteSize.X, (int)spriteSize.Y), 
                    Color.White * 0.80f, 0f, new Vector2(0, 0), SpriteEffects.None, 0);
        }
    }
}
