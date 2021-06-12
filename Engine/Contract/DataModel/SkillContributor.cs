using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAASS.Engine.Contract.DataModel
{
    public class SkillContributor
    {
        [JsonConstructor]
        public SkillContributor(
            string skillContributorId,
            int armorPoints,
            IList<int> providedDecoSlots,
            ArmorSlot slot,
            IList<SkillValue> providedSkillValues,
            string setId = null,
            int fireRes = 0,
            int iceRes = 0,
            int waterRes = 0,
            int thunderRes = 0,
            int dragonRes = 0,
            int minRank = 0)
        {
            this.SkillContributorId = skillContributorId;
            this.SetId = setId;
            this.ArmorPoints = armorPoints;
            this.ProvidedDecoSlots = providedDecoSlots;
            this.Slot = slot;
            this.ProvidedSkillValues = providedSkillValues;
            this.FireRes = fireRes;
            this.IceRes = iceRes;
            this.WaterRes = waterRes;
            this.ThunderRes = thunderRes;
            this.DragonRes = dragonRes;
            this.MinRank = minRank;
        }

        public string SkillContributorId { get; private set; }
        /// <summary>
        /// Used to group items together in sets, and to show to user what set it comes from.
        /// </summary>
        public string SetId { get; set; }
        public int ArmorPoints { get; private set; }
        public IList<int> ProvidedDecoSlots { get; private set; }
        public ArmorSlot Slot { get; private set; }
        public IList<SkillValue> ProvidedSkillValues { get; private set; }

        public int MinRank { get; private set; }
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
