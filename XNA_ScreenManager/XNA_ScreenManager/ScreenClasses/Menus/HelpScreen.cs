using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace XNA_ScreenManager
{
    public class HelpScreen : GameScreen
    {
        public HelpScreen(Game game, Texture2D background)
            : base(game)
        {
            Components.Add(new BackgroundComponent(game, background));
            base.Hide();
        }
    }
}
