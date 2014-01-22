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

            int finalDamage = 0, bDamage = 0, wDamage = 0;

            int dodgerate = 100 - (playerinfo.Hit - monsterinfo.FLEE);

            if (Randomizer.generateRandom(0, 100) >= dodgerate)
            {
                bDamage = (playerinfo.Atk * 2) - monsterinfo.DEF;
                wDamage = playerinfo.WeaponATK - monsterinfo.DEF;
                finalDamage = bDamage + wDamage;
            }
            else
                finalDamage = 0;

            // return damage
            if (finalDamage < 0)
                return 0;
            else
                return finalDamage;
        }
    }
}
