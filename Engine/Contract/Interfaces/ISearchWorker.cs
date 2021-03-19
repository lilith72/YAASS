using JustinsASS.Engine.Contract.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Contract.Interfaces
{
    public interface ISearchWorker
    {
        IEnumerable<Solution> FindAllSolutions(
            Inventory inventory,
            SearchTarget target,
            IList<int> weaponDecoSlots = null);
    }
}
