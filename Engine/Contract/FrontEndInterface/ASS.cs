﻿using JustinsASS.Engine.Contract.DataModel;
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

        private List<SkillContributor> allInventoryFromFile;
        private Dictionary<string, int> skillNameToMaxValue;

        public ASS()
        {
            this.inventoryProvider = new CsvInventoryProvider();
            this.searchWorker = new SearchWorker();
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
            IList<int> weaponDecoSlots,
            IList<Func<SkillContributor, bool>> inventoryFilters)  // this filtering probably needs work, a bit clunky
        {
            SearchTarget searchTarget = new SearchTarget(skillNameToDesiredLevel.Select(kvp => new SkillValue(kvp.Key, kvp.Value)).ToList());
            Inventory targetInventory = new Inventory(new List<SkillContributor>(allInventoryFromFile));
            foreach (Func<SkillContributor, bool> filter in inventoryFilters)
            {
                targetInventory = targetInventory.FilterInventory(filter);
            }
            return searchWorker.FindAllSolutions(targetInventory, searchTarget, weaponDecoSlots).ToList();
        }

        public IList<Solution> SortSolutionsByGivenConditions(
            IList<Solution> unorderedSolutions,
            IList<SolutionSortCondition> sortConditions)
        {
            throw new NotImplementedException();
        }
    }
}