using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustinsASS.Engine.Contract.DataModel;
using Newtonsoft.Json;
using System.IO;

namespace JustinsASS.Engine.Data
{
    public class PersistedStorageHelper
    {
        private const string TalismansFileName = "./UserData/Talismans.json";

        // talisman id -> talisman
        private Dictionary<string, SkillContributor> customTalismans;

        public PersistedStorageHelper()
        {
            this.customTalismans = new Dictionary<string, SkillContributor>();
        }

        public bool TryAddTalisman(
            SkillContributor sc)
        {
            // TODO check for conflicts against pinned sets
            customTalismans.Add(sc.SkillContributorId, sc);
            TryPersistObjectToFile(this.customTalismans, TalismansFileName);
            return false;
        }

        public Dictionary<string, SkillContributor> GetCustomTalismans()
        {
            return this.customTalismans;        
        }

        public static bool TryPersistObjectToFile<T>(
            T data,
            string fileName)
        {
            try
            {
                string serializedData = JsonConvert.SerializeObject(data);
                Console.WriteLine($"Serialized json object: {serializedData}");
                File.WriteAllText(fileName, serializedData);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception occurred while trying to persist data. {e}");
                return false;
            }
        }

        public static T TryFetchObjectFromFile<T>(
            T data,
            string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
