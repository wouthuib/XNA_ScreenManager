using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.ScreenClasses.SubComponents;

namespace XNA_ScreenManager
{
    public class CreatePCScreen : GameScreen
    {
        MenuComponent menu;
        public KeyboardInput keyboardiput;
        BackgroundComponent bgcomp;

        int name = 0;
        bool gender = false;
        int difficultyLevel = 1;

        string[] menuItems = {
            "Change Name", 
            "Change Gender", 
            "Change Profession",
            "Continue" };

        int className = 0;
        string[] classNames = {
            "Fighter",
            "Archer",
            "Wizard",
            "Priest", 
            "Monk" };

        SpriteFont spriteFont;
        SpriteBatch spriteBatch;

        public CreatePCScreen(Game game, SpriteFont spriteFont, Texture2D background)
            : base(game)
        {
            spriteBatch = 
                (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            this.spriteFont = spriteFont;
            menu = new MenuComponent(game, spriteFont);
            keyboardiput = new KeyboardInput(game, spriteFont);
            bgcomp = new BackgroundComponent(game, background);
            menu.SetMenuItems(menuItems);
            Components.Add(bgcomp);
            Components.Add(menu);
            Components.Add(keyboardiput);
        }

        public int SelectedIndex
        {
            get { return menu.SelectedIndex; }
        }

        public int Name
        {
            get { return name; }
        }

        public int DifficultyLevel
        {
            get { return difficultyLevel; }
        }

        public bool Gender
        {
            get { return gender; }
        }

        public void ChangeGender()
        {
            menu.SetMenuItems(menuItems);
            gender = !gender;
        }

        public void ChangeClass()
        {
            className++;
            if (className == classNames.GetLength(0))
                className = 0;
        }

        public override void Show()
        {
            menu.Position = new Vector2((Game.Window.ClientBounds.Width -
            menu.Width) / 2, 300);
            base.Show();
        }

        public override void Update(GameTime gameTime)
        {
            if (keyboardiput.Active)
                bgcomp.Hue = true;
            else
                bgcomp.Hue = false;

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            // Draw the base first
            base.Draw(gameTime);

            if (!keyboardiput.Active)
            {
                // the menu items second
                Vector2 position = new Vector2();

                position = new Vector2(220, 120 - spriteFont.LineSpacing - 5);

                spriteBatch.DrawString(spriteFont,
                    "Name",
                    position,
                    Color.Yellow);

                position.X = 370;

                spriteBatch.DrawString(spriteFont,
                    "Gender",
                    position,
                    Color.Yellow);

                position.X = 520;

                spriteBatch.DrawString(spriteFont,
                    "Profession",
                    position,
                    Color.Yellow);

                position = new Vector2(220, 120);

                spriteBatch.DrawString(spriteFont,
                    PlayerClasses.PlayerInfo.Instance.Name,
                    position,
                    Color.White);

                if (gender)
                {
                    position.X = 370;

                    spriteBatch.DrawString(spriteFont,
                        "Female",
                        position,
                        Color.White);
                }
                else
                {
                    position.X = 370;

                    spriteBatch.DrawString(spriteFont,
                        "Male",
                        position,
                        Color.White);
                }

                position.X = 520;

                spriteBatch.DrawString(spriteFont,
                    classNames[className],
                    position,
                    Color.White);
            }
        }
    }
}
