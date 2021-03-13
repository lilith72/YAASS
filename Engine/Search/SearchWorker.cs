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

        public IEnumerable<Solution> SearchForSolutions(
            Inventory inventory,
            SearchTarget target,
            Solution partialSolution = null)
        {
            seenPartialSolutions = new HashSet<Solution>();
            throw new NotImplementedException();
            /*
            approach 1: Naive DFS
            result = Clone partialSolution
            pick vacant slot in result
            sort inventory by desired skills contribution
            pick first N, recurse
             * */

            /*
            approach 2: A* search, where distance to goal = points needed to achieve SearchTarget, (potentially minus pessimistic points from decorations)
            k = max size of candidates
            candidates = SortedList<Pair<Contributor, Rating>> = sorted list by rating
            foreach chooseable contributor c:
                if (seen(c)):
                    continue
                rating = rate(c);
                if (rating > candidates.Worst):
                    candidates.add(rating)
                    if (candidates.Count > k):
                        candidates.Remove(worst)

            */

            //IntervalHeap<int> heap;

            
        }

        private IEnumerable<Solution> SearchForSolutionsRecursive(
            Inventory inventory,
            SearchTarget target,
            Solution partialSolution = null)
        {
            List<Solution> resultSolutions = new List<Solution>();
            if (partialSolution.partialSolutionDistance <= 0)
            {
                resultSolutions.Add(partialSolution);
                return resultSolutions;
            }

            Inventory helpfulInventory = inventory.FilterInventory((SkillContributor sc) =>
            {
                return target.SkillContributorHelpsTarget(sc, partialSolution); // TODO add consideration for armor that does not contribute useful skills but does contribute deco slots
            });

            foreach (SkillContributor chosenItem in helpfulInventory.AllContributors)
            {
                Solution newSolution = partialSolution
            }
        }

        public int RateChoice(
            SkillContributor candidate,
            Solution partialSolutionBeforeAdd,
            SearchTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
