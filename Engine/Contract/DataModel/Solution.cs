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
            }
        }

        public void RemovePiece(SkillContributor piece)
        {
            // Search for piece in ArmorPieces
            // if !exist, throw argument exception
            // if all its deco slots aren't open, throw argument exception
            // remove piece and its deco slots
            throw new NotImplementedException();
        }


        public bool CanFitNewPiece(SkillContributor candidate)
        {
            if (candidate is Decoration deco)
            {
                return this.CanFitNewDeco(deco);
            }

            if (this.Contributors.Any((piece) => 
                (piece is VacantSlot)
                || Utils.AllowsDuplicates(piece.Slot)))
            {
                return false;
            }
            return true;
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
    }
}
