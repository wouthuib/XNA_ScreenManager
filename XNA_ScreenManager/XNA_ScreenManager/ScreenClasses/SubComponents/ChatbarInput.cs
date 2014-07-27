using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNA_ScreenManager.GameWorldClasses.Effects;
using XNA_ScreenManager.MapClasses;
using System.Text;
using XNA_ScreenManager.PlayerClasses;
using XNA_ScreenManager.Networking;

namespace XNA_ScreenManager.ScreenClasses.SubComponents
{
    public class ChatbarInput : KeyboardInput
    {
        GameWorld world = GameWorld.GetInstance;
        KeyboardState newState, oldState;
        public string textlog = "";

        public ChatbarInput(Game game, SpriteFont spriteFont)
            : base(game, spriteFont, Vector2.Zero)
        {
            this.outline = outlinings[0];
        }

        public override void Update(GameTime gameTime)
        {
            newState = Keyboard.GetState();

            if(CheckKey(Keys.LeftControl))
                this.Active = !Active;

            if (CheckKey(Keys.Enter) && this.Active)
                createBaloon();

            base.Update(gameTime);

            oldState = newState;
        }

        public SpriteFont Font
        {
            get { return spriteFont; }
            set { spriteFont = value; }
        }

        private void createBaloon()
        {
            world.newEffect.Add(new ChatBalloon(world.playerSprite.PlayerName, Result, Content.Load<SpriteFont>(@"font\Arial_12px")));
            updateTextlog(world.playerSprite.PlayerName, Result);
            NetworkGameData.Instance.sendChatData(Result);
            Activate("");            
        }

        public void updateTextlog(string name, string newtext)
        {
            textlog += "[Local] " + name + ": " + newtext + "\n";

            if (textlog.ToString().Split('\n').Length > 8)
            {
                string newtextlog = "";
                for (int i = textlog.ToString().Split('\n').Length - 8; i < textlog.ToString().Split('\n').Length -1; i++)
                {
                    string addstr = textlog.ToString().Split('\n')[i] + "\n";
                    newtextlog += addstr;
                }

                textlog = newtextlog;
            }
        }

        private bool CheckKey(Keys theKey)
        {
            return oldState.IsKeyDown(theKey) && newState.IsKeyUp(theKey);
        }
    }
}
