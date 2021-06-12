using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAASS.Engine.Contract.Interfaces
{
    public interface IAssConfig
    {
        int GetSearchTimeoutSeconds();
        int GetSearchMaxResults();
        bool GetEnableDebugAssertions();

    }
}
