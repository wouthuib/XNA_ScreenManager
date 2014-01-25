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
        Item item;

        Vector2 spritesize = new Vector2(32, 32);
        Vector2 circleOrigin = Vector2.Zero;

        private float transperant = 0;
        private float angle = 1;
        private bool angledirection = false;
        #endregion

        public ItemSprite(Vector2 position, int itemID) : 
            base()
        {
            // Link properties to instance
            this.itemDB = ItemStore.Instance;
            this.inventory = Inventory.Instance;
            this.world = GameWorld.GetInstance;

            // get item information from general DB
            item = itemDB.getItem(itemID);

            // general properties
            this.sprite = world.Content.Load<Texture2D>(@"" + item.itemSpritePath);
            this.position = position;

            // set the correct item sprite in item spritesheet
            this.spriteFrame = new Rectangle(
                (int)(spritesize.X * item.SpriteFrameX),
                (int)(spritesize.Y * item.SpriteFrameY),
                (int)spritesize.X, (int)spritesize.Y);

            //this.SpriteFrame = new Rectangle(60, 10, 34, 34);
            circleOrigin = new Vector2(SpriteFrame.Width * 0.5f, SpriteFrame.Height * 0.5f);
        }

        public void pickupItem()
        {
            // add item to inventory
            inventory.addItem(this.item);

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
            if (angledirection)
            {
                angle += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (angle >= 1.5f)
                    angledirection = false;
            }
            else
            {
                angle -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (angle <= 0)
                    angledirection = true;
            }

            // Check player collision
            if (new Rectangle((int)(world.playerSprite.Position.X + world.playerSprite.SpriteFrame.Width * 0.60f), 
                (int)world.playerSprite.Position.Y,
                (int)(world.playerSprite.SpriteFrame.Width * 0.30f), 
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
