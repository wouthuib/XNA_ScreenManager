using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA_ScreenManager.PlayerClasses;
using XNA_ScreenManager.MapClasses;
using Microsoft.Xna.Framework;
using XNA_ScreenManager.ItemClasses;
using MapleLibrary;

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

        public void sendPlayerData(string action = "", PlayerInfo player = null)
        {
            playerData p = new playerData();

            switch(action)
            {
                case "create":
                    p.Name = player.Name;
                    p.skincol = (string)player.skin_color.ToString();
                    p.facespr = (string)player.faceset_sprite;
                    p.hairspr = (string)player.hair_sprite;
                    p.hailcol = (string)player.hair_color.ToString();
                    p.armor = (string)player.equipment.item_list.Find(x => x.Slot == ItemSlot.Bodygear).itemName.ToString();
                    //p.headgear = (string)player.equipment.item_list.Find(x => x.Slot == ItemSlot.Headgear).itemName.ToString();
                    p.weapon = (string)player.equipment.item_list.Find(x => x.Slot == ItemSlot.Weapon).itemName.ToString();
                    break;
                default:
                    p.Name = PlayerClasses.PlayerStore.Instance.activePlayer.Name;
                    p.Action = (string)action.ToString();
                    p.PositionX = (float)GameWorld.GetInstance.playerSprite.PositionX;
                    p.PositionY = (float)GameWorld.GetInstance.playerSprite.PositionY;
                    p.spritename = (string)GameWorld.GetInstance.playerSprite.spritename;
                    p.spritestate = (string)GameWorld.GetInstance.playerSprite.State.ToString();
                    p.direction = (string)GameWorld.GetInstance.playerSprite.Direction.ToString();
                    p.prevspriteframe = (int)GameWorld.GetInstance.playerSprite.prevspriteframe;
                    p.maxspriteframe = (int)GameWorld.GetInstance.playerSprite.maxspriteframe;
                    p.attackSprite = (string)GameWorld.GetInstance.playerSprite.attackSprite;
                    p.spriteEffect = (string)GameWorld.GetInstance.playerSprite.spriteEffect.ToString();
                    p.mapName = (string)GameWorld.GetInstance.map.Properties.Values[1].ToString();
                    p.skincol = (string)getPlayer().skin_color.ToString();
                    p.facespr = (string)getPlayer().faceset_sprite;
                    p.hairspr = (string)getPlayer().hair_sprite;
                    p.hailcol = (string)getPlayer().hair_color.ToString();
                    p.armor = (string)getEquipment(ItemSlot.Bodygear);
                    p.headgear = (string)getEquipment(ItemSlot.Headgear);
                    p.weapon = (string)getEquipment(ItemSlot.Weapon);
                    break;
             }

            tcpclient.SendData(p);
        }

        public void sendChatData(string newtext)
        {
            ChatData c = new ChatData()
            {
                Name = PlayerClasses.PlayerStore.Instance.activePlayer.Name,
                Text = newtext,
                PositionX = (int)GameWorld.GetInstance.playerSprite.PositionX,
                PositionY = (int)GameWorld.GetInstance.playerSprite.PositionY
            };

            tcpclient.SendData(c);
        }

        public void sendScreenData(string newscreen)
        {
            ScreenData c = new ScreenData()
            {
                MainScreenName = newscreen,
                MainScreenPhase = "",
                MainScreenMenu = "",
                SubScreenName = "",
                SubScreenPhase = "",
                SubScreenMenu = ""
            };

            tcpclient.SendData(c);
        }

        public void sendAccountData(string username, string password)
        {
            AccountData c = new AccountData()
            {
                Username = username,
                Password = password
            };

            tcpclient.SendData(c);
        }

        public void sendCastEffect(string path, int framecount, Vector2 offset, 
            Vector2 position, string playername, string instanceID)
        {
            EffectData e = new EffectData()
            {
                Path = path,
                FrameCount = framecount,
                OffsetX = (int)offset.X,
                OffsetY = (int)offset.Y,
                PositionX = (int)position.X,
                PositionY = (int)position.Y,
                PlayerLockName = playername,
                InstanceLockName = instanceID
            };

            tcpclient.SendData(e);
        }

        public void sendDmgArea(string owner, Vector2 position, Vector2 area, string permanent,
            int mobcount, float timer, int dmgpercent)
        {
            DmgAreaData d = new DmgAreaData()
            {
                Owner = (string)owner,
                PositionX = (int)position.X,
                PositionY = (int)position.Y,
                AreaWidth = (int)area.X,
                AreaHeigth = (int)area.Y,
                Permanent = (string)permanent,
                MobHitCount = (int)mobcount,
                Timer = (float)timer,
                DmgPercent = (int)dmgpercent
            };

            tcpclient.SendData(d);
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
