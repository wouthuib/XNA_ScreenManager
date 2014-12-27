using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.GameWorldClasses.Effects;
using XNA_ScreenManager.ScreenClasses.MainClasses;
using XNA_ScreenManager.MonsterClasses;
using XNA_ScreenManager.CharacterClasses;
using XNA_ScreenManager.ItemClasses;
using XNA_ScreenManager.PlayerClasses;
using XNA_ScreenManager.ScreenClasses;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MapleLibrary;
using System.Net;
using System.Collections.Generic;

namespace XNA_ScreenManager.Networking
{
    /// <summary>
    /// Client socket Example, The listener process
    /// </summary>
    /// <param name="portNr">http://msdn.microsoft.com/en-us/library/bew39x2a(v=vs.110).aspx</param>
    public class TCPClient
    {
        #region properties
        Socket sender;

        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        //Byte array that is populated when a user receives data
        private byte[] readBuffer = new byte[StateObject.BufferSize];
        private List<byte[]> objectList = new List<byte[]>();
        private bool Sending = false;

        public static TCPClient instance;
        private object lockstream = new Object();
        public Thread sendloop;

        public bool Connected = false;
        public bool encryption = false;
        #endregion

        public TCPClient()
        {
            TCPClient.instance = this;
            StartInstance();
        }

        public void Disconnect()
        {
            sender.Close();
            Connected = false;
        }
        private void StartInstance()
        {
            try
            {
                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.
                IPAddress ipAddress = IPAddress.Parse(ServerProperties.xmlgetvalue("address").ToString());
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Convert.ToInt32(ServerProperties.xmlgetvalue("port")));

                // Create a TCP/IP  socket.
                sender = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                sender.BeginConnect(remoteEP,
                        new AsyncCallback(ConnectCallback), sender);
                connectDone.WaitOne();

                // setup client listener
                StartListening();

                // start send loop
                sendloop = new Thread(new ThreadStart(this.SendingLoop));
                sendloop.Start();

                Connected = true;
            }
            catch
            {
                ScreenManager.Instance.activeScreen.topmessage.Display("Cannot connect with server.", Color.PaleVioletRed, 5.0f);
                ScreenManager.Instance.actionScreen.hud.chatbarInput.updateTextlog("[System]", "Cannot connect with server.");
                Connected = false;
            }
        }
        public void SendData(Object obj)
        {
            if (Connected)
            {
                lock (lockstream)
                {
                    objectList.Add(SerializeToStream(obj).ToArray());
                }
            }
            else
                ScreenManager.Instance.activeScreen.topmessage.Display("Cannot connect with server.", Color.PaleVioletRed, 5.0f);
        }

        public void SendingLoop()
        {
            while (true)
            {
                if (!Sending)
                {
                    if(objectList.Count > 0)
                    {
                        lock(lockstream)
                        {
                            Sending = true;
                            byte[] bytestream = new byte[objectList[0].Length];
                            bytestream = objectList[0];         //pick oldest from list
                            objectList.Remove(bytestream);      //remove from list
                            Send(sender, bytestream);
                        }
                    }
                }            
                Thread.Sleep(10);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void StartListening()
        {
            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = sender;

            // Begin receiving the data from the remote device.
            sender.BeginReceive(readBuffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                //An error happened that created bad data
                if (bytesRead == 0)
                {
                    receiveDone.Set();
                    Console.WriteLine("Server suddenly disconnected!");
                    return;
                }

                //Create the byte array with the number of bytes read
                byte[] data = new byte[bytesRead];

                //Populate the array
                for (int i = 0; i < bytesRead; i++)
                    data[i] = readBuffer[i];

                // start new listener
                StartListening();

                // read incoming data
                ReadDataStream(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void Send(Socket client, byte[] byteData)
        {
            try
            {
                // Begin sending the data to the remote device.
                client.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), client);
                sendDone.WaitOne();
            }
            catch (SocketException ee)
            {
                SocketErrorHandler(ee.ErrorCode);
            }
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();

                //Sending Done
                Sending = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void ReadUserData(byte[] byteArray)
        {
            try
            {
                //message has successfully been received
                MemoryStream ms = new MemoryStream(byteArray);

                object obj = DeserializeFromStream(ms);

                if (obj is playerData)
                {
                    playerData player = (playerData)obj;

                    Console.Write("Server Message: \n");
                    Console.Write(player.Name + "'s Position X:" + player.PositionX.ToString() + "\t");
                    Console.Write("Position Y:" + player.PositionY.ToString() + "\n");
                    Console.Write("----------------\n");
                }
            }
            catch
            {
                Console.Write("Incoming bytecode conversion error \n");
            }
        }
        private void SocketErrorHandler(int errorcode)
        {
            switch (errorcode)
            {
                case 10054:     // connection was forecely closed
                case 10060:     // connection timeout
                default:        // other socket exceptions
                    Disconnect();
                    break;
            }
        }
        
        // Wouter's methods

        //reading network data
        private void ReadDataStream(byte[] byteArray)
        {
            try
            {
                MemoryStream ms = new MemoryStream(byteArray);
                object obj = DeserializeFromStream(ms);

                if (obj is playerData)
                {
                    if (ScreenManager.Instance.activeScreen == ScreenManager.Instance.actionScreen)
                        incomingPlayerData(obj as playerData);
                    else if (ScreenManager.Instance.activeScreen == ScreenManager.Instance.selectCharScreen)
                        incomingCharSelect(obj as playerData);
                }
                else if (obj is ChatData)
                    incomingChatData(obj as ChatData);
                else if (obj is MonsterData)
                    incomingMonsterData(obj as MonsterData);
                else if (obj is AccountData)
                    incomingAccountData(obj as AccountData);
                else if (obj is EffectData)
                    incomingEffectData(obj as EffectData);
                else if (obj is ItemData)
                    incomingItemData(obj as ItemData);
                else if (obj is HudData)
                    incomingHudData(obj as HudData);
            }
            catch (Exception ee)
            {
                string error = ee.ToString();
            }
        }

        // reading object data
        private void incomingAccountData(AccountData account)
        {
            if (Convert.ToBoolean(account.Connected))
            {
                if (ScreenManager.Instance.activeScreen == ScreenManager.Instance.loginScreen)
                {
                    ScreenManager.Instance.setScreen("selectCharScreen"); // change screen
                    NetworkGameData.Instance.sendScreenData("charselect"); // inform server
                }
            }
            else
            {
                ScreenManager.Instance.activeScreen.topmessage.Display("Wrong Username and Password", Color.White, 5f);
            }
        }
        private void incomingPlayerData(playerData player)
        {
            if (player.Action == "Remove")
                GameWorld.GetInstance.listEntity.Find(p => p.EntityName == player.Name).KeepAliveTime = 0; // removed in next update
            else if (player.Action == "Online")
            { }
            else if (player.Action == "Sprite_Update" && player.Name == PlayerStore.Instance.activePlayer.Name)
            {
                PlayerSprite sprite = GameWorld.GetInstance.playerSprite;

                if (sprite.State.ToString() != player.spritestate ||
                    sprite.State == EntityState.Ladder ||
                    sprite.State == EntityState.Rope)
                {
                    sprite.State = (EntityState)Enum.Parse(typeof(EntityState), player.spritestate);
                    sprite.Position = new Vector2(player.PositionX, player.PositionY);
                    //sprite.PLAYER_SPEED = 190;
                }
                else if (Math.Abs(sprite.Position.X - player.PositionX) >= 1 &&
                         Math.Abs(sprite.Position.X - player.PositionX) <= 80) // avoid lag
                {
                    if (sprite.spriteEffect == SpriteEffects.None)
                        sprite.PLAYER_SPEED += (int)(sprite.Position.X - player.PositionX) * 2;
                    else
                        sprite.PLAYER_SPEED -= (int)(sprite.Position.X - player.PositionX) * 2;

                    sprite.PLAYER_SPEED = Clamp(sprite.PLAYER_SPEED, 165, 265);
                }
                else if (Math.Abs(sprite.Position.X - player.PositionX) > 80)
                    sprite.Position = new Vector2(player.PositionX, player.PositionY);
            }
            else if (player.Name != PlayerStore.Instance.activePlayer.Name) // Networkplayer
            {
                if (GameWorld.GetInstance.listEntity.FindAll(x => x.EntityName == player.Name).Count > 0)
                {
                    NetworkPlayerSprite sprite = (NetworkPlayerSprite)GameWorld.GetInstance.listEntity.Find(x => x.EntityName == player.Name);
                    sprite.MapName = player.mapName;

                    if (sprite.State.ToString() != player.spritestate ||
                        sprite.spriteEffect.ToString() != player.spriteEffect ||
                        ((sprite.State == EntityState.Ladder || sprite.State == EntityState.Rope) && 
                        sprite.Direction != NetworkPlayerSprite.getVector(player.direction)))
                    {

                        sprite.previousPosition = sprite.Position;   // save previous postion
                        sprite.PreviousState = sprite.State;         // save previous state before
                        sprite.previousDirection = sprite.Direction; // save previous direction

                        sprite.State = (EntityState)Enum.Parse(typeof(EntityState), player.spritestate);
                        sprite.Position = new Vector2(player.PositionX, player.PositionY);
                        sprite.spriteEffect = (SpriteEffects)Enum.Parse(typeof(SpriteEffects), player.spriteEffect);
                        sprite.Direction = NetworkPlayerSprite.getVector(player.direction);
                        //sprite.PLAYER_SPEED = 190;
                    }
                    else if (Math.Abs(sprite.Position.X - player.PositionX) >= 1 &&
                            Math.Abs(sprite.Position.X - player.PositionX) <= 80) // avoid lag
                    {
                        if (sprite.spriteEffect == SpriteEffects.None)
                            sprite.PLAYER_SPEED += (int)(sprite.Position.X - player.PositionX) * 2;
                        else
                            sprite.PLAYER_SPEED -= (int)(sprite.Position.X - player.PositionX) * 2;

                        sprite.PLAYER_SPEED = Clamp(sprite.PLAYER_SPEED, 165, 265);
                    }
                    else if (Math.Abs(sprite.Position.X - player.PositionX) > 80)
                        sprite.Position = new Vector2(player.PositionX, player.PositionY);
                }
                else
                {
                    if (GameWorld.GetInstance.newEntity.FindAll(x => x.EntityName == player.Name).Count == 0)
                        GameWorld.GetInstance.newEntity.Add(
                            new NetworkPlayerSprite(
                                player.Name, player.IP,
                                player.PositionX, player.PositionY,
                                player.spritename, player.spritestate,
                                player.prevspriteframe, player.maxspriteframe,
                                player.attackSprite, player.spriteEffect,
                                player.mapName, player.skincol,
                                player.facespr, player.hairspr,
                                player.hailcol, player.armor,
                                player.headgear, player.weapon));
                }
            }
        }
        private void incomingChatData(ChatData chatdata)
        {
            GameWorld.GetInstance.newEffect.Add(new ChatBalloon(chatdata.Name, chatdata.Text, ResourceManager.GetInstance.Content.Load<SpriteFont>(@"font\Arial_12px")));
            ScreenManager.Instance.actionScreen.hud.chatbarInput.updateTextlog(chatdata.Name, chatdata.Text);
        }
        private void incomingMonsterData(MonsterData mobdata)
        {
            bool found = false;

            foreach(var entity in GameWorld.GetInstance.listEntity)
            {
                if (entity is NetworkMonsterSprite)
                {
                    NetworkMonsterSprite monster = (NetworkMonsterSprite)entity;

                    if (monster.InstanceID.ToString() == mobdata.InstanceID)
                    {
                        found = true; // update existing monster

                        if (mobdata.Action == "Sprite_Update")
                        {
                            if (Math.Abs(monster.Position.X - mobdata.PositionX) >= 2) // avoid lag
                            {
                                if (monster.spriteEffect == SpriteEffects.None)
                                    monster.WALK_SPEED += (int)(monster.Position.X - mobdata.PositionX) * 2;
                                else
                                    monster.WALK_SPEED -= (int)(monster.Position.X - mobdata.PositionX) * 2;

                                monster.WALK_SPEED = Clamp(monster.WALK_SPEED, 50, 170);
                            }
                            else
                                monster.WALK_SPEED = 97;
                        } 
                        if (mobdata.Action == "Agressive_Update")
                        {
                            monster.RUN_SPEED = 122;
                            monster.update_server(
                                new Vector2(mobdata.PositionX, mobdata.PositionY),
                                (EntityState)Enum.Parse(typeof(EntityState), mobdata.spritestate),
                                (SpriteEffects)Enum.Parse(typeof(SpriteEffects), mobdata.spriteEffect));

                        }
                        else if (mobdata.Action == "Died")
                        {
                            monster.KeepAliveTime = 0; // remove at next update round
                        }
                        else
                        {
                            monster.WALK_SPEED = 97;
                            monster.update_server(
                                new Vector2(mobdata.PositionX, mobdata.PositionY),
                                (EntityState)Enum.Parse(typeof(EntityState), mobdata.spritestate),
                                (SpriteEffects)Enum.Parse(typeof(SpriteEffects), mobdata.spriteEffect));
                        }

                    }
                }
            }

            if (!found) // add new monster
                if (GameWorld.GetInstance.newEntity.FindAll(x => x.InstanceID.ToString() == mobdata.InstanceID).Count == 0)
                    GameWorld.GetInstance.newEntity.Add(
                        new NetworkMonsterSprite(mobdata.MonsterID, mobdata.InstanceID, 
                            new Vector2(mobdata.PositionX, mobdata.PositionY), 
                            new Vector2(mobdata.BorderMin, mobdata.BorderMax)));
        }
        private void incomingCharSelect(playerData playerdata)
        {
            bool found = false;
            for(int i = 0; i < PlayerStore.Instance.playerlist.Length; i++)
            {
                if (PlayerStore.Instance.playerlist[i] != null)
                {
                    if (PlayerStore.Instance.playerlist[i].Name == playerdata.Name) // avoid duplicates
                    {
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                PlayerInfo player = new PlayerInfo();

                player.AccountID = playerdata.AccountID;
                player.CharacterID = playerdata.CharacterID;

                player.Name = playerdata.Name;
                player.skin_color = getColor(playerdata.skincol);
                player.faceset_sprite = playerdata.facespr;
                player.hair_sprite = playerdata.hairspr;
                player.hair_color = getColor(playerdata.hailcol);

                if (playerdata.armor != null)
                    if (ItemStore.Instance.item_list.FindAll(x => x.itemName == playerdata.armor).Count > 0)
                        player.equipment.addItem(ItemStore.Instance.item_list.Find(x => x.itemName == playerdata.armor));
                if (playerdata.weapon != null)
                    if (ItemStore.Instance.item_list.FindAll(x => x.itemName == playerdata.weapon).Count > 0)
                        player.equipment.addItem(ItemStore.Instance.item_list.Find(x => x.itemName == playerdata.weapon));
                if (playerdata.headgear != null)
                    if (ItemStore.Instance.item_list.FindAll(x => x.itemName == playerdata.headgear).Count > 0)
                        player.equipment.addItem(ItemStore.Instance.item_list.Find(x => x.itemName == playerdata.headgear));

                PlayerStore.Instance.addPlayer(player);
            }
        }
        private void incomingEffectData(EffectData effectdata)
        {
            if (effectdata.Name == "DamageBaloon")
            {
                GameWorld.GetInstance.newEffect.Add(new DamageBaloon(
                    ResourceManager.GetInstance.Content.Load<Texture2D>(@effectdata.Path),
                    new Vector2(effectdata.PositionX, effectdata.PositionY),
                        effectdata.Value_01));
            }
            else if (effectdata.Name == "AddArrow")
            {
                // temporary static sprite (can be changed later in time)
                Texture2D sprite = ResourceManager.GetInstance.Content.Load<Texture2D>(@"gfx\gameobjects\arrow");

                if (effectdata.Value_01 == 1)
                    GameWorld.GetInstance.newEffect.Add(new Arrow(
                        sprite, effectdata.InstanceID, new Vector2(effectdata.PositionX, effectdata.PositionY + sprite.Height * 0.6f),
                        800, new Vector2(1, 0), Vector2.Zero));
                else
                    GameWorld.GetInstance.newEffect.Add(new Arrow(
                        sprite, effectdata.InstanceID ,new Vector2(effectdata.PositionX, effectdata.PositionY + sprite.Height * 0.6f),
                        800, new Vector2(-1, 0), Vector2.Zero));

            }
            else if (effectdata.Name == "DeleteArrow")
            {
                if (GameWorld.GetInstance.listEffect.FindAll(x => x.instanceID == effectdata.InstanceID).Count > 0)
                {
                    Arrow arrow = GameWorld.GetInstance.listEffect.Find(x => x.instanceID == effectdata.InstanceID) as Arrow;
                    arrow.KeepAliveTimer = 0;
                }
            }
            else if (effectdata.Name == "AddItemSprite")
            {
                GameWorld.GetInstance.newEffect.Add(new ItemSprite(
                    new Vector2(effectdata.PositionX, effectdata.PositionY),
                        effectdata.Value_01, effectdata.InstanceID));
            }
            else if (effectdata.Name == "DeleteItemSprite")
            {
                if (GameWorld.GetInstance.listEffect.FindAll(x => x.instanceID == effectdata.InstanceID).Count > 0)
                {
                    ItemSprite item = GameWorld.GetInstance.listEffect.Find(x => x.instanceID == effectdata.InstanceID) as ItemSprite;
                    item.KeepAliveTimer = 0;
                }
            }
        }
        private void incomingItemData(ItemData itemdata)
        {
            switch (itemdata.action)
            {
                case "AddInventory":
                    if (ItemStore.Instance.item_list.FindAll(x => x.itemID == itemdata.ID).Count > 0)
                    {
                        Item item = ItemStore.Instance.item_list.Find(x => x.itemID == itemdata.ID);
                        PlayerStore.Instance.activePlayer.inventory.addItem(item);
                    }
                    break;
                case "DelInventory":
                    if (PlayerStore.Instance.activePlayer.inventory.item_list.FindAll(x => x.itemID == itemdata.ID).Count > 0)
                    {
                        PlayerStore.Instance.activePlayer.inventory.removeItem(itemdata.ID);
                    }
                    break;
                case "FinInventory":
                    ScreenManager.Instance.itemMenuScreen.ServerReqFinish();
                    break;
                case "ResInventory":
                    PlayerStore.Instance.activePlayer.inventory.item_list.Clear();
                    break;
                case "AddEquipment":
                    if (ItemStore.Instance.item_list.FindAll(x => x.itemID == itemdata.ID).Count > 0)
                    {
                        Item item = ItemStore.Instance.item_list.Find(x => x.itemID == itemdata.ID);
                        PlayerStore.Instance.activePlayer.equipment.addItem(item);
                    }
                    break;
                case "FinEquipment":
                    ScreenManager.Instance.itemMenuScreen.ServerReqFinish();
                    break;
                case "ResEquipment":
                    PlayerStore.Instance.activePlayer.equipment.item_list.Clear();
                    break;
                case "SwitchEquipment":
                    if (itemdata.player_name != PlayerStore.Instance.activePlayer.Name)
                    {
                        // Bind item
                        Item item = ItemStore.Instance.item_list.Find(x => x.itemID == itemdata.ID);

                        // find network player with matching name
                        NetworkPlayerSprite player = null;

                        if (GameWorld.GetInstance.listEntity.FindAll(x => x.EntityName == itemdata.player_name).Count > 0)
                            player = GameWorld.GetInstance.listEntity.Find(x => x.EntityName == itemdata.player_name) as NetworkPlayerSprite;

                        if (player != null)
                        {
                            switch (item.Slot)
                            {
                                case ItemSlot.Bodygear:
                                    player.armor_name = item.itemName;
                                    break;
                                case ItemSlot.Weapon:
                                    player.weapon_name = item.itemName;
                                    break;
                                case ItemSlot.Headgear:
                                    player.headgear_name = item.itemName;
                                    break;
                            }
                        }
                    }
                    break;
            }
        }
        private void incomingHudData(HudData huddata)
        {
            PlayerInfo player = null;
            player = Array.Find(PlayerStore.Instance.playerlist, x => x.Name == huddata.player_name);

            switch (huddata.action)
            {
                case "EXP":
                    player.Exp += huddata.value;
                    break;
                case "HP":
                    player.HP += huddata.value;
                    break;
                default:
                    break;
            }

        }

        // conversions
        private Color getColor(string colorcode)
        {
            string[] values = colorcode.Split(':');

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = values[i].Trim(new char[] { ' ', 'R', 'G', 'B', 'A', '{', '}' });
            }

            return new Color(
                Convert.ToInt32(values[1]),
                Convert.ToInt32(values[2]),
                Convert.ToInt32(values[3]));
        }
        private int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        // create and initialize a crypto algorithm
        private static SymmetricAlgorithm getAlgorithm(string password)
        {
            SymmetricAlgorithm algorithm = Rijndael.Create();
            Rfc2898DeriveBytes rdb = new Rfc2898DeriveBytes(
                password, new byte[] {
            0x53,0x6f,0x64,0x69,0x75,0x6d,0x20,             // salty goodness
            0x43,0x68,0x6c,0x6f,0x72,0x69,0x64,0x65
        }
            );
            algorithm.Padding = PaddingMode.ISO10126;
            algorithm.Key = rdb.GetBytes(32);
            algorithm.IV = rdb.GetBytes(16);
            return algorithm;
        }
        public static string encryptString(string clearText, string password)
        {
            SymmetricAlgorithm algorithm = getAlgorithm(password);
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, algorithm.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearBytes, 0, clearBytes.Length);
            cs.Close();
            return Convert.ToBase64String(ms.ToArray());
        }
        public static string decryptString(string cipherText, string password)
        {
            SymmetricAlgorithm algorithm = getAlgorithm(password);
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, algorithm.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherBytes, 0, cipherBytes.Length);
            cs.Close();
            return System.Text.Encoding.Unicode.GetString(ms.ToArray());
        }

        // memory stream serializations
        public static MemoryStream SerializeToStream(object o)
        {
            using (MemoryStream stream = new MemoryStream(new byte[2048]))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, o);
                return stream;
            }
        }
        public static object DeserializeFromStream(MemoryStream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);

            // Allows us to manually control type-casting based on assembly/version and type name
            // formatter.Binder = new OverrideBinder();

            stream.Seek(0, SeekOrigin.Begin);
            object o = formatter.Deserialize(stream); // Unable to find assembly

            return o;
        }

        // obsolete methods
        /// <summary>
        /// Start listening for new data
        /// </summary>
        //private void StartListening()
        //{
        //    //server.GetStream().BeginRead(readBuffer, 0, StateObject.BufferSize, StreamReceived, null);
        //    sender.BeginReceive(readBuffer, 0, StateObject.BufferSize, 0,
        //                   new AsyncCallback(Read_Callback), so2);
        //}

        //private void ReadUserDataXml(byte[] byteArray)
        //{
        //    string s = System.Text.Encoding.UTF8.GetString(byteArray);

        //    //message has successfully been received
        //    string bytestring = new ASCIIEncoding().GetString(byteArray, 0, byteArray.Length);

        //    if (encryption)
        //    {
        //        bytestring = decryptString(bytestring.ToString(), "Assesjode");
        //    }

        //    XDocument[] docs = new XDocument[60];
        //    int count = 0;

        //    try
        //    {
        //        // fetch all data within the incoming networkstream
        //        for (int val = 0; val < bytestring.Split('?').Length - 1; val++)
        //        {
        //            string content = "<?" + bytestring.Split('?')[val + 1].ToString() + "?" + bytestring.Split('?')[val + 2].ToString();

        //            char last = content[content.Length - 1];
        //            if (last == '<')
        //                content = content.Substring(0, content.Length - 1);
        //            try
        //            {
        //                docs[count] = XDocument.Parse(content);
        //            }
        //            catch
        //            {
        //                break;
        //            }
        //            finally
        //            {
        //                count++;
        //                val++;
        //            }
        //        }
        //    }
        //    catch (Exception ee)
        //    {
        //        string error = ee.ToString();
        //    }

        //    try
        //    {
        //        foreach (var doc in docs)
        //        {
        //            if (doc != null)
        //            {
        //                string rootelement = doc.Root.Name.ToString();
        //                Type elementType = Type.GetType("XNA_ScreenManager.Networking.ServerClasses." + rootelement);

        //                //Object obj = DeserializeFromXml<Object>(new ASCIIEncoding().GetString(byteArray, 0, byteArray.Length), elementType);

        //                StringBuilder builder = new StringBuilder();
        //                using (TextWriter writer = new StringWriter(builder))
        //                {
        //                    doc.Save(writer);
        //                }

        //                Object obj = DeserializeFromXml<Object>(builder.ToString(), elementType);

        //                if (obj is playerData)
        //                {
        //                    if (ScreenManager.Instance.activeScreen == ScreenManager.Instance.actionScreen)
        //                        incomingPlayerData(obj as playerData);
        //                    else if (ScreenManager.Instance.activeScreen == ScreenManager.Instance.selectCharScreen)
        //                        incomingCharSelect(obj as playerData);
        //                }
        //                else if (obj is ChatData)
        //                    incomingChatData(obj as ChatData);
        //                else if (obj is MonsterData)
        //                    incomingMonsterData(obj as MonsterData);
        //                else if (obj is AccountData)
        //                    incomingAccountData(obj as AccountData);
        //                else if (obj is EffectData)
        //                    incomingEffectData(obj as EffectData);
        //                else
        //                    break;
        //            }
        //        }
        //    }
        //    catch (Exception ee)
        //    {
        //        string error = ee.ToString();
        //    }
        //}

        /// <summary>
        /// Data was received
        /// </summary>
        /// <param name="ar">Async status</param>
        //private void StreamReceived(IAsyncResult ar)
        //{
        //    int bytesRead = 0;

        //    try
        //    {
        //        lock (server.GetStream())
        //        {
        //            bytesRead = server.GetStream().EndRead(ar);
        //        }
        //    }

        //    catch (Exception e) { string error = e.ToString(); }

        //    //An error happened that created bad data
        //    if (bytesRead == 0)
        //    {
        //        Disconnect();
        //        ScreenManager.Instance.actionScreen.topmessage.Display("Disconnected from server.", Color.PaleVioletRed, 5.0f);
        //        ScreenManager.Instance.actionScreen.hud.chatbarInput.updateTextlog("[System]", "Disconnected from server.");
        //        Connected = false;
        //        return;
        //    }

        //    //Create the byte array with the number of bytes read
        //    byte[] data = new byte[bytesRead];

        //    //Populate the array
        //    for (int i = 0; i < bytesRead; i++)
        //        data[i] = readBuffer[i];

        //    //ReadUserDataXml(data);
        //    ReadUserDataStream(data);

        //    // clear readbuffer
        //    Array.Clear(readBuffer, 0, readBuffer.Length);
        //    server.GetStream().Flush();

        //    //Listen for new data
        //    StartListening();
        //}

        //public static T DeserializeFromXml<T>(string xml, Type type)
        //{
        //    T result;
        //    XmlSerializer ser = new XmlSerializer(type);
        //    using (TextReader tr = new StringReader(xml))
        //    {
        //        result = (T)ser.Deserialize(tr);
        //    }
        //    return result;
        //}
    }

    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 40000;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }

}
