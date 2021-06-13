using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAASS.Engine.Contract.DataModel
{
    public class SkillValue
    {
        public string SkillId { get; set; }
        public int Points { get; set; }
        public SkillValue(string skillId, int points)
        {
            this.SkillId = skillId;
            this.Points = points;
        }
    }
}
