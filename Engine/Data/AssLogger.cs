using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAASS.Engine.Contract.DataModel;
using YAASS.Engine.Contract.Interfaces;

namespace YAASS.Engine.Data
{
    public class AssLogger : IAssLogger
    {
        private const string logsFileName = "assLogs.txt";
        private object LogLock = new object();
        private IAssConfigProvider config;
        public AssLogger(IAssConfigProvider config)
        {
            this.config = config;
        }

        public void Log(string message, AssLogLevel level)
        {
            string logString = BuildLogString(message, level);
            Console.WriteLine(logString);
            if (this.config.GetConfig().GetEnableLoggingToDisk())
            {
                lock (LogLock)
                {
                    try
                    {
                        File.AppendAllLines(
                            $"{config.GetConfig().GetLogOutputFolder()}/{logsFileName}",
                            new List<string>() { logString });
                    }
                    catch (DirectoryNotFoundException)
                    {
                        Directory.CreateDirectory(config.GetConfig().GetLogOutputFolder());
                        File.AppendAllLines(
                            $"{config.GetConfig().GetLogOutputFolder()}/{logsFileName}",
                            new List<string>() { logString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"DEBUG WARNING -- failed to write to disk with exception {e}");
                        // Give up on logging to disk -- this can sometimes happen 
                    }
                }
            }
        }

        private string BuildLogString(string message, AssLogLevel level)
        {
            return $"{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff tt")},{level.ToString("G")},{message}";
        }
    }
}
