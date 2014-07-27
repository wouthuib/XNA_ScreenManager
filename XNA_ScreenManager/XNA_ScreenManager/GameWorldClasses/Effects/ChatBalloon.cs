using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA_ScreenManager.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using XNA_ScreenManager.ScreenClasses.MainClasses;
using XNA_ScreenManager.PlayerClasses;

namespace XNA_ScreenManager.GameWorldClasses.Effects
{
    public class ChatBalloon : GameEffect
    {
        SpriteBatch spriteBatch = null;
        ContentManager Content;
        GraphicsDevice gfxdevice;

        SpriteFont spriteFont;
        Texture2D arrow, top, side, LTcorner, LBcorner, RTcorner, RBcorner;

        bool ArrowOn = false;
        int width, height;
        string PlayerName, Text;

        public ChatBalloon(string name, string text, SpriteFont font)
            : base()
        {
            spriteBatch = (SpriteBatch)ResourceManager.GetInstance.spriteBatch;
            Content = (ContentManager)ResourceManager.GetInstance.Content;
            gfxdevice = (GraphicsDevice)ResourceManager.GetInstance.gfxdevice;

            arrow = Content.Load<Texture2D>(@"gfx\NPCs\textballoon\arrow");
            top = Content.Load<Texture2D>(@"gfx\NPCs\textballoon\top");
            side = Content.Load<Texture2D>(@"gfx\NPCs\textballoon\side");
            LTcorner = Content.Load<Texture2D>(@"gfx\NPCs\textballoon\LTcorner");
            LBcorner = Content.Load<Texture2D>(@"gfx\NPCs\textballoon\LBcorner");
            RTcorner = Content.Load<Texture2D>(@"gfx\NPCs\textballoon\RTcorner");
            RBcorner = Content.Load<Texture2D>(@"gfx\NPCs\textballoon\RBcorner");

            // Initize variables
            this.PlayerName = name;
            this.Text = text;
            this.spriteFont = font;

            width = (int)MathHelper.Clamp(
                (int)(spriteFont.MeasureString(text).X * 0.52f),
                (int)(arrow.Width * 0.5f), // min
                100); // max

            height = (text.ToString().Split('\n').Length * spriteFont.LineSpacing);
            ArrowOn = true;
            this.keepAliveTimer = 4;
        }

        public override void Update(GameTime gameTime)
        {
            // Make the item slowly disapear
            if (transperant > 0)
                transperant -= (float)gameTime.ElapsedGameTime.TotalSeconds * 3;

            // base Effect Update
            base.Update(gameTime);
        }

        public override Vector2 Position 
        { 
            get 
            {
                if (PlayerName == GameWorld.GetInstance.playerSprite.PlayerName)
                    return new Vector2(
                        (GameWorld.GetInstance.playerSprite.Position.X + 30) - (width * 0.50f),
                        GameWorld.GetInstance.playerSprite.Position.Y - 50);
                else
                {
                    foreach (var player in NetworkPlayerStore.Instance.playersprites)
                    {
                        if (player.Name == PlayerName)
                            return new Vector2(
                                (player.Position.X + 30) - (width * 0.50f),
                                 player.Position.Y - 50);
                    }

                    return Vector2.Zero; // cannot find any players that match the name
                }
            } 
        }

        public Vector2 ArrowPosition
        {
            get
            {
                return new Vector2(
                    (this.Position.X + this.width * 0.75f),
                     this.Position.Y + 32);
            } 
        }

        public override void Draw(SpriteBatch spriteBatch)
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
                    Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
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
                spriteBatch.Draw(
                    arrow,
                    new Rectangle((int)ArrowPosition.X, (int)ArrowPosition.Y, arrow.Width, arrow.Height), 
                    new Rectangle(0, 0, arrow.Width, arrow.Height),
                    Color.White, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
            }

            //Draw text inside balloon
            spriteBatch.DrawString(spriteFont, Text, new Vector2(Position.X + 15, Position.Y + 15), Color.Black);
        }
    }
}
