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
            Inventory mockInventory = GenerateInventory(skillIds, generateDecos: false);
            SearchWorker worker = new SearchWorker();
            List<Solution> actualSolutions = worker.FindAllSolutions(mockInventory, new SearchTarget(new List<SkillValue>())).ToList();
            Assert.AreEqual(1, actualSolutions.Count());
            actualSolutions.First().Contributors.ForEach(contributor => Assert.IsTrue(contributor is VacantSlot));
            AssertListDistinct(actualSolutions);
        }

        [Test]
        public void FindAllSolutions_OneSkillTarget()
        {
            List<string> skillIds = GenerateSkillIds(2);
            Inventory mockInventory = GenerateInventory(skillIds, generateDecos: false);
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
            AssertListDistinct(actualSolutions);
        }

        [Test]
        public void FindAllSolutions_TwoSkillTarget()
        {
            List<string> skillIds = GenerateSkillIds(2);
            Inventory mockInventory = GenerateInventory(skillIds, generateDecos: false);
            SearchWorker worker = new SearchWorker();
            List<Solution> actualSolutions = worker.FindAllSolutions(mockInventory, new SearchTarget(new List<SkillValue>()
            {
                new SkillValue(skillIds[0], 2),
                new SkillValue(skillIds[1], 2)
            })).ToList();
            actualSolutions.ForEach((solution) =>
            {
                // Takes 2 nonvacant slots to get the skill
                Assert.AreEqual(4, solution.Contributors.Where(contr => !(contr is VacantSlot)).Count());
                Assert.AreEqual(6, solution.Contributors.Count());
            });

            // Number solutions too hard to calculate; assert invariants
            AssertListDistinct(actualSolutions);
        }

        [Test]
        public void FindAllSolutions_Decorations_OneSlotDecos()
        {
            List<string> skillIds = GenerateSkillIds(2);
            Inventory mockInventory = GenerateInventory(skillIds, maxSkillValuesForDecos: 1, generateDecos: true);
            SearchWorker worker = new SearchWorker();
            List<Solution> actualSolutions = worker.FindAllSolutions(mockInventory, new SearchTarget(new List<SkillValue>()
            {
                new SkillValue(skillIds[0], 7)
            })).ToList();
            actualSolutions.ForEach((solution) =>
            {
                // Takes 2 nonvacant slots to get the skill
                int countVacantSlots = solution.Contributors.Where(contr => !(contr is VacantSlot)).Count();
                Assert.AreEqual(0, countVacantSlots,
                    $"Expected 4 empty slots but was {countVacantSlots}, solution: {solution}");
                Assert.AreEqual(7, solution.Contributors.Count());
            });

            // Number solutions too hard to calculate; assert invariants
            AssertListDistinct(actualSolutions);
        }

        [Test]
        public void FindAllSolutions_Decorations_OversizeSlotDecos()
        {
            List<string> skillIds = GenerateSkillIds(2);
            Dictionary<int, int> slotSizes = new Dictionary<int, int>()
            {
                { 1, 0 },
                { 2, 0 },
                { 3, 0 },
                { 4, 1 },
            };
            Inventory mockInventory = GenerateInventory(skillIds, maxSkillValuesForDecos: 1, generateDecos: true);
            SearchWorker worker = new SearchWorker();
            List<Solution> actualSolutions = worker.FindAllSolutions(mockInventory, new SearchTarget(new List<SkillValue>()
            {
                new SkillValue(skillIds[0], 7)
            })).ToList();
            actualSolutions.ForEach((solution) =>
            {
                // Takes 2 nonvacant slots to get the skill
                int countVacantSlots = solution.Contributors.Where(contr => !(contr is VacantSlot)).Count();
                Assert.AreEqual(0, countVacantSlots,
                    $"Expected 4 empty slots but was {countVacantSlots}, solution: {solution}");
                Assert.AreEqual(7, solution.Contributors.Count());
            });

            // Number solutions too hard to calculate; assert invariants
            AssertListDistinct(actualSolutions);
        }

        [Test]
        public void FindAllSolutions_LargeSearchSet()
        {
            List<string> skillIds = GenerateSkillIds(6);
            Inventory mockInventory = GenerateInventory(skillIds, maxSkillValues: 5, generateDecos: false);
            SearchWorker worker = new SearchWorker();
            List<Solution> actualSolutions = worker.FindAllSolutions(mockInventory, new SearchTarget(new List<SkillValue>()
            {
                new SkillValue(skillIds[0], 5),
                new SkillValue(skillIds[1], 5),
                new SkillValue(skillIds[2], 5),
                new SkillValue(skillIds[3], 5),
                new SkillValue(skillIds[4], 5)
            })).ToList();
            actualSolutions.ForEach((solution) =>
            {
                // Takes 2 nonvacant slots to get the skill
                Assert.AreEqual(4, solution.Contributors.Where(contr => !(contr is VacantSlot)).Count());
                Assert.AreEqual(6, solution.Contributors.Count());
            });

            // Number solutions too hard to calculate; assert invariants
            AssertListDistinct(actualSolutions);
        }

        [Test]
        public void FindAllSolutions_LargeSearchSet_ManyIrrelevantSkills()
        {
            List<string> skillIds = GenerateSkillIds(40);
            Inventory mockInventory = GenerateInventory(skillIds, maxSkillValues: 5, generateDecos: false);
            SearchWorker worker = new SearchWorker();
            List<Solution> actualSolutions = worker.FindAllSolutions(mockInventory, new SearchTarget(new List<SkillValue>()
            {
                new SkillValue(skillIds[0], 5),
                new SkillValue(skillIds[1], 5),
                new SkillValue(skillIds[2], 5),
                new SkillValue(skillIds[3], 5),
                new SkillValue(skillIds[4], 5)
            })).ToList();
            actualSolutions.ForEach((solution) =>
            {
                // Takes 2 nonvacant slots to get the skill
                Assert.AreEqual(4, solution.Contributors.Where(contr => !(contr is VacantSlot)).Count());
                Assert.AreEqual(6, solution.Contributors.Count());
            });

            // Number solutions too hard to calculate; assert invariants
            AssertListDistinct(actualSolutions);
        }

        private Inventory GenerateInventory(
            List<string> skillIds,
            bool generateDecos = true,
            int maxSkillValues = 1,
            int maxSkillValuesForDecos = 1,
            Dictionary<int, int> slotSizesPerArmor = null,
            List<SkillContributor> specialContributors = null)
        {
            if (slotSizesPerArmor == null)
            {
                slotSizesPerArmor = new Dictionary<int, int>()
                {
                    { 1, 1 },
                    { 2, 0 },
                    { 3, 0 },
                    { 4, 0 }
                };
            }
            List<SkillContributor> availableContributors = new List<SkillContributor>();
            if (specialContributors != null)
            {
                availableContributors.AddRange(specialContributors);
            }

            for (int i = 0; i < skillIds.Count(); i++)
            {
                foreach (ArmorSlot slot in Enum.GetValues(typeof(ArmorSlot)))
                {
                    if (slot == ArmorSlot.None || (!generateDecos && slot == ArmorSlot.Deco))
                    {
                        continue;
                    }

                    if (slot == ArmorSlot.Deco)
                    {
                        for (int j = 1; j <= maxSkillValuesForDecos; j++)
                        {
                            availableContributors.Add(new Decoration(
                                id: $"mockSkillContributorId_{skillIds[i]}_slot{slot}",
                                slotSize: 1,
                                skillValues: new List<SkillValue>() { new SkillValue(skillIds[i], j) }));
                        }
                    }
                    else
                    {
                        for (int j = 1; j <= maxSkillValues; j++)
                        {
                            availableContributors.Add(new SkillContributor(
                                id: $"mockSkillContributorId_{skillIds[i]}_slot{slot}",
                                setId: "mockSetId",
                                armorPoints: 0,
                                decoSlots: makeDecoSlotList(slotSizesPerArmor[1], slotSizesPerArmor[2], slotSizesPerArmor[3], slotSizesPerArmor[4]),
                                slot: slot,
                                new List<SkillValue>() { new SkillValue(skillIds[i], j) }));
                        }
                    }
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
                result.Add(4);
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

        private static void AssertListDistinct(List<Solution> solutions)
        {
            HashSet<Solution> distinctSolutions = new HashSet<Solution>(solutions);
            Assert.AreEqual(solutions.Count(), distinctSolutions.Count(),
                $"Solutions list contained {solutions.Count() - distinctSolutions.Count()} duplicates.");
        }
    }
}