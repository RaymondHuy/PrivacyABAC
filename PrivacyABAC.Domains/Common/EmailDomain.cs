using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Domains.Common
{
    public class EmailDomain : IPluginDomain
    {

        public string GetName() => "EmailDomain";

        public string[] GetRegisteredFunctions() => new string[] { "HideName" };

        public string ExecuteFunction(string functionName, params string[] parameters)
        {
            if (functionName.Equals("HideName", StringComparison.OrdinalIgnoreCase))
                return HideName(parameters[0]);

            throw new FunctionNotFoundException(string.Format("Can not find {0}", functionName));
        }

        public string HideName(string email)
        {
            var tokens = email.Split("@");
            if (tokens.Length > 1)
                return $"x@{tokens[1]}";
            else return email;
        }
        
    }
}
