using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNA_ScreenManager.CharacterClasses;
using XNA_ScreenManager.ItemClasses;
using XNA_ScreenManager.MapClasses;
using Microsoft.Xna.Framework.Graphics;

namespace XNA_ScreenManager.PlayerClasses.JobClasses
{
    public enum SkillState
    {
        ArrowShower,
        DoubleStrave,
        ArrowWave,
        None
    }

    public class Bowman : PlayerSprite
    {
        float previousGameTimeMsec,                                                                 // GameTime in Miliseconds
              previousEffectTimeSec;                                                                // GameTime in Miliseconds
        private SkillState SkillState = SkillState.None;
        private bool SkillActive = false;
        private Vector2 curving;
        private int arrow_count;

        public Bowman(int _X, int _Y, Vector2 _tileSize) 
            : base(_X, _Y, _tileSize)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Active)
            {
                switch (state)
                {
                    #region Skill State
                    case EntityState.Skill:
                        switch (SkillState)
                        {
                            #region ArrowShower Skill
                            case SkillState.ArrowShower:
                                Speed = 0;
                                Direction = Vector2.Zero;
                                Velocity = Vector2.Zero;

                                // Move the Character
                                OldPosition = Position;

                                // player animation
                                spriteOfset = new Vector2(spriteFrame.Width * 0, spriteFrame.Height * 4);
                                spriteFrame.Y = (int)spriteOfset.Y;

                                // reduce timer
                                previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                                if (previousGameTimeMsec < 0)
                                {
                                    spriteFrame.X += spriteWidth;

                                    if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D1))
                                    {
                                        // Later = charge arrow skill
                                        if (spriteFrame.X > spriteOfset.X + (spriteWidth * 1))
                                            spriteFrame.X = (int)spriteOfset.X + spriteWidth;
                                    }
                                    else
                                    {
                                        if (spriteFrame.X > spriteOfset.X + (spriteWidth * 2))
                                        {
                                            // make sure the world is connected
                                            if (world == null)
                                                world = GameWorld.GetInstance;

                                            int arrow_amount = 1;
                                            Vector2 curving = Vector2.Zero;

                                            if(SkillSlots.Instance.active)
                                            {
                                                arrow_amount = 5;
                                                curving = new Vector2(0, -0.3f);
                                            }

                                            for(int i = 0; i < arrow_amount; i++)
                                            {
                                                if(SkillSlots.Instance.active)
                                                    curving += new Vector2(0, 0.1f);

                                                // create and release an arrow
                                                if(spriteEffect == SpriteEffects.FlipHorizontally)
                                                    world.newEffect.Add(new Arrow(Content.Load<Texture2D>(@"gfx\gameobjects\arrow"),
                                                        new Vector2(this.Position.X, this.Position.Y + this.spriteFrame.Height * 0.6f),
                                                        800, new Vector2(1, 0), curving));
                                                else
                                                    world.newEffect.Add(new Arrow(Content.Load<Texture2D>(@"gfx\gameobjects\arrow"),
                                                        new Vector2(this.Position.X, this.Position.Y + this.spriteFrame.Height * 0.6f),
                                                        800, new Vector2(-1, 0), curving));
                                            }

                                            // Disable Skillslot
                                            SkillSlots.Instance.active = false;

                                            // Set the timer for cooldown
                                            previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + (float)((350 - playerinfo.activePlayer.ASPD * 12) * 0.0006f) + 0.05f;

                                            // reset sprite frame and change state
                                            // start cooldown
                                            spriteFrame.X = 0;
                                            state = EntityState.Cooldown;
                                            SkillState = JobClasses.SkillState.None;
                                        }
                                    }
                                }
                                break;
                            #endregion
                            #region ArrowShower Wave
                            case SkillState.ArrowWave:
                                Speed = 0;
                                Direction = Vector2.Zero;
                                Velocity = Vector2.Zero;

                                // Move the Character
                                OldPosition = Position;

                                // player animation
                                spriteOfset = new Vector2(spriteFrame.Width * 0, spriteFrame.Height * 4);
                                spriteFrame.Y = (int)spriteOfset.Y;

                                // reduce timer
                                previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                                if (previousGameTimeMsec < 0)
                                {
                                    spriteFrame.X += spriteWidth;

                                    if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D2)
                                        && !SkillActive)
                                    {
                                        // Later = charge arrow skill
                                        if (spriteFrame.X > spriteOfset.X + (spriteWidth * 1))
                                            spriteFrame.X = (int)spriteOfset.X + spriteWidth;
                                    }
                                    else
                                    {
                                        // make sure the world is connected
                                        if (world == null)
                                            world = GameWorld.GetInstance;

                                        if (!SkillActive)
                                        {
                                            curving = new Vector2(0, -0.3f);
                                            SkillActive = true;
                                            arrow_count = 10;
                                        }
                                        else
                                        {
                                            if (arrow_count > 0)
                                            {
                                                arrow_count--;

                                                curving += new Vector2(0, 0.01f);

                                                world.newEffect.Add(new Arrow(Content.Load<Texture2D>(@"gfx\gameobjects\arrow"),
                                                    new Vector2(this.Position.X, this.Position.Y + this.spriteFrame.Height * 0.6f),
                                                    800, new Vector2(-1, 0), curving));

                                                // Set the timer for cooldown
                                                previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.4f;
                                            }
                                            else
                                            {
                                                // reset sprite frame and change state
                                                // start cooldown
                                                spriteFrame.X = 0;
                                                state = EntityState.Cooldown;
                                                SkillState = JobClasses.SkillState.None;
                                            }
                                        }
                                    }
                                }
                                break;
                            #endregion
                        }
                        break;
                    #endregion
                    #region Standing State
                    case EntityState.Stand:
                        if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D1))
                        {
                            SkillSlots.Instance.active = true;

                            // check if weapon is equiped
                            if (equipment.item_list.FindAll(delegate(Item item) { return item.Type == ItemType.Weapon; }).Count > 0)
                            {
                                WeaponType weapontype = equipment.item_list.Find(delegate(Item item) { return item.Type == ItemType.Weapon; }).WeaponType;

                                // check the weapon type
                                if (weapontype == WeaponType.Bow)
                                {
                                    previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + (float)((350 - playerinfo.activePlayer.ASPD * 12) * 0.0006f) + 0.05f;

                                    spriteFrame.X = 0;
                                    state = EntityState.Skill;
                                    SkillState = JobClasses.SkillState.ArrowShower;
                                }
                            }
                        }
                        else if (keyboardStateCurrent.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D2))
                        {
                            SkillSlots.Instance.active = true;

                            // check if weapon is equiped
                            if (equipment.item_list.FindAll(delegate(Item item) { return item.Type == ItemType.Weapon; }).Count > 0)
                            {
                                WeaponType weapontype = equipment.item_list.Find(delegate(Item item) { return item.Type == ItemType.Weapon; }).WeaponType;

                                // check the weapon type
                                if (weapontype == WeaponType.Bow)
                                {
                                    if (!SkillActive)
                                    {
                                        previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + (float)((350 - playerinfo.activePlayer.ASPD * 12) * 0.0006f) + 0.05f;
                                        spriteFrame.X = 0;
                                        state = EntityState.Skill;
                                        SkillState = JobClasses.SkillState.ArrowWave;
                                    }
                                }
                            }
                        }
                        break;
                    #endregion
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
