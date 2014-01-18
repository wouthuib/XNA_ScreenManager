using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace XNA_ScreenManager
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class BackgroundComponent : DrawableGameComponent
    {
        Texture2D background;
        SpriteBatch spriteBatch = null;
        Rectangle bgRect;

        public BackgroundComponent(Game game, Texture2D texture)
            : base(game)
        {
            this.background = texture;
            spriteBatch =
            (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            bgRect = new Rectangle(0,
            0,
            Game.Window.ClientBounds.Width,
            Game.Window.ClientBounds.Height);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(background, bgRect, Color.White);
            base.Draw(gameTime);
        }
    }
}
