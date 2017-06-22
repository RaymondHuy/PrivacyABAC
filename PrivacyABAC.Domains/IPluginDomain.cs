using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Domains
{
    public interface IPluginDomain
    {
        string GetName();

        string[] GetRegisteredFunctions();

        string ExecuteFunction(string functionName, params string[] parameters);
    }
}
