using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Contract.DataModel
{
    public class SkillContributor
    {
        public int ArmorPoints { get; private set; }
        public IList<int> ProvidedSkillSlots { get; private set; }
        public ArmorSlot Slot { get; private set; }
    }
}
