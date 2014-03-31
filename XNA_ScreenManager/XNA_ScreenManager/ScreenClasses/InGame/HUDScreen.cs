using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XNA_ScreenManager.PlayerClasses;


namespace XNA_ScreenManager.ScreenClasses
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class HUDScreen : DrawableGameComponent
    {
        SpriteBatch spriteBatch = null;
        SpriteFont spriteFont;
        ContentManager Content;
        GraphicsDevice gfxdevice;

        PlayerInfo playerInfo = PlayerInfo.Instance;
        Texture2D Border, HP, MP, EXP;
        Vector2 position = new Vector2();

        public HUDScreen(Game game)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));
            gfxdevice = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));

            LoadContent();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            spriteFont = Content.Load<SpriteFont>(@"font\Arial_12px");

            Border = Content.Load<Texture2D>(@"gfx\hud\border");
            HP = Content.Load<Texture2D>(@"gfx\hud\HP");
            MP = Content.Load<Texture2D>(@"gfx\hud\MP");
            EXP = Content.Load<Texture2D>(@"gfx\hud\EXP");
        }

        public bool Active { get; set; }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (Active)
            {
                Texture2D rect = new Texture2D(gfxdevice, 120, 110);

                Color[] data = new Color[120 * 110];
                for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
                rect.SetData(data);

                spriteBatch.Draw(rect, new Vector2(position.X, position.Y + 0), Color.White * 0.5f);

                // Name
                spriteBatch.DrawString(spriteFont, "Name: " + playerInfo.Name, new Vector2(Position.X + 5, Position.Y + 5), Color.White);

                // HP
                spriteBatch.DrawString(spriteFont, "HP: " + playerInfo.HP + " of " + playerInfo.MAXHP, new Vector2(Position.X + 5, Position.Y + 20), Color.White);
                spriteBatch.Draw(HP, new Vector2(position.X + 5, position.Y + 35), 
                    new Rectangle(0, 0,
                        (int)((Border.Width * 0.01f) * (playerInfo.HP * 100 / playerInfo.MAXHP)), 
                        Border.Height), 
                    Color.White * 0.75f);
                spriteBatch.Draw(Border, new Vector2(position.X + 5, position.Y + 35), Color.White * 0.75f);

                // MP
                spriteBatch.DrawString(spriteFont, "SP: " + playerInfo.SP + " of " + playerInfo.MAXSP, new Vector2(Position.X + 5, Position.Y + 50), Color.White);
                spriteBatch.Draw(MP, new Vector2(position.X + 5, position.Y + 65),
                    new Rectangle(0, 0,
                        (int)((Border.Width * 0.01f) * (playerInfo.SP * 100 / playerInfo.MAXSP)),
                        Border.Height),
                    Color.White * 0.75f);
                spriteBatch.Draw(Border, new Vector2(position.X + 5, position.Y + 65), Color.White * 0.75f);

                // EXP
                spriteBatch.DrawString(spriteFont, "EXP: " + playerInfo.Exp + " of " + playerInfo.NextLevelExp, new Vector2(Position.X + 5, Position.Y + 80), Color.White);
                spriteBatch.Draw(EXP, new Vector2(position.X + 5, position.Y + 95),
                    new Rectangle(0, 0,
                        (int)((Border.Width * 0.01f) * (playerInfo.Exp * 100 / playerInfo.NextLevelExp)),
                        Border.Height),
                    Color.White * 0.75f);
                spriteBatch.Draw(Border, new Vector2(position.X + 5, position.Y + 95), Color.White * 0.75f);
                
                //spriteBatch.DrawString(spriteFont, "Level: " + playerInfo.Level, new Vector2(Position.X + 5, Position.Y + 65), Color.White);
                //spriteBatch.DrawString(spriteFont, "Gold: " + playerInfo.Gold, new Vector2(Position.X + 5, Position.Y + 80), Color.White);
            }
        }
    }
}
