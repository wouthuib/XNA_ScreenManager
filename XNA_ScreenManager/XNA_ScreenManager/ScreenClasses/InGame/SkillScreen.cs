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
using XNA_ScreenManager.SkillClasses;
using XNA_ScreenManager.PlayerClasses;

namespace XNA_ScreenManager.ScreenClasses.InGame
{
    public class SkillScreen : GameScreen
    {
        #region properties
        Inventory inventory = Inventory.Instance;
        Equipment equipment = Equipment.Instance;
        ScreenManager manager = ScreenManager.Instance;

        SpriteFont smallFont, normalFont;
        SpriteBatch spriteBatch;
        GraphicsDevice graphics;
        ContentManager Content;

        Color normalColor = Color.Yellow;
        Color hiliteColor = Color.Red;

        KeyboardState oldState;

        string[] menuOptions = { 
             "Select", 
             "Quick Slot", 
             "Cancel" };

        string[] menuItems = {
            "Select", 
            "Create", 
            "Delete",
            "Continue",
            "Back"};

        public bool OptionsActive = true,
                    SelectActive = false,
                    QuickSlotActive = false;
        private int selectedOption = 0;
        private int selectedColumn = 0, selectedRow = 0;
        private int width, height;
        private string[,] record = new string[4, 5];

        #endregion

        public SkillScreen(Game game, Texture2D background)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            graphics = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));

            this.smallFont = Content.Load<SpriteFont>(@"font\Comic_Sans_15px");
            this.normalFont = Content.Load<SpriteFont>(@"font\Comic_Sans_18px");

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
                Vector2 size = normalFont.MeasureString(slot);
                if (size.X > width)
                    width = (int)size.X;
                height += normalFont.LineSpacing;
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

        public int SelectedColumn
        {
            get { return selectedColumn; }
            set 
            {
                if (value < 0)
                    value = 3;
                else if (value > 3)
                    value = 0; 

                if (record[value, selectedRow] != null)
                    selectedColumn = value;                    
            }
        }

        public int SelectedRow
        {
            get { return selectedRow; }
            set
            {
                if (value < 0)
                    value = 4;
                else if (value > 4)
                    value = 0; 

                if (record[selectedColumn, value] != null)
                    selectedRow = value;
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
            else if (SelectActive)
            {
                if (CheckKey(Keys.Right))
                    SelectedColumn++;
                else if (CheckKey(Keys.Left))
                    SelectedColumn--;
                else if (CheckKey(Keys.Up))
                    SelectedRow--;
                else if (CheckKey(Keys.Down))
                    SelectedRow++;
            }

            // update 2d array records with skill items
            record = fetchSkills();  // <-- get skill data

            // base update
            base.Update(gameTime);

            // save keyboard state
            oldState = newState;
        }
        
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Vector2 position = new Vector2();
            Color myColor, adColor;

            #region menu options
            // Draw Menu Option Types
            position = new Vector2(80, 60);

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

                spriteBatch.DrawString(normalFont,
                menuOptions[i],
                position,
                myColor);

                if (i < menuOptions.Length - 1)
                    position.X += 50 + (menuOptions[i].Length * 6);
            }
            #endregion

            #region skills
            // Draw Skills in columns
            position = new Vector2(80, 150);

            // draw all skills in columns and rows            
            int row = 0, col = 0;

            for (col = 0; col < 4; col++)
                for (row = 0; row < 4; row++)
                    if (record[col, row] != null)
                    {
                        if (SelectActive && col == selectedColumn && row == selectedRow)
                        {
                            // check if the skill prerequisites are met
                            if (SkillTree.Instance.getSkillRequiments(SkillStore.Instance.getSkill(record[col, row]).SkillID))
                            {
                                myColor = Color.Red;
                                adColor = new Color(255, 81, 81);
                            }
                            else
                            {
                                myColor = new Color(161, 41, 41);
                                adColor = new Color(155, 102, 102);
                            }
                        }
                        else
                        {
                            // check if the skill prerequisites are met
                            if (SkillTree.Instance.getSkillRequiments(SkillStore.Instance.getSkill(record[col, row]).SkillID))
                            {
                                myColor = Color.Yellow;
                                adColor = Color.Yellow;
                            }
                            else
                            {
                                myColor = Color.Gray;
                                adColor = Color.Gray;
                            }
                        }

                        spriteBatch.DrawString(smallFont, record[col, row], new Vector2(position.X + col * 180, position.Y + row * 40), myColor);

                        if(SkillTree.Instance.getSkill(record[col, row]) != null)
                            spriteBatch.DrawString(smallFont, 
                                "Level - " + SkillTree.Instance.getSkill(record[col, row]).Level.ToString(),
                                new Vector2(position.X + col * 180, position.Y + (row * 40) + 15), adColor);
                        else
                            spriteBatch.DrawString(smallFont,
                                "Level - 0",
                                new Vector2(position.X + col * 180, position.Y + (row * 40) + 15), adColor);
                    }

            #endregion
        }

        private string[,] fetchSkills()
        {
            string[,] record = new string[4, 5];

            foreach (Skill skill in SkillStore.Instance.skill_list.FindAll(delegate(Skill getclass)
                           { return getclass.Class.ToString() == PlayerStore.Instance.activePlayer.Jobclass.ToString(); }))
            {
                for (int i = 0; i < 5; i++)
                    if (record[skill.SkillTreeColumn, i] == null)
                    {
                        record[skill.SkillTreeColumn, i] = skill.SkillName;
                        break;
                    }
            }

            return record;
        }
    }
}
