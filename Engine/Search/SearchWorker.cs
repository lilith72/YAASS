﻿using YAASS.Engine.Contract.DataModel;
using YAASS.Engine.Contract.Interfaces;
using YAASS.Engine.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAASS.Engine.Search
{
    public class SearchWorker : ISearchWorker
    {
        // TODO move all these to config file
        private bool enableSearchDecosOnlyAfterArmorExhausted = true;
        private bool enableStopAfterNSeconds = true;
        private bool enableStopAfterNSolutions = true;

        // in practice this doesn't help much and causes search to skip many valid solutions. Best to leave it off.
        private bool enableGreedySkillSelectionHeuristic = false;
        private int greedySkillSelectionCutoff = 4;
        // Calculate decorations in the set without generic search, but with naive deco population once armors are filled.
        // Good optimization with theoretically no downside as long as enableSearchDecosOnlyAfterArmorExhausted is true.
        // hard coded not to do anyhthing if enableSearchDecosOnlyAfterArmorExhausted is false.
        private readonly bool enableSpecialDecoHandling = true;

        private readonly IAssConfigProvider assConfigProvider;
        private readonly IAssLogger logger;
        
        private ISet<Solution> seenPartialSolutions = null;
        private Stopwatch stopwatch;
        private int solutionsCount;
        private int exploredNodes;
        private int trimmedNodes;

        // precomputed fields
        private Dictionary<string, Decoration> SkillNameToProvidingDeco;

        public SearchWorker(
            IAssConfigProvider configProvider,
            IAssLogger logger)
        {
            this.assConfigProvider = configProvider;
            this.logger = logger;
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

            if (this.assConfigProvider.GetConfig().GetEnableDebugAssertions())
            {
                HashSet<string> seenSkillIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (Decoration deco in inventory.AllContributors
                    .Where(sc => sc is Decoration)
                    .Select(sc => sc as Decoration))
                {
                    if (deco.ProvidedSkillValues.Count() != 1)
                    {
                        throw new Exception("Bad state of decos: provided more than one skill. This is not supported currently.");
                    }
                    if (seenSkillIds.Contains(deco.ProvidedSkillValues.First().SkillId))
                    {
                        throw new Exception("Bad state of decos: More than one deco provides same skill. This is not supported currently.");
                    }
                    seenSkillIds.Add(deco.ProvidedSkillValues.First().SkillId);
                }
            }

            SkillNameToProvidingDeco = inventory.AllContributors
                .Where(sc => sc is Decoration)
                .Select(sc => sc as Decoration)
                .ToList()
                .ToDictionary(deco => deco.ProvidedSkillValues.First().SkillId);

            IEnumerable<Solution> result = SearchForSolutionsRecursive(
                inventory,
                target,
                partialSolution);
            this.logger.Log($"search took {stopwatch.ElapsedMilliseconds}ms and found {result.Count()} solutions.", AssLogLevel.Info);
            this.logger.Log($"search explored {exploredNodes} nodes, {trimmedNodes} of which were skipped due to duplicate.", AssLogLevel.Info);
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
                this.logger.Log($"so far found {solutionsCount} solutions after {stopwatch.ElapsedMilliseconds}ms", AssLogLevel.Verbose);
                resultSolutions.Add(partialSolution);
                return resultSolutions;
            }

            if (enableStopAfterNSeconds &&
                stopwatch.ElapsedMilliseconds > this.assConfigProvider.GetConfig().GetSearchTimeoutSeconds() * 1000)
            {
                return resultSolutions;
            }

            if (enableStopAfterNSolutions && solutionsCount >= this.assConfigProvider.GetConfig().GetSearchMaxResults())
            {
                // Their search is not refined; just quit.
                return resultSolutions;
            }

            Dictionary<string, int> remainingSkillPoints = target.GetRemainingSkillPointsGivenSolution(partialSolution);

            Inventory helpfulInventory = inventory.FilterInventory((SkillContributor sc) => 
                this.SkillContributorHelpsTarget(sc, partialSolution, remainingSkillPoints));

            // optimization A: only check decorations if we're out of armors
            // this does decrease quality of results by preventing surfacing sets with decos but without armors.
            bool areArmorsLeft = helpfulInventory.AllContributors.Any(s => !(s is Decoration) 
                && !s.ProvidedSkillValues.Any(sv => sv.SkillId.Equals("Stormsoul")));
            if (enableSpecialDecoHandling && !areArmorsLeft)
            {
                if (TryCompleteSolutionWithDecos(partialSolution, target, out Solution completedSolution))
                {
                    solutionsCount++;
                    this.logger.Log($"so far found {solutionsCount} solutions after {stopwatch.ElapsedMilliseconds}ms", AssLogLevel.Verbose);
                    resultSolutions.Add(completedSolution);
                }
                // return early since decos shouldn't be handled by standard search with this optimization
                return resultSolutions;
            }

            IEnumerable<SkillContributor> skillContrsToTry = enableSearchDecosOnlyAfterArmorExhausted && areArmorsLeft
                ? helpfulInventory.AllContributors.Where(s => !(s is Decoration))
                : helpfulInventory.AllContributors;

            // optimization B: deterministically sort the skills we still need, and only investigate armors which give the front one.
            if (enableGreedySkillSelectionHeuristic && areArmorsLeft)
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

            // potential optimization: if assembled armor is not within N points, where N is number of deco slots,
            // stop the search for that path.

            // optimization B2: same as optimization b1, but also use a heuristic to bring rarer skills to the front? or skills which don't have decos?
            // or skills which need more points?

            foreach (SkillContributor chosenItem in skillContrsToTry)
            {
                /*if (partialSolution.Contributors.Any(sc => sc.SkillContributorId.Equals("Brigade Lobos S"))
                    && partialSolution.Contributors.Any(sc => sc.SkillContributorId.Equals("Kamura Garb S"))
                    && partialSolution.Contributors.Any(sc => sc.SkillContributorId.Equals("Ludroth Bracers S"))
                    && partialSolution.Contributors.Any(sc => sc.SkillContributorId.Equals("Nargacuga Coil S"))
                    && partialSolution.Contributors.Any(sc => sc.SkillContributorId.Equals("Anjanath Greaves S"))
                    && chosenItem.SkillContributorId.Contains("staminasurge2plus11"))
                {
                    Console.WriteLine("DEBUG BREAK");
                }*/
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
                    this.logger.Log($"explored {exploredNodes} nodes", AssLogLevel.Verbose);
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
                if (enableStopAfterNSolutions && solutionsCount >= this.assConfigProvider.GetConfig().GetSearchMaxResults())
                {
                    return resultSolutions;
                }
            }
            return resultSolutions;
        }

        private bool SkillContributorHelpsTarget(
            SkillContributor contributor,
            Solution partialSolution,
            Dictionary<string, int> remainingSkillPoints)
        {
            if (!partialSolution.CanFitNewPiece(contributor)
                && !(partialSolution.Contributors.Any(contr => contr is VacantSlot) && contributor is Decoration)) // Don't discard decos we could potentially fit later
            {
                return false;
            }

            if (contributor.ProvidedSkillValues.Any(sv => sv.SkillId.Equals("Stormsoul", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            // TODO: take armors that don't help skills but do add skill slots

            return contributor.ProvidedSkillValues.Any((skillValue) =>
                remainingSkillPoints.ContainsKey(skillValue.SkillId)
                && remainingSkillPoints[skillValue.SkillId] > 0);
        }

        // decoration helpers
        // TODO move these to a separate class
        private bool IsSolutionFeasiblyWithinDecoRange(
            Dictionary<string, int> neededSkillCounts,
            List<int> openDecoSlots)
        {
            // assumption: we get exactly one deco point for each skill slot.
            return openDecoSlots.Count() >= neededSkillCounts.Values.Sum();
        }

        private bool TryCompleteSolutionWithDecos(
            Solution partialSolution,
            SearchTarget target,
            out Solution completedSolution)
        {
            Dictionary<string, int> neededSkillCounts = target.GetRemainingSkillPointsGivenSolution(partialSolution)
                .Where(kvp => kvp.Value != 0)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (!IsSolutionFeasiblyWithinDecoRange(neededSkillCounts, partialSolution.OpenDecoSlots))
            {
                completedSolution = null;
                return false;
            }

            if (neededSkillCounts.Any(kvp => !SkillNameToProvidingDeco.ContainsKey(kvp.Key)))
            {
                completedSolution = null;
                return false;
            }
            
            // Solution is worth trying to complete, so try it.
            completedSolution = partialSolution.Clone();
            //order skills by the size of slot needed to get it
            foreach (KeyValuePair<string, int> neededSkillCount in 
                neededSkillCounts.OrderByDescending(kvp => SkillNameToProvidingDeco[kvp.Key].SlotSize))
            {
                Decoration decoToAdd = SkillNameToProvidingDeco[neededSkillCount.Key];
                for (int i = 0; i < neededSkillCount.Value; i++)
                {
                    if (!completedSolution.CanFitNewDeco(decoToAdd))
                    {
                        completedSolution = null;
                        return false;
                    }
                    completedSolution.AddNewPiece(decoToAdd);
                }
            }
            if (this.assConfigProvider.GetConfig().GetEnableDebugAssertions())
            {
                if (!target.SolutionFulfillsTarget(completedSolution))
                {
                    throw new Exception($"Debug assertion failed: finished completing solution" +
                        $" but it did not fulfill target." +
                        $" original partial solution: {partialSolution}" +
                        $" completed solution: {completedSolution}");
                }
            }
            return true;
        }
    }
}
