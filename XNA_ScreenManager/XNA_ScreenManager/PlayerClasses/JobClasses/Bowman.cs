using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNA_ScreenManager.CharacterClasses;
using XNA_ScreenManager.ItemClasses;
using XNA_ScreenManager.MapClasses;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        Texture2D animation;
        float previousGameTimeMsec;                                                                 // GameTime in Miliseconds
        private SkillState SkillState = SkillState.None;
        private bool SkillActive = false;
        private Vector2 curving;
        private int arrow_count, ani_count, cast_time;

        public Bowman(int _X, int _Y, Vector2 _tileSize) 
            : base(_X, _Y, _tileSize)
        {
            animation = Content.Load<Texture2D>(@"gfx\skills\bowman\boost\effect_0");
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

                                // cast should be completed
                                if (!CastAnimation(gameTime))
                                {
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

                                                if (SkillSlots.Instance.active)
                                                {
                                                    arrow_amount = 5;
                                                    curving = new Vector2(0, -0.3f);
                                                }

                                                for (int i = 0; i < arrow_amount; i++)
                                                {
                                                    if (SkillSlots.Instance.active)
                                                        curving += new Vector2(0, 0.1f);

                                                    // create and release an arrow
                                                    if (spriteEffect == SpriteEffects.FlipHorizontally)
                                                        world.newEffect.Add(new Arrow(Content.Load<Texture2D>(@"gfx\gameobjects\arrow"),
                                                            new Vector2(this.Position.X, this.Position.Y + this.spriteFrame.Height * 0.6f),
                                                            800, new Vector2(1, 0), curving));
                                                    else
                                                        world.newEffect.Add(new Arrow(Content.Load<Texture2D>(@"gfx\gameobjects\arrow"),
                                                            new Vector2(this.Position.X, this.Position.Y + this.spriteFrame.Height * 0.6f),
                                                            800, new Vector2(-1, 0), curving));
                                                }
                                                
                                                // reset sprite frame and change state
                                                // start cooldown
                                                SkillSlots.Instance.active = false;
                                                spriteFrame.X = 0;
                                                ani_count = 0;
                                                state = EntityState.Cooldown;
                                                SkillState = JobClasses.SkillState.None;
                                                SkillActive = false;
                                            }
                                        }
                                    }
                                }
                                break;
                            #endregion
                            #region ArrowShower Wave
                            case SkillState.ArrowWave:

                                // cast should be completed
                                if (!CastAnimation(gameTime))
                                {
                                    // reduce timer
                                    previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                                    if (previousGameTimeMsec < 0)
                                    {
                                        spriteFrame.X += spriteWidth;

                                        // make sure the world is connected
                                        if (world == null)
                                            world = GameWorld.GetInstance;

                                        if (arrow_count > 0)
                                        {
                                            arrow_count--;
                                            curving += new Vector2(0, 0.05f);

                                            if (spriteEffect == SpriteEffects.FlipHorizontally)
                                                world.newEffect.Add(new Arrow(Content.Load<Texture2D>(@"gfx\gameobjects\arrow"),
                                                    new Vector2(this.Position.X, this.Position.Y + this.spriteFrame.Height * 0.6f),
                                                    800, new Vector2(1, 0), curving));
                                            else
                                                world.newEffect.Add(new Arrow(Content.Load<Texture2D>(@"gfx\gameobjects\arrow"),
                                                    new Vector2(this.Position.X, this.Position.Y + this.spriteFrame.Height * 0.6f),
                                                    800, new Vector2(-1, 0), curving));

                                            // Set the timer for cooldown
                                            previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.1f;

                                            // make the player rappidly shoot
                                            if (spriteFrame.X > spriteOfset.X + (spriteWidth * 2))
                                                spriteFrame.X = 1;
                                        }
                                        else
                                        {
                                            // reset sprite frame and change state
                                            // start cooldown
                                            SkillSlots.Instance.active = false;
                                            spriteFrame.X = 0;
                                            ani_count = 0;
                                            state = EntityState.Cooldown;
                                            SkillState = JobClasses.SkillState.None;
                                            SkillActive = false;
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
                        if (CheckKey(Keys.D1) || CheckKey(Keys.D2))
                        {
                            if (!SkillSlots.Instance.active)
                            {
                                SkillSlots.Instance.active = true;

                                // check if weapon is equiped
                                if (equipment.item_list.FindAll(delegate(Item item) { return item.Type == ItemType.Weapon; }).Count > 0)
                                {
                                    WeaponType weapontype = equipment.item_list.Find(delegate(Item item) { return item.Type == ItemType.Weapon; }).WeaponType;

                                    // check the weapon type
                                    if (weapontype == WeaponType.Bow)
                                    {
                                        spriteFrame.X = 0;
                                        ani_count = 1;
                                        state = EntityState.Skill;

                                        if (CheckKey(Keys.D1))
                                        {
                                            SkillState = JobClasses.SkillState.ArrowShower;
                                            cast_time = 8;
                                        }
                                        else if (CheckKey(Keys.D2))
                                        {
                                            SkillState = JobClasses.SkillState.ArrowWave;
                                            cast_time = 14;
                                            arrow_count = 10; // 10 arrows
                                            curving = new Vector2(0, -0.3f);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    #endregion
                    #region Hit, Recover State
                    case EntityState.Frozen:
                        // reset sprite frame and change state
                        // start cooldown
                        SkillSlots.Instance.active = false;
                        spriteFrame.X = 0;
                        ani_count = 0;
                        SkillState = JobClasses.SkillState.None;
                        SkillActive = false;
                        break;
                    #endregion
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (!SkillActive && SkillSlots.Instance.active && ani_count <= 11)
                spriteBatch.Draw(animation, new Vector2(position.X - 50, position.Y - 85), Color.White * 0.75f);
        }

        public bool CheckKey(Microsoft.Xna.Framework.Input.Keys theKey)
        {
            return Keyboard.GetState().IsKeyDown(theKey);
        }

        public bool CastAnimation(GameTime gameTime)
        {
            if (ani_count <= 11)
            {
                // reduce timer
                previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (previousGameTimeMsec <= 0)
                {
                    animation = Content.Load<Texture2D>(@"gfx\skills\general\cast\effect0_" + ani_count.ToString());
                    previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + (0.01f * cast_time);
                    ani_count++;
                }
                return true;
            }
            else
            {
                SkillActive = true;
                return false; // cast ends
            }
        }
    }
}
