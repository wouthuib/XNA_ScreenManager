using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Specialized;
using System.Collections.Generic;
using XNA_ScreenManager.ItemClasses;
using XNA_ScreenManager.MapClasses;

namespace XNA_ScreenManager.ScreenClasses.SubComponents
{
    class ItemlistComponent : DrawableGameComponent
    {
        Inventory inventory = Inventory.Instance;
        GameWorld world;

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
        public List<Item> menuItemsnoDupes = new List<Item>();
        int width, height;

        public ItemlistComponent(Game game, SpriteFont spriteFont)
            : base(game)
        {
            this.spriteFont = spriteFont;
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            Initialize();
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
            menuItemsnoDupes.Clear();
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
            world = GameWorld.GetInstance;
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

                    // normal color counter
                    spriteBatch.DrawString(spriteFont, menuItems.FindAll(delegate(Item item) { return item.itemID == menuItemsnoDupes[i].itemID; }).Count.ToString() + "x",
                    new Vector2(menuPosition.X - 40, menuPosition.Y), myColor);

                    // Draw item Sprite
                    Texture2D sprite = world.Content.Load<Texture2D>(menuItems.Find(delegate(Item item) { return item.itemID == menuItemsnoDupes[i].itemID; }).itemSpritePath);
                    Rectangle srcframe = new Rectangle(menuItems.Find(delegate(Item item) { return item.itemID == menuItemsnoDupes[i].itemID; }).SpriteFrameX * 48,
                                                       menuItems.Find(delegate(Item item) { return item.itemID == menuItemsnoDupes[i].itemID; }).SpriteFrameY * 48,
                                                       48, 48);

                    Rectangle tarframe = new Rectangle((int)menuPosition.X - 15, (int)menuPosition.Y - 8, 35, 35);

                    spriteBatch.Draw(sprite, tarframe, srcframe, Color.White);

                    // black back color name
                    spriteBatch.DrawString(spriteFont, menuItemsnoDupes[i].itemName,
                    new Vector2(menuPosition.X + 30, menuPosition.Y) + Vector2.One, Color.Black);

                    // normal color name
                    spriteBatch.DrawString(spriteFont, menuItemsnoDupes[i].itemName,
                    new Vector2(menuPosition.X + 30, menuPosition.Y), myColor);

                    menuPosition.Y += spriteFont.LineSpacing + 10;

                    if (i == 8 || i == 18) // 0 is also an id
                    {
                        menuPosition.X = menuPosition.X + 220;
                        menuPosition.Y = Position.Y;
                    }
                } 
            }            base.Draw(gameTime);
        }
    }
}
