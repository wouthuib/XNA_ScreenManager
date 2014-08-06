using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace XNA_ScreenManager.ScreenClasses.MainClasses
{
    public class ResourceManager
    {
        // Game Services
        public SpriteBatch spriteBatch = null;
        public ContentManager Content;
        public GraphicsDevice gfxdevice;
        public GameTime gameTime;

        private static ResourceManager mInstance;
        private static System.Object _mutex = new System.Object();

        private ResourceManager(Game game)
        {
            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            Content = (ContentManager)game.Services.GetService(typeof(ContentManager));
            gfxdevice = (GraphicsDevice)game.Services.GetService(typeof(GraphicsDevice));
            gameTime = (GameTime)game.Services.GetService(typeof(GameTime));
        }

        public static ResourceManager CreateInstance(Game game)
        {
            lock (_mutex) // now I can claim some form of thread safety...
            {
                if (mInstance == null)
                {
                    mInstance = new ResourceManager(game);
                }
            }
            return mInstance;
        }

        public static ResourceManager GetInstance
        {
            get
            {
                if (mInstance == null)
                {
                    throw new Exception("The ResourceManager is called, but has not yet been created!!");
                }
                return mInstance;
            }
        }

        // Singleton randomizer to provide unique values
        public class randomizer
        {
            private static randomizer instance;
            private Random rand;

            private randomizer()
            {
                rand = new Random();
            }

            public int generateRandom(int min, int max)
            {
                return rand.Next(min, max);
            }

            public static randomizer Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new randomizer();
                    }
                    return instance;
                }
            }
        }
    }
}
