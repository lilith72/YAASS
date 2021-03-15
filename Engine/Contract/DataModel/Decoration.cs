using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Contract.DataModel
{
    public class Decoration : SkillContributor
    {
        public Decoration(
            string id,
            int slotSize,
            IList<SkillValue> skillValues)
                : base(id: id,
                      armorPoints: 0,
                      decoSlots: new List<int>(),
                      slot: ArmorSlot.Deco,
                      skills: skillValues)
        {
            this.SlotSize = slotSize;
        }

        public int SlotSize { get; protected set; }
    }
}
