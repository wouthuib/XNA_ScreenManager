using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using XNA_ScreenManager.ScreenClasses.MainClasses;

namespace XNA_ScreenManager.GameAssets
{
    public class Menu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected SpriteBatch spritebatch = null;
        protected ContentManager content;
        protected GraphicsDevice gfxdevice;

        protected Texture2D sprite;
        public Vector2 Position, OldPosition;
        public Vector2 Viewport;

        public Rectangle Bounderies
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height); }
        }

        public string Name;
        public List<Button> listbutton = new List<Button>();
        public List<DraggableObject> listdraggables = new List<DraggableObject>();

        public bool DragLock = false;
        Vector2 DragPoint;
        public float LockTime = 0;

        private List<GameComponent> childComponents;

        public Menu(Game game, float locktime)
            : base(game)
        {
            spritebatch = ResourceManager.GetInstance.spriteBatch;
            content = ResourceManager.GetInstance.Content;
            gfxdevice = ResourceManager.GetInstance.gfxdevice;

            childComponents = new List<GameComponent>();
            Visible = false;
            Enabled = false;
            LockTime = locktime;
        }

        public List<GameComponent> Components
        {
            get { return childComponents; }
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (!MenuManager.Instance.itemdragged)
            {
                if (isDragged) /// check menu.
                {
                    MenuManager.Instance.menudragged = true; // set item dragged in menu manager.
                }
            }
            else
                MenuManager.Instance.itemdragged = true; // set item dragged in menu manager.

            if (DragLock)
                LockTime = (float)gameTime.TotalGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //foreach (GameComponent child in childComponents)
            //{
            //    if ((child is DrawableGameComponent) &&
            //    ((DrawableGameComponent)child).Visible)
            //    {
            //        ((DrawableGameComponent)child).Draw(gameTime);
            //    }
            //}
            base.Draw(gameTime);
        }

        public virtual void Show()
        {
            Visible = true;
            Enabled = true;
        }

        public virtual void Hide()
        {
            Visible = false;
            Enabled = false;
        }

        protected Button create_button(string name)
        {
            Button button = Button.createButton(name);
            listbutton.Add(button);
            button.ButtonOnclick += new ButtonOnClick(event_buttonOnClick);
            return button;
        }

        protected Button create_draggable(string name, object obj, string menu)
        {
            DraggableObject button = DraggableObject.createDraggable(name, obj, menu);
            listdraggables.Add(button);
            button.ButtonOnDrag += new ButtonOnDrag(event_buttonOnDrag);
            button.ButtonOnRelease += new ButtonOnRelease(
               DraggableObject.event_releaseDraggable); // new event
            return button;
        }
        
        protected virtual void event_buttonOnClick(object btn)
        {
            // write events here based on the button
        }

        protected virtual void event_buttonOnDrag(object btn)
        {
            // write events here based on the button
        }

        protected bool isDragged
        {
            get 
            {
                if (MouseManager.Instance.MouseButtonIsDown(MouseButtons.Left) &&
                    MouseManager.Instance.MousePosition.Intersects(Bounderies) ||
                    DragLock)
                {
                    // sometimes the mouse pointer gets outside the menu, 
                    // but as long as left button is pressed the menu keeps moving
                    if (!MouseManager.Instance.MouseButtonIsDown(MouseButtons.Left) ||
                        MenuManager.Instance.active_menu(this) == true ||
                        MenuManager.Instance.itemdragged == true)
                    {
                        MenuManager.Instance.menudragged = false; // unset menu manager.
                        DragLock = false;
                        return false;
                    }

                    // save old poistion for change calculation
                    OldPosition = Position;

                    if (!DragLock)
                    {
                        this.DragPoint = new Vector2(MouseManager.Instance.MousePosition.X - this.Position.X,
                            MouseManager.Instance.MousePosition.Y - this.Position.Y);
                        DragLock = true;
                    }

                    this.Position = new Vector2(MouseManager.Instance.MousePosition.X - this.DragPoint.X,
                            MouseManager.Instance.MousePosition.Y - this.DragPoint.Y);

                    if (Bounderies.Top > gfxdevice.Viewport.Height - 5 || Bounderies.Bottom < 5 ||
                        Bounderies.Left > gfxdevice.Viewport.Width - 5 || Bounderies.Right < 5)
                        Position = OldPosition;
                    
                    // Update button positions
                    foreach (var button in listbutton)
                        button.Position += (Position - OldPosition);
                    foreach (var button in listdraggables)
                        button.Position += (Vector2)((Position - OldPosition) / 2);

                    return true;
                }
                else
                {
                    DragLock = false;
                    return false;
                }
            }
        }
    }
}
