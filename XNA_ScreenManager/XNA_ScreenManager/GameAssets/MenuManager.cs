using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XNA_ScreenManager.GameAssets.InGame;

namespace XNA_ScreenManager.GameAssets
{
    class MenuManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region properties

        private List<Menu> listmenu = new List<Menu>();

        public bool itemdragged = false; // when an item is dragged, freeze menu's
        public bool menudragged = false; // when an item is dragged, freeze menu's

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

                if(active)
                {
                    foreach (Menu menu in listmenu)
                        menu.Show();
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

            base.Update(gameTime);
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

        #endregion
    }
}
