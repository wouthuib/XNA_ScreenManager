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
using XNA_ScreenManager.ItemClasses;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.ScreenClasses.SubComponents;


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

        public ChatbarInput chatbarInput;
        PlayerStore playerInfo = PlayerStore.Instance;
        Texture2D Border, HP, SP, EXP, Skillbar, Chatbar, Chatbartop;
        Vector2 position = new Vector2(), skillbar_pos, chatbar_pos;
        #endregion

        #region contructor
        public HUDScreen(Game game)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));
            gfxdevice = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));

            chatbarInput = new ChatbarInput(game, Content.Load<SpriteFont>(@"font\Arial_10px"));

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
            Chatbar = Content.Load<Texture2D>(@"gfx\hud\chatbar");
            Chatbartop = Content.Load<Texture2D>(@"gfx\hud\chatbar_top");
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
            chatbarInput.Update(gameTime);
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

                DrawChatBar(gameTime);
            }
        }

        public void DrawSkillBar(GameTime gameTime)
        {
            skillbar_pos = new Vector2(
                position.X + (gfxdevice.Viewport.Width - Skillbar.Width) + 3,
                position.Y + (gfxdevice.Viewport.Height - Skillbar.Height) + 3);

            spriteBatch.Draw(Skillbar, skillbar_pos, Color.White * 0.75f);

            QuickSlotBar quickslotbar = playerInfo.activePlayer.quickslotbar;

            for (int i = 0; i < quickslotbar.quickslot.Length; i++)
            {
                if (quickslotbar.quickslot[i] != null)
                {
                    if (quickslotbar.Quickslot(i) is Skill)
                    {
                        Skill skill = quickslotbar.Quickslot(i) as Skill;

                        // icon
                        Texture2D sprite = Content.Load<Texture2D>(skill.IconSpritePath);
                        spriteBatch.Draw(
                            sprite,
                            new Vector2(skillbar_pos.X + 6 + (sprite.Width * i + 6), skillbar_pos.Y + 13),
                            Color.White * 0.75f);

                        // level
                        spriteBatch.DrawString(spriteFont, skill.Level.ToString(),
                            new Vector2(skillbar_pos.X + 26 + (sprite.Width * i + 6), skillbar_pos.Y + 431),
                            Color.Black);
                        spriteBatch.DrawString(spriteFont, skill.Level.ToString(),
                            new Vector2(skillbar_pos.X + 25 + (sprite.Width * i + 6), skillbar_pos.Y + 430),
                            Color.LightGreen);
                    }
                    else if (quickslotbar.Quickslot(i) is Item)
                    {
                        Item item = quickslotbar.Quickslot(i) as Item;
                        int count = PlayerStore.Instance.activePlayer.inventory.item_list.FindAll(x =>x.itemName == item.itemName).Count;

                        // initize sprite icon
                        Texture2D sprite = Content.Load<Texture2D>(item.itemSpritePath);
                        Rectangle frame = new Rectangle(item.SpriteFrameX * 48 + 6, item.SpriteFrameY * 48 + 12, 30, 28); // +6 +12 is the offset for red potion

                        Color color = Color.White, textcolor = Color.LightGreen;
                        float trans = 1.0f;

                        if (GameWorld.GetInstance.playerSprite.ItemActive)
                        {
                            color = Color.LightGray;
                            trans = 0.80f;
                            textcolor = Color.Red;
                        }

                        // initize icon rectangle
                        Texture2D rect = new Texture2D(gfxdevice, 20, 22);

                        Color[] data = new Color[20 * 22];
                        for (int ii = 0; ii < data.Length; ++ii) data[ii] = Color.Black;
                        rect.SetData(data);

                        // draw icon rectangle
                        spriteBatch.Draw(rect, new Vector2(skillbar_pos.X + 16 + (32 * i), skillbar_pos.Y + 16), Color.White * 0.70f);

                        // draw sprite icon
                        spriteBatch.Draw(sprite, new Rectangle((int)skillbar_pos.X + 14 + (32 * i), (int)skillbar_pos.Y + 15, 26, 28), frame, color * trans);

                        // draw item count
                        spriteBatch.DrawString(spriteFont, count.ToString(),
                            new Vector2(skillbar_pos.X + 26 + (32 * i), skillbar_pos.Y + 31),
                            Color.Black);
                        spriteBatch.DrawString(spriteFont, count.ToString(),
                            new Vector2(skillbar_pos.X + 25 + (32 * i), skillbar_pos.Y + 30),
                            textcolor * trans);
                    }
                }
            }
        }

        public void DrawChatBar(GameTime gameTime)
        {
            chatbar_pos = new Vector2(position.X, position.Y + gfxdevice.Viewport.Height - Chatbar.Height);

            spriteBatch.Draw(Chatbar, chatbar_pos, Color.White * 0.80f);

            Texture2D rect = new Texture2D(gfxdevice, (int)(Chatbar.Width - 3), 75);

            Color[] data = new Color[(int)(Chatbar.Width - 3) * 75];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
            rect.SetData(data);

            spriteBatch.Draw(rect, new Vector2(position.X, chatbar_pos.Y - 74), Color.White * 0.5f);

            spriteBatch.Draw(Chatbartop, new Vector2(position.X, chatbar_pos.Y - 70 - Chatbartop.Height), Color.White * 0.80f);

            // text input 

            chatbarInput.Position = new Vector2(chatbar_pos.X + 80, chatbar_pos.Y + 10);
            chatbarInput.Draw(gameTime);

            // draw textlog
            spriteBatch.DrawString(
                Content.Load<SpriteFont>(@"font\Arial_10px"),
                chatbarInput.textlog,
                new Vector2(
                    chatbar_pos.X + 2,
                    chatbar_pos.Y - chatbarInput.textlog.ToString().Split('\n').Length * (spriteFont.LineSpacing * 0.77f) + 10)
                    + Vector2.One,
                    Color.Black);

            spriteBatch.DrawString(
                Content.Load<SpriteFont>(@"font\Arial_10px"), 
                chatbarInput.textlog,
                new Vector2(
                    chatbar_pos.X + 2, 
                    chatbar_pos.Y - chatbarInput.textlog.ToString().Split('\n').Length * (spriteFont.LineSpacing * 0.77f) + 10), 
                    Color.LightGreen);
        }
    }
}
