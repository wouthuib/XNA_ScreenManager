using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

        public void loadMonster(string file)
        {
            string dir = @"c:\Temp";
            string serializationFile = Path.Combine(dir, file);

            //deserialize
            using (Stream stream = File.Open(serializationFile, FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                monster_list = (List<Monster>)bformatter.Deserialize(stream);
            }
        }

        public void saveMonster(string file)
        {
            string dir = @"c:\Temp";
            string serializationFile = Path.Combine(dir, file);

            //serialize
            using (Stream stream = File.Open(serializationFile, FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                bformatter.Serialize(stream, monster_list);
            }
        }
    }
}
