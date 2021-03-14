using JustinsASS.Engine.Contract.DataModel;
using JustinsASS.Engine.Search;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASSTests
{
    public class SearchWorkerTests
    {
        private int countOfDistinctNonDecoSlots;
        private int countOfDistinctSlots;

        [SetUp]
        public void Setup()
        {
            this.countOfDistinctNonDecoSlots =
                new List<ArmorSlot>((ArmorSlot[])Enum.GetValues(typeof(ArmorSlot)))
                    .Where(slot => slot != ArmorSlot.None && slot != ArmorSlot.Deco)
                    .Count();

            this.countOfDistinctSlots =
                new List<ArmorSlot>((ArmorSlot[])Enum.GetValues(typeof(ArmorSlot)))
                    .Where(slot => slot != ArmorSlot.None)
                    .Count();
        }

        [Test]
        public void FindAllSolutions_EmptyTarget()
        {
            List<string> skillIds = GenerateSkillIds(2);
            Inventory mockInventory = GenerateInventory(skillIds);
            SearchWorker worker = new SearchWorker();
            List<Solution> actualSolutions = worker.FindAllSolutions(mockInventory, new SearchTarget(new List<SkillValue>())).ToList();
            Assert.AreEqual(1, actualSolutions.Count());
            actualSolutions.First().Contributors.ForEach(contributor => Assert.IsTrue(contributor is VacantSlot));
        }

        [Test]
        public void FindAllSolutions_OneSkillTarget()
        {
            List<string> skillIds = GenerateSkillIds(2);
            Inventory mockInventory = GenerateInventory(skillIds);
            SearchWorker worker = new SearchWorker();
            List<Solution> actualSolutions = worker.FindAllSolutions(mockInventory, new SearchTarget(new List<SkillValue>()
            {
                new SkillValue(skillIds[0], 2) // 1 point per piece, 2 points target
            })).ToList();
            actualSolutions.ForEach((solution) =>
            {
                // Takes 2 nonvacant slots to get the skill
                Assert.AreEqual(2, solution.Contributors.Where(contr => !(contr is VacantSlot)).Count());
                Assert.AreEqual(6, solution.Contributors.Count());
            });

            // N choose 2 solutions
            int expectedSolutions = biCoefficient(countOfDistinctNonDecoSlots, 2);
            Assert.AreEqual(expectedSolutions, actualSolutions.Count(),
                $"Got {actualSolutions.Count()} solutions, expected {expectedSolutions}. Solutions:{string.Join(",", actualSolutions)}");
        }

        private Inventory GenerateInventory(
            List<string> skillIds,
            bool generateDecos = true)
        {
            List<SkillContributor> availableContributors = new List<SkillContributor>();
            for (int i = 0; i < skillIds.Count(); i++)
            {
                foreach (ArmorSlot slot in Enum.GetValues(typeof(ArmorSlot)))
                {
                    if (slot == ArmorSlot.None || (!generateDecos && slot == ArmorSlot.Deco))
                    {
                        continue;
                    }
                    availableContributors.Add(new SkillContributor(
                        id: $"mockSkillContributorId_{skillIds[i]}_slot{slot}",
                        armorPoints: 0,
                        decoSlots: makeDecoSlotList(0, 0, 0, 0),
                        slot: slot,
                        new List<SkillValue>() { new SkillValue(skillIds[i], 1) }));
                }
            }
            return new Inventory(availableContributors);
        }

        private List<string> GenerateSkillIds(int numSkills)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < numSkills; i++)
            {
                result.Add($"testSkillId_{i}");
            }
            return result;
        }

        private List<int> makeDecoSlotList(int size1Slots, int size2Slots, int size3Slots, int size4Slots)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < size1Slots; i++)
            {
                result.Add(1);
            }
            for (int i = 0; i < size2Slots; i++)
            {
                result.Add(2);
            }
            for (int i = 0; i < size3Slots; i++)
            {
                result.Add(3);
            }
            for (int i = 0; i < size4Slots; i++)
            {
                result.Add(size4Slots);
            }
            return result;
        }

        private static int biCoefficient(int n, int k)
        {
            if (k > n - k)
            {
                k = n - k;
            }

            int c = 1;
            for (int i = 0; i < k; i++)
            {
                c = c * (n - i);
                c = c / (i + 1);
            }
            return c;
        }
    }
}