using JustinsASS.Engine.Contract.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Contract.Interfaces
{
    /// <summary>
    /// Interface meant to be utilized by front end to get the data it needs.
    /// </summary>
    public interface IASS
    {
        /// <summary>
        /// Returns list of Solution which fills requirement set by skillNameToDesiredLevel.
        /// </summary>
        /// <param name="skillNameToDesiredLevel">skill name -> desired level (level meaning actual level in game, not "points")</param>
        /// <param name="weaponDecoSlots">Decoration slots provided by the weapon. value of each entry represents size of the slot.</param>
        /// <param name="InventoryFilters">Takes a function string(armor/deco Id)=>bool(Can be used according to user input) to determine which items should be used.</param>
        /// <returns></returns>
        IList<Solution> GetSolutionsForSearch(
            IDictionary<string, int> skillNameToDesiredLevel,
            IList<int> weaponDecoSlots = null,
            IList<Func<SkillContributor, bool>> inventoryFilters = null,
            IList<SkillContributor> talismans = null);

        /// <summary>
        /// Returns all skill names in the game (key) and their max level (value).
        /// </summary>
        /// <returns></returns>
        IDictionary<string, int> GetSkillNamesToMaxLevelMapping();

        /// <summary>
        ///  Gets all skill contributors. 
        /// </summary>
        /// <returns></returns>
        IList<SkillContributor> GetAllSkillContributors();

        /// <summary>
        /// Returns skill contributor looked up by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SkillContributor GetSkillContributorById(string id);

        /// <summary>
        /// Returns all "Skill Contributor" (armor/deco/charm) ids. These can be shown to user for purpose of filtering out armors / charms they don't have, etc.
        /// </summary>
        /// <returns></returns>
        IList<string> GetAllSkillContributorIds();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unorderedSolutions"></param>
        /// <param name="sortConditions">Sorts by sortConditions[0], then by sortConditions[1], etc</param>
        /// <returns></returns>
        IList<Solution> SortSolutionsByGivenConditions(
            IList<Solution> unorderedSolutions,
            IList<SolutionSortCondition> sortConditions);

        /// <summary>
        /// Forces application to reread the data from the files.
        /// </summary>
        void RefreshDataFromFiles();

        void PersistCustomInventoryAddition(
            SkillContributor talismanToAdd);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idToRemove"></param>
        /// <returns></returns>
        bool TryPersistCustomInventoryDeletion(
            string idToRemove);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">The solution to pin</param>
        void PersistPinnedSolution(
            Solution s);

        /// <summary>
        /// 
        /// </summary>
        void FetchAllPinnedSolutions();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">The solution to be returned.</param>
        /// <returns>true if unpin was successful, false if there was an error.</returns>
        bool TryUnpinSolution(
            Solution s,
            out string errorMessage);
    }
}
