using JustinsASS.Engine.Contract.DataModel;
using JustinsASS.Engine.Contract.Interfaces;
using JustinsASS.Engine.DataReader;
using JustinsASS.Engine.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Contract.FrontEndInterface
{
    public class ASS : IASS
    {
        private readonly IInventoryProvider inventoryProvider;
        private readonly ISearchWorker searchWorker;
        private readonly ISolutionSorter solutionSorter;

        private List<SkillContributor> allInventoryFromFile;
        private Dictionary<string, int> skillNameToMaxValue;

        public ASS()
        {
            this.inventoryProvider = new CsvInventoryProvider();
            this.searchWorker = new SearchWorker();
            this.solutionSorter = new SolutionSorter();
            this.RefreshDataFromFiles();
        }

        public void RefreshDataFromFiles()
        {
            this.allInventoryFromFile = inventoryProvider.GetSkillContributors();
            this.skillNameToMaxValue = inventoryProvider.GetSkillNameToMaxLevelMapping();
        }

        public IList<string> GetAllSkillContributorIds()
        {
            return allInventoryFromFile.Select(skillContributor => skillContributor.SkillContributorId).ToList();
        }

        public IList<SkillContributor> GetAllSkillContributors()
        {
            return allInventoryFromFile;
        }

        public IDictionary<string, int> GetSkillNamesToMaxLevelMapping()
        {
            return this.skillNameToMaxValue;
        }

        public IList<Solution> GetSolutionsForSearch(
            IDictionary<string, int> skillNameToDesiredLevel,
            IList<int> weaponDecoSlots = null,
            IList<Func<SkillContributor, bool>> inventoryFilters = null,
            IList<SkillContributor> talismans = null)  // this filtering probably needs work, a bit clunky
        {
            if (talismans != null && talismans.Any(t => t.Slot != ArmorSlot.Talisman))
            {
                throw new ArgumentException($"Custom talisman list contained non-talisman skill contributor:" +
                    $" {talismans.Where(t => t.Slot != ArmorSlot.Talisman).First().SkillContributorId}");
            }
            SearchTarget searchTarget = new SearchTarget(skillNameToDesiredLevel.Select(kvp => new SkillValue(kvp.Key, kvp.Value)).ToList());
            Inventory targetInventory = new Inventory(new List<SkillContributor>(allInventoryFromFile));
            if (inventoryFilters != null)
            {
                foreach (Func<SkillContributor, bool> filter in inventoryFilters)
                {
                    targetInventory = targetInventory.FilterInventory(filter);
                }
            }
            return searchWorker.FindAllSolutions(targetInventory, searchTarget, weaponDecoSlots).ToList();
        }

        public IList<Solution> SortSolutionsByGivenConditions(
            IList<Solution> unorderedSolutions,
            IList<SolutionSortCondition> sortConditions)
        {
            return solutionSorter.ReturnSortedSolutions(unorderedSolutions, sortConditions);
        }
    }
}
