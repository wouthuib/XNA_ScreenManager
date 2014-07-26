using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA_ScreenManager.Networking.ServerClasses;

namespace XNA_ScreenManager.PlayerClasses
{
    class NetworkPlayerStore
    {
        private static NetworkPlayerStore instance;
        private int maxplayers = 10, playercounter;
        public playerData[] playerlist;
        public NetworkPlayerSprite[] playersprites;

        private NetworkPlayerStore()
        {
            playerlist = new playerData[maxplayers];
            playersprites = new NetworkPlayerSprite[maxplayers];
        }

        public static NetworkPlayerStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NetworkPlayerStore();
                }
                return instance;
            }
        }

        public void addPlayer(playerData player)
        {
            if (playercounter < maxplayers)
            {
                playerlist[playercounter] = player;

                playersprites[playercounter] = 
                    new NetworkPlayerSprite(
                        player.Name, 
                        player.PositionX, 
                        player.PositionY, 
                        player.spritename,
                        player.spritestate, 
                        player.prevspriteframe, 
                        player.maxspriteframe, 
                        player.attackSprite, 
                        player.spriteEffect, 
                        player.mapName,
                        player.skincol,
                        player.facespr,
                        player.hairspr,
                        player.hailcol,
                        player.armor,
                        player.headgear,
                        player.weapon);

                playercounter++;
            }
        }
    }
}
