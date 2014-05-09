using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using XNA_ScreenManager.ItemClasses;

namespace XNA_ScreenManager.SkillClasses
{
    public sealed class SkillStore
    {
        public List<Skill> skill_list { get; set; }

        private static SkillStore instance;
        private SkillStore()
        {
            skill_list = new List<Skill>();
        }

        public static SkillStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SkillStore();
                }
                return instance;
            }
        }

        public void addSkill(Skill addSkill)
        {
            skill_list.Add(addSkill);
        }

        public void removeSkill(string name)
        {
            skill_list.Remove(new Skill() { SkillName = name });

        }

        public Skill getSkill(int ID)
        {
            return this.skill_list.Find(delegate(Skill skill) { return skill.SkillID == ID; });
        }

        public Skill getSkill(string Name)
        {
            return this.skill_list.Find(delegate(Skill skill) { return skill.SkillName == Name; });
        }

        public bool hasSkillRequirement(int ID)
        {
            for (int i = 0; i < 4; i++)
                if (skill_list.Find(delegate(Skill skill) { return skill.SkillID == ID; }).UnlockSkill[i] != null)
                    return true;
            // no requirements found
            return false;
        }

        public void loadSkills(string dir, string file)
        {
            using (var reader = new StreamReader(Path.Combine(dir, file)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    try
                    {
                        if (values[0] != "ID[0]")
                        {
                            // create new skill in skilllist
                            this.addSkill(Skill.create(Convert.ToInt32(values[0]), TrimParameter(values[1]), (SkillType)Enum.Parse(typeof(SkillType), values[27])));

                            // Link new Skill object to Skill database
                            Skill skill = this.getSkill(Convert.ToInt32(values[0]));

                            // Fill in the Skill properties if applicable
                            skill.SkillDescription = TrimParameter(values[2]);
                            skill.SkillIconSpritePath = StringtoPath(values[3]);

                            // magic casting properties
                            skill.MagicCost = Convert.ToInt32(values[4]);
                            skill.CastTime = Convert.ToInt32(values[5]) * 0.01f;
                            skill.CooldownTime = Convert.ToInt32(values[6]) * 0.01f;

                            // Check the animation counts and sprites
                            if (Convert.ToInt32(values[7]) > 0)
                            {
                                skill.launchAnimationCount = Convert.ToInt32(values[7]);
                                skill.launchSpritePath = StringtoPath(values[8]);
                            }
                            if (Convert.ToInt32(values[9]) > 0)
                            {
                                skill.bulletAnimationCount = Convert.ToInt32(values[9]);
                                skill.bulletSpritePath = StringtoPath(values[10]);
                            }
                            if (Convert.ToInt32(values[11]) > 0)
                            {
                                skill.hitAnimationCount = Convert.ToInt32(values[11]);
                                skill.hitSpritePath = StringtoPath(values[12]);
                            }

                            // Offensive skill
                            skill.BulletCount = Convert.ToInt32(values[13]);
                            skill.BulletAngle = (float)(Convert.ToInt32(values[14]) * 0.01f);
                            skill.BulletSpeed = Convert.ToInt32(values[15]);
                            skill.StaticDamage = (float)(Convert.ToInt32(values[16]) * 0.01f);
                            skill.DynamicDamage = (float)(Convert.ToInt32(values[17]) * 0.01f);

                            // Skill Level properties
                            skill.MaxLevel = Convert.ToInt32(values[18]);
                            skill.SkillTreeColumn = Convert.ToInt32(values[19]); 

                            for (int i = 0; i < 4; i++)
                            {
                                if (TrimParameter(values[20 + (i * 2)]) != "")
                                {
                                    skill.UnlockSkill[i] = TrimParameter(values[20 + (i * 2)]);
                                    skill.UnlockLevel[i] = Convert.ToInt32(values[21 + (i * 2)]);
                                }
                            }

                            skill.Type = (SkillType)Enum.Parse(typeof(SkillType), values[28]);
                            skill.Class = (SkillClass)Enum.Parse(typeof(SkillClass), values[29]);

                        }
                    }
                    catch (Exception ee)
                    {
                        string exception = ee.ToString();
                    }
                }
            }
        }

        public string StringtoPath(string path)
        {
            path = Regex.Replace(path, "\"", "");
            path = Regex.Replace(path, " ", "");
            path = Regex.Replace(path, @"\t|\n|\r", "");

            return @"" + path;
        }

        public string TrimParameter(string param)
        {
            param = Regex.Replace(param, "\"", "");
            param = Regex.Replace(param, @"\t|\n|\r", "");

            return param;
        }

    }
}
