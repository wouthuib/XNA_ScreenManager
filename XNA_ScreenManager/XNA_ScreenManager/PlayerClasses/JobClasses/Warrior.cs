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
using XNA_ScreenManager.GameWorldClasses.Entities;
using XNA_ScreenManager.SkillClasses;
using XNA_ScreenManager.PlayerClasses.StatusClasses;
using System.Text.RegularExpressions;
using XNA_ScreenManager.ScriptClasses;

namespace XNA_ScreenManager.PlayerClasses.JobClasses
{

    class Warrior : PlayerSprite
    {
        Skill skill;
        Texture2D cast_animation, skill_animation;
        float previousGameTimeMsec, previousSkillTimeMsec, previousCastTimeMsec;
        private Vector2 animationOffset;
        private int ani_count;

        public Warrior(int _X, int _Y, Vector2 _tileSize) 
            : base(_X, _Y)
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
                        switch (skill.Name)
                        {
                            case "Slash Blast":
                                Skill_SlashBlast(gameTime);
                                break;
                            case "Power Strike":
                                Skill_PowerStrike(gameTime);
                                break;
                            case "Iron Body":
                                Skill_HPBoost(gameTime);
                                break;
                        }
                        break;
                    case EntityState.Stand:
                        
                        IdleState(gameTime);

                        if(ItemActive)
                            item_CoolDown(gameTime);
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

        private int KeyToInt()
        {
            if (CheckKeyDown(Keys.F1))
                return 0;
            else if (CheckKeyDown(Keys.F2))
                return 1;
            else if (CheckKeyDown(Keys.F3))
                return 2;
            else if (CheckKeyDown(Keys.F4))
                return 3;
            else if (CheckKeyDown(Keys.F5))
                return 4;
            else if (CheckKeyDown(Keys.F6))
                return 5;
            else if (CheckKeyDown(Keys.F7))
                return 6;
            else if (CheckKeyDown(Keys.F8))
                return 7;
            else if (CheckKeyDown(Keys.F9))
                return 8;

            return -1;
        }

        private WeaponType getWeaponType()
        {
            return getPlayer().equipment.item_list.Find(delegate(Item item) { return item.Type == ItemType.Weapon; }).WeaponType;
        }

        private void resetState(GameTime gametime)
        {
            // reset sprite frame and change state
            // start cooldown
            SkillSlots.Instance.active = false;
            spriteFrame.X = 0;
            ani_count = 0;
            SkillActive = false;
            ItemActive = false;
            cast_animation = null;
            skill_animation = null;
            animationOffset = Vector2.Zero;
            skill = null;
        }

        private void IdleState(GameTime gameTime)
        {
            int slot = KeyToInt();

            if (slot >= 0)
            {
                if (!SkillSlots.Instance.active)
                {                    
                    if (getPlayer().quickslotbar.Quickslot(slot) is Skill)
                    {
                        // check if weapon is equiped
                        if (getPlayer().equipment.item_list.FindAll(delegate(Item item) { return item.Type == ItemType.Weapon; }).Count > 0)
                        {
                            SkillSlots.Instance.active = true;
                            spriteFrame.X = 0;
                            ani_count = 1;              // reset for cast animations
                            state = EntityState.Skill;

                            // new skill 
                            skill = playerStore.activePlayer.quickslotbar.Quickslot(slot) as Skill;
                            previousCastTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + skill.CastTime;
                        }
                    }
                    else if (getPlayer().quickslotbar.Quickslot(slot) is Item)
                    {
                        Item selectedItem = getPlayer().quickslotbar.Quickslot(slot) as Item;

                        if(playerStore.activePlayer.inventory.item_list.FindAll(x => x.itemID == selectedItem.itemID).Count > 0)
                        {
                            if (!ItemActive)
                            {
                                ItemActive = true;
                                previousCastTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 1.2f;
                                item_Consume(gameTime, selectedItem);
                            }
                        }
                    }
                }
            }
        }

        private void item_Consume(GameTime gameTime, Item selectedItem)
        {
            if (selectedItem.Type == ItemType.Consumable)
            {
                string script = selectedItem.Script;

                // remove beginning and ending spaces

                script = Regex.Replace(script, "{", "");
                script = Regex.Replace(script, "}", "");
                script = Regex.Replace(script, " ", "");

                // call static class that handles item scripts
                // use the script interpretter to read the content

                ScriptInterpreter.Instance.loadScript(script);
                ScriptInterpreter.Instance.StartReading = true;
                ScriptInterpreter.Instance.Property = null;
                ScriptInterpreter.Instance.Values.Clear();

                ScriptInterpreter.Instance.readScript();

                // clear script, reset to begin
                ScriptInterpreter.Instance.clearInstance();
            }

            // remove the item
            playerStore.activePlayer.inventory.removeItem(selectedItem.itemID);

            // cleanup quickslot if inventory is empty
            if(playerStore.activePlayer.inventory.item_list.FindAll(x => x.itemID == selectedItem.itemID).Count == 0)
            {
                for(int i = 0; i < playerStore.activePlayer.quickslotbar.quickslot.Length; i++)
                {
                    if (playerStore.activePlayer.quickslotbar.quickslot[i] is Item)
                    {
                        Item quickslotitem = playerStore.activePlayer.quickslotbar.quickslot[i] as Item;

                        if (quickslotitem.itemID == selectedItem.itemID)
                            playerStore.activePlayer.quickslotbar.quickslot[i] = null;
                    }
                }
            }
        }

        private void item_CoolDown(GameTime gameTime)
        {
            previousCastTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (previousCastTimeMsec <= 0)
            {
                resetState(gameTime);
            }
        }

        private void CastAnimation(GameTime gameTime)
        {
            // reduce timer
            previousGameTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            previousCastTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (previousGameTimeMsec <= 0)
            {
                spritename = "stand1_0";
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

        private void Skill_SlashBlast(GameTime gameTime)
        {
            // cast should be completed
            if (SkillActive)
            {
                // reduce timer
                previousSkillTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (previousSkillTimeMsec <= 0)
                {
                    skill_animation = Content.Load<Texture2D>(@"gfx\skills\warrior\Slash Blast\effect_" + ani_count.ToString());
                    previousSkillTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.10f;

                    if (this.spriteEffect == SpriteEffects.FlipHorizontally)
                        animationOffset = new Vector2(60, 50);
                    else
                        animationOffset = new Vector2(-60, 50);

                    ani_count++;

                    // player sprite animation and move
                    switch (ani_count)
                    {
                        case 3:
                            spritename = "swingOF_0";
                            for (int i = 0; i < spritepath.Length; i++)
                                playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                            break;
                        case 4:
                            spritename = "swingOF_1";
                            for (int i = 0; i < spritepath.Length; i++)
                                playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                            break;
                        case 5:
                            spritename = "swingOF_2";
                            for (int i = 0; i < spritepath.Length; i++)
                                playerStore.activePlayer.spriteOfset[i] = getoffset(i);

                            // Reduce SP by skill
                            getPlayer().SP -= skill.MagicCost;

                            // Calculate Skill damage modifier
                            int damagePercent = 120 + (5 * skill.Level);

                            // create skill swing effect
                            if (spriteEffect == SpriteEffects.FlipHorizontally)
                            {
                                Vector2 pos = new Vector2(this.Position.X + this.SpriteFrame.Width, this.Position.Y);
                                GameWorld.GetInstance.newEffect.Add(new DamageArea(new Vector2(pos.X - 10, pos.Y), new Rectangle(0, 0, 200, 80), false, 6,
                                    (float)gameTime.ElapsedGameTime.TotalSeconds + 0.4f, damagePercent,
                                    true, @"gfx\skills\warrior\Slash Blast\hit.0_", 4));
                            }
                            else
                            {
                                Vector2 pos = new Vector2(this.Position.X, this.Position.Y);
                                GameWorld.GetInstance.newEffect.Add(new DamageArea(new Vector2(pos.X - 180, pos.Y), new Rectangle(0, 0, 200, 80), false, 6,
                                    (float)gameTime.ElapsedGameTime.TotalSeconds + 0.4f, damagePercent,
                                    true, @"gfx\skills\warrior\Slash Blast\hit.0_", 4));
                            }
                            break;
                        case 6:
                        case 7:
                        case 8:
                            // Player animation
                            if (prevspriteframe != spriteframe)
                            {
                                prevspriteframe = spriteframe;
                                for (int i = 0; i < spritepath.Length; i++)
                                {
                                    spritename = "swingOF_3";
                                    playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                                }
                            }
                            break;
                        case 9:
                            // finish skill and reset state
                            state = EntityState.Cooldown;
                            resetState(gameTime);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                // Check Correct WeaponType and current SP
                if ((getWeaponType() != WeaponType.One_handed_Sword &&
                    getWeaponType() != WeaponType.Two_handed_Sword) ||
                    getPlayer().SP < skill.MagicCost)
                {
                    // abort Skill
                    state = EntityState.Cooldown;
                    resetState(gameTime);
                }
                else
                    CastAnimation(gameTime);
            }
        }

        private void Skill_PowerStrike(GameTime gameTime)
        {
            // cast should be completed
            if (SkillActive)
            {
                // reduce timer
                previousSkillTimeMsec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (previousSkillTimeMsec <= 0)
                {
                    skill_animation = null;
                    previousSkillTimeMsec = (float)gameTime.ElapsedGameTime.TotalSeconds + 0.12f;
                    animationOffset = new Vector2(0, 50);

                    if (this.spriteEffect == SpriteEffects.FlipHorizontally)
                        animationOffset = new Vector2(0, 50);
                    else
                        animationOffset = new Vector2(0, 50);

                    ani_count++;

                    // player sprite animation and move
                    if (ani_count <= 5)
                    {
                        skill_animation = Content.Load<Texture2D>(@"gfx\skills\warrior\Power Strike\effect_" + ani_count.ToString());

                        // Player animation
                        if (prevspriteframe != spriteframe)
                        {
                            prevspriteframe = spriteframe;
                            for (int i = 0; i < spritepath.Length; i++)
                            {
                                spritename = "swingO1_0";
                                playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                            }
                        }
                    }
                    else if (ani_count == 6)
                    {
                        spritename = "swingO1_1";
                        for (int i = 0; i < spritepath.Length; i++)
                            playerStore.activePlayer.spriteOfset[i] = getoffset(i);

                        // Reduce SP by skill
                        getPlayer().SP -= skill.MagicCost;

                        // Calculate Skill damage modifier
                        int damagePercent = 180 + (5 * skill.Level);

                        // create skill swing effect
                        if (spriteEffect == SpriteEffects.FlipHorizontally)
                        {
                            Vector2 pos = new Vector2(this.Position.X + this.SpriteFrame.Width, this.Position.Y);
                            GameWorld.GetInstance.newEffect.Add(new DamageArea(new Vector2(pos.X - 10, pos.Y), new Rectangle(0, 0, 100, 80), false, 1,
                                (float)gameTime.ElapsedGameTime.TotalSeconds + 0.4f, damagePercent,
                                true, @"gfx\skills\warrior\Power Strike\hit.0_", 2));
                        }
                        else
                        {
                            Vector2 pos = new Vector2(this.Position.X, this.Position.Y);
                            GameWorld.GetInstance.newEffect.Add(new DamageArea(new Vector2(pos.X - 80, pos.Y), new Rectangle(0, 0, 100, 80), false, 1,
                                (float)gameTime.ElapsedGameTime.TotalSeconds + 0.4f, damagePercent,
                                true, @"gfx\skills\warrior\Power Strike\hit.0_", 2));
                        }
                    }
                    else if (ani_count == 7)
                    {
                        spritename = "swingO1_2";
                        for (int i = 0; i < spritepath.Length; i++)
                            playerStore.activePlayer.spriteOfset[i] = getoffset(i);
                    }
                    else if (ani_count >= 8)
                    {
                        // finish skill and reset state
                        state = EntityState.Cooldown;
                        resetState(gameTime);
                    }
                }
            }
            else
            {
                // Check Correct WeaponType and current SP
                if ((getWeaponType() != WeaponType.One_handed_Sword &&
                    getWeaponType() != WeaponType.Two_handed_Sword) ||
                    getPlayer().SP < skill.MagicCost)
                {
                    // abort Skill
                    state = EntityState.Cooldown;
                    resetState(gameTime);
                }
                else
                    CastAnimation(gameTime);
            }
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

                        // create swing effect
                        if (spriteEffect == SpriteEffects.FlipHorizontally)
                        {
                            Vector2 pos = new Vector2(this.Position.X + this.SpriteFrame.Width * 1.4f, this.Position.Y);
                            GameWorld.GetInstance.newEffect.Add(new DamageArea(new Vector2(pos.X - 40, pos.Y), new Rectangle(0, 0, 200, 80), true, 6,
                                (float)gameTime.ElapsedGameTime.TotalSeconds + 0.4f, 2));
                        }
                        else
                        {
                            Vector2 pos = new Vector2(this.Position.X, this.Position.Y);
                            GameWorld.GetInstance.newEffect.Add(new DamageArea(new Vector2(pos.X - 180, pos.Y), new Rectangle(0, 0, 200, 80), true, 6,
                                (float)gameTime.ElapsedGameTime.TotalSeconds + 0.4f, 2));
                        }
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
            {
                // Check Correct WeaponType
                if (getWeaponType() != WeaponType.One_handed_Sword &&
                    getWeaponType() != WeaponType.Two_handed_Sword)
                {
                    // Incorrect weapon equiped, abort Skill
                    state = EntityState.Cooldown;
                    resetState(gameTime);
                }
                else
                    CastAnimation(gameTime);
            }
        }

        private void Skill_HPBoost(GameTime gameTime)
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
                        // get skill level
                        int level = (int)MathHelper.Clamp(skill.Level, 1, 20);

                        // start buff with timer
                        status_list.Add(new StatusUpdateClass(skill.Name, (float)gameTime.ElapsedGameTime.TotalSeconds + 10f,
                            "b_def", level * 5, false));

                        // finish skill and reset state
                        state = EntityState.Cooldown;
                        resetState(gameTime);
                    }
                }
            }
            else
            {
                // Check current SP and exsiting buffs
                if (this.status_list.FindAll(x => x.SkillName == skill.Name).Count > 0 ||
                    getPlayer().SP < skill.MagicCost)
                {
                    // abort Skill
                    state = EntityState.Cooldown;
                    resetState(gameTime);
                }
                else
                    CastAnimation(gameTime);
            }
        }
    }
}
