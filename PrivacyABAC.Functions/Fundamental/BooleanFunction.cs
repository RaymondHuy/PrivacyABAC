using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PrivacyABAC.Functions.Fundamental
{
    public class BooleanFunction : IPluginFunction
    {
        public string GetClassName() => "Boolean";

        public FunctionInfo[] GetRegisteredFunctions()
        {
            return new FunctionInfo[]
            {
                new FunctionInfo("Equal", 2),
                new FunctionInfo("GreaterThan", 2),
                new FunctionInfo("LessThan", 2)
            };
        }

        public bool Equal(string s1, string s2)
        {
            bool b1, b2 = false;
            bool valid = bool.TryParse(s1, out b1) && bool.TryParse(s2, out b2);
            if (valid)
                return b1 == b2;
            else throw new InvalidFormatException("Can not execute Equal function between two parameters : " + s1 + " " + s2);
        }

        public string ExecuteFunction(string functionName, object[] parameters)
        {
            if (functionName.Equals("Equal", StringComparison.OrdinalIgnoreCase))
                return Equal(parameters[0].ToString(), parameters[1].ToString()).ToString();

            throw new FunctionNotFoundException(string.Format("Can not find {0}", functionName));
        }
    }
}
