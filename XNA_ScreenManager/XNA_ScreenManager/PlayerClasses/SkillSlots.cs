using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA_ScreenManager.ItemClasses;
using Microsoft.Xna.Framework;
using XNA_ScreenManager.MapClasses;

namespace XNA_ScreenManager.PlayerClasses
{
    public class SkillSlots
    {
        #region contructor
        private static SkillSlots instance;

        private SkillSlots()
        {
        }

        public static SkillSlots Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SkillSlots();
                }
                return instance;
            }
        }
        #endregion

        public bool active{ get; set;}

        
    }
}
