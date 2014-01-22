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
        Texture2D HudPicture;
        Vector2 position = new Vector2();

        public HUDScreen(Game game, SpriteFont spriteFont)
            : base(game)
        {
            this.spriteFont = spriteFont;
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));
            gfxdevice = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));

            LoadContent();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            spriteFont = Content.Load<SpriteFont>(@"font\gamefont");
            HudPicture = Content.Load<Texture2D>(@"gfx\hud\facebox01");
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

                spriteBatch.Draw(HudPicture, new Vector2(position.X + 5, position.Y + 5), new Rectangle(0, 58, 96, 40), Color.White * 0.75f);
                spriteBatch.DrawString(spriteFont, "Name: " + playerInfo.Name, new Vector2(Position.X + 5, Position.Y + 50), Color.White);
                spriteBatch.DrawString(spriteFont, "Level: " + playerInfo.Level, new Vector2(Position.X + 5, Position.Y + 65), Color.White);
                spriteBatch.DrawString(spriteFont, "Gold: " + playerInfo.Gold, new Vector2(Position.X + 5, Position.Y + 80), Color.White);
                spriteBatch.DrawString(spriteFont, "Exp: " + playerInfo.Exp, new Vector2(Position.X + 5, Position.Y + 95), Color.White);
            }
        }
    }
}
