using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA_ScreenManager.CharacterClasses;
using XNA_ScreenManager.PlayerClasses;

namespace XNA_ScreenManager.MapClasses
{
    // static battle calculation class
    public static class Battle
    {
        public static int battle_calc_damage(PlayerInfo playerinfo, Monster monsterinfo)
        {
            randomizer Randomizer = randomizer.Instance;

            int damage = 0,
                hit = Randomizer.generateRandom(0, playerinfo.Hit),
                flee = Randomizer.generateRandom(0, monsterinfo.FLEE),
                atk = Randomizer.generateRandom(0, playerinfo.Atk),
                def = Randomizer.generateRandom(0, monsterinfo.DEF);

            if (hit >= flee && hit - flee > 0)
            {
                damage = (int)(atk - def);
            }
            else
                damage = 0;

            // return damage
            if (damage < 0)
                return 0;
            else
                return damage;
        }
    }
}
