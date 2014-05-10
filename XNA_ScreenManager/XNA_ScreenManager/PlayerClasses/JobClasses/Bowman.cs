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
    public enum SkillStateBowman
    {
        ArrowShower,
        DoubleStrave,
        ArrowWave,
        ImprovedFocus,
        None
    }

    public class Bowman : PlayerSprite
    {
        Texture2D cast_animation, skill_animation;
        float previousGameTimeMsec, previousSkillTimeMsec, previousCastTimeMsec;
        private SkillStateBowman SkillState = SkillStateBowman.None;
        private bool SkillActive = false;
        private Vector2 curving;
        private int arrow_count, ani_count;

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
                    case EntityState.Skill:
                        switch (SkillState)
                        {
                            case SkillStateBowman.ArrowShower:
                                ArrowShower(gameTime);
                                break;
                            case SkillStateBowman.ArrowWave:
                                ArrowWave(gameTime);
                                break;
                            case SkillStateBowman.ImprovedFocus:
                                ImprovedFocus(gameTime);
                                break;
                        }
                        break;
                    case EntityState.Stand:
                        IdleState(gameTime);
                        break;
                    case EntityState.Frozen:
                        resetState(gameTime);
                        break;
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Draw Cast Animation
            if (!SkillActive && SkillSlots.Instance.active && ani_count <= 11)
                if (cast_animation != null)
                    spriteBatch.Draw(cast_animation, new Vector2(position.X - 50, position.Y - 85), Color.White * 0.75f);

            // Draw Skill Animation
            if (SkillActive && SkillSlots.Instance.active)
                if (skill_animation != null)
                    spriteBatch.Draw(skill_animation,
                        new Vector2(position.X - (skill_animation.Width - spriteFrame.Width) * 0.5f,
                                    position.Y - ((skill_animation.Height * 0.9f) - spriteFrame.Height)), 
                        Color.White * 0.75f);
        }

        private bool CheckKeyDown(Microsoft.Xna.Framework.Input.Keys theKey)
        {
            return Keyboard.GetState().IsKeyDown(theKey);
        }

        #region States and Skills

        private void resetState(GameTime gametime)
        {
            // reset sprite frame and change state
            // start cooldown
            SkillSlots.Instance.active = false;
            spriteFrame.X = 0;
            ani_count = 0;
            SkillState = JobClasses.SkillStateBowman.None;
            SkillActive = false;
            cast_animation = null;
            skill_animation = null;
        }

        private void IdleState(GameTime gameTime)
        {
            if (CheckKeyDown(Keys.D1) || 
                CheckKeyDown(Keys.D2) ||  
                CheckKeyDown(Keys.D3) ||  
                CheckKeyDown(Keys.D4) ||  
                CheckKeyDown(Keys.D5) ||  
                CheckKeyDown(Keys.D6) ||  
                CheckKeyDown(Keys.D7) ||
                CheckKeyDown(Keys.D8) ||   
                CheckKeyDown(Keys.D9)
                )
            {
                if (!SkillSlots.Instance.active)
                {
                    // check if weapon is equiped
                    if (equipment.item_list.FindAll(delegate(Item item) { return item.Type == ItemType.Weapon; }).Count > 0)
                    {
                        WeaponType weapontype = equipment.item_list.Find(delegate(Item item) { return item.Type == ItemType.Weapon; }).WeaponType;

                        // check the weapon type
                        if (weapontype == WeaponType.Bow)
                        {
                            SkillSlots.Instance.active = true;
                            spriteFrame.X = 0;
                            ani_count = 1;              // reset for cast animations
                            state = EntityState.Skill;

                            if (CheckKeyDown(Keys.D1))
                            {
                                SkillState = JobClasses.SkillStateBowman.ArrowShower;
                                previousCastTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 1.2f;
                            }
                            else if (CheckKeyDown(Keys.D2))
                            {
                                SkillState = JobClasses.SkillStateBowman.ArrowWave;
                                previousCastTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 2.1f;

                                // needs to be stored in skill info:
                                arrow_count = 10; // 10 arrows
                                curving = new Vector2(0, -0.3f);
                            }
                            else if (CheckKeyDown(Keys.D3))
                            {
                                SkillState = JobClasses.SkillStateBowman.ImprovedFocus;
                                previousCastTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 2.1f;
                                previousSkillTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                        }
                    }
                }
            }
        }

        private void CastAnimation(GameTime gameTime)
        {
            // reduce timer
            previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            previousCastTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (previousGameTimeMsec <= 0)
            {
                cast_animation = Content.Load<Texture2D>(@"gfx\skills\general\cast\effect0_" + ani_count.ToString());
                previousGameTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
                ani_count++;

                if (ani_count >= 11)
                    ani_count = 1;
            }

            if (previousCastTimeMsec <= 0)
            {
                SkillActive = true;
                ani_count = 0;      // reset for skill animations
            }
        }

        private void ArrowShower(GameTime gameTime)
        {
            // cast should be completed
            if (SkillActive)
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

                            // Skill is finished reset state
                            state = EntityState.Cooldown;
                            resetState(gameTime);
                        }
                    }
                }
            }
            else
                CastAnimation(gameTime);
        }

        private void ArrowWave(GameTime gameTime)
        {
            // cast should be completed
            if (SkillActive)
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
                        state = EntityState.Cooldown;
                        resetState(gameTime);
                    }
                }
            }
            else
                CastAnimation(gameTime);
        }

        private void ImprovedFocus(GameTime gameTime)
        {
            // cast should be completed
            if (SkillActive)
            {
                // reduce timer
                previousSkillTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (previousSkillTimeMsec <= 0)
                {
                    skill_animation = Content.Load<Texture2D>(@"gfx\skills\bowman\boost\effect_" + ani_count.ToString());
                    previousSkillTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
                    ani_count++;

                    if (ani_count >= 9)
                    {
                        // start buff with timer

                        // finish skill and reset state
                        state = EntityState.Cooldown;
                        resetState(gameTime);
                    }
                }
            }
            else
                CastAnimation(gameTime);
        }

        #endregion
    }
}
