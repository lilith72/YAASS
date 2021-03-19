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
            IList<SkillValue> skills,
            int fireRes = 0,
            int iceRes = 0,
            int waterRes = 0,
            int thunderRes = 0,
            int dragonRes = 0)
        {
            this.SkillContributorId = id;
            this.ArmorPoints = armorPoints;
            this.ProvidedDecoSlots = decoSlots;
            this.Slot = slot;
            this.ProvidedSkillValues = skills;
            this.FireRes = fireRes;
            this.IceRes = iceRes;
            this.WaterRes = waterRes;
            this.ThunderRes = thunderRes;
            this.DragonRes = dragonRes;
        }

        public string SkillContributorId { get; private set; }
        public int ArmorPoints { get; private set; }
        public IList<int> ProvidedDecoSlots { get; private set; }
        public ArmorSlot Slot { get; private set; }
        public IList<SkillValue> ProvidedSkillValues { get; private set; }

        public int FireRes { get; private set; }
        public int IceRes { get; private set; }
        public int WaterRes { get; private set; }
        public int ThunderRes { get; private set; }
        public int DragonRes { get; private set; }

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
