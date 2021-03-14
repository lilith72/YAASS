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
            return OpenDecoSlots.Contains(deco.SlotSize);
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
            return $"=============" + Environment.NewLine +
                $"Head:\t{Contributors.Find(contr => contr.Slot == ArmorSlot.Head)}" + Environment.NewLine +
                $"Chest:\t{Contributors.Find(contr => contr.Slot == ArmorSlot.Chest)}" + Environment.NewLine +
                $"Arm:\t{Contributors.Find(contr => contr.Slot == ArmorSlot.Arm)}" + Environment.NewLine +
                $"Waist:\t{Contributors.Find(contr => contr.Slot == ArmorSlot.Waist)}" + Environment.NewLine +
                $"Feet:\t{Contributors.Find(contr => contr.Slot == ArmorSlot.Feet)}" + Environment.NewLine +
                $"Charm:\t{Contributors.Find(contr => contr.Slot == ArmorSlot.Charm)}" + Environment.NewLine +
                string.Join(Environment.NewLine, Contributors.Where(contr => contr.Slot == ArmorSlot.Deco).Select(s => $"Deco:\t{s}")) + Environment.NewLine +
                $"=============" + Environment.NewLine;


        }
    }
}
