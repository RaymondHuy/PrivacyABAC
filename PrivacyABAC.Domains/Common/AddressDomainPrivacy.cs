using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.Domains.Common
{
    public class AddressDomainPrivacy : IPluginDomain
    {

        public string ShowStreetNumber(string s)
        {
            string[] arr = s.Split(',');
            if (arr.Length > 0)
                return arr[0];
            else return s;
        }

        public string ShowStreetName(string s)
        {
            string[] arr = s.Split(',');
            if (arr.Length > 1)
                return arr[1];
            else return s;
        }

        public string ShowDistrictNumber(string s)
        {
            string[] arr = s.Split(',');
            if (arr.Length > 2)
                return arr[2];
            else return s;
        }

        public string GetName() => "AddressDomain";

        public string[] GetRegisteredFunctions()
        {
            return new string[] { "ShowStreetNumber", "ShowDistrictNumber", "ShowStreetName" };
        }

        public string ExecuteFunction(string functionName, params string[] parameters)
        {
            if (functionName.Equals("ShowStreetNumber", StringComparison.OrdinalIgnoreCase))
                return ShowStreetNumber(parameters[0]);
            else if (functionName.Equals("ShowStreetName", StringComparison.OrdinalIgnoreCase))
                return ShowStreetName(parameters[0]);
            else if (functionName.Equals("ShowDistrictNumber", StringComparison.OrdinalIgnoreCase))
                return ShowDistrictNumber(parameters[0]);

            throw new FunctionNotFoundException(string.Format("Can not find {0}", functionName));
        }
    }
}
