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
    public enum SkillStateWarrior
    {
        AGIup,
        ATKup,
        DEFup,
        Slash1,
        Wave,
        None
    }

    class Warrior : PlayerSprite2
    {
        Texture2D cast_animation, skill_animation;
        float previousGameTimeMsec, previousSkillTimeMsec, previousCastTimeMsec;
        private SkillStateWarrior SkillState = SkillStateWarrior.None;
        private bool SkillActive = false;
        private Vector2 animationOffset;
        private int ani_count;


        public Warrior(int _X, int _Y, Vector2 _tileSize) 
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
                            case SkillStateWarrior.Slash1:
                                Skill_SlashType1(gameTime);
                                break;
                            case SkillStateWarrior.Wave:
                                Skill_Wave(gameTime);
                                break;
                            case SkillStateWarrior.AGIup:
                                Skill_AGIup(gameTime);
                                break;
                            case SkillStateWarrior.ATKup:
                                Skill_AGIup(gameTime);
                                break;
                            case SkillStateWarrior.DEFup:
                                Skill_AGIup(gameTime);
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
                    spriteBatch.Draw(cast_animation, new Vector2(position.X - 60, position.Y - 85), Color.White * 0.75f);

            // Draw Skill Animation
            if (SkillActive && SkillSlots.Instance.active)
                if (skill_animation != null)
                {
                    if (this.spriteEffect == SpriteEffects.None)
                        spriteBatch.Draw(skill_animation,
                            new Vector2(position.X - ((skill_animation.Width - SpriteFrame.Width) * 0.5f) + animationOffset.X,
                                    position.Y - ((skill_animation.Height * 0.9f) - SpriteFrame.Height) + animationOffset.Y),
                            Color.White * 0.75f);
                    else
                        spriteBatch.Draw(skill_animation,
                            new Rectangle((int)(position.X - ((skill_animation.Width - SpriteFrame.Width) * 0.5f) + animationOffset.X),
                                          (int)(position.Y - ((skill_animation.Height * 0.9f) - SpriteFrame.Height) + animationOffset.Y),
                                          skill_animation.Width, skill_animation.Height),
                            new Rectangle(0, 0, skill_animation.Width,skill_animation.Height),
                            Color.White * 0.75f, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                }
        }

        private bool CheckKeyDown(Microsoft.Xna.Framework.Input.Keys theKey)
        {
            return Keyboard.GetState().IsKeyDown(theKey);
        }

        private void resetState(GameTime gametime)
        {
            // reset sprite frame and change state
            // start cooldown
            SkillSlots.Instance.active = false;
            spriteFrame.X = 0;
            ani_count = 0;
            SkillState = JobClasses.SkillStateWarrior.None;
            SkillActive = false;
            cast_animation = null;
            skill_animation = null;
            animationOffset = Vector2.Zero;
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
                        if (weapontype == WeaponType.One_handed_Sword ||
                            weapontype == WeaponType.Two_handed_Sword ||
                            weapontype == WeaponType.Dagger)
                        {
                            SkillSlots.Instance.active = true;
                            spriteFrame.X = 0;
                            ani_count = 1;              // reset for cast animations
                            state = EntityState.Skill;

                            if (CheckKeyDown(Keys.D1))
                            {
                                SkillState = JobClasses.SkillStateWarrior.Slash1;
                                previousCastTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 1.2f;
                            }
                            else if (CheckKeyDown(Keys.D2))
                            {
                                SkillState = JobClasses.SkillStateWarrior.Wave;
                                previousCastTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 2.1f;
                            }
                            else if (CheckKeyDown(Keys.D3))
                            {
                                SkillState = JobClasses.SkillStateWarrior.AGIup;
                                previousCastTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 2.1f;
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

        private void Skill_SlashType1(GameTime gameTime)
        {
            // cast should be completed
            if (SkillActive)
            {
                // reduce timer
                previousSkillTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (previousSkillTimeMsec <= 0)
                {
                    skill_animation = Content.Load<Texture2D>(@"gfx\skills\warrior\slash1\effect_" + ani_count.ToString());
                    previousSkillTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;

                    if (this.spriteEffect == SpriteEffects.FlipHorizontally)
                        animationOffset = new Vector2(60, 50);
                    else
                        animationOffset = new Vector2(-60, 50);

                    ani_count++;

                    if (ani_count >= 9)
                    {
                        // finish skill and reset state
                        state = EntityState.Cooldown;
                        resetState(gameTime);
                    }

                    // player sprite animation and move
                    if (ani_count <= 4)
                    {
                        spritename = "swingO1_0";
                        for (int i = 0; i < spritepath.Length; i++)
                            playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                    }
                    else if (ani_count == 5)
                    {
                        spritename = "swingO1_1";
                        for (int i = 0; i < spritepath.Length; i++)
                            playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                    }
                    else if (ani_count >= 6)
                    {
                        spritename = "swingO1_2";
                        for (int i = 0; i < spritepath.Length; i++)
                            playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                    }
                }
            }
            else
                CastAnimation(gameTime);
        }

        private void Skill_Wave(GameTime gameTime)
        {
            // cast should be completed
            if (SkillActive)
            {
                // reduce timer
                previousSkillTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (previousSkillTimeMsec <= 0)
                {
                    skill_animation = Content.Load<Texture2D>(@"gfx\skills\warrior\wave\effect_" + ani_count.ToString());
                    previousSkillTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;

                    if (this.spriteEffect == SpriteEffects.FlipHorizontally)
                        animationOffset = new Vector2(50, 0);
                    else
                        animationOffset = new Vector2(-50, 0);

                    ani_count++;

                    if (ani_count >= 8)
                    {
                        // finish skill and reset state
                        state = EntityState.Cooldown;
                        resetState(gameTime);
                    }

                    // player sprite animation and move
                    if (ani_count == 1)
                    {
                        spritename = "swingO1_0";
                        for (int i = 0; i < spritepath.Length; i++)
                            playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                    }
                    else if (ani_count == 2)
                    {
                        spritename = "swingO1_1";
                        for (int i = 0; i < spritepath.Length; i++)
                            playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                    }
                    else if (ani_count == 3)
                    {
                        spritename = "swingO1_2";
                        for (int i = 0; i < spritepath.Length; i++)
                            playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                    }
                }
            }
            else
                CastAnimation(gameTime);
        }

        private void Skill_AGIup(GameTime gameTime)
        {
            // cast should be completed
            if (SkillActive)
            {
                // reduce timer
                previousSkillTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (previousSkillTimeMsec <= 0)
                {
                    skill_animation = Content.Load<Texture2D>(@"gfx\skills\beginner\agi-up\effect_" + ani_count.ToString());
                    previousSkillTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;
                    ani_count++;

                    if (ani_count >= 11)
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
    }
}
