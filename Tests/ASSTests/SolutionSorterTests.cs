using JustinsASS.Engine.Contract.DataModel;
using JustinsASS.Engine.Search;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASSTests
{
    public class SolutionSorterTests
    {

        [Test]
        public void ReturnSortedSolutions_SortByArmorPoints()
        {
            ReturnSortedSolutions_Helper(
                high: MakeSolutionWithTwoPiecesGivenStats(armorPoints: 10),
                mid: MakeSolutionWithTwoPiecesGivenStats(armorPoints: 5),
                low: MakeSolutionWithTwoPiecesGivenStats(armorPoints: 1),
                conditions: new List<SolutionSortCondition>() { SolutionSortCondition.ArmorPoints });
        }

        [Test]
        public void ReturnSortedSolutions_SortByFireResistance()
        {
            ReturnSortedSolutions_Helper(
                high: MakeSolutionWithTwoPiecesGivenStats(fireRes: 10),
                mid: MakeSolutionWithTwoPiecesGivenStats(fireRes: 5),
                low: MakeSolutionWithTwoPiecesGivenStats(fireRes: 1),
                conditions: new List<SolutionSortCondition>() { SolutionSortCondition.FireResistance });
        }

        [Test]
        public void ReturnSortedSolutions_SortByIceResistance()
        {
            ReturnSortedSolutions_Helper(
                high: MakeSolutionWithTwoPiecesGivenStats(iceRes: 10),
                mid: MakeSolutionWithTwoPiecesGivenStats(iceRes: 5),
                low: MakeSolutionWithTwoPiecesGivenStats(iceRes: 1),
                conditions: new List<SolutionSortCondition>() { SolutionSortCondition.IceResistance });
        }

        [Test]
        public void ReturnSortedSolutions_SortByDragonResistance()
        {
            ReturnSortedSolutions_Helper(
                high: MakeSolutionWithTwoPiecesGivenStats(dragonRes: 10),
                mid: MakeSolutionWithTwoPiecesGivenStats(dragonRes: 5),
                low: MakeSolutionWithTwoPiecesGivenStats(dragonRes: 1),
                conditions: new List<SolutionSortCondition>() { SolutionSortCondition.DragonResistance });
        }

        [Test]
        public void ReturnSortedSolutions_SortByWaterResistance()
        {
            ReturnSortedSolutions_Helper(
                high: MakeSolutionWithTwoPiecesGivenStats(waterRes: 10),
                mid: MakeSolutionWithTwoPiecesGivenStats(waterRes: 5),
                low: MakeSolutionWithTwoPiecesGivenStats(waterRes: 1),
                conditions: new List<SolutionSortCondition>() { SolutionSortCondition.WaterResistance });
        }

        [Test]
        public void ReturnSortedSolutions_SortByThunderResistance()
        {
            ReturnSortedSolutions_Helper(
                high: MakeSolutionWithTwoPiecesGivenStats(thunderRes: 10),
                mid: MakeSolutionWithTwoPiecesGivenStats(thunderRes: 5),
                low: MakeSolutionWithTwoPiecesGivenStats(thunderRes: 1),
                conditions: new List<SolutionSortCondition>() { SolutionSortCondition.ThunderResistance });
        }

        [Test]
        public void ReturnSortedSolutions_SortByTotalExtraSkillLevels()
        {
            ReturnSortedSolutions_Helper(
                high: MakeSolutionWithTwoPiecesGivenStats(numSkillPoints: 10),
                mid: MakeSolutionWithTwoPiecesGivenStats(numSkillPoints: 5),
                low: MakeSolutionWithTwoPiecesGivenStats(numSkillPoints: 1),
                conditions: new List<SolutionSortCondition>() { SolutionSortCondition.TotalExtraSkillLevels });
        }

        [Test]
        public void ReturnSortedSolutions_SortByTotalExtraDecorationSlots()
        {
            ReturnSortedSolutions_Helper(
                high: MakeSolutionWithTwoPiecesGivenStats(countDecoSlots: 10),
                mid: MakeSolutionWithTwoPiecesGivenStats(countDecoSlots: 5),
                low: MakeSolutionWithTwoPiecesGivenStats(countDecoSlots: 1),
                conditions: new List<SolutionSortCondition>() { SolutionSortCondition.TotalExtraDecorationSlots });
        }

        [Test]
        public void ReturnSortedSolutions_SortByTotalTotalEmptyArmorSlots()
        {
            ReturnSortedSolutions_Helper(
                high: MakeSolutionWithTwoPiecesGivenStats(countExtraPieces: 0),
                mid: MakeSolutionWithTwoPiecesGivenStats(countExtraPieces: 1),
                low: MakeSolutionWithTwoPiecesGivenStats(countExtraPieces: 2),
                conditions: new List<SolutionSortCondition>() { SolutionSortCondition.TotalEmptyArmorSlots });
        }

        [Test]
        public void ReturnSortedSolutions_SortByThunderResistanceThenByFireResistance()
        {
            ReturnSortedSolutions_Helper(
                high: MakeSolutionWithTwoPiecesGivenStats(thunderRes: 10, fireRes: 1),
                mid: MakeSolutionWithTwoPiecesGivenStats(thunderRes: 5, fireRes: 5),
                low: MakeSolutionWithTwoPiecesGivenStats(thunderRes: 5, fireRes: 2),
                conditions: new List<SolutionSortCondition>() {
                    SolutionSortCondition.ThunderResistance,
                    SolutionSortCondition.FireResistance });
        }

        private void ReturnSortedSolutions_Helper(
            Solution high,
            Solution mid,
            Solution low,
            List<SolutionSortCondition> conditions)
        {
            // Might as well test all combinations to be sure, definitely overkill
            IList<IList<Solution>> inputListList = new List<IList<Solution>>() {
                new List<Solution>() { low, mid, high },
                new List<Solution>() { low, high, mid },
                new List<Solution>() { mid, low, high },
                new List<Solution>() { mid, high, low },
                new List<Solution>() { high, mid, low },
                new List<Solution>() { high, low, mid },
            };
            foreach (IList<Solution> inputList in inputListList)
            {
                IList<Solution> outputList = new SolutionSorter().ReturnSortedSolutions(
                    inputList,
                    conditions);

                // Check high -- double check the IsFalse cases just in case of == weirdness
                Assert.IsFalse(outputList[0] == low);
                Assert.IsFalse(outputList[0] == mid);
                Assert.IsTrue(outputList[0] == high);

                // Check mid
                Assert.IsFalse(outputList[1] == low);
                Assert.IsTrue(outputList[1] == mid);
                Assert.IsFalse(outputList[1] == high);

                // Check low
                Assert.IsTrue(outputList[2] == low);
                Assert.IsFalse(outputList[2] == mid);
                Assert.IsFalse(outputList[2] == high);
            }
        }

        private Solution MakeSolutionWithTwoPiecesGivenStats(
            int armorPoints = 100,
            int countDecoSlots = 100,
            int numSkillPoints = 100,
            int fireRes = 100,
            int iceRes = 100,
            int thunderRes = 100,
            int dragonRes = 100,
            int waterRes = 100,
            int countExtraPieces = 1)
        {
            Solution result = new Solution();
            if (countExtraPieces >= 1)
            {
                // Add occupied piece that doesn't do anything
                result.AddNewPiece(new SkillContributor(
                    id: "Armor" + Guid.NewGuid().ToString(),
                    setId: "mockSetId",
                    armorPoints: 0,
                    decoSlots: new List<int>(),
                    slot: ArmorSlot.Head,
                    skills: new List<SkillValue>()));
            }

            if (countExtraPieces >= 2)
            {
                // Add occupied piece that doesn't do anything
                result.AddNewPiece(new SkillContributor(
                    id: "Armor" + Guid.NewGuid().ToString(),
                    setId: "mockSetId",
                    armorPoints: 0,
                    decoSlots: new List<int>(),
                    slot: ArmorSlot.Feet,
                    skills: new List<SkillValue>()));
            }

            result.AddNewPiece(new SkillContributor(
                id: "Armor" + Guid.NewGuid().ToString(),
                setId: "mockSetId",
                armorPoints: armorPoints,
                decoSlots: new List<int>() { countDecoSlots },
                slot: ArmorSlot.Arm,
                skills: new List<SkillValue>() { 
                    new SkillValue("Skill" + Guid.NewGuid().ToString(), numSkillPoints) },
                fireRes: fireRes,
                iceRes: iceRes,
                thunderRes: thunderRes,
                dragonRes: dragonRes,
                waterRes: waterRes));
            return result;
        }
    }
}
