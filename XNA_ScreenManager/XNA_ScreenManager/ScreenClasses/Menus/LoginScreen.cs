using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.ScreenClasses.SubComponents;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using XNA_ScreenManager.Networking;

namespace XNA_ScreenManager.ScreenClasses.Menus
{
    public class LoginScreen : GameScreen
    {
        string[] phases = new string[] { "username", "password", "login" };
        int phase;
        
        SpriteBatch spriteBatch;
        ContentManager Content;
        KeyboardInput keyboardiput;
        
        KeyboardState newState, oldState;
        SpriteFont spriteFont;
        string username = "", password = "";

        public MenuComponent menu;
        string[] menuItems = {
            "Login",
            ""};

        public LoginScreen(Game game, Texture2D background)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));

            Components.Add(new BackgroundComponent(game, background));

            menu = new MenuComponent(game, Content.Load<SpriteFont>(@"font\Comic_Sans_18px"));

            // menu options
            menu.SetMenuItems(menuItems);
            menu.StartIndex = 0;
            menu.SelectedIndex = 1;
            menu.Position = new Vector2(610, 195);
            menu.NormalColor = Color.White;
            menu.HiliteColor = Color.Red;
            menu.EndIndex = 1;

            spriteFont = Content.Load<SpriteFont>(@"font\Arial_12px");
            keyboardiput = new KeyboardInput(game, spriteFont, new Vector2(450, 186));
            keyboardiput.outline = "left";

            phase = 0;
            keyboardiput.Activate(username);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            newState = Keyboard.GetState();
            int oldphase = phase;

            keyboardiput.Update(gameTime);

            if (CheckKey(Keys.Down) || CheckKey(Keys.Tab) || (CheckKey(Keys.Enter) && phases[phase] != "login"))
                phase++;
            else if (CheckKey(Keys.Escape) || CheckKey(Keys.Up))
                phase--;
            else if (CheckKey(Keys.Enter) && phases[phase] == "login")
                NetworkGameData.Instance.sendAccountData(username, password);

            if (phase > 2)
                phase = 0;
            else if (phase < 0)
                phase = 2;

            if (oldphase != phase)
            {
                switch (phases[phase])
                {
                    case "username":
                        if (oldphase == 1)
                            password = keyboardiput.Result;
                        keyboardiput.Position = new Vector2(450, 186);
                        keyboardiput.Password = false;
                        keyboardiput.Activate(username);
                        menu.SelectedIndex = 1;
                        break;
                    case "password":
                        if (oldphase == 0)
                            username = keyboardiput.Result;
                        keyboardiput.Position = new Vector2(450, 213);
                        keyboardiput.Password = true;
                        keyboardiput.Activate(password);
                        menu.SelectedIndex = 1;
                        break;
                    case "login":
                        if (oldphase == 0)
                            username = keyboardiput.Result;
                        else if (oldphase == 1)
                            password = keyboardiput.Result; 
                        keyboardiput.Active = false;
                        menu.SelectedIndex = 0;
                        break;
                }
            }

            oldState = newState;
        }

        public override void Draw(GameTime gameTime)
        {
            /// Top message and other subcomponents
            base.Draw(gameTime);

            keyboardiput.Draw(gameTime);

            if (username != "" && phases[phase] != "username")
            {
                spriteBatch.DrawString(spriteFont, username, new Vector2(450, 186) + Vector2.One, Color.Black);
                spriteBatch.DrawString(spriteFont, username, new Vector2(450, 186), Color.White);
            }
            if (password != "" && phases[phase] != "password")
            {
                spriteBatch.DrawString(spriteFont, HiddenPassword, new Vector2(450, 213) + Vector2.One, Color.Black);
                spriteBatch.DrawString(spriteFont, HiddenPassword, new Vector2(450, 213), Color.White);
            }

            // Draw menu items
            menu.Draw(gameTime);

            topmessage.Draw(gameTime);
        }

        private bool CheckKey(Keys theKey)
        {
            return oldState.IsKeyDown(theKey) && newState.IsKeyUp(theKey);
        }

        private string HiddenPassword
        {
            get
            {
                if(password == "")
                    return "";

                string output = "";
                for (int c = 0; c < password.Length; c++)
                    output += "*";

                return output;
            }
        }
    }
}
