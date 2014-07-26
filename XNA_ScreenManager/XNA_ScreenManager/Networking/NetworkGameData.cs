using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA_ScreenManager.PlayerClasses;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.Networking.ServerClasses;
using Microsoft.Xna.Framework;
using XNA_ScreenManager.ItemClasses;

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
                skincol = (string)getPlayer().skin_color.ToString(),
                facespr = (string)getPlayer().faceset_sprite,
                hairspr = (string)getPlayer().hair_sprite,
                hailcol = (string)getPlayer().hair_color.ToString(),
                armor = (string)getEquipment(ItemSlot.Bodygear),
                headgear = (string)getEquipment(ItemSlot.Headgear),
                weapon = (string)getEquipment(ItemSlot.Weapon)
            };

            tcpclient.SendData(p);
        }

        private PlayerInfo getPlayer()
        {
            return PlayerClasses.PlayerStore.Instance.activePlayer;
        }

        private string getEquipment(ItemSlot slot)
        {
            if (getPlayer().equipment.item_list.FindAll(x => x.Slot == slot).Count > 0)
                return (string)getPlayer().equipment.item_list.Find(x => x.Slot == slot).itemName.ToString();
            else
                return null;
        }
    }
}
