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
        //Inventory inventory = Inventory.Instance;
        //Equipment equipment = Equipment.Instance;
        ScreenManager manager = ScreenManager.Instance;

        SpriteFont smallFont, normalFont;
        SpriteBatch spriteBatch;
        GraphicsDevice graphics;
        ContentManager Content;

        public MenuComponent options;

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

        public bool menuOptionsActive = true,
                    SelectActive = false,
                    QuickSlotActive = false,
                    skillOptionsActive = false;
        private int selectedOption = 0;
        public int selectedColumn = 0, selectedRow = 0;
        private int width, height;
        public string[,] record = new string[4, 5];

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
            options = new MenuComponent(game, smallFont);
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

            if (menuOptionsActive)
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
            else if (skillOptionsActive)
            {
                // update skill components
                options.Update(gameTime);
            }
            else if (QuickSlotActive)
            {
                updateQuickSlot(gameTime);

                // update skill components
                options.Update(gameTime);
            }

            // update 2d array records with skill items
            record = fetchSkills();  // <-- get skill data

            // base update
            base.Update(gameTime);

            // save keyboard state
            oldState = newState;
        }

        private void updateQuickSlot(GameTime gameTime)
        {
            Skill selectedskill = SkillStore.Instance.getSkill(record[selectedColumn, selectedRow]);

            // Check quickslot options
            if (CheckKey(Keys.F1))
            {
                PlayerStore.Instance.activePlayer.skillbar.skillslot[0] = selectedskill;
                QuickSlotActive = false;
                SelectActive = true;
            }
            else if (CheckKey(Keys.F2))
            {
                PlayerStore.Instance.activePlayer.skillbar.skillslot[1] = selectedskill;
                QuickSlotActive = false;
                SelectActive = true;
            }
            else if (CheckKey(Keys.F3))
            {
                PlayerStore.Instance.activePlayer.skillbar.skillslot[2] = selectedskill;
                QuickSlotActive = false;
                SelectActive = true;
            }
            else if (CheckKey(Keys.F4))
            {
                PlayerStore.Instance.activePlayer.skillbar.skillslot[3] = selectedskill;
                QuickSlotActive = false;
                SelectActive = true;
            }
            else if (CheckKey(Keys.F5))
            {
                PlayerStore.Instance.activePlayer.skillbar.skillslot[4] = selectedskill;
                QuickSlotActive = false;
                SelectActive = true;
            }
            else if (CheckKey(Keys.F6))
            {
                PlayerStore.Instance.activePlayer.skillbar.skillslot[5] = selectedskill;
                QuickSlotActive = false;
                SelectActive = true;
            }
            else if (CheckKey(Keys.F7))
            {
                PlayerStore.Instance.activePlayer.skillbar.skillslot[6] = selectedskill;
                QuickSlotActive = false;
                SelectActive = true;
            }
            else if (CheckKey(Keys.F8))
            {
                PlayerStore.Instance.activePlayer.skillbar.skillslot[7] = selectedskill;
                QuickSlotActive = false;
                SelectActive = true;
            }
            else if (CheckKey(Keys.F9))
            {
                PlayerStore.Instance.activePlayer.skillbar.skillslot[8] = selectedskill;
                QuickSlotActive = false;
                SelectActive = true;
            }
            else if (CheckKey(Keys.F10))
            {
                PlayerStore.Instance.activePlayer.skillbar.skillslot[9] = selectedskill;
                QuickSlotActive = false;
                SelectActive = true;
            }
            else if (CheckKey(Keys.F11))
            {
                PlayerStore.Instance.activePlayer.skillbar.skillslot[10] = selectedskill;
                QuickSlotActive = false;
                SelectActive = true;
            }
            else if (CheckKey(Keys.F12))
            {
                PlayerStore.Instance.activePlayer.skillbar.skillslot[11] = selectedskill;
                QuickSlotActive = false;
                SelectActive = true;
            }

        }
        
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Vector2 position = new Vector2();
            Color myColor, adColor, icColor;

            #region menu options
            // Draw Menu Option Types
            position = new Vector2(80, 60);

            for (int i = 0; i < menuOptions.Length; i++)
            {
                if (menuOptionsActive)
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
                            if (PlayerStore.Instance.activePlayer.skilltree.getSkillRequiments(SkillStore.Instance.getSkill(record[col, row]).ID))
                            {
                                myColor = Color.Red;
                                adColor = new Color(255, 81, 81);
                                icColor = Color.White;
                            }
                            else
                            {
                                myColor = new Color(161, 41, 41);
                                adColor = new Color(155, 102, 102);
                                icColor = Color.Gray;
                            }

                            // Draw Selected Skill Description
                            spriteBatch.DrawString(smallFont,
                                SkillStore.Instance.getSkill(record[col, row]).Description, 
                                new Vector2(50, 450), Color.Yellow);
                        }
                        else
                        {
                            // check if the skill prerequisites are met
                            if (PlayerStore.Instance.activePlayer.skilltree.getSkillRequiments(SkillStore.Instance.getSkill(record[col, row]).ID))
                            {
                                myColor = Color.Yellow;
                                adColor = Color.Yellow;
                                icColor = Color.White;
                            }
                            else
                            {
                                myColor = Color.Gray;
                                adColor = Color.Gray;
                                icColor = Color.Gray;
                            }
                        }

                        // Draw Skill Name
                        spriteBatch.DrawString(smallFont, record[col, row], new Vector2(position.X + col * 180, position.Y + row * 40), myColor);

                        // Draw Skill Icon
                        if (SkillStore.Instance.getSkill(record[col, row]).IconSpritePath != null)
                            spriteBatch.Draw(Content.Load<Texture2D>(@"" + SkillStore.Instance.getSkill(record[col, row]).IconSpritePath),
                                new Vector2(position.X + col * 180 - 45, position.Y + (row * 40)), icColor);

                        // Draw Current skill Level
                        if (PlayerStore.Instance.activePlayer.skilltree.getSkill(record[col, row]) != null)
                            spriteBatch.DrawString(smallFont,
                                "Level - " + PlayerStore.Instance.activePlayer.skilltree.getSkill(record[col, row]).Level.ToString(),
                                new Vector2(position.X + col * 180, position.Y + (row * 40) + 15), adColor);
                        else
                            spriteBatch.DrawString(smallFont,
                                "Level - 0",
                                new Vector2(position.X + col * 180, position.Y + (row * 40) + 15), adColor);
                    }

            #endregion

            #region skilloption popup
            // item options
            if (skillOptionsActive || QuickSlotActive)
            {
                Texture2D rect = new Texture2D(graphics, (int)options.getBounds().X, options.MenuItems.Count * 20),
                          rect2 = new Texture2D(graphics, (int)options.getBounds().X, options.MenuItems.Count * 20);

                Color[] data = new Color[(int)options.getBounds().X * options.MenuItems.Count * 20];

                Vector2 PopupPosition = new Vector2((graphics.Viewport.Width * 0.5f) - (options.getBounds().X * 0.5f),
                                                    (graphics.Viewport.Height * 0.5f) - (options.MenuItems.Count * 10));

                // set colors for menu borders and fill
                for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
                rect.SetData(data);
                for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
                rect2.SetData(data);

                // draw menu fill 20% transperancy
                spriteBatch.Draw(rect,
                    new Rectangle((int)(PopupPosition.X - 5), (int)(PopupPosition.Y - 5), rect.Width + 10, rect.Height + 10), Color.White * 0.8f);

                // draw borders
                spriteBatch.Draw(rect2,
                    new Rectangle((int)(PopupPosition.X - 5), (int)(PopupPosition.Y - 5), 5, (int)options.MenuItems.Count * 20 + 10),
                    new Rectangle(0, 0, 5, 5), Color.White);
                spriteBatch.Draw(rect2,
                    new Rectangle((int)(PopupPosition.X - 5), (int)(PopupPosition.Y - 5), (int)rect.Width + 10, 5),
                    new Rectangle(0, 0, 5, 5), Color.White);
                spriteBatch.Draw(rect2,
                    new Rectangle((int)(PopupPosition.X - 5), (int)(PopupPosition.Y + (options.MenuItems.Count * 20) + 5), (int)rect.Width + 15, 5),
                    new Rectangle(0, 0, 5, 5), Color.White);
                spriteBatch.Draw(rect2,
                    new Rectangle((int)(PopupPosition.X + rect.Width + 5), (int)(PopupPosition.Y - 5), 5, (int)options.MenuItems.Count * 20 + 15),
                    new Rectangle(0, 0, 5, 5), Color.White);


                Vector2 optionPos = new Vector2(PopupPosition.X, PopupPosition.Y);

                options.Position = optionPos;
                options.Draw(gameTime);
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
                        record[skill.SkillTreeColumn, i] = skill.Name;
                        break;
                    }
            }

            return record;
        }
    }
}
