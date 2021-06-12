using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAASS.Engine.Contract.DataModel;

namespace YAASS.Engine.Contract.Interfaces
{
    public interface IAssLogger
    {
        void Log(string message, AssLogLevel level);
    }
}
