using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAASS.Engine.Contract.DataModel;
using Newtonsoft.Json;
using System.IO;
using YAASS.Engine.Contract.Interfaces;

namespace YAASS.Engine.Data
{
    public class PersistedStorageHelper : IPersistedStorageHelper
    {
        private string UserDataFolderPath;
        private const string TalismansFileName = "Talismans.json";
        private const string PinnedSolutionsFileName = "PinnedSolutions.json";

        // talisman id -> talisman
        private readonly Dictionary<string, SkillContributor> customTalismans;
        private readonly HashSet<Solution> pinnedSolutions;

        public PersistedStorageHelper()
        {
            UserDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/YAASS/UserData/";
            if (!TryFetchObjectFromFile($"{UserDataFolderPath}{TalismansFileName}", out customTalismans))
            {
                this.customTalismans = new Dictionary<string, SkillContributor>();
            }
            if (!TryFetchObjectFromFile($"{UserDataFolderPath}{PinnedSolutionsFileName}", out pinnedSolutions))
            {
                this.pinnedSolutions = new HashSet<Solution>();
            }
        }

        public bool TryAddTalisman(
            SkillContributor sc)
        {
            if (customTalismans.ContainsKey(sc.SkillContributorId))
            {
                return false;
            }
            customTalismans.Add(sc.SkillContributorId, sc);
            TryPersistObjectToFile(this.customTalismans, UserDataFolderPath, TalismansFileName);
            return false;
        }

        public bool TryRemoveTalisman(
            string talismanId)
        {
            // TODO check for conflicts against pinned sets, when those exist
            if (this.customTalismans.Remove(talismanId))
            {
                return TryPersistObjectToFile(this.customTalismans, UserDataFolderPath, TalismansFileName);
            }
            return false;
        }

        public Dictionary<string, SkillContributor> GetCustomTalismans()
        {
            return this.customTalismans;        
        }

        public static bool TryPersistObjectToFile<T>(
            T data,
            string parentDirectory,
            string fileName)
        {
            try
            {
                if (!Directory.Exists(parentDirectory))
                {
                    Directory.CreateDirectory(parentDirectory);
                }
                string serializedData = JsonConvert.SerializeObject(data);
                Console.WriteLine($"Serialized json object: {serializedData}");
                File.WriteAllText($"{parentDirectory}{fileName}", serializedData);
                Console.WriteLine($"Finished persisting data.");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception occurred while trying to persist data. {e}");
                return false;
            }
        }

        public static bool TryFetchObjectFromFile<T>(
            string fileFullPath,
            out T result)
        {
            try
            {
                string data = File.ReadAllText(fileFullPath);
                result = JsonConvert.DeserializeObject<T>(data);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception occurred while trying to read data. {e}");
                result = default;
                return false;
            }
        }

        public void PinSolution(
            Solution s)
        {
            this.pinnedSolutions.Add(s);
            TryPersistObjectToFile(this.pinnedSolutions, UserDataFolderPath, PinnedSolutionsFileName);
        }

        public ISet<Solution> FetchAllPinnedSolutions()
        {
            return this.pinnedSolutions;
        }

        public bool TryUnpinSolution(Solution s, out string errorMessage)
        {
            try
            {
                this.pinnedSolutions.Remove(s);
                TryPersistObjectToFile(this.pinnedSolutions, UserDataFolderPath, PinnedSolutionsFileName);
                errorMessage = null;
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }
    }
}
