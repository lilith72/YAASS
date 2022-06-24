using YAASS.Engine.Contract.DataModel;
using YAASS.Engine.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAASS.Engine.DataReader
{
    public class CsvInventoryProvider : IInventoryProvider
    {
        private const string SkillContributorFilePath = "./AppData/SkillContributors/skillContributors.csv";
        private const string SunbreakSkillContributorsFilePath = "./AppData/SkillContributors/sunbreakSkillContributors.csv";
        private const string SkillMetadataFilePath = "./AppData/SkillMetadata/skillMetadata.csv";
        // Decos could be combined into skill contributors file but this makes data entry easier
        // plus it helps keep data organized
        private const string DecorationsFilePath = "./AppData/Decorations/decorations.csv";

        public List<SkillContributor> GetSkillContributors(
            bool useHighRankArmors,
            bool useGRankArmors)
        {
            List<SkillContributor> results = new List<SkillContributor>();
            HashSet<string> seenArmorNames = new HashSet<string>();
            IEnumerable<Dictionary<string, string>> rawData = Enumerable.Empty<Dictionary<string, string>>();
            if (useHighRankArmors)
            {
                rawData = rawData.Union(this.GetCsvRows(SkillContributorFilePath));
            }
            if (useGRankArmors)
            {
                rawData = rawData.Union(this.GetCsvRows(SunbreakSkillContributorsFilePath));
            }
            foreach (Dictionary<string, string> entry in this.GetCsvRows(SkillContributorFilePath))
            {
                List<SkillValue> skillValues = GetSkillValuesFromCsvEntry(entry);
                ArmorSlot slot = (ArmorSlot) Enum.Parse(typeof(ArmorSlot), entry["Equip Type"], ignoreCase: true);
                string itemId = entry["Name"];
                string setId = entry["Set ID"];
                if (seenArmorNames.Contains(itemId))
                {
                    throw new Exception($"Found duplicate itemId in skillContributors: {itemId}");
                }
                seenArmorNames.Add(itemId);
                // TODO broken: decos will be read on separate page
                /*if (slot == ArmorSlot.Deco)
                {
                    int slotSize = int.Parse(entry["decoSlotSizeIfDeco"]);
                    results.Add(new Decoration(itemId, slotSize, skillValues));
                }*/
                // Any empty field will default to 0
                int.TryParse(entry["Armor Points"], out int armorPoints);
                int.TryParse(entry["Size Deco Slot 1"], out int slotsSize1);
                int.TryParse(entry["Size Deco Slot 2"], out int slotsSize2);
                int.TryParse(entry["Size Deco Slot 3"], out int slotsSize3);
                //int slotsSize4 = 0;
                int.TryParse(entry["Fire Res"], out int fireRes);
                int.TryParse(entry["Ice Res"], out int iceRes);
                int.TryParse(entry["Water Res"], out int waterRes);
                int.TryParse(entry["Thunder Res"], out int thunderRes);
                int.TryParse(entry["Dragon Res"], out int dragonRes);
                //int.TryParse(entry["MinRank"], out int minRank);
                List<int> decoSlots = new List<int>();
                if (!string.IsNullOrEmpty(entry["Size Deco Slot 1"])) decoSlots.Add(slotsSize1);
                if (!string.IsNullOrEmpty(entry["Size Deco Slot 2"])) decoSlots.Add(slotsSize2);
                if (!string.IsNullOrEmpty(entry["Size Deco Slot 3"])) decoSlots.Add(slotsSize3);

                results.Add(new SkillContributor(
                    skillContributorId: itemId,
                    armorPoints: armorPoints,
                    decoSlots,
                    slot: slot,
                    providedSkillValues: skillValues,
                    setId: string.IsNullOrWhiteSpace(setId) ? null : setId,
                    fireRes: fireRes,
                    iceRes: iceRes,
                    waterRes: waterRes,
                    thunderRes: thunderRes,
                    dragonRes: dragonRes));
            }

            foreach (Dictionary<string, string> entry in this.GetCsvRows(DecorationsFilePath))
            {
                List<SkillValue> skillValues = GetSkillValuesFromCsvEntry(entry);
                string itemId = entry["Name"];
                if (seenArmorNames.Contains(itemId))
                {
                    throw new Exception($"Found duplicate itemId in skillContributors: {itemId}");
                }
                seenArmorNames.Add(itemId);

                int slotSize = int.Parse(entry["Slot Size"]);

                results.Add(new Decoration(itemId, slotSize, GetSkillValuesFromCsvEntry(entry)));
            }

            return results;
        }

        // #Fields:SkillName,MaxLevel
        public Dictionary<string, int> GetSkillNameToMaxLevelMapping()
        {
            Dictionary<string, int> result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (Dictionary<string, string> entry in this.GetCsvRows(SkillMetadataFilePath))
            {
                string skillName = entry["Skill Name"];
                if (result.ContainsKey(skillName) || string.IsNullOrWhiteSpace(skillName))
                {
                    throw new Exception($"Duplicate or empty key in skill metadata: {result}");
                }
                int maxLevel = int.Parse(entry["Max Level"]);
                result[skillName] = maxLevel;
            }
            return result;
        }

        private List<SkillValue> GetSkillValuesFromCsvEntry(Dictionary<string, string> csvEntry)
        {
            List<SkillValue> results = new List<SkillValue>();
            // lazy implementation; Assume they're named SkillNameX and SkillPointsX respectively
            for (int i = 1; i < 8; i++)
            {
                string skillNameKey = $"Skill Name {i}";
                string skillPointsKey = $"Skill Points {i}";
                if (csvEntry.ContainsKey(skillNameKey) && !string.IsNullOrWhiteSpace(csvEntry[skillNameKey]))
                {
                    results.Add(new SkillValue(csvEntry[skillNameKey], int.Parse(csvEntry[skillPointsKey])));
                }
            }
            return results;
        }

        private IEnumerable<Dictionary<string, string>> GetCsvRows(string filePath)
        {
            string[] csvFileLines = File.ReadAllLines(filePath);
            // Assume that first line defines schema
            string[] keys = csvFileLines[0].Split(new[] { "," }, StringSplitOptions.None);
            for (int i = 1; i < csvFileLines.Length; i++)
            {
                string line = csvFileLines[i];
                if (line.StartsWith("#"))
                {
                    continue;
                }

                Dictionary<string, string> lineResults = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string[] values = line.Split(new[] { "," }, StringSplitOptions.None);
                for (int j = 0; j < values.Length; j++)
                {
                    lineResults[keys[j]] = values[j].Trim();
                }
                yield return lineResults;
            }
        }
    }
}
