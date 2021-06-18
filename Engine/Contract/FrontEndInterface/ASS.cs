using YAASS.Engine.Contract.DataModel;
using YAASS.Engine.Contract.Interfaces;
using YAASS.Engine.Data;
using YAASS.Engine.DataReader;
using YAASS.Engine.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAASS.Engine.Contract.FrontEndInterface
{
    public class ASS : IASS
    {
        public static ASS Instance;

        private readonly IInventoryProvider inventoryProvider;
        private readonly ISearchWorker searchWorker;
        private readonly ISolutionSorter solutionSorter;
        private readonly IPersistedStorageHelper persistedStorageHelper;
        private readonly IAssConfigProvider assConfigProvider;
        private readonly IAssLogger logger;

        private List<SkillContributor> allInventoryFromFile;
        private Dictionary<string, int> skillNameToMaxValue;

        public ASS()
        {
            if (ASS.Instance != null)
            {
                throw new Exception("ASS does not support duplicate instance construction and should be used as a singleton.");
            }
            ASS.Instance = this;
            this.assConfigProvider = new AssConfigProvider();
            this.logger = new AssLogger(assConfigProvider);
            this.inventoryProvider = new CsvInventoryProvider();
            this.searchWorker = new SearchWorker(this.assConfigProvider, this.logger);
            this.solutionSorter = new SolutionSorter();
            this.persistedStorageHelper = new PersistedStorageHelper();
            this.RefreshDataFromFiles();

            /*List<Dictionary<string, int>> debugSearches = new List<Dictionary<string, int>>()
            {
                new Dictionary<string, int>()
                {
                    { "Botanist", 4 },
                    { "Evade Extender", 3 },
                    { "Geologist", 3 },
                    { "Hunger Resistance", 3 },
                    { "Marathon Runner", 3 },
                    { "Stamina Surge", 3 },
                    { "Wall Runner", 3 },
                    { "Wirebug Whisperer", 3 },
                },
            };
            for (int i = 0; i < debugSearches.Count(); i++)
            {
                Console.WriteLine($"=============START SEARCH {i}=============");
                IEnumerable<Solution> search1 = GetSolutionsForSearch(
                    debugSearches[i],
                    weaponDecoSlots: new List<int> { 2, 1, 1 },
                    talismans: this.GetAllCustomTalismans().Values.ToList());
                Console.WriteLine($"=============END SEARCH {i}=============");
            }*/
        }

        public void RefreshDataFromFiles()
        {
            this.allInventoryFromFile = inventoryProvider.GetSkillContributors();
            this.skillNameToMaxValue = inventoryProvider.GetSkillNameToMaxLevelMapping();
        }

        public IList<string> GetAllSkillContributorIds()
        {
            return allInventoryFromFile.Select(skillContributor => skillContributor.SkillContributorId)
                .Union(this.GetAllCustomTalismans().Keys).ToList();
        }

        public IList<SkillContributor> GetAllSkillContributors()
        {
            return allInventoryFromFile.Union(this.GetAllCustomTalismans().Values).ToList();
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
            Inventory targetInventory = new Inventory(talismans == null ? 
                allInventoryFromFile : 
                allInventoryFromFile.Concat(talismans).ToList());
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

        public void PersistCustomInventoryAddition(
            SkillContributor talismanToAdd)
        {
            this.persistedStorageHelper.TryAddTalisman(talismanToAdd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idToRemove"></param>
        /// <returns></returns>
        public bool TryPersistCustomInventoryDeletion(
            string idToRemove)
        {
            return this.persistedStorageHelper.TryRemoveTalisman(idToRemove);
        }

        public Dictionary<string, SkillContributor> GetAllCustomTalismans()
        {
            return this.persistedStorageHelper.GetCustomTalismans();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">The solution to pin</param>
        public void PinSolution(
            Solution s)
        {
            this.persistedStorageHelper.PinSolution(s);
        }

        /// <summary>
        /// 
        /// </summary>
        public ISet<Solution> GetAllPinnedSolutions()
        {
            return this.persistedStorageHelper.FetchAllPinnedSolutions();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">The solution to be returned.</param>
        /// <returns>true if unpin was successful, false if there was an error.</returns>
        public bool TryUnpinSolution(Solution s, out string errorMessage)
        {
            return this.persistedStorageHelper.TryUnpinSolution(s, out errorMessage);
        }

        public IAssLogger GetInstanceLogger()
        {
            return this.logger;
        }
    }
}
