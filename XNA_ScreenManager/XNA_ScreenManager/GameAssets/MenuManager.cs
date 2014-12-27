using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNA_ScreenManager.GameAssets.InGame;

namespace XNA_ScreenManager.GameAssets
{
    class MenuManager
    {
        #region properties
        public Game game;
        public List<Menu> listmenu = new List<Menu>();
        bool active;
        #endregion

        #region constructor
        private static MenuManager instance;

        private MenuManager()
        {
        }

        public static MenuManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new MenuManager();

                return instance;
            }
        }
        
        // When the game begins
        public void StartManager()
        {
            listmenu.Find(x => x is ItemMenu).Hide();
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
    }
}
