using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA_ScreenManager.CharacterClasses;
using Microsoft.Xna.Framework;
using XNA_ScreenManager.PlayerClasses.StatusClasses;
using XNA_ScreenManager.MapClasses;
using XNA_ScreenManager.GameWorldClasses.Effects;

namespace XNA_ScreenManager.PlayerClasses
{
    public class PlayerRoutines : Entity
    {
        // Static PlayerStore
        protected PlayerStore playerStore = PlayerStore.Instance;
        protected List<StatusUpdateClass> status_list = new List<StatusUpdateClass>();
        public bool SkillActive = false, ItemActive = false;

        // Timers
        float previousRecoveryTime;

        public PlayerRoutines()
            : base()
        {
        }

        public override void Update(GameTime gameTime)
        {
            RecoverHealth(gameTime);
            StatusUpdates(gameTime);
            
            // read playerinfo exp for level up
            if (getPlayer().Exp >= getPlayer().NextLevelExp)
                PlayerLevelUp();
        }

        private void StatusUpdates(GameTime gameTime)
        {
            // remove expired status updates
            for (int i = 0; i < status_list.Count; i++ )
                if (status_list[i].Remove)
                    status_list.Remove(status_list[i]);

            // update remaining status updates
            foreach (var status in status_list)
                status.Update(gameTime);
        }

        private void RecoverHealth(GameTime gameTime)
        {
            previousRecoveryTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (previousRecoveryTime <= 0)
            {
                previousRecoveryTime = (float)gameTime.ElapsedGameTime.TotalSeconds + 8;

                getPlayer().HP += (int)(getPlayer().MAXHP * 0.05f);
                getPlayer().SP += (int)(getPlayer().MAXSP * 0.05f);
            }

            if (getPlayer().HP >= getPlayer().MAXHP)
                getPlayer().HP = getPlayer().MAXHP;
            if (getPlayer().SP >= getPlayer().MAXSP)
                getPlayer().SP = getPlayer().MAXSP;
        }

        private void PlayerLevelUp()
        {
            getPlayer().Level++;
            getPlayer().Skillpoints++;
            getPlayer().Statpoints++;
            getPlayer().Exp -= getPlayer().NextLevelExp;
            getPlayer().NextLevelExp = (int)(getPlayer().Level ^ 4 + (1000 * getPlayer().Level));
            GameWorld.GetInstance.listEffect.Add(new CastEffect(@"gfx\effects\LevelUp\LevelUp_", 20, new Vector2(-76, -170), Vector2.Zero, getPlayer().Name, null));
        }

        #region bound new player
        protected virtual PlayerInfo getPlayer()
        {
            // check which player is bound
            if (this.Player == null)
                if (playerStore.activePlayer != null)
                    return playerStore.activePlayer;
                else
                    return new PlayerInfo();
            else
                return this.Player;
        }

        // for chatlog
        public virtual string PlayerName
        {
            get { return getPlayer().Name; }
        }

        public PlayerInfo Player { get; set; }
        #endregion
    }
}
