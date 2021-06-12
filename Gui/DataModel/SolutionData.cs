using JustinsASS.Engine.Contract.DataModel;
using JustinsASS.Engine.Contract.FrontEndInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JustinsASS.Gui.DataModel
{
    public class SolutionData
    {
        //public IList<String> ContributorIds { get; private set; }
        public Set Contributors { get; private set; }
        public IDictionary<string, SkillLvlMax> Skills { get; private set; }
        public IList<int> SpareSlots { get; private set; }
        public int ArmorPoints { get; private set; }
        public int FireRes { get; private set; }
        public int WaterRes { get; private set; }
        public int IceRes { get; private set; }
        public int ThunderRes { get; private set; }
        public int DragonRes { get; private set; }
        public bool CanPin { get; private set; }
        public bool CanRemove { get; private set; }
        public int? Index { get; private set; }

        // Needed to properly reconstruct solution, but shouldn't need to be exposed to user
        private Dictionary<string, int> SetIdTally { get; set; }

        public SolutionData(Solution solution, int? index = null, bool pinnable = false, bool removable = false)
        {
            Regex vacantPattern = new Regex(@"vacant.*slot");
            this.ArmorPoints = solution.GetTotalArmorPoints();
            this.FireRes = solution.GetTotalFireResistance();
            this.WaterRes = solution.GetTotalWaterResistance();
            this.IceRes = solution.GetTotalIceResistance();
            this.ThunderRes = solution.GetTotalThunderResistance();
            this.DragonRes = solution.GetTotalDragonResistance();
            this.SpareSlots = solution.GetSpareSlots().OrderByDescending(s => s).ToList();
            this.CanPin = pinnable;
            this.CanRemove = removable;
            this.Index = index;
            Skills = Helper.GetSkillsWithMax(solution.GetSkillValues()).OrderBy(s => s.Key).ToDictionary(s => s.Key, s => s.Value);
            this.Contributors = new Set(solution.Contributors);
            this.SetIdTally = solution.SetIdTally;
        }
        public Solution GetAsEngineSolution()
        {
            
            IList<string> contributorIds = new List<string>();
            
            contributorIds.Add(this.Contributors.Head);
            contributorIds.Add(this.Contributors.Chest);
            contributorIds.Add(this.Contributors.Arms);
            contributorIds.Add(this.Contributors.Waist);
            contributorIds.Add(this.Contributors.Legs);
            contributorIds.Add(this.Contributors.Talisman);
            foreach (string id in this.Contributors.Decos)
            {
                contributorIds.Add(id);
            };
            Solution solution = new Solution(
                contributorIds
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Select(s => ASS.Instance.GetAllSkillContributors().Where(
                                c => c.SkillContributorId.Equals(s)).FirstOrDefault())
                    .ToList(),
                this.SpareSlots.ToList(),
                this.SetIdTally);
            return solution;
        }

        public class Set
        {
            public string Head { get; set; }
            public string Chest { get; set; }
            public string Arms { get; set; }
            public string Waist { get; set; }
            public string Legs { get; set; }
            public string Talisman { get; set; }
            public IList<string> Decos { get; set; }

            public Set()
            {
                Decos = new List<string>();
            }

            public Set(IList<SkillContributor> contributors)
            {
                Decos = new List<string>();
                Regex vacantPattern = new Regex(@"vacant.*slot");
                foreach (SkillContributor contributor in contributors)
                {
                    if (!vacantPattern.IsMatch(contributor.SkillContributorId.ToLower()))
                    {
                        switch (contributor.Slot)
                        {
                            case ArmorSlot.Head:
                                this.Head = contributor.SkillContributorId;
                                break;
                            case ArmorSlot.Chest:
                                this.Chest = contributor.SkillContributorId;
                                break;
                            case ArmorSlot.Arms:
                                this.Arms = contributor.SkillContributorId;
                                break;
                            case ArmorSlot.Waist:
                                this.Waist = contributor.SkillContributorId;
                                break;
                            case ArmorSlot.Legs:
                                this.Legs = contributor.SkillContributorId;
                                break;
                            case ArmorSlot.Talisman:
                                this.Talisman = contributor.SkillContributorId;
                                break;
                            case ArmorSlot.Deco:
                                this.Decos.Add(contributor.SkillContributorId);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
