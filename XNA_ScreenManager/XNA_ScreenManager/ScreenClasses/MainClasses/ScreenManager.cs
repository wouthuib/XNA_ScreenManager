﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.ScreenClasses.InGame;
using XNA_ScreenManager.ScreenClasses.Menus;
using XNA_ScreenManager.PlayerClasses;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.SkillClasses;

namespace XNA_ScreenManager.ScreenClasses
{
    public class ScreenManager
    {
        public Game game;
        public StartScreen startScreen;
        public HelpScreen helpScreen;
        public ActionScreen actionScreen;
        public CharacterCreationScreen createCharScreen;
        public CharacterSelectionScreen selectCharScreen;
        public InGameMainMenuScreen ingameMenuScreen;
        public ItemMenuScreen itemMenuScreen;
        public EquipmentMenuScreen equipmentMenuScreen;
        public SkillScreen skillScreen;
        public StatusScreen statusScreen;
        public ShopMenuScreen shopMenuScreen;
        public MessagePopup MessagePopupScreen;
        public LoadingScreen loadingScreen;

        public GameScreen activeScreen;
        KeyboardState newState, oldState;

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
            ingameMenuScreen.Hide();
            itemMenuScreen.Hide();
            equipmentMenuScreen.Hide();
            skillScreen.Hide();
            statusScreen.Hide();
            shopMenuScreen.Hide();
            MessagePopupScreen.Hide();
            loadingScreen.Hide();
            createCharScreen.Hide();
            selectCharScreen.Hide();

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
            else if (activeScreen == skillScreen)
            {
                HandleskillMenuScreen();
            }
            else if (activeScreen == statusScreen)
            {
                HandlestatusMenuScreen();
            }
            else if (activeScreen == shopMenuScreen)
            {
                HandleshopMenuScreen();
            }
            else if (activeScreen == loadingScreen)
            {
                //HandleloadingScreen();
            }
            else if (activeScreen == createCharScreen)
            {
                HandleCreateCharScreen();
            }
            else if (activeScreen == selectCharScreen)
            {
                HandleSelectCharScreen();
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
                        activeScreen = selectCharScreen;
                        activeScreen.Show();
                        break;
                    case 1:
                        ingameMenuScreen.SelectedIndex = 0;
                        PlayerStore.Instance.loadPlayerStore("savegame.bin");
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
                        activeScreen = skillScreen;
                        activeScreen.Show();
                        break;
                    case 2:
                        activeScreen.Hide();
                        activeScreen = equipmentMenuScreen;
                        activeScreen.Show();
                        break;
                    case 3:
                        activeScreen.Hide();
                        activeScreen = statusScreen;
                        activeScreen.Show();
                        break;
                    case 4:
                        activeScreen.Hide();
                        activeScreen = startScreen;
                        ingameMenuScreen.SelectedIndex = 0;
                        PlayerStore.Instance.savePlayerStore("savegame.bin");
                        activeScreen.Show();
                        break;
                    case 5:
                        activeScreen.Hide();
                        activeScreen = startScreen;
                        ingameMenuScreen.SelectedIndex = 0;
                        PlayerStore.Instance.loadPlayerStore("savegame.bin");
                        activeScreen.Show();
                        break;
                    case 6:
                        activeScreen.Hide();
                        activeScreen = startScreen;
                        ingameMenuScreen.SelectedIndex = 0;
                        activeScreen.Show();
                        break;
                }
            }
            else if (CheckKey(Keys.Back) || CheckKey(Keys.Escape))
            {
                activeScreen.Hide();
                activeScreen = actionScreen;
                ingameMenuScreen.SelectedIndex = 0;
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

        private void HandleskillMenuScreen()
        {
            if (skillScreen.menuOptionsActive)
            {
                if (CheckKey(Keys.Back) || CheckKey(Keys.Escape))
                {
                    QuitskillMenuScreen();
                }
                else if (CheckKey(Keys.Enter) || CheckKey(Keys.Space))
                {
                    switch (skillScreen.SelectedOption)
                    {
                        case 0:
                            skillScreen.menuOptionsActive = false;
                            skillScreen.skillOptionsActive = false;
                            skillScreen.SelectActive = true;
                            break;
                        case 1:
                            skillScreen.menuOptionsActive = false;
                            skillScreen.skillOptionsActive = false;
                            skillScreen.SelectActive = true;
                            break;
                        case 2:
                            QuitskillMenuScreen();
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (skillScreen.SelectActive)
            {
                if (CheckKey(Keys.Enter))
                {
                    if (SkillStore.Instance.getSkill(skillScreen.record[skillScreen.selectedColumn, skillScreen.selectedRow]) != null)
                    {
                        skillScreen.skillOptionsActive = true;
                        skillScreen.SelectActive = false;
                        skillScreen.options.Style = OrderStyle.Central;
                        skillScreen.options.SetMenuItems(
                            new string[]{ SkillStore.Instance.getSkill(skillScreen.record[skillScreen.selectedColumn, skillScreen.selectedRow]).Name + " - Choose an Option.", "",
                            "Add QuickSlot", "Level Up", "More Details", "Cancel"});
                        skillScreen.options.StartIndex = 2;
                        skillScreen.options.SelectedIndex = 2;
                    }
                }
                else if (CheckKey(Keys.Back) || CheckKey(Keys.Escape))
                {
                    skillScreen.SelectActive = false;
                    skillScreen.menuOptionsActive = true;
                }
            }
            else if (skillScreen.skillOptionsActive)
            {
                // Check item options
                if (CheckKey(Keys.Enter))
                {
                    switch (skillScreen.options.SelectedIndex)
                    {
                        case 0:
                        case 1:
                            break;
                        case 2:
                            skillScreen.skillOptionsActive = false;
                            skillScreen.QuickSlotActive = true;
                            skillScreen.options.Style = OrderStyle.Central;
                            skillScreen.options.SetMenuItems(
                                new string[]{ "Please choose a slot by pressing F1 to F12", "", "Cancel"});
                            skillScreen.options.StartIndex = 2;
                            skillScreen.options.SelectedIndex = 2;
                            break;
                        case 3:
                            // do something
                            break;
                        case 4:
                            // do something
                            break;
                        case 5:
                            skillScreen.skillOptionsActive = false;
                            skillScreen.SelectActive = true;
                            break;
                    }
                }
                else if (CheckKey(Keys.Back) || CheckKey(Keys.Escape))
                {
                    skillScreen.skillOptionsActive = false;
                    skillScreen.SelectActive = true;
                }
            }
            else if (skillScreen.QuickSlotActive)
            {
                // Check item options
                if (CheckKey(Keys.Enter) || CheckKey(Keys.Back) || CheckKey(Keys.Escape))
                {
                    skillScreen.QuickSlotActive = false;
                    skillScreen.SelectActive = true;
                }
            }
        }

        private void QuitskillMenuScreen()
        {
            skillScreen.menuOptionsActive = true;
            skillScreen.SelectActive = false;
            skillScreen.skillOptionsActive = false;
            skillScreen.QuickSlotActive = false;
            skillScreen.SelectedOption = 0;
            activeScreen.Hide();
            activeScreen = ingameMenuScreen;
            activeScreen.Show();
        }

        private void HandlestatusMenuScreen()
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
            // do nothing all managed in screen
        }

        private void HandleCreateCharScreen()
        {

            if (CheckKey(Keys.Escape))
            {
                if (createCharScreen.phase == Phase.Name)
                {
                    activeScreen.Hide();
                    activeScreen = selectCharScreen;
                    activeScreen.Show();
                }
                else
                    createCharScreen.phase = Phase.Name;
            }

            if (CheckKey(Keys.Enter))
            {
                if (createCharScreen.phase == Phase.Properties)
                {
                    createCharScreen.phase = Phase.Name;
                    PlayerStore.Instance.addPlayer(createCharScreen.newPlayer);
                    activeScreen.Hide();
                    activeScreen = selectCharScreen;
                    activeScreen.Show();
                }
                else
                {
                    // check if name already exists
                    bool namematch = false;

                    for(int i = 0; i < PlayerStore.Instance.Count; i++)
                        if (createCharScreen.keyboardiput.Result == PlayerStore.Instance.playerlist[i].Name)
                            namematch = true;

                    if (!namematch) // no matches found
                    {
                        createCharScreen.phase = Phase.Properties;
                        createCharScreen.newPlayer.Name = createCharScreen.keyboardiput.Result;
                    }
                }
            }
        }

        private void HandleSelectCharScreen()
        {
            if (CheckKey(Keys.Back) || CheckKey(Keys.Escape))
            {
                if (selectCharScreen.menu.EndIndex == 3)
                {
                    activeScreen.Hide();
                    activeScreen = startScreen;
                    activeScreen.Show();
                }
                else
                {
                    selectCharScreen.menu.SelectedIndex = 0;
                    selectCharScreen.menu.StartIndex = 0;
                    selectCharScreen.menu.EndIndex = 3;
                }
            }
            else if (CheckKey(Keys.Space) || CheckKey(Keys.Enter))
            {
                switch (selectCharScreen.SelectedIndex)
                {
                    case 0:
                        if (PlayerStore.Instance.Count > 0)
                        {
                            selectCharScreen.menu.StartIndex = 3;
                            selectCharScreen.menu.EndIndex = 5;
                        }
                        break;
                    case 1:
                        if (createCharScreen.initize())
                        {
                            selectCharScreen.menu.SelectedIndex = 0;
                            selectCharScreen.menu.StartIndex = 0;
                            selectCharScreen.menu.EndIndex = 3;
                            activeScreen.Hide();
                            activeScreen = createCharScreen;
                            activeScreen.Show();
                        }
                        break;
                    case 2:
                    case 3:
                        selectCharScreen.menu.SelectedIndex = 0;
                        selectCharScreen.menu.StartIndex = 0;
                        selectCharScreen.menu.EndIndex = 3;
                        GameWorld.GetInstance.ChangeJobClass(PlayerStore.Instance.activePlayer);
                        activeScreen.Hide();
                        activeScreen = actionScreen;
                        activeScreen.Show();
                        break;
                    case 4:
                        selectCharScreen.menu.SelectedIndex = 0;
                        selectCharScreen.menu.StartIndex = 0;
                        selectCharScreen.menu.EndIndex = 3;
                        activeScreen.Hide();
                        activeScreen = startScreen;
                        activeScreen.Show();
                        break;
                }
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
                case "startScreen":
                    activeScreen = startScreen;
                    break;
                case "loadingScreen":
                    activeScreen = loadingScreen;
                    break;
                case "shopMenuScreen":
                    activeScreen = shopMenuScreen;
                    break;
            }

            activeScreen.Show();
        }

        public void messageScreen(bool activated, Rectangle getpos, string name, string script)
        {
            if (activated)
            {
                MessagePopupScreen.Active = true;
                MessagePopupScreen.Show();
                MessagePopupScreen.LoadAssets(getpos, name, script);
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
