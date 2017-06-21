using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Functions
{
    public interface IPluginFunction
    {
        string GetClassName();

        string ExecuteFunction(string functionName, params string[] parameters);

        FunctionInfo[] GetRegisteredFunctions();
    }
}
