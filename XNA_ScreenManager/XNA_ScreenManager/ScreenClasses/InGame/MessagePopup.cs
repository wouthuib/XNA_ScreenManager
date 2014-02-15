using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNA_ScreenManager.PlayerClasses;
using XNA_ScreenManager.ScriptClasses;
using System.Text;

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
        PlayerInfo playerInfo = PlayerInfo.Instance;
        ScriptInterpreter scriptManager = ScriptInterpreter.Instance;
        Texture2D NPCPicture;

        StringBuilder t_complete = new StringBuilder();
        StringBuilder t_display = new StringBuilder();
        int t_index;

        int previousGameTimeMsec,
            previousGameTimeSec;

        const int textoffsetX = 100; // draw offset

        Vector2 position,
                size;
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

            LoadContent();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            size = new Vector2((gfxdevice.Viewport.Width / 8) * 7, gfxdevice.Viewport.Height / 4);
            position = new Vector2(gfxdevice.Viewport.Width - size.X, gfxdevice.Viewport.Height - size.Y);
        }

        public void LoadAssets(Texture2D picture, string scriptfile)
        {
            // this will become the contructor
            t_display.Length = 0;
            t_display.Capacity = 0;
            t_complete.Length = 0;
            t_complete.Capacity = 0;
            t_index = 0;

            scriptfile = Game.Content.RootDirectory + @"\scriptDB\npc01.txt";
            scriptManager.loadFile(scriptfile);
            this.scriptActive = false;

            spriteFont = Content.Load<SpriteFont>(@"font\gamefont");
            NPCPicture = picture;
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
                        //this.scriptActive = false;
                        menu.SetMenuItems(new string[0]);
                        menu.Show = false;
                        menu.Active = false;
                        this.Active = false;
                    }
                    else if (scriptManager.Property == "next")
                    {
                        this.scriptActive = false;
                        clearStringBuilders();
                    }
                    else if (scriptManager.Property == "chooise")
                    {
                        //Random rand = new Random();
                        //scriptManager.setChooise((int)rand.Next(1, 5)); // temporary
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
            //base.Draw(gameTime);

            if (Active)
            {
                Rectangle bounds = new Rectangle((int)(position.X + gfxdevice.Viewport.Width - size.X),
                                                 (int)(position.Y + gfxdevice.Viewport.Height - size.Y),
                                                 (int)size.X, (int)size.Y);

                // Draw transperant background window
                Texture2D rect = new Texture2D(gfxdevice, bounds.Width, bounds.Height);

                Color[] data = new Color[bounds.Width * bounds.Height];
                for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
                rect.SetData(data);

                spriteBatch.Draw(rect, new Vector2(bounds.X, bounds.Y), Color.White * 0.8f);

                // Draw NPC picture if applicable
                if (NPCPicture != null)
                    spriteBatch.Draw(NPCPicture,
                        new Rectangle((int)position.X, (int)position.Y + 100, (int)200, (int)gfxdevice.Viewport.Height - 100),
                        Color.White);

                // Here we draw the text items
                if (t_display != null)
                {
                    spriteBatch.DrawString(spriteFont, t_display,
                        new Vector2(bounds.X + textoffsetX, bounds.Y + 20), Color.White);

                    if (t_index == t_complete.Length)
                    {
                        switch(scriptManager.Property)
                        {
                            case "close":
                                spriteBatch.DrawString(spriteFont, "Close",
                                new Vector2(bounds.X + textoffsetX, bounds.Bottom - 20), Color.Yellow);
                                break;
                            case "next":
                                spriteBatch.DrawString(spriteFont, "Next >>",
                                new Vector2(bounds.X + textoffsetX, bounds.Bottom - 20), Color.Yellow);
                                break;
                            case "chooise":
                                /*foreach (var option in scriptManager.Values)
                                {
                                    spriteBatch.DrawString(spriteFont, option.ToString(),
                                    new Vector2(bounds.X + textoffsetX, bounds.Bottom - 20), Color.Yellow);
                                    textoffsetX = textoffsetX + 100;
                                }*/
                                menu.Position = new Vector2(bounds.X + textoffsetX, bounds.Bottom - (20 * scriptManager.Values.Count));
                                break;
                        }

                    }
                    else if (t_display.ToString().Split('\n').Length > 4)
                    {
                        spriteBatch.DrawString(spriteFont, "Next >>",
                        new Vector2(bounds.X + 50, bounds.Bottom - 20), Color.Yellow);
                    }
                }
            }

            base.Draw(gameTime);
        }
        #endregion

        #region functions
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
        }

        public int SelectedItem
        {
            get { return menu.SelectedIndex; }
        }

        public void ChooiseList()
        {
            string[] displayitems = new string[scriptManager.Values.Count];

            for (int id = 0; id < scriptManager.Values.Count; id++)
            {
                displayitems[id] = scriptManager.Values[id].ToString();
            }
            menu.SetMenuItems(displayitems);
            menu.SpriteFont = spriteFont;
            menu.Show = true;
            menu.Active = true;
            menu.SelectedIndex = 0;
        }
        #endregion
    }
}
