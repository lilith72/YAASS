using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Gui.DataModel
{
    public class SkillLvlMax
    {
        public int Level { get; set; }
        public int Max { get; set; }

        public SkillLvlMax(int level, int max)
        {
            Level = level;
            Max = max;
        }
    }
}
