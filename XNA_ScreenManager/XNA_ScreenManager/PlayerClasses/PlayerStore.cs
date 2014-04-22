using System;
using System.Collections.Generic;
using System.Text;

namespace XNA_ScreenManager.PlayerClasses
{
    public class PlayerStore
    {
        public PlayerInfo[] playerlist;
        int activeSlot = 0, count = 0;

        private static PlayerStore instance;
        private PlayerStore()
        {
            playerlist = new PlayerInfo[6];
        }

        public static PlayerStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PlayerStore();
                }
                return instance;
            }
        }

        public int ActiveSlot 
        {
            get { return activeSlot; }
            set { activeSlot = value; }
        }

        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        public void addPlayer(PlayerInfo player = null)
        {
            if (player == null)
                playerlist[count] = new PlayerInfo();
            else
                playerlist[count] = player;

            this.count++;
            this.activeSlot = count -1;
        }

        public void removePlayer(string name, int slot = -1)
        {
            if (slot >= 0)
            {
                playerlist[slot] = null;
                this.count--;
            }

            for(int i = 0; i < playerlist.Length; i++)
            {
                if (playerlist[i].Name == name)
                {
                    playerlist[i] = null;
                    this.count--;
                }
            }

            if (activeSlot > count)
                activeSlot = count - 1;
        }

        public PlayerInfo activePlayer
        {
            get { return playerlist[activeSlot]; }
        }

        public PlayerInfo getPlayer(string name = null, int slot = -1)
        {
            if (slot >= 0)
                return playerlist[slot];

            for (int i = 0; i < playerlist.Length; i++)
            {
                if (playerlist[i].Name == name)
                    return playerlist[i];
            }

            return null;
        }
    }
}
