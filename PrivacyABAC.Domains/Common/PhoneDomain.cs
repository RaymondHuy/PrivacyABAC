using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Domains.Common
{
    public class PhoneDomain : IPluginDomain
    {

        public string GetName() => "Phone";

        public string[] GetRegisteredFunctions() => new string[] { "ShowFirstThreeNumber", "ShowLastThreeNumber" };

        public string ExecuteFunction(string functionName, params string[] parameters)
        {
            throw new NotImplementedException();
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
