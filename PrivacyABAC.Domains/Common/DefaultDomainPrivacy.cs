using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Domains.Common
{
    public class DefaultDomainPrivacy : IPluginDomain
    {

        public string GetName() => "Default";

        public string[] GetRegisteredFunctions() => new string[] { "Show", "Hide" };

        public string ExecuteFunction(string functionName, params string[] parameters)
        {
            if (functionName.Equals("Show", StringComparison.OrdinalIgnoreCase))
                return Show(parameters[0]);
            else if (functionName.Equals("Hide", StringComparison.OrdinalIgnoreCase))
                return Hide(parameters[0]);

            throw new FunctionNotFoundException(string.Format("Can not find {0}", functionName));
        }

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
