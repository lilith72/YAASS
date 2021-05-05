using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Contract.DataModel
{
    public class VacantSlot : SkillContributor
    {
        public VacantSlot(ArmorSlot slot) : base(
            skillContributorId: $"Vacant {slot} slot",
            armorPoints: 0,
            providedDecoSlots: new List<int>(),
            slot: slot,
            providedSkillValues: new List<SkillValue>())
        {
            // Intentionally Empty
        }
    }
}
