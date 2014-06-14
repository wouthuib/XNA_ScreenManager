using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_ScreenManager.SkillClasses
{
    [Serializable]
    public class SkillBar
    {
        #region properties

        public Skill[] skillslot = new Skill[12];
        public bool visible = true;

        #endregion

        public SkillBar()
        {
        }

        public int getSlot(string skillname)
        {
            for (int i = 0; i < skillslot.Length; i++)
            {
                if (skillslot[i] != null)
                    if (skillslot[i].Name == skillname)
                        return i;
            }

            return -1;
        }
    }
}
