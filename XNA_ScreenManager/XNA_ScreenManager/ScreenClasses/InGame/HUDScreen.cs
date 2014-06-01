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
using XNA_ScreenManager.SkillClasses;


namespace XNA_ScreenManager.ScreenClasses
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class HUDScreen : DrawableGameComponent
    {
        #region properties
        SpriteBatch spriteBatch = null;
        SpriteFont spriteFont, smallFont;
        ContentManager Content;
        GraphicsDevice gfxdevice;

        PlayerStore playerInfo = PlayerStore.Instance;
        Texture2D Border, HP, SP, EXP, Skillbar;
        Vector2 position = new Vector2();
        #endregion

        #region contructor
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

            smallFont = Content.Load<SpriteFont>(@"font\Arial_10px");
            spriteFont = Content.Load<SpriteFont>(@"font\Arial_12px");

            Border = Content.Load<Texture2D>(@"gfx\hud\border");
            HP = Content.Load<Texture2D>(@"gfx\hud\HP");
            SP = Content.Load<Texture2D>(@"gfx\hud\SP");
            EXP = Content.Load<Texture2D>(@"gfx\hud\EXP");

            Skillbar = Content.Load<Texture2D>(@"gfx\hud\skillbar");
        }
        #endregion

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

                string message = PlayerStore.Instance.activePlayer.Name + " - " + PlayerStore.Instance.activePlayer.Jobclass + " - Level: " + PlayerStore.Instance.activePlayer.Level;

                Texture2D rect = new Texture2D(gfxdevice, (int)(spriteFont.MeasureString(message).X + 20), 75);

                Color[] data = new Color[(int)(spriteFont.MeasureString(message).X + 20) * 75];
                for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
                rect.SetData(data);

                spriteBatch.Draw(rect, new Vector2(position.X, position.Y + 0), Color.White * 0.5f);

                // Name
                spriteBatch.DrawString(spriteFont, message, new Vector2(Position.X + 5, Position.Y + 5), Color.White);

                // HP
                message = PlayerStore.Instance.activePlayer.HP + " / " + PlayerStore.Instance.activePlayer.MAXHP;
                spriteBatch.DrawString(spriteFont, "HP: ", new Vector2(Position.X + 5, Position.Y + 25), Color.White);
                spriteBatch.Draw(HP, new Vector2(position.X + 40, position.Y + 25), 
                    new Rectangle(0, 0,
                        (int)((Border.Width * 0.01f) * (PlayerStore.Instance.activePlayer.HP * 100 / PlayerStore.Instance.activePlayer.MAXHP)), 
                        Border.Height), 
                    Color.White * 0.75f);
                spriteBatch.Draw(Border, new Vector2(position.X + 40, position.Y + 25), Color.White * 0.75f);
                spriteBatch.DrawString(smallFont, message.ToString(),
                    new Vector2((Position.X + 40 + Border.Width * 0.5f) - (smallFont.MeasureString(message).X * 0.5f), Position.Y + 25),
                    Color.White);

                // SP
                message = PlayerStore.Instance.activePlayer.SP + " / " + PlayerStore.Instance.activePlayer.MAXSP;
                spriteBatch.DrawString(spriteFont, "SP: ", new Vector2(Position.X + 5, Position.Y + 40), Color.White);
                spriteBatch.Draw(SP, new Vector2(position.X + 40, position.Y + 40),
                    new Rectangle(0, 0,
                        (int)((Border.Width * 0.01f) * (PlayerStore.Instance.activePlayer.SP * 100 / PlayerStore.Instance.activePlayer.MAXSP)),
                        Border.Height),
                    Color.White * 0.75f);
                spriteBatch.Draw(Border, new Vector2(position.X + 40, position.Y + 40), Color.White * 0.75f);
                spriteBatch.DrawString(smallFont, message.ToString(),
                    new Vector2((Position.X + 40 + Border.Width * 0.5f) - (smallFont.MeasureString(message).X * 0.5f), Position.Y + 40),
                    Color.White);


                // EXP
                spriteBatch.DrawString(spriteFont, "EXP: ", new Vector2(Position.X + 5, Position.Y + 55), Color.White);
                spriteBatch.Draw(EXP, new Vector2(position.X + 40, position.Y + 55),
                    new Rectangle(0, 0,
                        (int)((Border.Width * 0.01f) * (PlayerStore.Instance.activePlayer.Exp * 100 / PlayerStore.Instance.activePlayer.NextLevelExp)),
                        Border.Height),
                    Color.White * 0.75f);
                spriteBatch.Draw(Border, new Vector2(position.X + 40, position.Y + 55), Color.White * 0.75f);

                string percentExp = string.Format("{0:0.00}", 
                    Math.Round(Decimal.Divide((PlayerStore.Instance.activePlayer.Exp * 100), PlayerStore.Instance.activePlayer.NextLevelExp), 2) + "%");
                spriteBatch.DrawString(smallFont, percentExp.ToString(),
                    new Vector2((Position.X + 40 + Border.Width * 0.5f) - (smallFont.MeasureString(percentExp).X * 0.5f), Position.Y + 55),
                    Color.White);

                // Level and Gold
                // message = "Gold: " + playerInfo.Gold;
                // spriteBatch.DrawString(spriteFont, message, new Vector2(Position.X + 5, Position.Y + 70), Color.White);

                DrawSkillBar(gameTime);
            }
        }

        public void DrawSkillBar(GameTime gameTime)
        {
            spriteBatch.Draw(Skillbar, new Vector2(position.X + 200, position.Y + 430), Color.White * 0.75f);

            SkillBar skillbar = playerInfo.activePlayer.skillbar;

            for (int i = 0; i < skillbar.skillslot.Length - 1; i++)
            {
                if (skillbar.skillslot[i + 1] != null)
                {
                    Texture2D sprite = Content.Load<Texture2D>(skillbar.skillslot[i + 1].IconSpritePath);
                    spriteBatch.Draw(
                        sprite,
                        new Vector2(position.X + 200 + (sprite.Width * i), position.Y + 430),
                        Color.White * 0.75f);
                }
            }
        }
    }
}
