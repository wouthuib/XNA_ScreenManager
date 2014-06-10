using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA_ScreenManager.CharacterClasses;
using Microsoft.Xna.Framework;
using XNA_ScreenManager.PlayerClasses.StatusClasses;

namespace XNA_ScreenManager.PlayerClasses
{
    public class PlayerRoutines : Entity
    {
        // Static PlayerStore
        protected PlayerStore playerStore = PlayerStore.Instance;
        protected List<StatusUpdateClass> status_list = new List<StatusUpdateClass>();

        // Timers
        float previousRecoveryTime;

        public PlayerRoutines()
            : base()
        {
        }

        public override void Update(GameTime gameTime)
        {
            RecoverHealth(gameTime);
            ClearStatusUpdates(gameTime);
        }

        private void ClearStatusUpdates(GameTime gameTime)
        {
            for (int i = 0; i < status_list.Count; i++ )
            {
                if (status_list[i].Remove)
                    status_list.Remove(status_list[i]);
            }
        }

        private void RecoverHealth(GameTime gameTime)
        {
            previousRecoveryTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (previousRecoveryTime <= 0)
            {
                previousRecoveryTime = (float)gameTime.ElapsedGameTime.TotalSeconds + 8;

                getPlayer().HP += (int)(getPlayer().MAXHP * 0.05f);
                getPlayer().SP += (int)(getPlayer().MAXSP * 0.05f);

                if (getPlayer().HP >= getPlayer().MAXHP)
                    getPlayer().HP = getPlayer().MAXHP;
                if (getPlayer().SP >= getPlayer().MAXSP)
                    getPlayer().SP = getPlayer().MAXSP;
            }
        }

        #region bound new player
        protected PlayerInfo getPlayer()
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

        public PlayerInfo Player { get; set; }
        #endregion
    }
}
