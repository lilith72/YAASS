using JustinsASS.Engine.Contract.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustinsASS.Engine.Data
{
    public class AssConfigProvider : IAssConfigProvider
    {
        private const string configFileDirectory = "./AppData/Config/";
        private const string configFileName = "config.json";
        private FileSystemWatcher fileSystemWatcher;
        private IAssConfig currentConfig;

        public AssConfigProvider()
        {
            if (currentConfig == null)
            {
                OnConfigFileChanged(null, null);
                this.fileSystemWatcher = new FileSystemWatcher(configFileDirectory);
                this.fileSystemWatcher.Changed += OnConfigFileChanged;
                this.fileSystemWatcher.Created += OnConfigFileChanged;
                this.fileSystemWatcher.Deleted += OnConfigFileChanged;
                this.fileSystemWatcher.Renamed += OnConfigFileChanged;
                this.fileSystemWatcher.Filter = "*.*";
                this.fileSystemWatcher.EnableRaisingEvents = true;
            }
        }

        public IAssConfig GetConfig()
        {
            return this.currentConfig;
        }

        private IAssConfig ReadAssConfigFromFile()
        {
            using (var stream = File.Open(GetConfigFilePath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var sr = new StreamReader(stream))
                {
                    string configFileContents = sr.ReadToEnd();
                    AssConfig result = JsonConvert.DeserializeObject<AssConfig>(configFileContents);
                    return result;
                }
            }
        }

        private void OnConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File watcher fired, updating config");
            this.currentConfig = ReadAssConfigFromFile();
        }

        private string GetConfigFilePath()
        {
            return configFileDirectory + configFileName;
        }
    }
}
