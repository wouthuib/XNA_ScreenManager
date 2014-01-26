using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Specialized;
using System.Collections.Generic;
using XNA_ScreenManager.ItemClasses;

namespace XNA_ScreenManager.ScreenClasses.SubComponents
{
    class ItemlistComponent : DrawableGameComponent
    {
        Inventory inventory = Inventory.Instance;

        SpriteBatch spriteBatch = null;
        SpriteFont spriteFont;

        Color normalColor = Color.Yellow;
        Color hiliteColor = Color.Red;

        KeyboardState oldState;
        Vector2 position = new Vector2();
        public Vector2 selectPos = new Vector2();
        int selectedIndex = 0;
        bool show = true, active = true;


        List<Item> menuItems = new List<Item>();
        List<Item> menuItemsnoDupes = new List<Item>();
        int width, height;

        public ItemlistComponent(Game game, SpriteFont spriteFont)
            : base(game)
        {
            this.spriteFont = spriteFont;
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
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
                    ///menuItems.Count - 1);
                menuItemsnoDupes.Count - 1);
            }
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

        public void SetMenuItems(List<Item> items)
        {
            menuItems.Clear();
            menuItems.AddRange(items);

            // create new list here without duplicates
            for(int i = 0; i < menuItems.Count; i++)
            {
                if (menuItemsnoDupes.FindAll(delegate(Item item) { return item.itemID == menuItems[i].itemID; }).Count == 0)
                    menuItemsnoDupes.Add(menuItems[i]);
            }

            CalculateBounds();
        }

        private void CalculateBounds()
        {
            width = 0;
            height = 0;
            foreach (var item in menuItems)
            {
                Vector2 size = spriteFont.MeasureString(item.itemDescription);
                if (size.X > width)
                    width = (int)size.X;
                height += spriteFont.LineSpacing * 2;
            }
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
                    if (selectedIndex == menuItemsnoDupes.Count)
                        selectedIndex = 0;
                }

                if (CheckKey(Keys.Up))
                {
                    selectedIndex--;
                    if (selectedIndex == -1)
                    {
                        selectedIndex = menuItemsnoDupes.Count - 1;
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
                for (int i = 0; i < menuItemsnoDupes.Count; i++)
                {
                    if (i == SelectedIndex)
                    {
                        selectPos = menuPosition;
                        myColor = HiliteColor;
                    }
                    else
                        myColor = NormalColor;
                                        
                    // check if already writen on screen
                    // for (int a = 0; a < i; a++)
                    //    if (menuItems[i].itemID == menuItems[a].itemID)
                    //        draw = false;

                    
                    // black back color counter
                    spriteBatch.DrawString(spriteFont, menuItems.FindAll(delegate(Item item) { return item.itemID == menuItemsnoDupes[i].itemID; }).Count.ToString() + "x",
                    new Vector2(menuPosition.X - 40, menuPosition.Y) + Vector2.One, Color.Black);

                    // black back color name
                    spriteBatch.DrawString(spriteFont, menuItemsnoDupes[i].itemName,
                    menuPosition + Vector2.One, Color.Black);

                    // normal color counter
                    spriteBatch.DrawString(spriteFont, menuItems.FindAll(delegate(Item item) { return item.itemID == menuItemsnoDupes[i].itemID; }).Count.ToString() + "x",
                    new Vector2(menuPosition.X - 40, menuPosition.Y), myColor);

                    // normal color name
                    spriteBatch.DrawString(spriteFont, menuItemsnoDupes[i].itemName,
                    menuPosition, myColor);

                    menuPosition.Y += spriteFont.LineSpacing;

                    if (i == 9 || i == 19) // 0 is also an id
                    {
                        menuPosition.X = menuPosition.X + 220;
                        menuPosition.Y = Position.Y;
                    }
                } 
            }            base.Draw(gameTime);
        }
    }
}
