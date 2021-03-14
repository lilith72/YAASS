using JustinsASS.Engine.Contract.DataModel;
using JustinsASS.Engine.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Contract.FrontEndInterface
{
    public class ASS : IASS
    {
        public ASS()
        {
            // TODO:
            // - initialize inventory by reading JSON, on construction
        }

        public IList<string> GetAllArmorAndDecoIds()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, int> GetSkillNamesToMaxLevelMapping()
        {
            throw new NotImplementedException();
        }

        public IList<Solution> GetSolutionsForSearch(IDictionary<string, int> skillNameToDesiredLevel, IList<int> weaponDecoSlots, IList<Func<string, bool>> InventoryFilters)
        {
            throw new NotImplementedException();
        }
    }
}
