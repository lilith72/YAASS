using YAASS.Engine.Contract.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAASS.Engine.Data
{
    public class AssConfig : IAssConfig
    {
        private bool EnableDebugAssertions = false;
        private int SearchMaxResults = 100;
        private int SearchTimeoutSeconds = 120;

        [JsonConstructor]
        public AssConfig(
            bool EnableDebugAssertions = false,
            int SearchMaxResults = 100,
            int SearchTimeoutSeconds = 120)
        {
            this.EnableDebugAssertions = EnableDebugAssertions;
            this.SearchMaxResults = SearchMaxResults;
            this.SearchTimeoutSeconds = SearchTimeoutSeconds;
        }

        // Steps to add new config:
        // 1. Add public method below here to get config value
        // 2. Add the same to IAssConfig
        // 3. add private field for config value
        // 4. add param to AssConfig JsonConstructor

        public bool GetEnableDebugAssertions()
        {
            return this.EnableDebugAssertions;
        }

        public int GetSearchMaxResults()
        {
            return this.SearchMaxResults;
        }

        public int GetSearchTimeoutSeconds()
        {
            return this.SearchTimeoutSeconds;
        }
    }
}
