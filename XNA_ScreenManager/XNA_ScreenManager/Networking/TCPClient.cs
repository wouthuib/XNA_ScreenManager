using System;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.Networking.ServerClasses;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.GameWorldClasses.Effects;
using XNA_ScreenManager.ScreenClasses.MainClasses;
using XNA_ScreenManager.MonsterClasses;
using XNA_ScreenManager.CharacterClasses;
using XNA_ScreenManager.ItemClasses;
using XNA_ScreenManager.PlayerClasses;
using XNA_ScreenManager.ScreenClasses;

namespace XNA_ScreenManager.Networking
{
    public class TCPClient
    {
        private NetworkStream networkstream;
        TcpClient server;
        private byte[] readBuffer;
        public static TCPClient instance;
        public bool Connected = false;

        public TCPClient()
        {
            TCPClient.instance = this;

            byte[] data = new byte[10000];
            readBuffer = new byte[10000];
            Connect();
        }

        private void Disconnect()
        {
            server.Close();
        }

        private void Connect()
        {
            try
            {
                server = new TcpClient(ServerProperties.xmlgetvalue("address"), Convert.ToInt32(ServerProperties.xmlgetvalue("port")));
                networkstream = server.GetStream();

                StateObject state = new StateObject();
                state.workSocket = server.Client;
                Connected = true;

                // Welcome message
                //ScreenManager.Instance.actionScreen.hud.chatbarInput.updateTextlog("[System]", "Successfully connected with server ");
                //ScreenManager.Instance.actionScreen.hud.chatbarInput.updateTextlog("[System]", ServerProperties.xmlgetvalue("display"));
                //ScreenManager.Instance.actionScreen.hud.chatbarInput.updateTextlog("[System]", ServerProperties.xmlgetvalue("desc"));
                //ScreenManager.Instance.actionScreen.hud.chatbarInput.updateTextlog("[System]", "For online registration goto our website:");
                //ScreenManager.Instance.actionScreen.hud.chatbarInput.updateTextlog("[System]", ServerProperties.xmlgetvalue("registrationweb"));
            }
            catch
            {
                ScreenManager.Instance.activeScreen.topmessage.Display("Cannot connect with server.", Color.PaleVioletRed, 5.0f);
                ScreenManager.Instance.actionScreen.hud.chatbarInput.updateTextlog("[System]", "Cannot connect with server.");
                Connected = false;
            }

            // create new thread for incoming messages
            if (Connected)
            {
                networkstream.BeginRead(readBuffer, 0, StateObject.BufferSize, StreamReceived, null);
            }
        }

        public void SendData(Object obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Object));

            if (!Connected)
                Connect();

            if (Connected)
            {
                if (obj is playerData)
                    xmlSerializer = new XmlSerializer(typeof(playerData));
                else if (obj is ChatData)
                    xmlSerializer = new XmlSerializer(typeof(ChatData));
                else if (obj is DmgAreaData)
                    xmlSerializer = new XmlSerializer(typeof(DmgAreaData));
                else if (obj is AccountData)
                    xmlSerializer = new XmlSerializer(typeof(AccountData));
                else if (obj is ScreenData)
                    xmlSerializer = new XmlSerializer(typeof(ScreenData));

                if (networkstream.CanWrite)
                {
                    xmlSerializer.Serialize(networkstream, obj);
                }
                networkstream.Flush();

                Thread.Sleep(10);
            }
            else
                ScreenManager.Instance.activeScreen.topmessage.Display("Cannot connect with server.", Color.PaleVioletRed, 5.0f);
        }

        public byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
        
        /// <summary>
        /// Start listening for new data
        /// </summary>
        private void StartListening()
        {
            server.GetStream().BeginRead(readBuffer, 0, StateObject.BufferSize, StreamReceived, null);
        }

        /// <summary>
        /// Data was received
        /// </summary>
        /// <param name="ar">Async status</param>
        private void StreamReceived(IAsyncResult ar)
        {
            int bytesRead = 0;

            try
            {
                lock (server.GetStream())
                {
                    bytesRead = server.GetStream().EndRead(ar);
                }
            }

            catch (Exception e) { string error = e.ToString(); }

            //An error happened that created bad data
            if (bytesRead == 0)
            {
                Disconnect();
                ScreenManager.Instance.actionScreen.topmessage.Display("Disconnected from server.", Color.PaleVioletRed, 5.0f);
                ScreenManager.Instance.actionScreen.hud.chatbarInput.updateTextlog("[System]", "Disconnected from server.");
                Connected = false;
                return;
            }

            //Create the byte array with the number of bytes read
            byte[] data = new byte[bytesRead];

            //Populate the array
            for (int i = 0; i < bytesRead; i++)
                data[i] = readBuffer[i];

            ReadUserData(data);

            // clear readbuffer
            Array.Clear(readBuffer, 0, readBuffer.Length);
            server.GetStream().Flush();

            //Listen for new data
            StartListening();
        }

        // Wouter's methods

        private void ReadUserData(byte[] byteArray)
        {
            string s = System.Text.Encoding.UTF8.GetString(byteArray);

            //message has successfully been received
            string bytestring = new ASCIIEncoding().GetString(byteArray, 0, byteArray.Length);
            XDocument[] docs = new XDocument[60];
            int count = 0;

            try
            {
                // fetch all data within the incoming networkstream
                for (int val = 0; val < bytestring.Split('?').Length - 1; val++)
                {
                    string content = "<?" + bytestring.Split('?')[val + 1].ToString() + "?" + bytestring.Split('?')[val + 2].ToString();

                    char last = content[content.Length - 1];
                    if (last == '<')
                        content = content.Substring(0, content.Length - 1);
                    try
                    {
                        docs[count] = XDocument.Parse(content);
                    }
                    catch
                    {
                        break;
                    }
                    finally
                    {
                        count++;
                        val++;
                    }
                }
            }
            catch (Exception ee)
            {
                string error = ee.ToString();
            }

            try
            {
                foreach (var doc in docs)
                {
                    if (doc != null)
                    {
                        string rootelement = doc.Root.Name.ToString();
                        Type elementType = Type.GetType("XNA_ScreenManager.Networking.ServerClasses." + rootelement);

                        //Object obj = DeserializeFromXml<Object>(new ASCIIEncoding().GetString(byteArray, 0, byteArray.Length), elementType);
                        
                        StringBuilder builder = new StringBuilder();
                        using (TextWriter writer = new StringWriter(builder))
                        {
                            doc.Save(writer);
                        }
                        
                        Object obj = DeserializeFromXml<Object>(builder.ToString(), elementType);

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
                        else
                            break;
                    }
                }
            }
            catch(Exception ee) 
            {
                string error = ee.ToString();
            }
        }

        public static T DeserializeFromXml<T>(string xml, Type type)
        {
            T result;
            XmlSerializer ser = new XmlSerializer(type);
            using (TextReader tr = new StringReader(xml))
            {
                result = (T)ser.Deserialize(tr);
            }
            return result;
        }

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
            bool found = false;

            if (player.Action == "Remove")
            {
                NetworkPlayerStore.Instance.playerlist[NetworkPlayerSprite.NetworkStoreID(player.Name)] = null;
                GameWorld.GetInstance.listEntity.Find(p => p.EntityName == player.Name).KeepAliveTime = 0; // removed in next update
            }
            else if (player.Action == "Online")
            { }
            else if (player.Action == "Sprite_Update" && player.Name == PlayerStore.Instance.activePlayer.Name)
            {
                PlayerSprite sprite = GameWorld.GetInstance.playerSprite;

                if (sprite.State.ToString() != player.spritestate)
                {
                    sprite.State = (EntityState)Enum.Parse(typeof(EntityState), player.spritestate);
                    sprite.Position = new Vector2(player.PositionX, player.PositionY);
                }
            }
            else if (player.Name != PlayerStore.Instance.activePlayer.Name)
            {
                for (int i = 0; i < NetworkPlayerStore.Instance.playerlist.Length; i++)
                {
                    playerData entry = NetworkPlayerStore.Instance.playerlist[i];

                    if (entry != null)
                    {
                        if (entry.Name == player.Name)
                        {
                            found = true; // update existing player
                            NetworkPlayerStore.Instance.playerlist[i] = player;

                            // update mapname
                            if (GameWorld.GetInstance.listEntity.FindAll(x => x.EntityName == player.Name).Count > 0)
                            {
                                NetworkPlayerSprite NWplayer = (NetworkPlayerSprite)GameWorld.GetInstance.listEntity.Find(x => x.EntityName == player.Name);
                                NWplayer.MapName = player.mapName;
                            }
                        }
                    }
                }

                if (!found) // add new player
                    NetworkPlayerStore.Instance.addPlayer(player);
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

                        monster.update_server(
                            new Vector2(mobdata.PositionX, mobdata.PositionY),
                            (EntityState)Enum.Parse(typeof(EntityState), mobdata.spritestate),
                            (SpriteEffects)Enum.Parse(typeof(SpriteEffects), mobdata.spriteEffect));

                    }
                }
            }

            if (!found) // add new monster
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

                player.equipment.addItem(ItemStore.Instance.item_list.Find(x => x.itemName == playerdata.armor));
                player.equipment.addItem(ItemStore.Instance.item_list.Find(x => x.itemName == playerdata.weapon));

                PlayerStore.Instance.addPlayer(player);
            }
        }

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
    }

    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 10000;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

}
