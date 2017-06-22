using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Domains.Common
{
    class DefaultDomain : IPluginDomain
    {
        public string ExecuteFunction(string functionName, params string[] parameters)
        {
            throw new NotImplementedException();
        }

        public string GetName() => "Default";

        public string[] GetRegisteredFunctions() => new string[] { "Show", "Hide" };

        public string Show(string s)
        {
            return s;
        }

        public string Hide(string s)
        {
            return "";
        }
    }
}
