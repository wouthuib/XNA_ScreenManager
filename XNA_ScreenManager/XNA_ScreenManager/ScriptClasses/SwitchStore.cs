using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_ScreenManager.ScriptClasses
{
    [Serializable]
    public class Switch
    {
        public int switchID { get; set; }
        public string switchName { get; set; }
        public string switchValue { get; set; }

        public static Switch create(int identifier, string name, string value)
        {
            var results = new Switch();

            results.switchID = identifier;
            results.switchName = name;
            results.switchValue = value;
            return results;
        }
    }

    public sealed class SwitchStore
    {
        public List<Switch> switch_list { get; set; }

        private static SwitchStore instance;
        private SwitchStore()
        {
            switch_list = new List<Switch>();
        }

        public static SwitchStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SwitchStore();
                }
                return instance;
            }
        }

        public void setSwitch(int ID, string name, string value)
        {
            if (switch_list.FindAll(delegate(Switch switchobj) { return switchobj.switchName == name; }).Count == 0)
                switch_list.Add(Switch.create(ID, name, value));
            else
                switch_list.Find(delegate(Switch switchobj) { return switchobj.switchName == name; }).switchValue = value;
        }

        public void removeSwitch(string name)
        {
            switch_list.Remove(new Switch() { switchName = name });
        }

        public Switch getSwitch(int ID)
        {
            return this.switch_list.Find(delegate(Switch switchobj) { return switchobj.switchID == ID; });
        }
    }
}
