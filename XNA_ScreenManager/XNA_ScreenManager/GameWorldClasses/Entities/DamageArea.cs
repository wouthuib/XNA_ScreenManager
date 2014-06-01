using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNA_ScreenManager.CharacterClasses;
using XNA_ScreenManager.MapClasses;
using Microsoft.Xna.Framework.Graphics;
using XNA_ScreenManager.ScreenClasses.MainClasses;
using XNA_ScreenManager.PlayerClasses;
using XNA_ScreenManager.MonsterClasses;
using XNA_ScreenManager.GameWorldClasses.Effects;

namespace XNA_ScreenManager.GameWorldClasses.Entities
{
    class DamageArea : XNA_ScreenManager.MapClasses.Effect
    {
        #region properties

        Entity Owner;
        GraphicsDevice gfxdevice = ResourceManager.GetInstance.gfxdevice;
        Rectangle Area = Rectangle.Empty;
        float DamagePercent;
        bool Permanent = false;
        private bool debug = false;

        // skill hit sprite properties
        private bool hiteffect = false;
        private string hitsprpath = null;
        private int hitsprframes = 0;

        #endregion

        public DamageArea(
            Entity owner, 
            Vector2 position, 
            Rectangle area,
            bool permanent,
            float timer, 
            float dmgpercent,
            bool gethiteffect = false,
            string gethitsprpath = null,
            int gethitsprframes = 0) : base()
        {
            this.Owner = owner;
            this.Area = area;
            this.Position = position;
            this.SpriteFrame = new Rectangle(0, 0, area.Width, area.Height);
            this.DamagePercent = dmgpercent;
            this.Permanent = permanent;

            transperant = 0.5f;
            keepAliveTimer = timer;

            if (gethiteffect)
            {
                this.hiteffect = true;
                this.hitsprpath = gethitsprpath;
                this.hitsprframes = gethitsprframes;
            }
        }

        public override void Update(GameTime gameTime)
        {
            // check for monster collisions
            foreach (Entity entity in GameWorld.GetInstance.listEntity)
            {
                if (entity.EntityType == EntityType.Monster)
                {
                    MonsterSprite monster = (MonsterSprite)entity;

                    if (monster.SpriteBoundries.
                    Intersects(new Rectangle(
                        (int)Position.X, 
                        (int)Position.Y,
                        (int)SpriteFrame.Width, 
                        (int)SpriteFrame.Height)) == true)
                    {
                        if (monster.State != EntityState.Hit && 
                            monster.State != EntityState.Died && 
                            monster.State != EntityState.Spawn)
                        {
                            // remove effect if not permanent
                            if(Permanent == false)
                                KeepAliveTimer = 0;

                            // Start damage controll
                            int damage = (int)Battle.battle_calc_damage(PlayerStore.Instance.activePlayer, (MonsterSprite)monster, DamagePercent);
                            monster.HP -= damage;

                            // start skill hit effect
                            if (this.hiteffect)
                                 GameWorld.GetInstance.newEffect.Add(new WeaponHitEffect(this.hitsprpath, monster.Position, this.hitsprframes));

                            // create damage balloon
                            GameWorld.GetInstance.newEffect.Add(new DamageBaloon(
                                    ResourceManager.GetInstance.Content.Load<Texture2D>(@"gfx\effects\damage_counter1"),
                                    new Vector2((monster.Position.X + monster.SpriteFrame.Width * 0.45f) - damage.ToString().Length * 5,
                                    monster.Position.Y + monster.SpriteFrame.Height * 0.20f), damage));

                            monster.State = EntityState.Hit;
                        }
                    }
                }
            }

            // base Effect Update
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.debug)
            {
                Texture2D rect = new Texture2D(gfxdevice, (int)Math.Abs(Area.Width), (int)Area.Height);

                Color[] data = new Color[(int)Math.Abs(Area.Width) * (int)Area.Height];
                for (int i = 0; i < data.Length; ++i) data[i] = Color.Red;
                rect.SetData(data);

                spriteBatch.Draw(rect, new Rectangle((int)Position.X, (int)Position.Y,
                         (int)Math.Abs(Area.Width), (int)Area.Height),
                         SpriteFrame, Color.White * transperant, angle, origin, sprite_effect, 0f);
            }
        }
    }
}
