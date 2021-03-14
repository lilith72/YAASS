using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Contract.DataModel
{
    /// <summary>
    /// Contains all possible SkillContributor which can contribute to the search target.
    /// </summary>
    public class Inventory
    {
        public Inventory(IList<SkillContributor> contributors)
        {
            this.AllContributors = contributors;
        }

        public IList<SkillContributor> AllContributors { get; private set; }

        public Inventory FilterInventory(Func<SkillContributor, bool> filter)
        {
            return new Inventory(this.AllContributors.Where(contributor => filter(contributor)).ToList());
        }
    }
}
