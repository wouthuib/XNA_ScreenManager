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
        // player hits monster
        public static int battle_calc_damage(PlayerInfo playerinfo, MonsterSprite monsterinfo)
        {
            randomizer Randomizer = randomizer.Instance;

            int finalDamage = 0, bDamage = 0, wDamage = 0;

            int dodgerate = 100 - (playerinfo.HIT - monsterinfo.FLEE);

            if (Randomizer.generateRandom(0, 100) >= dodgerate)
            {
                bDamage = (playerinfo.ATK * 2) - monsterinfo.DEF;
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

        // monster hits player
        public static int battle_calc_damage_mob(MonsterSprite monsterinfo, PlayerInfo playerinfo)
        {
            randomizer Randomizer = randomizer.Instance;

            int finalDamage = 0;

            int dodgerate = 100 - (monsterinfo.HIT - playerinfo.FLEE);

            if (Randomizer.generateRandom(0, 100) >= dodgerate)
            {
                finalDamage = (monsterinfo.ATK * 2) - playerinfo.DEF;
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
