using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.ScreenClasses.InGame;

namespace XNA_ScreenManager.ScreenClasses
{
    public class ScreenManager
    {
        public Game game;
        public StartScreen startScreen;
        public HelpScreen helpScreen;
        public ActionScreen actionScreen;
        public CreatePCScreen createPCScreen;
        public InGameMainMenuScreen ingameMenuScreen;
        public ItemMenuScreen itemMenuScreen;
        public EquipmentMenuScreen equipmentMenuScreen;
        public ShopMenuScreen shopMenuScreen;
        public MessagePopup MessagePopupScreen;
        public LoadingScreen loadingScreen;

        GameScreen activeScreen;
        KeyboardState newState;
        KeyboardState oldState;

        private static ScreenManager instance;
        
        private ScreenManager()
        {
            Initize();
        }

        public static ScreenManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ScreenManager();

                return instance;
            }
        }

        // use this method to initize some parameters
        // keep in mind that the screens are empty here.
        public void Initize()
        {
        }

        // Return class name
        public string getActive()
        {
            string screen = activeScreen.GetType().FullName.ToString();
            return screen;
        }

        // When the game begins
        public void StartManager()
        {
            actionScreen.Hide();
            actionScreen.Hide();
            createPCScreen.Hide();
            ingameMenuScreen.Hide();
            itemMenuScreen.Hide();
            equipmentMenuScreen.Hide();
            shopMenuScreen.Hide();
            MessagePopupScreen.Hide();
            loadingScreen.Hide();

            startScreen.Show();
            activeScreen = startScreen;
        }

        public void Update(GameTime gameTime)
        {
            newState = Keyboard.GetState();

            if (activeScreen == startScreen)
            {
                HandleStartScreenInput();
            }
            else if (activeScreen == helpScreen)
            {
                HandleHelpScreenInput();
            }
            else if (activeScreen == createPCScreen)
            {
                HandleCreatePCScreenInput();
            }
            else if (activeScreen == actionScreen)
            {
                HandleActionScreenInput();
            }
            else if (activeScreen == ingameMenuScreen)
            {
                HandleingameMenuInput();
            }
            else if (activeScreen == itemMenuScreen)
            {
                HandleitemMenuScreen();
            }
            else if (activeScreen == equipmentMenuScreen)
            {
                HandleequipmentMenuScreen();
            }
            else if (activeScreen == shopMenuScreen)
            {
                HandleshopMenuScreen();
            }
            else if (activeScreen == loadingScreen)
            {
                //HandleloadingScreen();
            }

            oldState = newState;
        }


        private void HandleHelpScreenInput()
        {
            if (CheckKey(Keys.Space) ||
                CheckKey(Keys.Enter) ||
                CheckKey(Keys.Back) ||
                CheckKey(Keys.Escape))
            {
                activeScreen.Hide();
                activeScreen = startScreen;
                activeScreen.Show();
            }
        }

        private void HandleStartScreenInput()
        {
            if (CheckKey(Keys.Enter) || CheckKey(Keys.Space))
            {
                switch (startScreen.SelectedIndex)
                {
                    case 0:
                        activeScreen.Hide();
                        activeScreen = createPCScreen;
                        activeScreen.Show();
                        break;
                    case 1:
                        activeScreen.Hide();
                        activeScreen = actionScreen;
                        actionScreen.Show();
                        break;
                    case 2:
                        activeScreen.Hide();
                        activeScreen = helpScreen;
                        activeScreen.Show();
                        break;
                    case 3:
                        game.Exit();
                        break;
                }
            }
        }

        private void HandleCreatePCScreenInput()
        {
            if (CheckKey(Keys.Enter) || CheckKey(Keys.Space))
            {
                switch (createPCScreen.SelectedIndex)
                {
                    case 0:
                        createPCScreen.ChangeName();
                        break;
                    case 1:
                        createPCScreen.ChangeGender();
                        break;
                    case 2:
                        createPCScreen.ChangeClass();
                        break;
                    case 3:
                        activeScreen.Hide();
                        activeScreen = startScreen;
                        activeScreen.Show();
                        break;
                    case 4:
                        activeScreen.Hide();
                        activeScreen = actionScreen;
                        activeScreen.Show();
                        break;
                }
            }
        }

        private void HandleActionScreenInput()
        {
            if (CheckKey(Keys.Tab) || CheckKey(Keys.Escape))
            {
                activeScreen.Hide();
                activeScreen = ingameMenuScreen;
                activeScreen.Show();
            }
        }

        private void HandleingameMenuInput()
        {
            if (CheckKey(Keys.Enter))
            {
                switch (ingameMenuScreen.SelectedIndex)
                {
                    case 0:
                        activeScreen.Hide();
                        itemMenuScreen.updateItemList();
                        activeScreen = itemMenuScreen;
                        activeScreen.Show();
                        break;
                    case 1:
                        activeScreen.Hide();
                        activeScreen = shopMenuScreen;
                        activeScreen.Show();
                        break;
                    case 2:
                        activeScreen.Hide();
                        activeScreen = equipmentMenuScreen;
                        activeScreen.Show();
                        break;
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        activeScreen.Hide();
                        activeScreen = startScreen;
                        activeScreen.Show();
                        break;
                }
            }
            else if (CheckKey(Keys.Back) || CheckKey(Keys.Escape))
            {
                activeScreen.Hide();
                activeScreen = actionScreen;
                activeScreen.Show();
            }
        }

        private void HandleitemMenuScreen()
        {
            if (CheckKey(Keys.Enter))
            {
                switch (ingameMenuScreen.SelectedIndex)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        break;
                }
            }
            else if (CheckKey(Keys.Back) || CheckKey(Keys.Escape))
            {
                activeScreen.Hide();
                activeScreen = ingameMenuScreen;
                activeScreen.Show();
            }
        }

        private void HandleequipmentMenuScreen()
        {
            if (CheckKey(Keys.Back) || CheckKey(Keys.Escape))
            {
                activeScreen.Hide();
                activeScreen = ingameMenuScreen;
                activeScreen.Show();
            }
        }

        private void HandleshopMenuScreen()
        {
            if (CheckKey(Keys.Back) || CheckKey(Keys.Escape))
            {
                activeScreen.Hide();
                activeScreen = ingameMenuScreen;
                activeScreen.Show();
            }
        }

        public void ScreenViewport(Vector2 postion, string screenName)
        {
            switch (screenName)
            {
                case "MessagePopupScreen":
                    MessagePopupScreen.Position = postion;
                    break;
            }
        }

        // manually set screen
        public void setScreen(string screenName)
        {
            activeScreen.Hide();

            //Type type = this.GetType().Assembly.GetType("XNA_ScreenManager.ScreenClasses." + screenName);
            //object instance = Activator.CreateInstance(type);
            //activeScreen = (GameScreen)instance;

            switch (screenName)
            {
                case "itemMenuScreen":
                    activeScreen = itemMenuScreen;
                    break;
                case "InGameMainMenuScreen":
                    activeScreen = ingameMenuScreen;
                    break;
                case "actionScreen":
                    activeScreen = actionScreen;
                    break;
                case "helpScreen":
                    activeScreen = helpScreen;
                    break;
                case "createPCScreen":
                    activeScreen = createPCScreen;
                    break;
                case "startScreen":
                    activeScreen = startScreen;
                    break;
                case "loadingScreen":
                    activeScreen = loadingScreen;
                    break;
            }

            activeScreen.Show();
        }

        public void messageScreen(bool activated, Texture2D spriteNPC, string script)
        {
            if (activated)
            {
                MessagePopupScreen.Active = true;
                MessagePopupScreen.Show();
                MessagePopupScreen.LoadAssets(spriteNPC, script);
            }
            else
            {
                MessagePopupScreen.Active = false;
                MessagePopupScreen.Hide();
            }
        }

        private bool CheckKey(Keys theKey)
        {
            return oldState.IsKeyDown(theKey) && newState.IsKeyUp(theKey);
        }
    }
}
