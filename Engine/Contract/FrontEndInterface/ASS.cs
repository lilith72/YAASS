using JustinsASS.Engine.Contract.DataModel;
using JustinsASS.Engine.Contract.Interfaces;
using JustinsASS.Engine.Data;
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
        public static ASS Instance;

        private readonly IInventoryProvider inventoryProvider;
        private readonly ISearchWorker searchWorker;
        private readonly ISolutionSorter solutionSorter;
        private readonly IPersistedStorageHelper persistedStorageHelper;

        private List<SkillContributor> allInventoryFromFile;
        private Dictionary<string, int> skillNameToMaxValue;

        public ASS()
        {
            if (ASS.Instance != null)
            {
                throw new Exception("ASS does not support duplicate instance construction and should be used as a singleton.");
            }
            ASS.Instance = this;
            this.inventoryProvider = new CsvInventoryProvider();
            this.searchWorker = new SearchWorker();
            this.solutionSorter = new SolutionSorter();
            this.persistedStorageHelper = new PersistedStorageHelper();
            this.RefreshDataFromFiles();

            /*
            IEnumerable<Solution> alreadyPinnedSolutions = this.GetAllPinnedSolutions();
            Console.WriteLine($"Loaded pinned solutions and got {alreadyPinnedSolutions.Count()}");
            foreach(Solution s in alreadyPinnedSolutions)
            {
                Console.WriteLine($"pinned solution found: {s}");
            }

            IEnumerable<Solution> solutionsToPin = GetSolutionsForSearch(new Dictionary<string, int>()
            {
                { "Guard", 5 },
                { "Guard Up", 3 },
                { "Artillery", 3 },
                { "Load Shells", 2 },
                { "Diversion", 1 },
                { "Defense Boost", 3 },
                { "Hunger Resistance", 2 },
                { "Speed Sharpening", 1 },
                { "Flinch Free", 1 },
            }).Take(3);
            foreach(Solution s in solutionsToPin)
            {
                //this.PinSolution(s);
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
    }
}
