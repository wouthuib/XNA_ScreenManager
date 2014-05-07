using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNA_ScreenManager.SkillClasses
{
    public enum SkillType
    {
        Offensive, Devensive, Supportive,
    };

    public enum SkillClass
    {
        Warrior, Magician, Bowman, Thief, Pirate, All
    };

    [Serializable]
    public class Skill
    {
        // General properties
        public int SkillID { get; set; }
        public string SkillName { get; set; }
        public string SkillDescription { get; set; }
        public string SkillIconSpritePath { get; set; }

        // Casting properties
        public int MagicCost { get; set; }
        public float CastTime { get; set; }
        public float CooldownTime { get; set; }

        // Animation properties
        public string launchSpritePath { get; set; }
        public int launchAnimationCount { get; set; }
        public string bulletSpritePath { get; set; }
        public int bulletAnimationCount { get; set; }
        public string hitSpritePath { get; set; }
        public int hitAnimationCount { get; set; }

        // Offensive skill info
        public int BulletCount { get; set; }
        public float BulletAngle { get; set; }
        public float BulletSpeed { get; set; }
        public float StaticDamage { get; set; } // e.g. fixed damage = 1500
        public float DynamicDamage { get; set; } // e.g. 1500 * lvl * 0.75f

        // Skill unlocks other skills in skill tree
        public int MaxLevel { get; set; }
        public string[] UnlockSkill { get; set; }
        public int[] UnlockLevel { get; set; }

        public SkillType Type { get; set; }
        public SkillClass Class { get; set; }

        public static Skill create(int identifier, string name, SkillType type)
        {
            var results = new Skill();

            results.SkillID = identifier;
            results.SkillName = name;
            results.Type = type;

            return results;
        }
    }
}
