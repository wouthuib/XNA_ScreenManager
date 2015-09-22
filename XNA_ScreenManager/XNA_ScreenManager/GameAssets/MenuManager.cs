using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XNA_ScreenManager.GameAssets.InGame;
using Microsoft.Xna.Framework.Input;
using XNA_ScreenManager.Networking;

namespace XNA_ScreenManager.GameAssets
{
    class MenuManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region properties

        private List<Menu> listmenu = new List<Menu>();

        public bool itemdragged = false; // when an item is dragged, freeze menu's
        public bool menudragged = false; // when an item is dragged, freeze menu's

        // Keyboard- and Mousestate
        protected KeyboardState keyboardStateCurrent, keyboardStatePrevious;

        bool active;

        public List<Menu> Components
        {
            get { return listmenu; }
        }

        #endregion

        #region constructor

        private static MenuManager instance;
        private static System.Object _mutex = new System.Object();

        private MenuManager(Game game)
            : base(game)
        {
            Components.Add(new EquipMenu(game, 0.01f));
            Components.Add(new ItemMenu(game, 0.02f));
            Components.Add(new ShopMenu(game, 0.03f));
        }

        public static MenuManager CreateInstance(Game game)
        {
            lock (_mutex) // now I can claim some form of thread safety...
            {
                if (instance == null)
                {
                    instance = new MenuManager(game);
                }
            }
            return instance;
        }

        public static MenuManager Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new Exception("The GameWorld is called, but has not yet been created!!");
                }
                return instance;
            }
        }
        
        // When the game begins
        public void StartManager()
        {
            listmenu.Find(x => x is ItemMenu).Hide();
            listmenu.Find(x => x is EquipMenu).Hide();
        }

        public void SetViewport(Vector2 viewport)
        {
            foreach (Menu menu in listmenu)
            {
                menu.Viewport = viewport;
            }
        }

        public bool Active
        {
            get { return active; }
            set
            {
                active = value;

                if (active)
                {
                    // disabled to avoid opening all menu's when NPC chat is finished

                    // foreach (Menu menu in listmenu)
                    //    menu.Show();
                }
                else
                {
                    foreach (Menu menu in listmenu)
                        menu.Hide();
                }
            }
        }

        #endregion

        #region update

        public override void Update(GameTime gameTime)
        {
            // Sort all menus latest in locktime first
            listmenu.Sort(delegate(Menu a, Menu b)
            {
                int diff = b.LockTime.CompareTo(a.LockTime);
                return diff;
            });

            foreach (GameComponent child in listmenu)
            {
                if (child.Enabled)
                {
                    child.Update(gameTime);
                }
            }

            Readkeys(); // short keys to open and close menu's

            base.Update(gameTime);
        }

        private void Readkeys()
        {
            keyboardStateCurrent = Keyboard.GetState();

            if (keyboardStateCurrent.IsKeyDown(Keys.LeftControl))
            {
                if (CheckKey(Keys.I))
                {
                    Menu menu = Components.Find(x => x is ItemMenu) as ItemMenu;
                    menu.Trigger();
                }
                else if (CheckKey(Keys.E))
                {
                    Menu menu = Components.Find(x => x is EquipMenu) as EquipMenu;
                    menu.Trigger();
                }
                else if (CheckKey(Keys.S))
                {
                    Menu menu = Components.Find(x => x is ShopMenu) as ShopMenu;
                    NetworkGameData.Instance.sendNPCData(0, "OpenShop", 1000000); // <= ID will be trigger from NPC
                }
            }

            keyboardStatePrevious = keyboardStateCurrent;
        }

        #endregion

        #region draw

        public override void Draw(GameTime gameTime)
        {
            // Sort all menus latest in locktime last (last is on top of rest)
            listmenu.Sort((menu1,menu2) => menu1.LockTime.CompareTo(menu2.LockTime));

            foreach (GameComponent child in listmenu)
            {
                if ((child is DrawableGameComponent) &&
                ((DrawableGameComponent)child).Visible)
                {
                    ((DrawableGameComponent)child).Draw(gameTime);
                }
            }
            base.Draw(gameTime);
        }

        #endregion

        #region functions

        public bool active_menu(Menu menu)
        {
            if (listmenu.FindAll(x => x.DragLock == true && x != menu).Count > 0)
                return true;

            return false;
        }

        public bool CheckKey(Microsoft.Xna.Framework.Input.Keys theKey)
        {
            KeyboardState keyboardStateCurrent = Keyboard.GetState();
            return keyboardStatePrevious.IsKeyDown(theKey) && keyboardStateCurrent.IsKeyUp(theKey);
        }

        #endregion
    }
}
