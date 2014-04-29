using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using XNA_ScreenManager.ScreenClasses.SubComponents;
using XNA_ScreenManager.ItemClasses;
using Microsoft.Xna.Framework.Input;

namespace XNA_ScreenManager.ScreenClasses.InGame
{
    public class SkillScreen : GameScreen
    {
        #region properties
        Inventory inventory = Inventory.Instance;
        Equipment equipment = Equipment.Instance;
        ScreenManager manager = ScreenManager.Instance;

        SpriteFont spriteFont;
        SpriteBatch spriteBatch;
        GraphicsDevice graphics;
        ContentManager Content;

        Color normalColor = Color.Yellow;
        Color hiliteColor = Color.Red;

        KeyboardState oldState;

        string[] menuOptions = { 
             "Change", 
             "UnEquip", 
             "Cancel" };

        string[] menuItems = {
            "Select", 
            "Create", 
            "Delete",
            "Continue",
            "Back"};

        private bool OptionsActive = true;
        private int selectedOption = 0;
        private int width, height;

        #endregion

        public SkillScreen(Game game, Texture2D background)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            graphics = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));

            this.spriteFont = Content.Load<SpriteFont>(@"font\Comic_Sans_15px");

            Components.Add(new BackgroundComponent(game, background));
        }

        public Color NormalColor
        {
            get { return normalColor; }
            set { normalColor = value; }
        }

        public Color HiliteColor
        {
            get { return hiliteColor; }
            set { hiliteColor = value; }
        }

        private void CalculateBounds()
        {
            width = 0;
            height = 0;
            foreach (string slot in Enum.GetNames(typeof(ItemSlot)))
            {
                Vector2 size = spriteFont.MeasureString(slot);
                if (size.X > width)
                    width = (int)size.X;
                height += spriteFont.LineSpacing;
            }
        }

        public bool CheckKey(Keys theKey)
        {
            KeyboardState newState = Keyboard.GetState();
            return oldState.IsKeyDown(theKey) && newState.IsKeyUp(theKey);
        }

        public int SelectedOption
        {
            get { return selectedOption; }
            set
            {
                selectedOption = (int)MathHelper.Clamp(
                value,
                -1,
                menuOptions.Length);
            }
        }

        public override void Update(GameTime gameTime)
        {
            //record new keyboard state
            KeyboardState newState = Keyboard.GetState();

            if (OptionsActive)
            {

                if (CheckKey(Keys.Right))
                {
                    SelectedOption++;

                    if (SelectedOption >= menuOptions.Length) // last slot is "None" we skip this one
                        SelectedOption = 0;
                }
                else if (CheckKey(Keys.Left))
                {
                    SelectedOption--;

                    if (SelectedOption <= -1)
                        SelectedOption = menuOptions.Length - 1; // last slot is "None" we skip this one
                }
            }
            else
            {
            }

            base.Update(gameTime);

            // save keyboard state
            oldState = newState;
        }
        
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Vector2 position = new Vector2();
            Color myColor;

            #region menu options
            // Draw Menu Option Types
            position = new Vector2(320, 130);

            for (int i = 0; i < menuOptions.Length; i++)
            {
                if (OptionsActive)
                {
                    if (i == SelectedOption)
                        myColor = HiliteColor;
                    else
                        myColor = NormalColor;
                }
                else
                    myColor = Color.DarkGray;

                spriteBatch.DrawString(spriteFont,
                menuOptions[i],
                position,
                myColor);

                if (i < menuOptions.Length - 1)
                    position.X += 50 + (menuOptions[i].Length * 6);
            }
            #endregion
        }
    }
}
