using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.ScreenClasses.SubComponents;
using Microsoft.Xna.Framework.Content;

namespace XNA_ScreenManager.ScreenClasses.Menus
{
    public class CharacterCreationScreen : GameScreen
    {
        SpriteBatch spriteBatch;
        ContentManager Content;

        KeyboardInput keyboardiput;
        BackgroundComponent bgcomp1, bgcomp2, bgcomp3;
        SpriteFont spriteFont;

        public CharacterCreationScreen(Game game)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));

            spriteFont = Content.Load<SpriteFont>(@"font\Arial_12px");

            keyboardiput = new KeyboardInput(game, spriteFont);
            bgcomp1 = new BackgroundComponent(game, Content.Load<Texture2D>(@"gfx\background\background03"));
            bgcomp2 = new BackgroundComponent(game, Content.Load<Texture2D>(@"gfx\background\character_create"));
            bgcomp3 = new BackgroundComponent(game, Content.Load<Texture2D>(@"gfx\background\frame2"));

            Components.Add(bgcomp1);
            Components.Add(bgcomp2);
            Components.Add(bgcomp3);
            Components.Add(keyboardiput);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

    }
}
