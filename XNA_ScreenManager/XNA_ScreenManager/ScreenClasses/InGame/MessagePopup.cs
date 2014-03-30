using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNA_ScreenManager.PlayerClasses;
using XNA_ScreenManager.ScriptClasses;
using System.Text;
using XNA_ScreenManager.ScreenClasses.SubComponents;

namespace XNA_ScreenManager.ScreenClasses
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MessagePopup : GameScreen
    {
        #region properties
        SpriteBatch spriteBatch = null;
        SpriteFont spriteFont;
        ContentManager Content;
        GraphicsDevice gfxdevice;

        KeyboardState keyboardStateCurrent, keyboardStatePrevious;

        MenuComponent menu;
        TextBalloon balloon;
        PlayerInfo playerInfo = PlayerInfo.Instance;
        ScriptInterpreter scriptManager = ScriptInterpreter.Instance;

        StringBuilder t_complete = new StringBuilder();
        StringBuilder t_display = new StringBuilder();
        int t_index;

        int previousGameTimeMsec,
            previousGameTimeSec;

        const int textoffsetX = 100; // draw offset

        Vector2 position,
                size;

        Rectangle NPCPostion;
        #endregion

        #region constructor
        public MessagePopup(Game game, SpriteFont spriteFont)
            : base(game)
        {
            this.spriteFont = spriteFont;
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));
            gfxdevice = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));

            menu = new MenuComponent(game, spriteFont);
            Components.Add(menu);
            balloon = new TextBalloon(game);

            LoadContent();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            size = new Vector2(gfxdevice.Viewport.Width * 0.85f, gfxdevice.Viewport.Height * 0.30f);
            position = new Vector2(gfxdevice.Viewport.Width - size.X, gfxdevice.Viewport.Height - size.Y);
        }

        public void LoadAssets(Rectangle NPCPos, string scriptfile)
        {
            // this will become the contructor
            t_display.Length = 0;
            t_display.Capacity = 0;
            t_complete.Length = 0;
            t_complete.Capacity = 0;
            t_index = 0;

            if (scriptfile == null)
                scriptfile = Game.Content.RootDirectory + @"\scriptDB\npc01.txt";
            else
                scriptfile = Game.Content.RootDirectory + @"\scriptDB\" + scriptfile + ".txt";

            scriptManager.loadFile(scriptfile);
            this.scriptActive = false;

            spriteFont = Content.Load<SpriteFont>(@"font\Comic_Sans_13px");
            this.NPCPostion = NPCPos;
        }

        public bool Active { get; set; }

        public bool scriptActive { get; set; }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public override void Show()
        {
            base.Show();
            Enabled = true;
            Visible = true;
        }

        public override void Hide()
        {
            base.Hide();
            Enabled = false;
            Visible = false;
        }

        public override void Initialize()
        {
            base.Initialize();
        }
        #endregion

        #region update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if (scriptActive)
            {
                updateDialogue(gameTime);
            }
            else
            {
                scriptManager.readScript(); // initial read

                switch (scriptManager.Property)
                {
                    case "mes":
                        t_complete.Clear();
                        foreach (var value in scriptManager.Values)
                        {
                            t_complete.Append(value.ToString());
                            t_complete.Append(Environment.NewLine);
                        }
                        break;
                    case "next":
                        scriptActive = true;
                        break;
                    case "close":
                        scriptActive = true;
                        break;
                    case "chooise":
                        scriptActive = true;
                        ChooiseList();
                        break;
                    case "openshop":
                        scriptActive = true;
                        OpenShop();
                        break;
                }
            }
        }

        private void updateDialogue(GameTime gameTime)
        {
            keyboardStateCurrent = Keyboard.GetState();

            // animated text
            if (previousGameTimeMsec <= (int)gameTime.TotalGameTime.Milliseconds
                || previousGameTimeSec != (int)gameTime.TotalGameTime.Seconds)
            {
                previousGameTimeMsec = (int)gameTime.TotalGameTime.Milliseconds + 80;
                previousGameTimeSec = (int)gameTime.TotalGameTime.Seconds;

                if (t_index < t_complete.Length)
                {
                    if (t_display.ToString().Split('\n').Length <= 4)
                    {
                        t_display.Append(t_complete[t_index]);
                        t_index++;
                    }
                }
            }

            // Read keyboard
            if (CheckKeySpace())
            {
                if (t_index == t_complete.Length) // Close
                {
                    if (scriptManager.Property == "close")
                    {
                        scriptManager.clearInstance();
                        clearStringBuilders();
                        this.Active = false;
                    }
                    else if (scriptManager.Property == "next")
                    {
                        this.scriptActive = false;
                        clearStringBuilders();
                    }
                    else if (scriptManager.Property == "chooise")
                    {
                        scriptManager.setChooise(menu.SelectedIndex + 1);
                        clearStringBuilders();
                        this.scriptActive = false;
                    }
                }
                else if (t_display.ToString().Split('\n').Length > 4) // Next
                {
                    t_display.Length = 0;
                    t_display.Capacity = 0;
                }
            }
            if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space) == true)
            {
                previousGameTimeMsec = 20; // speed up the text
            }

            keyboardStatePrevious = keyboardStateCurrent;
        }
        #endregion

        #region draw
        public override void Draw(GameTime gameTime)
        {

            if (Active)
            {
                Rectangle bounds = new Rectangle((int)(position.X + gfxdevice.Viewport.Width - size.X),
                                                 (int)(NPCPostion.Y + NPCPostion.Height * 0.95f),
                                                 (int)size.X, (int)size.Y);

                // Here we draw the text items
                if (t_display != null)
                {
                    // draw the text balloon
                    balloon.Active = true;
                    balloon.Position = new Vector2(bounds.X + (textoffsetX - 20), bounds.Y);
                    balloon.Width = 200;
                    balloon.Height = (t_display.ToString().Split('\n').Length * spriteFont.LineSpacing);
                    balloon.ArrowPosition = new Vector2(NPCPostion.X + NPCPostion.Width * 0.5f, 0);

                    if (t_index == t_complete.Length)
                    {
                        switch (scriptManager.Property)
                        {
                            case "close":
                            case "next":
                                balloon.Height = (int)(t_complete.ToString().Split('\n').Length * spriteFont.LineSpacing);
                                break;
                            case "chooise":
                                balloon.Height = (int)(t_complete.ToString().Split('\n').Length * spriteFont.LineSpacing) +
                                (scriptManager.Values.Count * 12);
                                break;
                        }
                    }

                    balloon.Draw(gameTime);

                    // Draw text based on message
                    spriteBatch.DrawString(spriteFont, t_display,
                        new Vector2(bounds.X + textoffsetX, bounds.Y + 10), Color.Black);

                    // Draw special options if applicable
                    if (t_index == t_complete.Length)
                    {
                        // property
                        int propertyposy = bounds.Y + (int)(t_complete.ToString().Split('\n').Length * spriteFont.LineSpacing);

                        switch(scriptManager.Property)
                        {
                            case "close":
                                spriteBatch.DrawString(spriteFont, "Close",
                                new Vector2(bounds.X + textoffsetX, propertyposy) + Vector2.One, Color.Black);
                                spriteBatch.DrawString(spriteFont, "Close",
                                new Vector2(bounds.X + textoffsetX, propertyposy), Color.Red);
                                break;
                            case "next":
                                spriteBatch.DrawString(spriteFont, "Next >>",
                                new Vector2(bounds.X + textoffsetX, propertyposy) + Vector2.One, Color.Black);
                                spriteBatch.DrawString(spriteFont, "Next >>",
                                new Vector2(bounds.X + textoffsetX, propertyposy), Color.Red);
                                break;
                            case "chooise":
                                menu.Position = new Vector2(bounds.X + textoffsetX, propertyposy);
                                menu.Show = true;
                                break;
                        }

                    }
                    else if (t_display.ToString().Split('\n').Length > 4)
                    {
                        // property
                        int propertyposy = bounds.Y + (int)(t_complete.ToString().Split('\n').Length * spriteFont.LineSpacing);

                        spriteBatch.DrawString(spriteFont, "Next >>",
                        new Vector2(bounds.X + 50, propertyposy) + Vector2.One, Color.Black);
                        spriteBatch.DrawString(spriteFont, "Next >>",
                        new Vector2(bounds.X + 50, propertyposy), Color.Red);
                    }
                }
            }

            base.Draw(gameTime);
        }
        #endregion

        #region functions
        // Message Functions
        private bool CheckKeySpace()
        {
            if (keyboardStateCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Space) == true &&
                keyboardStatePrevious.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space) == true)
                return true;
            else
                return false;
        }

        private void clearStringBuilders()
        {
            // reset the text and wrapper
            t_display.Length = 0;
            t_display.Capacity = 0;
            t_display.Clear();
            t_complete.Length = 0;
            t_complete.Capacity = 0;
            t_complete.Clear();
            t_index = 0;
            menu.Show = false;
            menu.SetMenuItems(new string[0]);
            menu.Active = false;
        }

        public int SelectedItem
        {
            get { return menu.SelectedIndex; }
        }

        // Script Functions
        public void ChooiseList()
        {
            string[] displayitems = new string[scriptManager.Values.Count];

            for (int id = 0; id < scriptManager.Values.Count; id++)
            {
                displayitems[id] = scriptManager.Values[id].ToString();
            }
            menu.SetMenuItems(displayitems);
            menu.SpriteFont = spriteFont;
            menu.Active = true;
            menu.SelectedIndex = 0;
        }

        public void OpenShop()
        {
            string[] displayitems = new string[scriptManager.Values.Count];

            for (int id = 0; id < scriptManager.Values.Count; id++)
            {
                displayitems[id] = scriptManager.Values[id].ToString();
            }

            // stop message popup
            this.Active = false;

            // clear script, reset to begin
            scriptManager.clearInstance();

            // open shope menu
            ScreenManager.Instance.shopMenuScreen.SetShopItems(displayitems);
            ScreenManager.Instance.setScreen("shopMenuScreen");
        }

        #endregion
    }
}
