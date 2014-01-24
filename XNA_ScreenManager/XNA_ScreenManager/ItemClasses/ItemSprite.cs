using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA_ScreenManager.CharacterClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.MapClasses;
using Microsoft.Xna.Framework.Content;

namespace XNA_ScreenManager.ItemClasses
{
    class ItemSprite : XNA_ScreenManager.MapClasses.Effect
    {
        #region properties
        ItemStore itemDB;
        Inventory inventory;
        GameWorld world;

        Vector2 spriteoffSet = new Vector2(60, 10);
        Vector2 spritesize = new Vector2(34, 34);
        Vector2 cellspace = new Vector2(60, 22);
        Vector2 circleOrigin = Vector2.Zero;
        private int item;
        private float transperant = 0;
        private float angle = 0;
        #endregion

        public ItemSprite(Texture2D getsprite, Vector2 spriteFrame, Vector2 position, int itemID) : 
            base()
        {
            // general properties
            this.sprite = getsprite;
            this.position = position;
            this.item = itemID;

            /*this.spriteFrame = new Rectangle(
                (int)(spriteoffSet.X + (cellspace.X * spriteFrame.X)),
                (int)(spriteoffSet.Y + (cellspace.Y * spriteFrame.Y)),
                (int)spritesize.X, (int)spritesize.Y);*/

            this.SpriteFrame = new Rectangle(60, 10, 34, 34);

            circleOrigin = new Vector2(SpriteFrame.X + SpriteFrame.Width * 0.5f, SpriteFrame.Y + SpriteFrame.Height * 0.5f);

            // Link properties to instance
            this.itemDB = ItemStore.Instance;
            this.inventory = Inventory.Instance;
            this.world = GameWorld.GetInstance;
        }

        public void pickupItem()
        {
            // add item to inventory
            inventory.addItem(itemDB.getItem(item));

            // remove this sprite
            this.KeepAliveTime = 0;
        }

        public override void Update(GameTime gameTime)
        {
            // Start ItemSprite (default is -1)
            if (KeepAliveTime < 0)
                KeepAliveTime = (float)gameTime.TotalGameTime.Seconds + 20;

            // Remove ItemSprite Timer
            KeepAliveTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Make the item slowly appear
            if (transperant < 1)
                transperant += (float)gameTime.ElapsedGameTime.TotalSeconds / 2;

            // Make it slowly rotate
            angle += (float)gameTime.ElapsedGameTime.TotalSeconds / 2;

            // Check player collision
            if (new Rectangle((int)(world.playerSprite.Position.X + world.playerSprite.SpriteFrame.Width * 0.40f), 
                (int)world.playerSprite.Position.Y,
                (int)(world.playerSprite.SpriteFrame.Width * 0.40f), 
                (int)world.playerSprite.SpriteFrame.Height).
                Intersects(new Rectangle(
                    (int)Position.X, (int)Position.Y,
                    (int)SpriteFrame.Width, (int)SpriteFrame.Height)) == true && transperant >= 1)
                pickupItem();

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
           spriteBatch.Draw(sprite, new Rectangle((int)Position.X, (int)Position.Y,
                    (int)SpriteFrame.Width, (int)SpriteFrame.Height),
                    SpriteFrame, Color.White * transperant, angle, circleOrigin, SpriteEffects.None, 0f);
        }
    }
}
