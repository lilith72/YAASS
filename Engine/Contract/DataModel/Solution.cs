using JustinsASS.Engine.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Contract.DataModel
{
    public class Solution
    {
        public List<SkillContributor> Contributors { get; private set; }

        public List<int> OpenDecoSlots { get; private set; }
        public Solution()
        {
            this.Contributors = new List<SkillContributor>();

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

        public Solution Clone()
        {
            Solution result = new Solution();
            result.Contributors = new List<SkillContributor>();
            // SkillContributor is immutable so this is OK.
            result.Contributors.AddRange(this.Contributors);
            result.OpenDecoSlots = new List<int>();
            result.OpenDecoSlots.AddRange(this.OpenDecoSlots);
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
                int selectedSlotSize = deco.SlotSize;
                for (int i = 0; i < this.OpenDecoSlots.Count(); i++)
                {
                    if (OpenDecoSlots[i] >= deco.SlotSize && OpenDecoSlots[i] < selectedSlotSize)
                    {
                        selectedSlotSize = OpenDecoSlots[i];
                    }
                }
                this.OpenDecoSlots.Remove(deco.SlotSize);
            }
            else
            {
                this.Contributors.Add(piece);
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
                    $"Arm:\t{Contributors.Find(contr => contr.Slot == ArmorSlot.Arm)}",
                    $"Waist:\t{Contributors.Find(contr => contr.Slot == ArmorSlot.Waist)}",
                    $"Feet:\t{Contributors.Find(contr => contr.Slot == ArmorSlot.Feet)}",
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
            return this.Contributors.Sum(contr => contr.FireRes);
        }

        public int GetTotalIceResistance()
        {
            return this.Contributors.Sum(contr => contr.IceRes);
        }

        public int GetTotalWaterResistance()
        {
            return this.Contributors.Sum(contr => contr.WaterRes);
        }

        public int GetTotalThunderResistance()
        {
            return this.Contributors.Sum(contr => contr.ThunderRes);
        }

        public int GetTotalDragonResistance()
        {
            return this.Contributors.Sum(contr => contr.DragonRes);
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
            return skillsToTotals.Select(kvp => new SkillValue(kvp.Key, kvp.Value)).ToList();
        }
    }
}
