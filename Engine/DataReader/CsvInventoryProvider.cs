using JustinsASS.Engine.Contract.DataModel;
using JustinsASS.Engine.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.DataReader
{
    public class CsvInventoryProvider : IInventoryProvider
    {
        private const string SkillContributorFilePath = "./AppData/SkillContributors/skillContributors.csv";
        private const string SkillMetadataFilePath = "./AppData/SkillMetadata/skillMetadata.csv";

        public List<SkillContributor> GetSkillContributors()
        {
            List<SkillContributor> results = new List<SkillContributor>();
            HashSet<string> seenArmorNames = new HashSet<string>();
            foreach (Dictionary<string, string> entry in this.GetCsvRows(SkillContributorFilePath))
            {
                List<SkillValue> skillValues = GetSkillValuesFromCsvEntry(entry);
                ArmorSlot slot = (ArmorSlot) Enum.Parse(typeof(ArmorSlot), entry["slotType"], ignoreCase: true);
                string itemId = entry["ItemUniqueName"];
                string setId = entry["SetId"];
                if (seenArmorNames.Contains(itemId))
                {
                    throw new Exception($"Found duplicate itemId in skillContributors: {itemId}");
                }
                seenArmorNames.Add(itemId);
                if (slot == ArmorSlot.Deco)
                {
                    int slotSize = int.Parse(entry["decoSlotSizeIfDeco"]);
                    results.Add(new Decoration(itemId, slotSize, skillValues));
                }
                else
                {
                    // Any empty field will default to 0
                    int.TryParse(entry["armorPoints"], out int armorPoints);
                    int.TryParse(entry["size1DecoSlots"], out int slotsSize1);
                    int.TryParse(entry["size2DecoSlots"], out int slotsSize2);
                    int.TryParse(entry["size3DecoSlots"], out int slotsSize3);
                    int slotsSize4 = 0;
                    int.TryParse(entry["FireResist"], out int fireRes);
                    int.TryParse(entry["IceResist"], out int iceRes);
                    int.TryParse(entry["WaterResist"], out int waterRes);
                    int.TryParse(entry["ThunderResist"], out int thunderRes);
                    int.TryParse(entry["DragonResist"], out int dragonRes);
                    //int.TryParse(entry["MinRank"], out int minRank);
                    List<int> decoSlots = new List<int>();
                    for (int i = 0; i < slotsSize1; i++)
                    {
                        decoSlots.Add(1);
                    }
                    for (int i = 0; i < slotsSize2; i++)
                    {
                        decoSlots.Add(2);
                    }
                    for (int i = 0; i < slotsSize3; i++)
                    {
                        decoSlots.Add(3);
                    }
                    for (int i = 0; i < slotsSize4; i++)
                    {
                        decoSlots.Add(4);
                    }
                    results.Add(new SkillContributor(
                        id: itemId,
                        armorPoints: armorPoints,
                        decoSlots,
                        slot: slot,
                        skills: skillValues,
                        setId: string.IsNullOrWhiteSpace(setId) ? null : setId,
                        fireRes: fireRes,
                        iceRes: iceRes,
                        waterRes: waterRes,
                        thunderRes: thunderRes,
                        dragonRes: dragonRes));
                }
            }
            return results;
        }

        // #Fields:SkillName,MaxLevel
        public Dictionary<string, int> GetSkillNameToMaxLevelMapping()
        {
            Dictionary<string, int> result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (Dictionary<string, string> entry in this.GetCsvRows(SkillMetadataFilePath))
            {
                string skillName = entry["SkillName"];
                if (result.ContainsKey(skillName) || string.IsNullOrWhiteSpace(skillName))
                {
                    throw new Exception($"Duplicate or empty key in skill metadata: {result}");
                }
                int maxLevel = int.Parse(entry["MaxLevel"]);
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
                string skillNameKey = $"SkillName{i}";
                string skillPointsKey = $"SkillPoints{i}";
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
            if (!csvFileLines[0].StartsWith("#Fields:"))
            {
                throw new Exception("input CSV file was malformed; expected first line to define schema in the form of #Fields:field1,field2,...");
            }
            string[] keys = csvFileLines[0].Remove(0, "#Fields:".Length).Split(new[] { "," }, StringSplitOptions.None);
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
                    lineResults[keys[j]] = values[j];
                }
                yield return lineResults;
            }
        }
    }
}
