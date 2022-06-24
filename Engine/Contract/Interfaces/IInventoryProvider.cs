using YAASS.Engine.Contract.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAASS.Engine.Contract.Interfaces
{
    public interface IInventoryProvider
    {
        List<SkillContributor> GetSkillContributors(
            bool useHighRankArmors,
            bool useGRankArmors);

        Dictionary<string, int> GetSkillNameToMaxLevelMapping();
    }
}
