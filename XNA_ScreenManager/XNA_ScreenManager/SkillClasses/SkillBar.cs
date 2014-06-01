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
    }
}
