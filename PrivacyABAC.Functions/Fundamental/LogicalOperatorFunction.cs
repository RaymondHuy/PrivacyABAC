using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PrivacyABAC.Functions.Fundamental
{
    public class LogicalOperatorFunction : IPluginFunction
    {
        public string GetClassName() => "Logic";
        
        public FunctionInfo[] GetRegisteredFunctions()
        {
            return new FunctionInfo[]
            {
                new FunctionInfo("And", 2),
                new FunctionInfo("Or", 2),
                new FunctionInfo("Not", 2)
            };
        }
        public string ExecuteFunction(string functionName, params object[] parameters)
        {
            if (functionName.Equals("And", StringComparison.OrdinalIgnoreCase))
                return And(parameters[0].ToString(), parameters[1].ToString()).ToString();
            else if (functionName.Equals("Or", StringComparison.OrdinalIgnoreCase))
                return Or(parameters[0].ToString(), parameters[1].ToString()).ToString();
            else if (functionName.Equals("Not", StringComparison.OrdinalIgnoreCase))
                return Not(parameters[0].ToString()).ToString();

            throw new FunctionNotFoundException(string.Format("Can not find {0}", functionName));
        }

        public bool And(string s1, string s2)
        {
            bool b1, b2 = false;
            bool valid = bool.TryParse(s1, out b1) && bool.TryParse(s2, out b2);
            if (valid)
                return (b1 == true) && (b2 == true);
            else throw new InvalidFormatException("Can not execute And function between two parameters : " + s1 + " " + s2);
        }
        public bool Or(string s1, string s2)
        {
            bool b1, b2 = false;
            bool valid = bool.TryParse(s1, out b1) && bool.TryParse(s2, out b2);
            if (valid)
                return (b1 == true) || (b2 == true);
            else throw new InvalidFormatException("Can not execute Or function between two parameters : " + s1 + " " + s2);
        }

        public bool Not(string s1)
        {
            bool b1 = false;
            bool valid = bool.TryParse(s1, out b1);
            if (valid)
                return !b1;
            else throw new InvalidFormatException("Can not execute Not function of parameters : " + s1 );
        }
    }
}
