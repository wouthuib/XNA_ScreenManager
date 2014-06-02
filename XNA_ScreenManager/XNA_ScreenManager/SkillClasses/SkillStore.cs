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
            skill_list.Remove(new Skill() { Name = name });

        }

        public Skill getSkill(int ID)
        {
            return this.skill_list.Find(delegate(Skill skill) { return skill.ID == ID; });
        }

        public Skill getSkill(string Name)
        {
            return this.skill_list.Find(delegate(Skill skill) { return skill.Name == Name; });
        }

        public bool hasSkillRequirement(int ID)
        {
            for (int i = 0; i < 4; i++)
                if (skill_list.Find(delegate(Skill skill) { return skill.ID == ID; }).UnlockSkill[i] != null)
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
                            this.addSkill(Skill.create(Convert.ToInt32(values[0]), TrimParameter(values[1]), (SkillType)Enum.Parse(typeof(SkillType), values[17])));

                            // Link new Skill object to Skill database
                            Skill skill = this.getSkill(Convert.ToInt32(values[0]));

                            // Fill in the Skill properties if applicable
                            skill.Description = TrimParameter(values[2]);
                            skill.IconSpritePath = StringtoPath(values[3]);

                            // magic casting properties
                            skill.MagicCost = Convert.ToInt32(values[4]);
                            skill.CastTime = Convert.ToInt32(values[5]) * 0.01f;
                            skill.CooldownTime = Convert.ToInt32(values[6]) * 0.01f;

                            // Skill Level properties
                            skill.MaxLevel = Convert.ToInt32(values[7]);
                            skill.SkillTreeColumn = Convert.ToInt32(values[8]); 

                            // Skill Requirement properties
                            for (int i = 0; i < 4; i++)
                            {
                                if (TrimParameter(values[9 + (i * 2)]) != "")
                                {
                                    skill.UnlockSkill[i] = TrimParameter(values[9 + (i * 2)]);
                                    skill.UnlockLevel[i] = Convert.ToInt32(values[10 + (i * 2)]);
                                }
                            }

                            skill.Type = (SkillType)Enum.Parse(typeof(SkillType), values[17]);
                            skill.Class = (SkillClass)Enum.Parse(typeof(SkillClass), values[18]);

                        }
                    }
                    catch
                    {
                        throw new Exception("Error loading skilltable, please check the entries!");
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
