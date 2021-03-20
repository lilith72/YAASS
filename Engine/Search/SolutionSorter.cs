using JustinsASS.Engine.Contract.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Search
{
    // TODO add interface
    public class SolutionSorter
    {
        public IList<Solution> ReturnSortedSolutions(
            IList<Solution> unorderedSolutions,
            IList<SolutionSortCondition> orderedSortConditions)
        {
            if (orderedSortConditions.Count() <= 0)
            {
                throw new ArgumentException("No sort conditions passed to ReturnSortedSolutions.");
            }

            IOrderedEnumerable<Solution> result = unorderedSolutions.OrderBy(
                x => x, GetComparerForSolutionSortCondition(orderedSortConditions[0]));
            for (int i = 1; i < orderedSortConditions.Count(); i++)
            {
                result = result.ThenBy(x => x, GetComparerForSolutionSortCondition(orderedSortConditions[i]));
            }
            return result.ToList();
        }

        private IComparer<Solution> GetComparerForSolutionSortCondition(SolutionSortCondition condition)
        {
            switch (condition)
            {
                case SolutionSortCondition.ArmorPoints:
                    return Comparer<Solution>.Create((s1, s2) =>
                    {
                        return s1.GetTotalArmorPoints() - s2.GetTotalArmorPoints();
                    });
                default:
                    throw new NotImplementedException($"Unsupported SolutionSortCondition {condition}");

            }
        }
    }
}
