using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XNA_ScreenManager.ScreenClasses.SubComponents
{
    public class ChatbarInput : KeyboardInput
    {

        TextBalloon balloon;
        KeyboardState newState, oldState;

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
            // draw the text balloon
            balloon.Active = true;
            balloon.Position = new Vector2();
            balloon.Width = 200;
            balloon.Height = (t_display.ToString().Split('\n').Length * spriteFont.LineSpacing);
            balloon.ArrowPosition = new Vector2(NPCPostion.X + NPCPostion.Width * 0.5f, 0);
        }

        private bool CheckKey(Keys theKey)
        {
            return oldState.IsKeyDown(theKey) && newState.IsKeyUp(theKey);
        }

    }
}
