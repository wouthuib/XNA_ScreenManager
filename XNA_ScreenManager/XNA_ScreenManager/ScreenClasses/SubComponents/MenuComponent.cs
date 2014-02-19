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
        SpriteBatch spriteBatch = null;
        SpriteFont spriteFont;

        Color normalColor = Color.Yellow;
        Color hiliteColor = Color.Red;

        KeyboardState oldState;
        Vector2 position = new Vector2();
        public Vector2 selectPos = new Vector2();
        int selectedIndex = 0, startIndex = -1;
        bool show = true, active = true;
        OrderStyle style = OrderStyle.Left;

        private StringCollection menuItems = new StringCollection();
        int width, height;

        public MenuComponent(Game game, SpriteFont spriteFont)
            : base(game)
        {
            this.spriteFont = spriteFont;
            spriteBatch =
            (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
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

                if (CheckKey(Keys.Down))
                {
                    selectedIndex++;
                    if (selectedIndex == menuItems.Count)
                        selectedIndex = startIndex;
                }

                if (CheckKey(Keys.Up))
                {
                    selectedIndex--;
                    if (selectedIndex == startIndex - 1)
                    {
                        selectedIndex = menuItems.Count - 1;
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
                    if (selectedIndex < startIndex)
                        selectedIndex = startIndex;
                    else
                    {
                        if (style == OrderStyle.Central && selectedIndex > startIndex)
                            menuPosition.X = Position.X + (width * 0.5f) - (menuItems[i].Length * 3f);
                    }

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

                    spriteBatch.DrawString(
                    spriteFont,
                    menuItems[i],
                    menuPosition + Vector2.One,
                    Color.Black);

                    spriteBatch.DrawString(spriteFont,
                    menuItems[i],
                    menuPosition,
                    myColor);

                    menuPosition.Y += spriteFont.LineSpacing;

                    if (i == 12 || i == 25) // 0 is also an id
                    {
                        menuPosition.X = menuPosition.X + 220;
                        menuPosition.Y = Position.Y;
                    }
                } 
            }            base.Draw(gameTime);
        }
    }
}
