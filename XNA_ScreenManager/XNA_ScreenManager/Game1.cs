using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.ScreenClasses;
using XNA_ScreenManager.ScreenClasses.InGame;
using XNA_ScreenManager.ScreenClasses.Menus;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.ScreenClasses.MainClasses;
using XNA_ScreenManager.GameAssets;
using XNA_ScreenManager.GameAssets.InGame;

namespace XNA_ScreenManager
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ResourceManager resourcemanager;

        // Game Assets
        MouseManager mousemanager;

        // screens are drawble game objects
        StartScreen startScreen;
        HelpScreen helpScreen;         ActionScreen actionScreen;
        CharacterCreationScreen createCharScreen; // new
        CharacterSelectionScreen selectCharScreen; // new
        SkillScreen skillScreen; // new
        StatusScreen statusScreen; // new
        InGameMainMenuScreen ingameMenuScreen;
        ItemMenuScreen itemMenuScreen;
        EquipmentMenuScreen equipmentMenuScreen;
        ShopMenuScreen shopMenuScreen;
        MessagePopup MessagePopupScreen;
        LoadingScreen loadingScreen;
        LoginScreen loginScreen; // new

        SpriteFont normalFont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            ScreenManager.Instance.game = this;
            //MenuManager.Instance.game = this;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), spriteBatch);
            Services.AddService(typeof(ContentManager), Content);
            Services.AddService(typeof(GraphicsDeviceManager), graphics);
            Services.AddService(typeof(GraphicsDevice), GraphicsDevice);
            Services.AddService(typeof(GameTime), new GameTime());

            normalFont = Content.Load<SpriteFont>(@"font\Comic_Sans_18px");

            // create resource manager
            resourcemanager = ResourceManager.CreateInstance(this);

            // create screens
            ingameMenuScreen = new InGameMainMenuScreen(this, normalFont, Content.Load<Texture2D>(@"gfx\screens\game_menu2"));
            ScreenManager.Instance.ingameMenuScreen = ingameMenuScreen;
            Components.Add(ingameMenuScreen);

            // new 2014
            createCharScreen = new CharacterCreationScreen(this);
            ScreenManager.Instance.createCharScreen = createCharScreen;
            Components.Add(createCharScreen);

            selectCharScreen = new CharacterSelectionScreen(this);
            ScreenManager.Instance.selectCharScreen = selectCharScreen;
            Components.Add(selectCharScreen);

            // main screens
            startScreen = new StartScreen(this, normalFont, Content.Load<Texture2D>(@"gfx\screens\title_menu2"));
            ScreenManager.Instance.startScreen = startScreen;
            Components.Add(startScreen);

            helpScreen = new HelpScreen(this, Content.Load<Texture2D>(@"gfx\screens\system_menu2"));
            ScreenManager.Instance.helpScreen = helpScreen;
            Components.Add(helpScreen);

            loginScreen = new LoginScreen(this, Content.Load<Texture2D>(@"gfx\screens\maplestory_loginscreen"));
            ScreenManager.Instance.loginScreen = loginScreen;
            Components.Add(loginScreen);

            actionScreen = new ActionScreen(this, normalFont);
            ScreenManager.Instance.actionScreen = actionScreen;
            Components.Add(actionScreen);

            itemMenuScreen = new ItemMenuScreen(this, normalFont, Content.Load<Texture2D>(@"gfx\screens\item_menu2"));
            ScreenManager.Instance.itemMenuScreen = itemMenuScreen;
            Components.Add(itemMenuScreen);

            equipmentMenuScreen = new EquipmentMenuScreen(this, normalFont, Content.Load<Texture2D>(@"gfx\screens\equipment_menu2"));
            ScreenManager.Instance.equipmentMenuScreen = equipmentMenuScreen;
            Components.Add(equipmentMenuScreen);

            skillScreen = new SkillScreen(this, Content.Load<Texture2D>(@"gfx\screens\skilltree_menu2"));
            ScreenManager.Instance.skillScreen = skillScreen;
            Components.Add(skillScreen);

            statusScreen = new StatusScreen(this, Content.Load<Texture2D>(@"gfx\screens\status_menu2"));
            ScreenManager.Instance.statusScreen = statusScreen;
            Components.Add(statusScreen);

            shopMenuScreen = new ShopMenuScreen(this, normalFont, Content.Load<Texture2D>(@"gfx\screens\shop_menu2"));
            ScreenManager.Instance.shopMenuScreen = shopMenuScreen;
            Components.Add(shopMenuScreen);

            loadingScreen = new LoadingScreen(this);
            ScreenManager.Instance.loadingScreen = loadingScreen;
            Components.Add(loadingScreen);

            MessagePopupScreen = new MessagePopup(this, normalFont);
            ScreenManager.Instance.MessagePopupScreen = MessagePopupScreen;
            Components.Add(MessagePopupScreen);

            // Create Screen Manager
            ScreenManager.Instance.StartManager();

            // Create Mouse Manager
            mousemanager = MouseManager.Instance;
            mousemanager.StartManager();
            Components.Add(mousemanager);

            Components.Add(MenuManager.CreateInstance(this));

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            ScreenManager.Instance.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null);

            base.Draw(gameTime);

            spriteBatch.End();
        }
    }
}
