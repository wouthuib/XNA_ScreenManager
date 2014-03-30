using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Design;

namespace XNA_ScreenManager.ScreenClasses.SubComponents
{
    public class TextBalloon : DrawableGameComponent
    {
        SpriteBatch spriteBatch = null;
        ContentManager Content;
        GraphicsDevice gfxdevice;

        Texture2D arrow, top, side, LTcorner, LBcorner, RTcorner, RBcorner;

        bool ArrowOn = false;
        int width, height;
        Vector2 arrowPosition;

        public TextBalloon(Game game)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));
            gfxdevice = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));

            arrow = Content.Load<Texture2D>(@"gfx\NPCs\textballoon\arrow");
            top = Content.Load<Texture2D>(@"gfx\NPCs\textballoon\top");
            side = Content.Load<Texture2D>(@"gfx\NPCs\textballoon\side");
            LTcorner = Content.Load<Texture2D>(@"gfx\NPCs\textballoon\LTcorner");
            LBcorner = Content.Load<Texture2D>(@"gfx\NPCs\textballoon\LBcorner");
            RTcorner = Content.Load<Texture2D>(@"gfx\NPCs\textballoon\RTcorner");
            RBcorner = Content.Load<Texture2D>(@"gfx\NPCs\textballoon\RBcorner");

            // Initize variables
            Position = new Vector2(0, 0);
            Width = 2;
            Height = 2;
            Active = false;
        }

        public int Width
        {
            get { return width; }
            set { width = value;}
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public Vector2 Position { get; set; }

        public Vector2 ArrowPosition 
        { 
          get { return arrowPosition; } 
          set { arrowPosition = value; ArrowOn = true; } 
        }

        public bool Active { get; set; }
        
        public override void Draw(GameTime gameTime)
        {
            if (Active)
            {
                Vector2 pos = Position;

                // center rectangle (balloon fill)
                pos.X += LTcorner.Width * 0.5f;
                pos.Y += LTcorner.Height * 0.5f;

                Rectangle rect = Rectangle.Empty;

                for (int i = 0; i < width; i++)
                    rect.Width += top.Width;

                rect.Width += 17;

                for (int i = 0; i < height; i++)
                    rect.Height += side.Height;

                rect.Height += 17;

                Texture2D balloonfill = new Texture2D(gfxdevice, rect.Width, rect.Height);

                Color[] data = new Color[rect.Width * rect.Height];
                for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
                balloonfill.SetData(data);

                spriteBatch.Draw(balloonfill, pos, new Color(238, 238, 238));

                // top border
                pos = Position;

                spriteBatch.Draw(LTcorner, pos, Color.White);
                pos.X += LTcorner.Width;

                for (int i = 0; i < width; i++)
                {
                    spriteBatch.Draw(top, pos, Color.White);
                    pos.X += top.Width;
                }

                spriteBatch.Draw(RTcorner, pos, Color.White);

                // right border
                pos.Y += RTcorner.Height;
                pos.X += 4; // due to flip, slight correction

                for (int i = 0; i < height; i++)
                {
                    spriteBatch.Draw(side, new Rectangle((int)pos.X, (int)pos.Y, side.Width, side.Height), 
                        new Rectangle(0, 0, side.Width, side.Height),                         
                        Color.White, 0f, Vector2.Zero , SpriteEffects.FlipHorizontally, 0f);
                    pos.Y += side.Height;
                }

                // left border
                pos = Position;
                pos.Y += LTcorner.Height;

                for (int i = 0; i < height; i++)
                {
                    spriteBatch.Draw(side, pos, Color.White);
                    pos.Y += side.Height;
                }

                // bottom border
                pos.X = Position.X;
                spriteBatch.Draw(LBcorner, pos, Color.White);

                pos.X += LBcorner.Width;
                pos.Y += 4; // due to flip, slight correction

                for (int i = 0; i < width; i++)
                {
                    spriteBatch.Draw(top, new Rectangle((int)pos.X, (int)pos.Y, top.Width, top.Height),
                        new Rectangle(0, 0, top.Width, top.Height),
                        Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
                    pos.X += top.Width;
                }

                pos.Y -= 4;
                spriteBatch.Draw(RBcorner, pos, Color.White);

                // arrow
                if (ArrowOn)
                {
                    pos = Position;
                    pos.X = arrowPosition.X;
                    pos.Y -= 13; // arrow.height is 27 pxt, and top 10 pxt

                    spriteBatch.Draw(arrow, pos, Color.White);
                }
            }
        }
    }
}
