using JustinsASS.Engine.Contract.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Contract.Interfaces
{
    public interface ISolutionSorter
    {
        IList<Solution> ReturnSortedSolutions(
            IList<Solution> unorderedSolutions,
            IList<SolutionSortCondition> orderedSortConditions);
    }
}
