using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustinsASS.Engine.Contract.DataModel;
using Newtonsoft.Json;
using System.IO;
using JustinsASS.Engine.Contract.Interfaces;

namespace JustinsASS.Engine.Data
{
    public class PersistedStorageHelper : IPersistedStorageHelper
    {
        private const string UserDataFolderPath = "./AppData/UserData/";
        private const string TalismansFileName = "Talismans.json";

        // talisman id -> talisman
        private readonly Dictionary<string, SkillContributor> customTalismans;

        public PersistedStorageHelper()
        {
            if (!TryFetchObjectFromFile($"{UserDataFolderPath}{TalismansFileName}", out customTalismans))
            {
                this.customTalismans = new Dictionary<string, SkillContributor>();
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
    }
}
