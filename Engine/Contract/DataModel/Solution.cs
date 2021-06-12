using YAASS.Engine.Contract.FrontEndInterface;
using YAASS.Engine.Search;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAASS.Engine.Contract.DataModel
{
    public class Solution
    {
        public List<SkillContributor> Contributors { get; private set; }
        public Dictionary<string, int> SetIdTally { get; set; }

        public List<int> OpenDecoSlots { get; private set; }
        public Solution()
        {
            this.Contributors = new List<SkillContributor>();
            this.SetIdTally = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            // Initialize all the slots as empty
            // If there end up being different slots game, abstract this constructor to a factory.
            foreach (ArmorSlot slot in Enum.GetValues(typeof(ArmorSlot)))
            {
                if (slot == ArmorSlot.None || slot == ArmorSlot.Deco)
                {
                    continue;
                }
                this.Contributors.Add(new VacantSlot(slot));
            }

            this.OpenDecoSlots = new List<int>();
        }

        [JsonConstructor]
        public Solution(
            List<SkillContributor> contributors,
            List<int> openDecoSlots,
            Dictionary<string, int> setIdTally)
        {
            this.Contributors = contributors;
            this.OpenDecoSlots = openDecoSlots;
            this.SetIdTally = setIdTally;
        }

        public Solution Clone()
        {
            Solution result = new Solution();
            result.Contributors = new List<SkillContributor>();
            // SkillContributor is immutable so this is OK.
            result.Contributors.AddRange(this.Contributors);
            result.OpenDecoSlots = new List<int>();
            result.OpenDecoSlots.AddRange(this.OpenDecoSlots);
            foreach (string setId in this.SetIdTally.Keys)
            {
                result.SetIdTally[setId] = this.SetIdTally[setId];
            }
            return result;
        }

        public void AddNewPiece(SkillContributor piece)
        {
            // if !CanFitNewPiece throw exception
            // else add it to ArmorPieces and add its deco slots to OpenDecoSlots (if applicable)
            if (!CanFitNewPiece(piece))
            {
                throw new ArgumentException($"piece {piece} does not fit in solution: {this}");
            }

            if (piece is Decoration deco)
            {
                int selectedSlotSize = int.MaxValue;
                for (int i = 0; i < this.OpenDecoSlots.Count(); i++)
                {
                    if (OpenDecoSlots[i] >= deco.SlotSize && OpenDecoSlots[i] < selectedSlotSize)
                    {
                        selectedSlotSize = OpenDecoSlots[i];
                    }
                }
                if (selectedSlotSize == int.MaxValue)
                {
                    throw new Exception("Unexpected argument: Tried to add decoration to solution that doesn't fit it.");
                }
                this.OpenDecoSlots.Remove(selectedSlotSize);
                this.Contributors.Add(deco);
            }
            else
            {
                this.Contributors.Add(piece);
                this.IncrementSetIdTally(piece.SetId);
                foreach (int slot in piece.ProvidedDecoSlots)
                {
                    this.OpenDecoSlots.Add(slot);
                }
                this.Contributors.Remove(this.Contributors.Find(contributor => (contributor is VacantSlot) && contributor.Slot == piece.Slot));
            }
        }

        public bool CanFitNewPiece(SkillContributor candidate)
        {
            if (candidate is Decoration deco)
            {
                return this.CanFitNewDeco(deco);
            }

            return this.Contributors.Any((piece) =>
                piece.Slot == candidate.Slot
                 && ((piece is VacantSlot) || Utils.AllowsDuplicates(piece.Slot)));
        }

        public bool CanFitNewDeco(Decoration deco)
        {
            return OpenDecoSlots.Any(slot => slot >= deco.SlotSize);
        }

        public override bool Equals(object obj)
        {
            // Check if each skill contributor matches by name.
            if (obj == null)
            {
                return false;
            }

            if (obj is Solution solution)
            {
                return solution.Contributors.Count() == this.Contributors.Count()
                    && solution.Contributors.All(contributor => this.Contributors.Contains(contributor));
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hashResult = 0;
            foreach (SkillContributor contr in this.Contributors)
            {
                hashResult += contr.GetHashCode();
            }
            return hashResult;
        }

        public override string ToString()
        {
            return
                string.Join(Environment.NewLine,
                new[] {
                    $"=============",
                    $"Head:\t{Contributors.Find(contr => contr.Slot == ArmorSlot.Head)}",
                    $"Chest:\t{Contributors.Find(contr => contr.Slot == ArmorSlot.Chest)}",
                    $"Arm:\t{Contributors.Find(contr => contr.Slot == ArmorSlot.Arms)}",
                    $"Waist:\t{Contributors.Find(contr => contr.Slot == ArmorSlot.Waist)}",
                    $"Feet:\t{Contributors.Find(contr => contr.Slot == ArmorSlot.Legs)}",
                    $"Talisman:\t{Contributors.Find(contr => contr.Slot == ArmorSlot.Talisman)}",
                    $"---decorations---",
                    string.Join(Environment.NewLine, Contributors.Where(contr => contr.Slot == ArmorSlot.Deco).Select(s => $"Deco:\t{s}")),
                    $"---skills---",
                    string.Join(Environment.NewLine, this.GetSkillValues().Select(skillValue => $"{skillValue.SkillId}: Lv.{skillValue.Points}")),
                    $"---stats---",
                    $"ArmorPoints:\t{this.GetTotalArmorPoints()}",
                    $"Fire Resist:\t{this.GetTotalFireResistance()}",
                    $"Water Resist:\t{this.GetTotalWaterResistance()}",
                    $"Ice Resist:\t{this.GetTotalIceResistance()}",
                    $"Thunder Resist:\t{this.GetTotalThunderResistance()}",
                    $"Dragon Resist:\t{this.GetTotalDragonResistance()}",
                    $"Spare size 1 slots:\t{this.GetSpareSlots().Count(s => s == 1)}",
                    $"Spare size 2 slots:\t{this.GetSpareSlots().Count(s => s == 2)}",
                    $"Spare size 3 slots:\t{this.GetSpareSlots().Count(s => s == 3)}",
                    $"Spare size 4 slots:\t{this.GetSpareSlots().Count(s => s == 4)}",
                    $"============="
                }) + Environment.NewLine;
        }

        public int GetTotalFireResistance()
        {
            return this.Contributors.Sum(contr => contr.FireRes) + GetSetBonusResistance();
        }

        public int GetTotalIceResistance()
        {
            return this.Contributors.Sum(contr => contr.IceRes) + GetSetBonusResistance();
        }

        public int GetTotalWaterResistance()
        {
            return this.Contributors.Sum(contr => contr.WaterRes) + GetSetBonusResistance();
        }

        public int GetTotalThunderResistance()
        {
            return this.Contributors.Sum(contr => contr.ThunderRes) + GetSetBonusResistance();
        }

        public int GetTotalDragonResistance()
        {
            return this.Contributors.Sum(contr => contr.DragonRes) + GetSetBonusResistance();
        }

        public int GetTotalArmorPoints()
        {
            return this.Contributors.Sum(contr => contr.ArmorPoints);
        }

        // Index is slot size, value is number of slots spare
        public IList<int> GetSpareSlots()
        {
            return OpenDecoSlots;
        }

        public IList<SkillValue> GetSkillValues()
        {
            Dictionary<string, int> skillsToTotals = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (SkillContributor contributor in this.Contributors)
            {
                foreach (SkillValue skillValue in contributor.ProvidedSkillValues)
                {
                    if (!skillsToTotals.ContainsKey(skillValue.SkillId))
                    {
                        skillsToTotals[skillValue.SkillId] = 0;
                    }
                    skillsToTotals[skillValue.SkillId] += skillValue.Points;
                }
            }
            return skillsToTotals.Select(kvp =>
                {
                    ASS.Instance.GetSkillNamesToMaxLevelMapping().TryGetValue(kvp.Key, out int maxLevel);
                    if (maxLevel == 0)
                    {
                        maxLevel = int.MaxValue;
                    }
                    return new SkillValue(kvp.Key, Math.Min(kvp.Value, maxLevel));
                }).ToList();
        }

        private int GetSetBonusResistance()
        {
            if (this.SetIdTally.Values.Contains(3))
            {
                return 1;
            }
            if (this.SetIdTally.Values.Contains(4))
            {
                return 2;
            }
            if (this.SetIdTally.Values.Contains(5))
            {
                return 3;
            }
            return 0;
        }

        private void IncrementSetIdTally(string setId)
        {
            if (setId != null)
            {
                if (!this.SetIdTally.ContainsKey(setId))
                {
                    SetIdTally.Add(setId, 0);
                }
                SetIdTally[setId]++;
            }
        }
    }
}
