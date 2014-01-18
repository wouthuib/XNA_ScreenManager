using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_ScreenManager.PlayerClasses
{
    public sealed class PlayerInfo
    {
        private string NAME, GENDER, CLASS;
        private int MAXHP, HP, MAXSP, SP, EXP, NLEXP, LVL, Gold, ATK, DEF, MATK, MDEF;
        private int STR, AGI, VIT, INT, DEX, LUK;

        // General Info
        public string name
        {
            get { return this.NAME; }
            set { this.NAME = value; }
        }
        public string gender
        {
            get { return this.GENDER; }
            set { this.GENDER = value; }
        }
        public string job
        {
            get { return this.CLASS; }
            set { this.CLASS = value; }
        }
        public int exp
        {
            get { return this.EXP; }
            set { this.EXP = value; }
        }
        public int nextlevelexp
        {
            get { return this.NLEXP; }
            set { this.NLEXP = value; }
        }
        public int level
        {
            get { return this.LVL; }
            set { this.LVL = value; }
        }
        public int gold
        {
            get { return this.Gold; }
            set { this.Gold = value; }
        }

        // Battle Info
        public int atk
        {
            get { return this.ATK; }
            set { this.ATK = value; }
        }
        public int matk
        {
            get { return this.MATK; }
            set { this.MATK = value; }
        }
        public int def
        {
            get { return this.DEF; }
            set { this.DEF = value; }
        }
        public int mdef
        {
            get { return this.MDEF; }
            set { this.MDEF = value; }
        }
        public int health
        {
            get { return this.HP; }
            set { this.HP = value; }
        }
        public int maxhealth
        {
            get { return this.MAXHP; }
            set { this.MAXHP = value; }
        }
        public int mana
        {
            get { return this.SP; }
            set { this.SP = value; }
        }
        public int maxmana
        {
            get { return this.MAXSP; }
            set { this.MAXSP = value; }
        }
        
        // Player Stats
        public int strength
        {
            get { return this.STR; }
            set { this.STR = value; }
        }
        public int agility
        {
            get { return this.AGI; }
            set { this.AGI = value; }
        }
        public int vitality
        {
            get { return this.VIT; }
            set { this.VIT = value; }
        }
        public int intelligence
        {
            get { return this.INT; }
            set { this.INT = value; }
        }
        public int dexterity
        {
            get { return this.DEX; }
            set { this.DEX = value; }
        }
        public int luck
        {
            get { return this.LUK; }
            set { this.LUK = value; }
        }

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

        // init player values
        public void InitNewGame()
        {
            this.name = "Wouter";
            this.job = "Fighter";
            this.gold = 100;
            this.exp = 0;
            this.nextlevelexp = 1200;
            this.health = 100;
            this.maxhealth = 100;
        }
    }
}
