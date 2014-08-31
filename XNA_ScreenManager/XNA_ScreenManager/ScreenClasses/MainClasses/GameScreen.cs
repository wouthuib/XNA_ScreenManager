using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XNA_ScreenManager.ScreenClasses.InGame;


namespace XNA_ScreenManager
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GameScreen : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private List<GameComponent> childComponents;
        public TopMessageScreen topmessage;

        public GameScreen(Game game)
            : base(game)
        {
            childComponents = new List<GameComponent>();
            Visible = false;
            Enabled = false;

            topmessage = new TopMessageScreen(game);
            Components.Add(topmessage);
            topmessage.Active = false;
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
            foreach (GameComponent child in childComponents)
            {
                if (child.Enabled)
                {
                    child.Update(gameTime);
                }
            }

            if (topmessage.Active)
                topmessage.Position = new Vector2(0, 0);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (GameComponent child in childComponents)
            {
                if ((child is DrawableGameComponent) &&
                ((DrawableGameComponent)child).Visible)
                {
                    ((DrawableGameComponent)child).Draw(gameTime);
                }
            }
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
    }
}
