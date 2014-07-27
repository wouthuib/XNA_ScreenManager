using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace XNA_ScreenManager.ScreenClasses.SubComponents
{
    public class KeyboardInput : DrawableGameComponent
    {
        protected SpriteBatch spriteBatch = null;
        protected ContentManager Content;
        protected GraphicsDevice graphics;

        protected SpriteFont spriteFont;
        Keys[] keys;
        bool[] IskeyUp;
        string[] SC = { ")", "!", "@", "#", "$", "%", "^", "&", "*", "(" };//special characters
        private string result = "";
        protected string[] outlinings = new string[]{"left","central","right"};
        public string outline;
        public bool Active = false;
        float transperancy = 1,
              previousTimeSec = 0;
        Vector2 position;

        public KeyboardInput(Game game, SpriteFont spriteFont, Vector2 Position)
            : base(game)
        {
            // base variables for gfx
            this.spriteFont = spriteFont;
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));
            graphics = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));

            this.position = Position;
            this.outline = outlinings[1];

            // new keyboard variables
            keys = new Keys[38];
            Keys[] tempkeys;
            tempkeys = Enum.GetValues(typeof(Keys)).Cast<Keys>().ToArray<Keys>();
            int j = 0;
            for (int i = 0; i < tempkeys.Length; i++)
            {
                if ((i == 1 || i == 11) || (i > 26 && i < 63))//get the keys listed above as well as A-Z
                {
                    keys[j] = tempkeys[i];//fill our key array
                    j++;
                }
            }
            IskeyUp = new bool[keys.Length]; //boolean for each key to make the user have to release the key before adding to the string
            for (int i = 0; i < keys.Length; i++)
                IskeyUp[i] = true;
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                KeyboardState state = Keyboard.GetState();
                int i = 0;
                foreach (Keys key in keys)
                {
                    if (state.IsKeyDown(key))
                    {
                        if (IskeyUp[i])
                        {
                            if (key == Keys.Back && result != "") result = result.Remove(result.Length - 1);
                            if (GetLengthPxt() <= 140)
                            {
                                if (key == Keys.Space) result += " ";
                                if (i > 1 && i < 12)
                                {
                                    if (state.IsKeyDown(Keys.RightShift) || state.IsKeyDown(Keys.LeftShift))
                                        result += SC[i - 2];//if shift is down, and a number is pressed, using the special key
                                    else result += key.ToString()[1];
                                }
                                if (i > 11 && i < 38)
                                {
                                    if (state.IsKeyDown(Keys.RightShift) || state.IsKeyDown(Keys.LeftShift))
                                        result += key.ToString();
                                    else result += key.ToString().ToLower(); //return the lowercase char is shift is up.
                                }
                            }
                        }
                        IskeyUp[i] = false; //make sure we know the key is pressed
                    }
                    else if (state.IsKeyUp(key)) IskeyUp[i] = true;
                    i++;
                }

                UpdatePointer(gameTime);
            }
        }

        private float GetLengthPxt()
        {
            float width = 0;

            foreach(char i in result)
                width += spriteFont.MeasureString(i.ToString()).X;

            width -= 0.5f; // small correction

            return width;
        }

        private void UpdatePointer(GameTime gameTime)
        {
            previousTimeSec -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (previousTimeSec <= 0)
            {
                previousTimeSec = (float)gameTime.ElapsedGameTime.TotalMilliseconds + 600;

                if (transperancy == 1)
                    transperancy = 0;
                else
                    transperancy = 1;
            }
        }

        public string Result
        {
            get { return result; }
        }

        public void Activate(string import)
        {
            this.Active = true;
            result = import;
        }

        public override void Draw(GameTime gameTime)
        {
            if (Active)
            {                 
                // Draw the NameTag
                //Vector2 position = new Vector2(480, 75);
                //spriteBatch.Draw(nametag, position, Color.White);
                switch(outline)
                {
                    case "central":
                    // Draw result
                    spriteBatch.DrawString(spriteFont, result, new Vector2(position.X - (int)(GetLengthPxt() / 2), position.Y) + Vector2.One, Color.Black);
                    spriteBatch.DrawString(spriteFont, result, new Vector2(position.X - (int)(GetLengthPxt() / 2), position.Y), Color.White);

                    // Draw Pointer
                    spriteBatch.DrawString(spriteFont, "|", new Vector2(position.X + (int)(GetLengthPxt() / 2), position.Y), Color.White * transperancy);
                    break;

                    case "left":
                    // Draw result
                    spriteBatch.DrawString(spriteFont, result, new Vector2(position.X,position.Y) + Vector2.One, Color.Black);
                    spriteBatch.DrawString(spriteFont, result, new Vector2(position.X,position.Y), Color.White);

                    // Draw Pointer
                    spriteBatch.DrawString(spriteFont, "|", new Vector2(position.X + (int)(GetLengthPxt()),position.Y), Color.White * transperancy);
                    break;

                    case "right":
                    // Draw result
                    spriteBatch.DrawString(spriteFont, result, new Vector2(position.X - (int)(GetLengthPxt()), position.Y) + Vector2.One, Color.Black);
                    spriteBatch.DrawString(spriteFont, result, new Vector2(position.X - (int)(GetLengthPxt()), position.Y), Color.White);

                    // Draw Pointer
                    spriteBatch.DrawString(spriteFont, "|", new Vector2(position.X, position.Y), Color.White * transperancy);
                    break;

                }
            }
        }

    }
}
