using System;
using XNA_ScreenManager.Networking;

namespace XNA_ScreenManager
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
            
            // Properly close the Socket connection
            if (TCPClient.instance != null)
            {
                if (TCPClient.instance.Connected)
                    TCPClient.instance.Disconnect();

                if (TCPClient.instance.sendloop.IsAlive)
                    TCPClient.instance.sendloop.Abort();
            }
        }
    }
#endif
}

