using JustinsASS.Engine.Contract.DataModel;
using JustinsASS.Engine.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Search
{
    public class SearchWorker : ISearchWorker
    {
        private ISet<Solution> seenPartialSolutions = null;

        public SearchWorker()
        {
            // Intentionally empty
        }

        public IEnumerable<Solution> FindAllSolutions(Inventory inventory, SearchTarget target, IList<int> weaponDecoSlots = null)
        {
            seenPartialSolutions = new HashSet<Solution>();

            Solution partialSolution = new Solution();
            if (weaponDecoSlots != null)
            {
                partialSolution.OpenDecoSlots.AddRange(weaponDecoSlots);
            }

            return SearchForSolutionsRecursive(
                inventory,
                target,
                partialSolution);
        }

        private IEnumerable<Solution> SearchForSolutionsRecursive(
            Inventory inventory,
            SearchTarget target,
            Solution partialSolution)
        {
            List<Solution> resultSolutions = new List<Solution>();
            
            if (target.SolutionFulfillsTarget(partialSolution))
            {
                resultSolutions.Add(partialSolution);
                return resultSolutions;
            }

            Inventory helpfulInventory = inventory.FilterInventory((SkillContributor sc) => target.SkillContributorHelpsTarget(sc, partialSolution));

            foreach (SkillContributor chosenItem in helpfulInventory.AllContributors)
            {
                if (chosenItem.SkillContributorId.Equals("TEST"))
                {
                    int a = 4;
                }
                // Check that the chosen item fits on the set
                if (!partialSolution.CanFitNewPiece(chosenItem))
                {
                    continue;
                }
                // Create new solution with chosen item
                Solution newPartialSolution = partialSolution.Clone();
                newPartialSolution.AddNewPiece(chosenItem);

                // Skip new solution if we've checked this path before
                if (seenPartialSolutions.Contains(newPartialSolution))
                {
                    continue;
                }
                seenPartialSolutions.Add(newPartialSolution);

                // Recurse and check using this chosen item
                resultSolutions.AddRange(SearchForSolutionsRecursive(helpfulInventory, target, newPartialSolution));
            }
            return resultSolutions;
        }
    }
}
