using System;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;
using XNA_ScreenManager.PlayerClasses;
using XNA_ScreenManager.ScreenClasses;
using System.Xml.Linq;
using XNA_ScreenManager.Networking.ServerClasses;
using System.IO;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.GameWorldClasses.Effects;
using XNA_ScreenManager.ScreenClasses.MainClasses;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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

            byte[] data = new byte[1024];
            readBuffer = new byte[1024];
            try
            {
                server = new TcpClient(ServerProperties.xmlgetvalue("address"), Convert.ToInt32(ServerProperties.xmlgetvalue("port")));
                networkstream = server.GetStream();

                StateObject state = new StateObject();
                state.workSocket = server.Client;
                Connected = true;

                // Welcome message
                ScreenManager.Instance.actionScreen.hud.chatbarInput.updateTextlog("[System]", "Successfully connected with server ");
                ScreenManager.Instance.actionScreen.hud.chatbarInput.updateTextlog("[System]", ServerProperties.xmlgetvalue("display"));
                ScreenManager.Instance.actionScreen.hud.chatbarInput.updateTextlog("[System]", ServerProperties.xmlgetvalue("desc"));
                ScreenManager.Instance.actionScreen.hud.chatbarInput.updateTextlog("[System]", "For online registration goto our website:");
                ScreenManager.Instance.actionScreen.hud.chatbarInput.updateTextlog("[System]", ServerProperties.xmlgetvalue("registrationweb"));
            }
            catch
            {
                ScreenManager.Instance.actionScreen.topmessage.Display("Cannot connect with server.", Color.PaleVioletRed, 5.0f);
                ScreenManager.Instance.actionScreen.hud.chatbarInput.updateTextlog("[System]", "Cannot connect with server.");
                Connected = false;
            }

            // create new thread for incoming messages
            if(Connected)
                networkstream.BeginRead(readBuffer, 0, StateObject.BufferSize, StreamReceived, null);
        }

        private void Disconnect()
        {
            server.Close();
        }

        public void SendData(Object obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Object));

            if (Connected)
            {
                if (obj is playerData)
                    xmlSerializer = new XmlSerializer(typeof(playerData));
                else if (obj is ChatData)
                    xmlSerializer = new XmlSerializer(typeof(ChatData));

                if (networkstream.CanWrite)
                {
                    xmlSerializer.Serialize(networkstream, obj);
                }
                networkstream.Flush();
            }
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

        public void OnReceive(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            int bytesRead;

            if (handler.Connected)
            {

                // Read data from the client socket. 
                try
                {
                    bytesRead = handler.EndReceive(ar);
                    if (bytesRead > 0)
                    {
                        // There  might be more data, so store the data received so far.
                        state.sb.Remove(0, state.sb.Length);
                        state.sb.Append(Encoding.ASCII.GetString(
                                         state.buffer, 0, bytesRead));

                        // Display Text in Rich Text Box
                        content = state.sb.ToString();
                        Console.WriteLine(content);

                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(OnReceive), state);
                    }
                }

                catch (SocketException socketException)
                {
                    //WSAECONNRESET, the other side closed impolitely
                    if (socketException.ErrorCode == 10054 || ((socketException.ErrorCode != 10004) && (socketException.ErrorCode != 10053)))
                    {
                        handler.Close();
                    }
                }

            // Eat up exception....Hmmmm I'm loving eat!!!
                catch
                {
                    throw new Exception("Oops! Something went wrong");
                    //MessageBox.Show(exception.Message + "\n" + exception.StackTrace);
                }
            }
            else
            {
                handler.Close();
                this.Disconnect();
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

            //Listen for new data
            StartListening();
        }

        // Wouter's methods

        private void ReadUserData(byte[] byteArray)
        {
            //message has successfully been received
            ASCIIEncoding encoder = new ASCIIEncoding();
            System.Diagnostics.Debug.WriteLine(encoder.GetString(byteArray, 0, byteArray.Length));

            try
            {
                string xmlDoc = encoder.GetString(byteArray, 0, byteArray.Length).ToString();
                XDocument doc = XDocument.Parse(xmlDoc);
                string rootelement = doc.Root.Name.ToString();
                Type elementType = Type.GetType("XNA_ScreenManager.Networking.ServerClasses." + rootelement);

                Object obj = DeserializeFromXml<Object>(encoder.GetString(byteArray, 0, byteArray.Length), elementType);

                if (obj is playerData)
                    incomingPlayerData(obj as playerData);
                else if (obj is ChatData)
                    incomingChatData(obj as ChatData);
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

        private void incomingPlayerData(playerData player)
        {
            bool found = false;

            if (player.Name != PlayerStore.Instance.activePlayer.Name)
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
    }

    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
}
