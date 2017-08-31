using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Domains.Common
{
    public class DefaultDomainPrivacy : IPluginDomain
    {

        public string GetName() => "DefaultDomainPrivacy";

        public string[] GetRegisteredFunctions() => new string[] { "Show", "Hide" };

        public string ExecuteFunction(string functionName, params string[] parameters)
        {
            if (functionName.Equals("Show", StringComparison.OrdinalIgnoreCase))
                return Show(parameters[0]);
            else if (functionName.Equals("Hide", StringComparison.OrdinalIgnoreCase))
                return Hide(parameters[0]);
            else if (string.Equals(functionName, "SubHide200"))
                return SubHide200(parameters[0].ToString());

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

        public string SubHide200(string s)
        {
            if (s.Length > 200)
                return s.Substring(0, 199);
            else return s;
        }
    }
}
