using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Contract.DataModel
{
    public class Solution
    {
        public IList<SkillContributor> ArmorPieces { get; set; }
        public int partialSolutionDistance { get; set; }

        public Solution Clone()
        {
            throw new NotImplementedException();
        }
    }
}
