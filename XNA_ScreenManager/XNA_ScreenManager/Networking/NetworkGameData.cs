using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA_ScreenManager.PlayerClasses;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.Networking.ServerClasses;
using Microsoft.Xna.Framework;

namespace XNA_ScreenManager.Networking
{
    public class NetworkGameData
    {
        TCPClient tcpclient;

        private static NetworkGameData instance;
        private NetworkGameData()
        {
            this.tcpclient = new TCPClient();
        }

        public static NetworkGameData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NetworkGameData();
                }
                return instance;
            }
        }

        public void sendPlayerData()
        {
            playerData p = new playerData()
            {
                Name = PlayerClasses.PlayerStore.Instance.activePlayer.Name,
                PositionX = (int)GameWorld.GetInstance.playerSprite.PositionX,
                PositionY = (int)GameWorld.GetInstance.playerSprite.PositionY,
                spritename = (string)GameWorld.GetInstance.playerSprite.spritename,
                spritestate = (string)GameWorld.GetInstance.playerSprite.State.ToString(),
                prevspriteframe = (int)GameWorld.GetInstance.playerSprite.prevspriteframe,
                maxspriteframe = (int)GameWorld.GetInstance.playerSprite.maxspriteframe,
                attackSprite = (string)GameWorld.GetInstance.playerSprite.attackSprite,
                spriteEffect = (string)GameWorld.GetInstance.playerSprite.spriteEffect.ToString(),
                mapName = (string)GameWorld.GetInstance.map.Properties.Values[1].ToString(),
                skincol = (string)PlayerClasses.PlayerStore.Instance.activePlayer.skin_color.ToString(),
                facespr = (string)PlayerClasses.PlayerStore.Instance.activePlayer.faceset_sprite,
                hairspr = (string)PlayerClasses.PlayerStore.Instance.activePlayer.hair_sprite,
                hailcol = (string)PlayerClasses.PlayerStore.Instance.activePlayer.hair_color.ToString()
            };

            tcpclient.SendData(p);
        }
    }
}
