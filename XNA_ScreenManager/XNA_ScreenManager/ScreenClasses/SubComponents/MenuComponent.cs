using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Specialized;


namespace XNA_ScreenManager
{
    public enum OrderStyle
    {
        Right, Central, Left
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MenuComponent : DrawableGameComponent
    {
        protected SpriteBatch spriteBatch = null;
        protected SpriteFont spriteFont;

        Color normalColor = Color.Yellow;
        Color hiliteColor = Color.Red;

        KeyboardState oldState;
        protected Vector2 position = new Vector2();
        public Vector2 selectPos = new Vector2();
        int selectedIndex = 0, startIndex = -1, endIndex = 4, menuItemSpace = 0;
        protected bool show = true, active = true, displaySingle = false, listDown = true, shade = true;
        float[] rotation = new float[20];
        Vector2[] offset = new Vector2[20];
        OrderStyle style = OrderStyle.Left;

        private StringCollection menuItems = new StringCollection();
        int width, height;

        public MenuComponent(Game game, SpriteFont spriteFont)
            : base(game)
        {
            this.spriteFont = spriteFont;
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            // rotation
            for (int i = 0; i < 20; i++)
            {
                rotation[i] = 0;
                offset[i] = Vector2.Zero;
            }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = (int)MathHelper.Clamp(
                value,
                0,
                menuItems.Count - 1);
            }
        }

        public int StartIndex
        {
            get { return startIndex; }
            set { startIndex = value; }
        }

        public int EndIndex
        {
            get { return endIndex; }
            set { endIndex = value; }
        }

        public Color NormalColor
        {
            get { return normalColor; }
            set { normalColor = value; }
        }

        public Color HiliteColor
        {
            get { return hiliteColor; }
            set { hiliteColor = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public SpriteFont SpriteFont
        {
            get { return spriteFont; }
            set { spriteFont = value; }
        }

        public OrderStyle Style
        {
            get { return style; }
            set { style = value; }
        }

        public int MenuItemSpace
        {
            get { return menuItemSpace; }
            set { menuItemSpace = value; }
        }

        public float[] Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector2[] Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public bool DisplaySingle
        {
            get { return displaySingle; }
            set { displaySingle = value; }
        }

        public bool ListDown
        {
            get { return listDown; }
            set { listDown = value; }
        }

        public bool Shade
        {
            get { return shade; }
            set { shade = value; }
        }

        public bool Show
        {
            get { return show; }
            set { show = value; }
        }

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public StringCollection MenuItems
        {
            get { return menuItems; }
        }

        public void SetMenuItems(string[] items)
        {
            menuItems.Clear();
            menuItems.AddRange(items);
            CalculateBounds();
            endIndex = menuItems.Count;
        }

        private void CalculateBounds()
        {
            width = 0;
            height = 0;
            foreach (string item in menuItems)
            {
                Vector2 size = spriteFont.MeasureString(item);
                if (size.X > width)
                    width = (int)size.X;
                height += spriteFont.LineSpacing;
            }
        }

        public Vector2 getBounds()
        {
            CalculateBounds();
            return new Vector2(width, height);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (active)
            {
                KeyboardState newState = Keyboard.GetState();

                if (listDown)
                {
                    if (CheckKey(Keys.Down))
                    {
                        selectedIndex++;
                        if (selectedIndex == endIndex) //menuItems.Count)
                            selectedIndex = startIndex;
                    }

                    if (CheckKey(Keys.Up))
                    {
                        selectedIndex--;
                        if (selectedIndex == startIndex - 1)
                            selectedIndex = endIndex - 1; //menuItems.Count - 1;
                    }
                }
                else
                {
                    if (CheckKey(Keys.Right))
                    {
                        selectedIndex++;
                        if (selectedIndex == endIndex) //menuItems.Count)
                            selectedIndex = startIndex;
                    }

                    if (CheckKey(Keys.Left))
                    {
                        selectedIndex--;
                        if (selectedIndex == startIndex - 1)
                            selectedIndex = endIndex - 1; //menuItems.Count - 1;
                    }
                }

                oldState = newState;
            }

            base.Update(gameTime);
        }

        public bool CheckKey(Keys theKey)
        {
            KeyboardState newState = Keyboard.GetState();
            return oldState.IsKeyDown(theKey) && newState.IsKeyUp(theKey);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 menuPosition = Position;
            Color myColor;

            if (show)
            {
                for (int i = 0; i < menuItems.Count; i++)
                {
                    // Make sure that Selected index is not smaller than Startindex
                    if (selectedIndex < startIndex)
                        selectedIndex = startIndex;

                    // Change position from left to central
                    if (i >= startIndex && style == OrderStyle.Central)
                        menuPosition.X = Position.X + (width * 0.5f) - (menuItems[i].Length * 4f);

                    // Maintain the Menuitems Colors 
                    if (i == SelectedIndex)
                    {
                        selectPos = menuPosition;
                        myColor = HiliteColor;
                    }
                    else
                    {
                        if (i >= startIndex)
                            myColor = NormalColor;
                        else
                            myColor = Color.LightBlue;
                    }

                    if (!displaySingle || selectedIndex == i)
                    {
                        // Start Drawing Menuitems
                        if(shade)
                            spriteBatch.DrawString(
                            spriteFont,
                            menuItems[i],
                            menuPosition + Vector2.One + offset[i],
                            Color.Black,
                            rotation[i],
                            Vector2.Zero,
                            1,
                            SpriteEffects.None,
                            0);

                        spriteBatch.DrawString(
                        spriteFont,
                        menuItems[i],
                        menuPosition + offset[i],
                        myColor,
                        rotation[i],
                        Vector2.Zero,
                        1,
                        SpriteEffects.None,
                        0);

                        if (!displaySingle)
                        {
                            menuPosition.Y += spriteFont.LineSpacing;
                            menuPosition.Y += menuItemSpace;

                            if (i == 12 || i == 25) // 0 is also an id
                            {
                                menuPosition.X = menuPosition.X + 220;
                                menuPosition.Y = Position.Y;
                            }
                        }
                    }
                } 
            }            base.Draw(gameTime);
        }
    }
}
