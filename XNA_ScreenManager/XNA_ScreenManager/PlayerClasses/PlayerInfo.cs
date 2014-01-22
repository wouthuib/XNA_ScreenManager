using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_ScreenManager.PlayerClasses
{
    public sealed class PlayerInfo
    {
        #region properties
        private string name, gender, jobclass;
        private int maxhp, hp, maxsp, sp, exp, nlexp, lvl, gold, atk, def, matk, mdef;
        private int str, agi, vit, intel, dex, luk;
        #endregion

        #region general info
        // General Info
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        public string Gender
        {
            get { return this.gender; }
            set { this.gender = value; }
        }
        public string Jobclass
        {
            get { return this.jobclass; }
            set { this.jobclass = value; }
        }
        public int Exp
        {
            get { return this.exp; }
            set { this.exp = value; }
        }
        public int NextLevelExp
        {
            get { return this.nlexp; }
            set { this.nlexp = value; }
        }
        public int Level
        {
            get { return this.lvl; }
            set { this.lvl = value; }
        }
        public int Gold
        {
            get { return this.gold; }
            set { this.gold = value; }
        }
#endregion

        #region battleinfo
        // Battle Info
        public int Atk
        {
            get { return this.atk; }
            set { this.atk = value; }
        }
        public int MAtk
        {
            get { return this.matk; }
            set { this.matk = value; }
        }
        public int Def
        {
            get { return this.def; }
            set { this.def = value; }
        }
        public int MDef
        {
            get { return this.mdef; }
            set { this.mdef = value; }
        }
        public int Hit
        {
            get { return this.Level + this.dex + 175; }
        }
        public int Flee
        {
            get { return this.Level + this.agi + 100; }
        }
        public int Health
        {
            get { return this.hp; }
            set { this.hp = value; }
        }
        public int MaxHealth
        {
            get { return this.maxhp; }
            set { this.maxhp = value; }
        }
        public int Mana
        {
            get { return this.sp; }
            set { this.sp = value; }
        }
        public int MaxMana
        {
            get { return this.maxsp; }
            set { this.maxsp = value; }
        }
        #endregion

        #region player stats
        // Player Stats
        public int Strength
        {
            get { return this.str; }
            set { this.str = value; }
        }
        public int Agility
        {
            get { return this.agi; }
            set { this.agi = value; }
        }
        public int Vitality
        {
            get { return this.vit; }
            set { this.vit = value; }
        }
        public int Intelligence
        {
            get { return this.intel; }
            set { this.intel = value; }
        }
        public int Dexterity
        {
            get { return this.dex; }
            set { this.dex = value; }
        }
        public int Luck
        {
            get { return this.luk; }
            set { this.luk = value; }
        }
        #endregion

        #region constructor
        private static PlayerInfo instance;
        private PlayerInfo(){}

        public static PlayerInfo Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PlayerInfo();
                }
                return instance;
            }
        }
        #endregion

        // init player values
        public void InitNewGame()
        {
            this.name = "Wouter";
            this.jobclass = "Fighter";
            this.gold = 100;
            this.exp = 0;
            this.nlexp = 1200;
            this.hp = 100;
            this.maxhp = 100;
        }
    }
}
