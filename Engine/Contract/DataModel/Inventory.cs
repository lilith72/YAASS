using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Contract.DataModel
{
    /// <summary>
    /// Contains all possible SkillContributor which can contribute to the search target.
    /// </summary>
    public class Inventory
    {
        // Should precompute indexes for various parameters -- e.g, look up all head pieces, look up all providers of a given skill.

        public IList<SkillContributor> AllContributors { get; set; }

        private IDictionary<string, int> skillNameToContributorIndex;

        private IDictionary<ArmorSlot, IList<string>> armorSlotToSkillNames;

        public static IList<SkillContributor> SortInventory(IComparer<SkillContributor> comparer)
        {
            throw new NotImplementedException();
        }

        public Inventory FilterInventory(Func<SkillContributor, bool> filter)
        {
            throw new NotImplementedException();
        }
    }
}
