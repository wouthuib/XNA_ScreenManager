using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_ScreenManager.ScriptClasses
{
    [Serializable]
    public class Quest
    {
        public int questID { get; set; }
        public string questName { get; set; }
        public string questDescription { get; set; }
        public string questStatus { get; set; }
        public string questItemCount { get; set; }
        public string questMonsterCount { get; set; }
        public string questRewardType { get; set; }
        public string questRewardValue { get; set; }

        public static Quest create(int identifier, string name, string status)
        {
            var results = new Quest();

            results.questID = identifier;
            results.questName = name;
            results.questStatus = status;
            return results;
        }
    }

    public sealed class QuestStore
    {
        public List<Quest> quest_list { get; set; }

        private static QuestStore instance;
        private QuestStore()
        {
            quest_list = new List<Quest>();
        }

        public static QuestStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new QuestStore();
                }
                return instance;
            }
        }

        public void addQuest(Quest addQuest)
        {
            quest_list.Add(addQuest);
        }

        public void removeQuest(string name)
        {
            quest_list.Remove(new Quest() { questName = name });
        }

        public Quest getQuest(int ID)
        {
            return this.quest_list.Find(delegate(Quest questobj) { return questobj.questID == ID; });
        }
    }
}
