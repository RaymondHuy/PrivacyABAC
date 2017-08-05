
using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.Domains.Common
{
    public class SSNDomainPrivacy : IPluginDomain
    {

        public string AreaNumber(string ssn)
        {
            return ssn.Split('-')[0];
        }
        public string GroupNumber(string ssn)
        {
            return ssn.Split('-')[1];
        }
        public string SerialNumber(string ssn)
        {
            return ssn.Split('-')[2];
        }

        public string ExecuteFunction(string functionName, string[] parameters)
        {
            if (functionName.Equals("AreaNumber", StringComparison.OrdinalIgnoreCase))
                return AreaNumber(parameters[0]);
            else if (functionName.Equals("GroupNumber", StringComparison.OrdinalIgnoreCase))
                return GroupNumber(parameters[0]);
            else if (functionName.Equals("SerialNumber", StringComparison.OrdinalIgnoreCase))
                return SerialNumber(parameters[0]);

            throw new FunctionNotFoundException(string.Format("Can not find {0}", functionName));
        }

        public string GetName() => "SSNDomain";

        public string[] GetRegisteredFunctions()
        {
            return new string[] { "AreaNumber", "GroupNumber", "SerialNumber" };
        }
    }
}
