using XNA_ScreenManager.CharacterClasses;
using XNA_ScreenManager.PlayerClasses;
using XNA_ScreenManager.ItemClasses;
using XNA_ScreenManager.ScreenClasses.MainClasses;

namespace XNA_ScreenManager.MapClasses
{
    // static battle calculation class
    public static class Battle
    {
        // player hits monster
        public static int battle_calc_damage(PlayerInfo playerinfo, MonsterSprite monsterinfo, float percent)
        {
            ResourceManager.randomizer Randomizer = ResourceManager.randomizer.Instance;
            Item Weapon = playerinfo.equipment.getEquip(ItemSlot.Weapon);

            int finalDamage = 0, 
                bDamage = 0, 
                wDamage = 0;

            int dodgerate = 100 - (playerinfo.HIT - monsterinfo.FLEE);

            if (Randomizer.generateRandom(0, 100) >= dodgerate)
            {
                bDamage = (playerinfo.ATK * 2) - monsterinfo.DEF;
                wDamage = playerinfo.WeaponATK - monsterinfo.DEF;
                finalDamage = (int)((bDamage + wDamage) * (percent * 0.01f) * (WeaponPenalty(Weapon, monsterinfo) * 0.01f));
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
        public static int battle_calc_damage_mob(Entity monsterinfo, PlayerInfo playerinfo)
        {
            ResourceManager.randomizer Randomizer = ResourceManager.randomizer.Instance;

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

        // size penalty weapons
        public static int WeaponPenalty(Item weapon, MonsterSprite monsterinfo)
        {
            switch (weapon.WeaponType)
            {
                case WeaponType.Bow:
                    if (monsterinfo.SIZE == "small")
                        return 75;
                    if (monsterinfo.SIZE == "medium")
                        return 100;
                    if (monsterinfo.SIZE == "big")
                        return 75;
                    break;
                case WeaponType.Dagger:
                    if (monsterinfo.SIZE == "small")
                        return 100;
                    if (monsterinfo.SIZE == "medium")
                        return 75;
                    if (monsterinfo.SIZE == "big")
                        return 50;
                    break;
                case WeaponType.Mace:
                    if (monsterinfo.SIZE == "small")
                        return 75;
                    if (monsterinfo.SIZE == "medium")
                        return 100;
                    if (monsterinfo.SIZE == "big")
                        return 100;
                    break;
                case WeaponType.One_handed_Axe:
                    if (monsterinfo.SIZE == "small")
                        return 50;
                    if (monsterinfo.SIZE == "medium")
                        return 75;
                    if (monsterinfo.SIZE == "big")
                        return 100;
                    break;
                case WeaponType.One_handed_Spear:
                    if (monsterinfo.SIZE == "small")
                        return 75;
                    if (monsterinfo.SIZE == "medium")
                        return 75;
                    if (monsterinfo.SIZE == "big")
                        return 100;
                    break;
                case WeaponType.One_handed_Sword:
                    if (monsterinfo.SIZE == "small")
                        return 75;
                    if (monsterinfo.SIZE == "medium")
                        return 100;
                    if (monsterinfo.SIZE == "big")
                        return 75;
                    break;
                case WeaponType.Staff:
                    if (monsterinfo.SIZE == "small")
                        return 100;
                    if (monsterinfo.SIZE == "medium")
                        return 100;
                    if (monsterinfo.SIZE == "big")
                        return 100;
                    break;
                case WeaponType.Two_handed_Axe:
                    if (monsterinfo.SIZE == "small")
                        return 50;
                    if (monsterinfo.SIZE == "medium")
                        return 75;
                    if (monsterinfo.SIZE == "big")
                        return 100;
                    break;
                case WeaponType.Two_handed_Spear:
                    if (monsterinfo.SIZE == "small")
                        return 75;
                    if (monsterinfo.SIZE == "medium")
                        return 75;
                    if (monsterinfo.SIZE == "big")
                        return 100;
                    break;
                case WeaponType.Two_handed_Sword:
                    if (monsterinfo.SIZE == "small")
                        return 75;
                    if (monsterinfo.SIZE == "medium")
                        return 75;
                    if (monsterinfo.SIZE == "big")
                        return 100;
                    break;
            }

            return 100;
        }
    }
}
