using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Specialized;
using System.Collections.Generic;
using XNA_ScreenManager.ItemClasses;
using XNA_ScreenManager.MapClasses;
using Microsoft.Xna.Framework.Content;

namespace XNA_ScreenManager.ScreenClasses.SubComponents
{
    public enum ShopPrice
    {
        Buy,
        Sell,
        None
    }

    class ItemlistComponent : DrawableGameComponent
    {
        #region properties
        Inventory inventory = Inventory.Instance;
        GameWorld world;

        ContentManager Content;
        SpriteBatch spriteBatch = null;
        SpriteFont spriteFont;

        Color normalColor = Color.Yellow;
        Color hiliteColor = Color.Red;

        KeyboardState oldState;
        Vector2 position = new Vector2();
        public Vector2 selectPos = new Vector2();
        int selectedIndex = 0, maxDisplay = 0;
        bool show = true, active = true;
        ShopPrice price = ShopPrice.None;

        List<Item> menuItems = new List<Item>();
        public List<Item> menuItemsnoDupes = new List<Item>();
        int width, height;
        private float transperancy = 1;
        #endregion

        public ItemlistComponent(Game game)
            : base(game)
        {
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            Content = (ContentManager)Game.Services.GetService(typeof(ContentManager));

            spriteFont = Content.Load<SpriteFont>(@"font\Comic_Sans_15px");
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

        public float Transperancy
        {
            get { return transperancy; }
            set { transperancy = value; }
        }

        public int SelectedIndex
        {
            get 
            {
                if (selectedIndex < menuItemsnoDupes.Count)
                    return selectedIndex;
                else
                {
                    if (menuItemsnoDupes.Count - 1 >= 0)
                    {
                        selectedIndex = menuItemsnoDupes.Count - 1;
                        return selectedIndex;
                    }
                    else
                        return 0;
                }
            }
            set
            {
                selectedIndex = (int)MathHelper.Clamp(
                value,
                0,
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

        public ShopPrice Price
        {
            get { return price; }
            set { price = value; }
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
                    {
                        if (MaxDisplay == 0)
                            selectedIndex = 0;
                        else
                            selectedIndex = menuItemsnoDupes.Count - 1;
                    }
                }

                if (CheckKey(Keys.Up))
                {
                    selectedIndex--;
                    if (selectedIndex == -1)
                    {
                        if (MaxDisplay == 0)
                            selectedIndex = menuItemsnoDupes.Count - 1;
                        else
                            selectedIndex = 0;
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

        public int MaxDisplay
        {
            get 
            {
                if (maxDisplay < menuItemsnoDupes.Count && maxDisplay != 0)
                {
                    if (IndexDisplay + maxDisplay < menuItemsnoDupes.Count)
                        return IndexDisplay + maxDisplay;
                    else
                        return menuItemsnoDupes.Count;
                }
                else
                    return menuItemsnoDupes.Count;
            }
            set 
            {
                maxDisplay = value;
            }
        }

        private int IndexDisplay
        {
            get
            {
                if (maxDisplay < menuItemsnoDupes.Count && maxDisplay != 0)
                {
                    if (SelectedIndex >= maxDisplay)
                    {
                        if (SelectedIndex + maxDisplay <= menuItemsnoDupes.Count)
                            return SelectedIndex;
                        else
                            return menuItemsnoDupes.Count - maxDisplay;
                    }
                    else
                        return 0;
                }
                else
                    return 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 menuPosition = Position;
            int offsetX = 0;
            Color myColor;

            if (show)
            {
                for (int i = IndexDisplay; i < MaxDisplay; i++)
                {
                    if (i == SelectedIndex)
                    {
                        selectPos = menuPosition;
                        myColor = HiliteColor;
                    }
                    else
                        myColor = NormalColor;
                    
                    // black back color counter
                    spriteBatch.DrawString(spriteFont, menuItems.FindAll(delegate(Item item) { return item.itemID == menuItemsnoDupes[i].itemID; }).Count.ToString() + "x",
                    new Vector2(menuPosition.X - 40, menuPosition.Y) + Vector2.One, Color.Black);

                    // normal color counter
                    spriteBatch.DrawString(spriteFont, menuItems.FindAll(delegate(Item item) { return item.itemID == menuItemsnoDupes[i].itemID; }).Count.ToString() + "x",
                    new Vector2(menuPosition.X - 40, menuPosition.Y), myColor);

                    // Draw item Sprite
                    Texture2D sprite = world.Content.Load<Texture2D>(@"" + menuItems.Find(delegate(Item item) { return item.itemID == menuItemsnoDupes[i].itemID; }).itemSpritePath);
                    Rectangle srcframe = new Rectangle(menuItems.Find(delegate(Item item) { return item.itemID == menuItemsnoDupes[i].itemID; }).SpriteFrameX * 48,
                                                       menuItems.Find(delegate(Item item) { return item.itemID == menuItemsnoDupes[i].itemID; }).SpriteFrameY * 48,
                                                       48, 48);

                    Rectangle tarframe = new Rectangle((int)menuPosition.X - 15, (int)menuPosition.Y - 8, 35, 35);

                    spriteBatch.Draw(sprite, tarframe, srcframe, Color.White * transperancy);

                    // black back color name
                    spriteBatch.DrawString(spriteFont, menuItemsnoDupes[i].itemName,
                    new Vector2(menuPosition.X + 30, menuPosition.Y) + Vector2.One, Color.Black);

                    // normal color name
                    spriteBatch.DrawString(spriteFont, menuItemsnoDupes[i].itemName,
                    new Vector2(menuPosition.X + 30, menuPosition.Y), myColor);

                    // display item price
                    if (price != ShopPrice.None)
                    {
                        int price_value = menuItems.Find(delegate(Item item) { return item.itemID == menuItemsnoDupes[i].itemID; }).Price;

                        // halve the price to avoid shop exploits
                        if (price == ShopPrice.Sell)
                            price_value = (int)(price_value / 2);

                        spriteBatch.DrawString(spriteFont, price_value.ToString() + " $",
                        new Vector2(Position.X + 300 - (price_value.ToString().Length * 5), menuPosition.Y), myColor);

                        // move the scroller to the right
                        offsetX = 180;
                    }

                    // Draw scroll down and up (hex 0x2193 = 8595 see spritefont range)
                    if (MaxDisplay < menuItemsnoDupes.Count)
                        spriteBatch.DrawString(spriteFont, ">>",
                        new Vector2(Position.X + 200 + offsetX, Position.Y + (int)(MaxDisplay * 25)), NormalColor);

                    if (IndexDisplay != 0)
                        spriteBatch.DrawString(spriteFont, "<<",
                        new Vector2(Position.X + 200 + offsetX, Position.Y), NormalColor);

                    // update line position
                    menuPosition.Y += spriteFont.LineSpacing + 10;

                    if ((i == 8 || i == 18) && maxDisplay == 0) // 0 is also an id
                    {
                        menuPosition.X = menuPosition.X + 220;
                        menuPosition.Y = Position.Y;
                    }
                } 
            }            base.Draw(gameTime);
        }
    }
}
