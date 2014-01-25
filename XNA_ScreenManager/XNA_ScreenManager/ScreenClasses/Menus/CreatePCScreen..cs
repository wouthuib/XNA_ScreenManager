using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA_ScreenManager
{
    public class CreatePCScreen : GameScreen
    {
        MenuComponent menu;

        int name = 0;
        bool gender = false;
        int difficultyLevel = 1;

        string[] menuItems = {
            "Select New Name", 
            "Make Him a Woman", 
            "Change Profession",
            "Return to Start Screen",
            "Begin the Adventure" };

        string[] maleNames = {
            "Aris",
            "Barton",
            "Evander",
            "Kalven",
            "Llelwyn" };

        string[] femaleNames = {
            "Anwyn",
            "Cantrinia",
            "Julia",
            "Lucy",
            "Zoey" };

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
            menu.SetMenuItems(menuItems);
            Components.Add(new BackgroundComponent(game, background));
            Components.Add(menu);
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

        public void ChangeName()
        {
            Random random = new Random();
            name++;
            if (name == femaleNames.GetLength(0))
                name = 0;
        }

        public void ChangeGender()
        {
            if (gender)
                menuItems[1] = "Make Him a Woman";
            else
                menuItems[1] = "Make Her a Man";
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
        public override void Draw(GameTime gameTime)
        {
            // Draw the base first
            base.Draw(gameTime);

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

            if (gender)
            {
                spriteBatch.DrawString(spriteFont,
                    femaleNames[name],
                    position,
                    Color.White);

                position.X = 370;

                spriteBatch.DrawString(spriteFont,
                    "Female",
                    position,
                    Color.White);
            }
            else
            {
                spriteBatch.DrawString(spriteFont,
                    maleNames[name],
                    position,
                    Color.White);

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
