using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace XNA_ScreenManager.GameAssets
{
    class MouseManager : GameComponent
    {
        #region properties

        private MouseState priviousMouseState;
        private MouseState currentMouseState;

        private readonly IDictionary<MouseButtons, Func<MouseState, ButtonState>> mouseButtonMaps;

        #endregion

        #region contsructor
        private static MouseManager instance;

        private MouseManager()
            : base(null)
        {
            mouseButtonMaps = new Dictionary<MouseButtons, Func<MouseState, ButtonState>>
			{
				{ MouseButtons.Left, s => s.LeftButton },
				{ MouseButtons.Right, s => s.RightButton },
				{ MouseButtons.Middle, s => s.MiddleButton },
				{ MouseButtons.Extra1, s => s.XButton1 },
				{ MouseButtons.Extra2, s => s.XButton2 }
			};
        }

        public static MouseManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new MouseManager();

                return instance;
            }
        }

        // When the game begins
        public void StartManager()
        {
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            // Mouse
            priviousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
 	        base.Update(gameTime);
        }

        public bool MouseButtonIsDown(MouseButtons button)
        {
            return mouseButtonMaps[button](currentMouseState) == ButtonState.Pressed;
        }

        public bool MouseButtonIsUp(MouseButtons button)
        {
            return mouseButtonMaps[button](currentMouseState) == ButtonState.Released;
        }

        public bool MouseButtonWasClicked(MouseButtons button)
        {
            return
                mouseButtonMaps[button](currentMouseState) == ButtonState.Released &&
                mouseButtonMaps[button](priviousMouseState) == ButtonState.Pressed;
        }

        public Rectangle MousePosition()
        {
            return new Rectangle((int)Mouse.GetState().X, (int)Mouse.GetState().Y, 1, 1);
        }
    }

    public enum MouseButtons
    {
        Left,
        Middle,
        Right,
        Extra1,
        Extra2
    }
}
