using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.ScreenClasses;
using XNA_ScreenManager.CharacterClasses;

namespace XNA_ScreenManager
{
    public class ActionScreen : GameScreen
    {
        SpriteFont gameFont;
        ContentManager Content;
        GraphicsDevice gfxdevice;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        ScreenManager screenManager = ScreenManager.Instance;
        HUDScreen hud;
        GameWorld world;
        public Camera2d cam;

        public ActionScreen(Game game, SpriteFont gameFont)
            : base(game)
        {
            this.gameFont = gameFont;

            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            gfxdevice = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));

            cam = new Camera2d();

            GameWorld createworld = GameWorld.CreateInstance(game, cam);
            world = createworld;
            world.Active = true;

            hud = new HUDScreen(game, spriteFont);
            Components.Add(hud);
            hud.Active = true;

            spriteFont = Content.Load<SpriteFont>(@"font\normalfont");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (world.Active)
            {
                world.Update(gameTime);
                moveCamera();
                manageScreens();
            }
        }

        public void manageScreens()
        {
            if (hud.Active)
                hud.Position = ViewPort();

            if (screenManager.MessagePopupScreen.Active)
            {
                world.Paused = true;
                hud.Active = false;
                screenManager.ScreenViewport(ViewPort(), "MessagePopupScreen");
            }
            else
            {
                if (world.Paused == true)
                    world.Paused = false;
                if (hud.Active == false)
                    hud.Active = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Stop the Base spritebatch
            spriteBatch.End();

            // Start the Game spritebatch
            spriteBatch.Begin(SpriteSortMode.Immediate,
                                     BlendState.AlphaBlend,
                                     null,
                                     null,
                                     null,
                                     null,
                                     cam.get_transformation(gfxdevice));

            if(world.Active)
                world.Draw(gameTime);

            base.Draw(gameTime);
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

        #region functions

        public Vector2 ViewPort()
        {
            return new Vector2(cam._pos.X - gfxdevice.Viewport.Width / 2, cam._pos.Y - gfxdevice.Viewport.Height / 2);
        }

        public void moveCamera()
        {
            if ((world.playerSprite.Position.X - (gfxdevice.Viewport.Width / 2) > 0 &&
                world.playerSprite.Position.X + (gfxdevice.Viewport.Width / 2) < world.map.Width * world.map.TileWidth)
                || cam._pos.X - (gfxdevice.Viewport.Width / 2) < 0)
            {
                cam._pos.X = world.playerSprite.Position.X;
            }

            if ((world.playerSprite.Position.Y - (gfxdevice.Viewport.Height / 2) > 0 &&
                world.playerSprite.Position.Y + (gfxdevice.Viewport.Height / 2) < world.map.Height * world.map.TileHeight)
                || cam._pos.Y - (gfxdevice.Viewport.Height / 2) < 0)
            {
                cam._pos.Y = world.playerSprite.Position.Y;
            }
        }

        #endregion
    }
}


























