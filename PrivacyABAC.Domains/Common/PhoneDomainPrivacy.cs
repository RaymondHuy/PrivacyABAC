using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Domains.Common
{
    public class PhoneDomainPrivacy : IPluginDomain
    {

        public string GetName() => "PhoneDomain";

        public string[] GetRegisteredFunctions() => new string[] { "ShowFirstThreeNumber", "ShowLastThreeNumber" };

        public string ExecuteFunction(string functionName, params string[] parameters)
        {
            if (functionName.Equals("ShowFirstThreeNumber", StringComparison.OrdinalIgnoreCase))
                return ShowFirstThreeNumber(parameters[0]);
            else if (functionName.Equals("ShowLastThreeNumber", StringComparison.OrdinalIgnoreCase))
                return ShowLastThreeNumber(parameters[0]);

            throw new FunctionNotFoundException(string.Format("Can not find {0}", functionName));
        }

        public string ShowFirstThreeNumber(string s)
        {
            string firstThreeNumber = string.Empty;
            if (s.Length > 3)
            {
                firstThreeNumber = s.Substring(0, 3);
                firstThreeNumber = string.Format("{0}xxxxxxx", firstThreeNumber);
            }
            else firstThreeNumber = s;
            return firstThreeNumber;
        }

        public string ShowLastThreeNumber(string s)
        {
            string lastThreeNumber = string.Empty;
            if (s.Length > 3)
            {
                lastThreeNumber = s.Substring(s.Length - 3, 3);
                lastThreeNumber = string.Format("xxxxxxx{0}", lastThreeNumber);
            }
            else lastThreeNumber = s;
            return lastThreeNumber;
        }
    }
}
