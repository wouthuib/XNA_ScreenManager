using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.ScreenClasses.SubComponents;
using Microsoft.Xna.Framework.Content;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.ScreenClasses.MainClasses;
using XNA_ScreenManager.PlayerClasses;
using Microsoft.Xna.Framework.Input;

namespace XNA_ScreenManager.ScreenClasses.Menus
{
    public class CharacterSelectionScreen : GameScreen
    {
        #region properties

        SpriteBatch spriteBatch;
        ContentManager Content;

        PlayerStore playerStore = PlayerStore.Instance;
        PlayerInfo player;

        KeyboardState oldState;
        BackgroundComponent bgcomp1, bgcomp2, bgcomp3;
        public MenuComponent menu;
        SpriteFont playerNameFont;

        Texture2D slot;
        Texture2D option_board, screen_board;
        PlayerSprite playersprite;
        public Vector2[] spriteOfset = new Vector2[6];

        string[] menuItems = {
            "Select", 
            "Create", 
            "Delete",
            "Continue",
            "Back"};
        #endregion

        public CharacterSelectionScreen(Game game)
            : base(game)
        {
            // select Resource services
            spriteBatch = ResourceManager.GetInstance.spriteBatch;
            Content = ResourceManager.GetInstance.Content;

            // select spritefont
            playerNameFont = Content.Load<SpriteFont>(@"font\Arial_12px");

            // define components
            bgcomp1 = new BackgroundComponent(game, Content.Load<Texture2D>(@"gfx\background\background1"));
            bgcomp2 = new BackgroundComponent(game, Content.Load<Texture2D>(@"gfx\background\character_selection"));
            bgcomp3 = new BackgroundComponent(game, Content.Load<Texture2D>(@"gfx\background\frame2"));
            menu = new MenuComponent(game, Content.Load<SpriteFont>(@"font\Comic_Sans_18px"));

            // add components
            Components.Add(bgcomp1);
            Components.Add(bgcomp2);
            Components.Add(bgcomp3);

            // menu options
            menu.SetMenuItems(menuItems);
            menu.StartIndex = 0;
            menu.Position = new Vector2(665, 230);
            menu.NormalColor = Color.White;
            menu.HiliteColor = Color.Red;
            menu.MenuItemSpace = 25;
            menu.Rotation[2] = 0.25f;
            menu.Offset[2] = new Vector2(0, -5);
            menu.Offset[3] = new Vector2(-8, 23);
            menu.Offset[4] = new Vector2(-526, -22);
            menu.EndIndex = 3;

            // player sprite
            playersprite = new PlayerSprite(100, 123);
            player = new PlayerInfo();
            playersprite.Player = this.player;

            // player offsets
            for (int i = 0; i < 6; i++)
                this.spriteOfset[i] = playersprite.getoffset(i);

            // options
            option_board = Content.Load<Texture2D>(@"gfx\screens\screenobjects\option_board");
            screen_board = Content.Load<Texture2D>(@"gfx\screens\screenobjects\next_board");

            // slots
            slot = Content.Load<Texture2D>(@"gfx\screens\screenobjects\empty_character_slot");            
        }
        
        public int SelectedIndex
        {
            get { return menu.SelectedIndex; }
        }

        public int SelectedPlayer
        {
            get { return PlayerStore.Instance.ActiveSlot; }
            set
            {
                PlayerStore.Instance.ActiveSlot = (int)MathHelper.Clamp(
                value,
                -1,
                PlayerStore.Instance.Count);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            menu.Update(gameTime);

            KeyboardState newState = Keyboard.GetState();

            if (SelectedIndex == 0)
            {
                if (CheckKey(Keys.Right))
                {
                    SelectedPlayer++;
                    updateOfset();
                    if (SelectedPlayer == PlayerStore.Instance.Count)
                        SelectedPlayer = 0;
                }

                if (CheckKey(Keys.Left))
                {
                    SelectedPlayer--;
                    updateOfset();
                    if (SelectedPlayer == - 1)
                        SelectedPlayer = PlayerStore.Instance.Count - 1;
                }
            }

            oldState = newState;
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 Position = new Vector2(100, 130);

            // Draw Backgrounds
            base.Draw(gameTime);

            // Draw Player
            for (int ID = 0; ID < 6; ID++)
            {
                if (playerStore.getPlayer(null, ID) != null)
                {
                    // Draw player slots
                    float transperancy = 1f;
                    playersprite.Position = new Vector2(Position.X, Position.Y - 10);

                    if (ID == playerStore.ActiveSlot)
                        playersprite.SetState = CharacterClasses.EntityState.Stand;
                    else
                    {
                        transperancy = 0.5f;
                        playersprite.SetState = CharacterClasses.EntityState.Sit;
                        playersprite.Position = new Vector2(Position.X, Position.Y);
                    }

                    playersprite.Player = playerStore.getPlayer(null, ID);
                    for (int i = 0; i < 6; i++)
                        playersprite.Player.spriteOfset[i] = playersprite.getoffset(i);

                    playersprite.Draw(spriteBatch);

                    // Draw player Name Rect
                    Texture2D rect = new Texture2D(ResourceManager.GetInstance.gfxdevice,
                        (int)(playerNameFont.MeasureString(playerStore.getPlayer(null, ID).Name).X),
                        (int)(playerNameFont.MeasureString(playerStore.getPlayer(null, ID).Name).Y));

                    Color[] data = new Color[(int)(playerNameFont.MeasureString(playerStore.getPlayer(null, ID).Name).X) * (int)(playerNameFont.MeasureString(playerStore.getPlayer(null, ID).Name).Y)];
                    for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
                    rect.SetData(data);

                    // Draw Player Name
                    spriteBatch.Draw(rect, new Vector2(
                        Position.X + (playersprite.SpriteFrame.Width * 0.5f) - (playerNameFont.MeasureString(playerStore.getPlayer(null, ID).Name).X * 0.5f),
                                    Position.Y + (playersprite.SpriteFrame.Height) + 5),
                        Color.White * 0.5f);

                    spriteBatch.DrawString(playerNameFont, playerStore.getPlayer(null, ID).Name,
                        new Vector2(Position.X + (playersprite.SpriteFrame.Width * 0.5f) - (playerNameFont.MeasureString(playerStore.getPlayer(null, ID).Name).X * 0.5f),
                                    Position.Y + (playersprite.SpriteFrame.Height) + 5),
                        Color.White * transperancy);
                }
                else
                {
                    // Draw Slots
                    spriteBatch.Draw(slot, Position, Color.White);
                }

                Position.X += 150;

                if (Position.X > 400)
                {
                    Position.X = 100;
                    Position.Y = 290;
                }
            }

            // Draw option board
            spriteBatch.Draw(option_board, new Vector2(620, 205), Color.White);

            // Draw screen boards
            spriteBatch.Draw(screen_board, new Rectangle(100, 372, screen_board.Width, screen_board.Height), 
                new Rectangle(0, 0, screen_board.Width, screen_board.Height), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(screen_board, new Vector2(650, 372), Color.White);

            // Draw menu items
            menu.Draw(gameTime);

        }

        public bool CheckKey(Microsoft.Xna.Framework.Input.Keys theKey)
        {
            KeyboardState newState = Keyboard.GetState();
            return oldState.IsKeyDown(theKey) && newState.IsKeyUp(theKey);
        }

        public void updateOfset()
        {
            foreach (var getplayer in PlayerStore.Instance.playerlist)
            {
                if(getplayer != null)
                    for (int ofset = 0; ofset < 6; ofset++)
                    {
                        playersprite.clearoffset(ofset);
                        getplayer.spriteOfset[ofset] = playersprite.getoffset(ofset); // update offsets
                    }
            }
        }
    }
}
