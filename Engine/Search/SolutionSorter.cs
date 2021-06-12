using YAASS.Engine.Contract.DataModel;
using YAASS.Engine.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAASS.Engine.Search
{
    public class SolutionSorter : ISolutionSorter
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
                        return s2.GetTotalArmorPoints() - s1.GetTotalArmorPoints();
                    });
                case SolutionSortCondition.FireResistance:
                    return Comparer<Solution>.Create((s1, s2) =>
                    {
                        return s2.GetTotalFireResistance() - s1.GetTotalFireResistance();
                    });
                case SolutionSortCondition.IceResistance:
                    return Comparer<Solution>.Create((s1, s2) =>
                    {
                        return s2.GetTotalIceResistance() - s1.GetTotalIceResistance();
                    });
                case SolutionSortCondition.DragonResistance:
                    return Comparer<Solution>.Create((s1, s2) =>
                    {
                        return s2.GetTotalDragonResistance() - s1.GetTotalDragonResistance();
                    });
                case SolutionSortCondition.WaterResistance:
                    return Comparer<Solution>.Create((s1, s2) =>
                    {
                        return s2.GetTotalWaterResistance() - s1.GetTotalWaterResistance();
                    });
                case SolutionSortCondition.ThunderResistance:
                    return Comparer<Solution>.Create((s1, s2) =>
                    {
                        return s2.GetTotalThunderResistance() - s1.GetTotalThunderResistance();
                    });
                case SolutionSortCondition.TotalExtraDecorationSlots:
                    return Comparer<Solution>.Create((s1, s2) =>
                    {
                        return s2.GetSpareSlots().Sum() - s1.GetSpareSlots().Sum();
                    });
                case SolutionSortCondition.TotalExtraSkillLevels:
                    // Don't need to actually calculate the "excess" because foreach, the excess is total - desired,
                    // and desired is same for all. So just order by total.
                    return Comparer<Solution>.Create((s1, s2) =>
                    {
                        return s2.GetSkillValues().Sum(val => val.Points) - s1.GetSkillValues().Sum(val => val.Points);
                    });
                case SolutionSortCondition.TotalEmptyArmorSlots:
                    return Comparer<Solution>.Create((s1, s2) =>
                    {
                        return s2.Contributors.Count(contributor => contributor is VacantSlot)
                            - s1.Contributors.Count(contributor => contributor is VacantSlot);
                    });
                default:
                    throw new NotImplementedException($"Unsupported SolutionSortCondition {condition}");
            }
        }
    }
}
