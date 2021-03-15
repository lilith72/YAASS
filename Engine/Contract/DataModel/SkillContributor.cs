using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Contract.DataModel
{
    public class SkillContributor
    {
        public SkillContributor(
            string id,
            int armorPoints,
            IList<int> decoSlots,
            ArmorSlot slot,
            IList<SkillValue> skills)
        {
            this.SkillContributorId = id;
            this.ArmorPoints = armorPoints;
            this.ProvidedDecoSlots = decoSlots;
            this.Slot = slot;
            this.ProvidedSkillValues = skills;
        }

        public string SkillContributorId { get; private set; }
        public int ArmorPoints { get; private set; }
        public IList<int> ProvidedDecoSlots { get; private set; }
        public ArmorSlot Slot { get; private set; }
        public IList<SkillValue> ProvidedSkillValues { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is SkillContributor other)
            {
                return this.SkillContributorId.Equals(other.SkillContributorId);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.SkillContributorId.GetHashCode();
        }

        public override string ToString()
        {
            return this.SkillContributorId;
        }
    }
}
