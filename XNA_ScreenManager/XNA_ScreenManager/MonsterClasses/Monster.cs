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
        public string monsterSprite { get; set; }

        public int Level { get; set; }
        public int HP { get; set; }
        public int Hit { get; set; }
        public int Flee { get; set; }
        public int DEF { get; set; }
        public int ATK { get; set; }
        public int Magic { get; set; }
        public int Speed { get; set; }

        public int drop01Item { get; set; }
        public int drop01Chance { get; set; }
        public int drop02Item { get; set; }
        public int drop02Chance { get; set; }
        public int drop03Item { get; set; }
        public int drop03Chance { get; set; }
        public int drop04Item { get; set; }
        public int drop04Chance { get; set; }
        public int drop05Item { get; set; }
        public int drop05Chance { get; set; }

        public static Monster create(int identifier, string name)
        {
            var results = new Monster();

            results.monsterID = identifier;
            results.monsterName = name;

            return results;
        }
    }
}
