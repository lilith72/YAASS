using JustinsASS.Engine.Contract.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Search
{
    public class SearchWorker
    {
        private ISet<Solution> seenPartialSolutions = null;

        public IEnumerable<Solution> FindAllSolutions(
            Inventory inventory,
            SearchTarget target)
        {
            seenPartialSolutions = new HashSet<Solution>();

            return SearchForSolutionsRecursive(
                inventory,
                target,
                new Solution());
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
