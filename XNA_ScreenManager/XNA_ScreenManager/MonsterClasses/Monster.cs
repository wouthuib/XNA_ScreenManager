using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace XNA_ScreenManager.MonsterClasses
{
    [Serializable]
    public class Monster
    {
        public int monsterID { get; set; }
        public string monsterName { get; set; }
        public string monsterDescription { get; set; }
        public string monsterSprite { get; set; }

        public int defModifier { get; set; }
        public int atkModifier { get; set; }
        public int magicModifier { get; set; }
        public int speedModifier { get; set; }

        public static Monster create(
            int identifier,
            string name, string description,
            int defMod, int atkMod, int mgcMod, int spdMod)
        {
            var results = new Monster();

            results.monsterID = identifier;
            results.monsterName = name;
            results.monsterDescription = description;
            results.defModifier = defMod;
            results.atkModifier = atkMod;
            results.magicModifier = mgcMod;
            results.speedModifier = spdMod;

            return results;
        }
    }
}
