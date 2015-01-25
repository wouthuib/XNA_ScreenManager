using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA_ScreenManager.GameAssets
{
    public delegate void ButtonOnClick(object sender);
    public delegate void ButtonOnHover(object sender);

    public class Button
    {
        public string Name = null, Menu = null;
        public bool Active = false;
        public Texture2D sprite, spr_normal, spr_onhover, spr_click;

        public event ButtonOnClick ButtonOnclick;
        public event ButtonOnHover ButtonOnhover;

        public Vector2 Position;
        public int Width = 0, Height = 0;

        public Rectangle Bounderies
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Width, Height); }
        }
              
        public static Button createButton(string name)
        {
            var results = new Button();
            
            results.Name = name;

            return results;
        }

        public bool onclick
        {
            get
            {
                if (
                    MouseManager.Instance.MouseButtonWasClicked(MouseButtons.Left) &&
                    MouseManager.Instance.MousePosition.Intersects(Bounderies))
                {
                    sprite = spr_click;
                    ButtonOnclick(this); // trigger event
                    return true;
                }
                else
                    return false;
            }
        }

        public bool onhover
        {
            get
            {
                if (MouseManager.Instance.MousePosition.Intersects(Bounderies))
                {
                    sprite = spr_onhover;
                    ButtonOnhover(this); // trigger event
                    return true;
                }
                else
                    return false;
            }
        }
    }
}
