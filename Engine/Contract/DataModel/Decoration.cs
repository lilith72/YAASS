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
                : base(skillContributorId: id,
                      armorPoints: 0,
                      providedDecoSlots: new List<int>(),
                      slot: ArmorSlot.Deco,
                      providedSkillValues: skillValues)
        {
            this.SlotSize = slotSize;
        }

        public int SlotSize { get; protected set; }
    }
}
