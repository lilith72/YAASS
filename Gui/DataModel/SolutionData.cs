using JustinsASS.Engine.Contract.DataModel;
using JustinsASS.Engine.Contract.FrontEndInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JustinsASS.Gui.DataModel
{
    public class SolutionData
    {
        public IList<String> ContributorIds { get; private set; }
        public IDictionary<string, SkillLvlMax> Skills { get; private set; }
        public IList<int> SpareSlots { get; private set; }
        public int ArmorPoints { get; private set; }
        public int FireRes { get; private set; }
        public int WaterRes { get; private set; }
        public int IceRes { get; private set; }
        public int ThunderRes { get; private set; }
        public int DragonRes { get; private set; }

        public SolutionData(Solution solution, ASS ass)
        {
            Regex vacantPattern = new Regex(@"vacant.*slot");
            this.ContributorIds = new List<string>();
            this.ArmorPoints = solution.GetTotalArmorPoints();
            this.FireRes = solution.GetTotalFireResistance();
            this.WaterRes = solution.GetTotalWaterResistance();
            this.IceRes = solution.GetTotalIceResistance();
            this.ThunderRes = solution.GetTotalThunderResistance();
            this.DragonRes = solution.GetTotalDragonResistance();
            this.SpareSlots = solution.GetSpareSlots();
            Skills = Helper.GetSkillsWithMax(solution.GetSkillValues(), ass);
            foreach (SkillContributor contributor in solution.Contributors)
            {
                if (!vacantPattern.IsMatch(contributor.SkillContributorId.ToLower()))
                {
                    ContributorIds.Add(contributor.SkillContributorId);
                }
            }
        }
    }
}
