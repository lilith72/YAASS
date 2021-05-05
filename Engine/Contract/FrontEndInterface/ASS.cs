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
        private readonly PersistedStorageHelper persistedStorageHelper;

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

            SkillContributor contr = new SkillContributor(
                "testTalisman",
                10,
                new List<int>() { 1 },
                ArmorSlot.Talisman,
                new List<SkillValue>() { new SkillValue("Guard", 3) },
                setId: "someTestTalismanSetId");

            this.persistedStorageHelper.TryAddTalisman(contr);

            /*
             * justins debug search
            GetSolutionsForSearch(new Dictionary<string, int>()
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
            });*/
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

        public void PersistCustomInventoryAddition(
            SkillContributor talismanToAdd)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idToRemove"></param>
        /// <returns></returns>
        public bool TryPersistCustomInventoryDeletion(
            string idToRemove)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">The solution to pin</param>
        public void PersistPinnedSolution(
            Solution s)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public void FetchAllPinnedSolutions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">The solution to be returned.</param>
        /// <returns>true if unpin was successful, false if there was an error.</returns>
        public bool TryUnpinSolution(
            Solution s,
            out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public SkillContributor GetSkillContributorById(string id)
        {

            throw new NotImplementedException();
        }
    }
}
