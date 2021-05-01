using JustinsASS.Engine.Contract.DataModel;
using JustinsASS.Engine.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Search
{
    public class SearchWorker : ISearchWorker
    {
        // TODO move all these to config file
        private bool enableSearchDecosOnlyAfterArmorExhausted = true;
        private bool enableStopAfterNSeconds = false;
        private int secondsToStopAt = 30;
        private bool enableStopAfterNSolutions = false;
        private int solutionsToStopAt = 1000;
        private bool enableGreedySkillSelection = false;
        private int greedySkillSelectionCutoff = 4;
        
        private ISet<Solution> seenPartialSolutions = null;
        private Stopwatch stopwatch;
        private int solutionsCount;
        private int exploredNodes;
        private int trimmedNodes;

        public SearchWorker()
        {
            // Intentionally empty
        }

        public IEnumerable<Solution> FindAllSolutions(Inventory inventory, SearchTarget target, IList<int> weaponDecoSlots = null)
        {
            this.stopwatch = Stopwatch.StartNew();
            this.solutionsCount = 0;
            this.exploredNodes = 0;
            this.trimmedNodes = 0;
            this.seenPartialSolutions = new HashSet<Solution>();

            Solution partialSolution = new Solution();
            if (weaponDecoSlots != null)
            {
                partialSolution.OpenDecoSlots.AddRange(weaponDecoSlots);
            }

            IEnumerable<Solution> result = SearchForSolutionsRecursive(
                inventory,
                target,
                partialSolution);
            Console.WriteLine($"search took {stopwatch.ElapsedMilliseconds}ms and found {result.Count()} solutions.");
            Console.WriteLine($"search explored {exploredNodes} nodes, {trimmedNodes} of which were skipped due to duplicate.");
            return result;
        }

        private IEnumerable<Solution> SearchForSolutionsRecursive(
            Inventory inventory,
            SearchTarget target,
            Solution partialSolution)
        {
            List<Solution> resultSolutions = new List<Solution>();
            
            if (target.SolutionFulfillsTarget(partialSolution))
            {
                solutionsCount++;
                Console.WriteLine($"so far found {solutionsCount} solutions after {stopwatch.ElapsedMilliseconds}ms");
                resultSolutions.Add(partialSolution);
                return resultSolutions;
            }

            if (enableStopAfterNSeconds && stopwatch.ElapsedMilliseconds > secondsToStopAt * 1000)
            {
                return resultSolutions;
            }

            if (enableStopAfterNSolutions && solutionsCount >= solutionsToStopAt)
            {
                // Their search is not refined; just quit.
                return resultSolutions;
            }

            Inventory helpfulInventory = inventory.FilterInventory((SkillContributor sc) => 
                target.SkillContributorHelpsTarget(sc, partialSolution));

            // optimization A: only check decorations if we're out of armors
            // this does decrease quality of results by preventing surfacing sets with decos but without armors.
            bool areArmorsLeft = helpfulInventory.AllContributors.Any(s => !(s is Decoration));
            IEnumerable<SkillContributor> skillContrsToTry = enableSearchDecosOnlyAfterArmorExhausted && areArmorsLeft
                ? helpfulInventory.AllContributors.Where(s => !(s is Decoration))
                : helpfulInventory.AllContributors;

            // optimization BN: deterministically sort the skills we still need, and only investigate armors which give the front one.
            if (enableGreedySkillSelection && areArmorsLeft)
            {
                List<string> selectedSkillNames = target.GetRemainingSkillPointsGivenSolution(partialSolution)
                    .Where(kvp => kvp.Value > 0)
                    .OrderBy(kvp => kvp.Key)
                    .Select(kvp => kvp.Key)
                    .Take(greedySkillSelectionCutoff)
                    .ToList();

                skillContrsToTry = skillContrsToTry
                    .Where(sc => sc.ProvidedSkillValues.Any(skill => selectedSkillNames.Any(
                        selectedSkillName => skill.SkillId.Equals(selectedSkillName, StringComparison.OrdinalIgnoreCase))));
            }

            // potential optimization: determine skills which don't have decos, and fill those first.
            // very small optimization so low-pri.
            // and, don't even bother searching the space of decos; fill it from largest slots to smallest.
            // will be a bit of a pain if they ever introduce size 4 / 2-skill slots though.

            // potential optimization: if assembled armor is not within N points, where N is number of deco slots,
            // stop the search for that path.

            // optimization B2: same as optimization b1, but also use a heuristic to bring rarer skills to the front? or skills which don't have decos?
            // or skills which need more points?

            foreach (SkillContributor chosenItem in skillContrsToTry)
            {
                /*if (chosenItem.SkillContributorId.Equals("Ironwall Jewel 2")
                    && partialSolution.Contributors.Count(contr => contr is VacantSlot) == 5
                    && partialSolution.Contributors.Any(contr => contr.SkillContributorId.Equals("Bazelgeuse Greaves")))
                {
                    int a = 4;
                }*/
                // Check that the chosen item fits on the set
                if (!partialSolution.CanFitNewPiece(chosenItem))
                {
                    continue;
                }

                exploredNodes++;
                if (exploredNodes % 100000 == 0)
                {
                    Console.WriteLine($"explored {exploredNodes} nodes");
                }
                // Create new solution with chosen item
                Solution newPartialSolution = partialSolution.Clone();
                newPartialSolution.AddNewPiece(chosenItem);

                // Skip new solution if we've checked this path before
                if (seenPartialSolutions.Contains(newPartialSolution))
                {
                    trimmedNodes++;
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
