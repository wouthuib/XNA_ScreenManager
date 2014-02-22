using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace XNA_ScreenManager.MonsterClasses
{
    public sealed class MonsterStore
    {
        public List<Monster> monster_list { get; set; }

        private static MonsterStore instance;
        private MonsterStore()
        {
            monster_list = new List<Monster>();
        }

        public static MonsterStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MonsterStore();
                }
                return instance;
            }
        }

        public void addMonster(Monster addItem)
        {
            monster_list.Add(addItem);
        }

        public void removeMonster(string name)
        {
            monster_list.Remove(new Monster() { monsterName = name });
        }

        public Monster getMonster(int ID)
        {
            return this.monster_list.Find(delegate(Monster mob) { return mob.monsterID == ID; });
        }

        public void loadMonster(string dir, string file)
        {
            using (var reader = new StreamReader(Path.Combine(dir, file)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    try
                    {
                        if (values[0] != "atkModifier")
                        {
                            // to do
                        }
                    }
                    catch (Exception ee)
                    {
                        string exception = ee.ToString();
                    }
                }
            }
        }

        public void saveItem(string dir, string file)
        {
            Type itemType = typeof(Monster);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(p => p.Name);

            using (var writer = new StreamWriter(Path.Combine(dir, file)))
            {
                writer.WriteLine(string.Join("; ", props.Select(p => p.Name)));

                foreach (var monster in monster_list)
                {
                    foreach (PropertyInfo propertyInfo in monster.GetType().GetProperties())
                    {
                        if (propertyInfo.Name != "itemID")
                            writer.Write("; ");

                        string value;

                        if (propertyInfo.Name == "itemSpritePath" || propertyInfo.Name == "equipSpritePath" ||
                            propertyInfo.Name == "itemDescription")
                        {
                            value = "\"" + propertyInfo.GetValue(monster, null) + "\"";
                            value = Regex.Replace(value, @"\t|\r|\n", "");
                        }
                        else
                        {
                            var getvalue = propertyInfo.GetValue(monster, null);
                            value = getvalue.ToString();
                        }

                        writer.Write(value);
                    }

                    writer.WriteLine(";");
                }
            }
        }
    }
}
