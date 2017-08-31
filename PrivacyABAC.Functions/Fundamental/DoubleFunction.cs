using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PrivacyABAC.Functions.Fundamental
{
    public class DoubleFunction : IPluginFunction
    {
        public string GetClassName() => "Double";

        public FunctionInfo[] GetRegisteredFunctions()
        {
            return new FunctionInfo[]
            {
                new FunctionInfo("Equal", 2),
                new FunctionInfo("GreaterThan", 2),
                new FunctionInfo("LessThan", 2)
            };
        }

        public string ExecuteFunction(string functionName, object[] parameters)
        {
            if (functionName.Equals("Equal", StringComparison.OrdinalIgnoreCase))
                return Equal(parameters[0].ToString(), parameters[1].ToString()).ToString();
            else if (functionName.Equals("GreaterThan", StringComparison.OrdinalIgnoreCase))
                return GreaterThan(parameters[0].ToString(), parameters[1].ToString()).ToString();
            else if (functionName.Equals("LessThan", StringComparison.OrdinalIgnoreCase))
                return LessThan(parameters[0].ToString(), parameters[1].ToString()).ToString();

            throw new FunctionNotFoundException(string.Format("Can not find {0}", functionName));
        }

        public bool Equal(string s1, string s2)
        {
            double n1, n2 = 0;
            bool valid = double.TryParse(s1, out n1) && double.TryParse(s2, out n2);
            if (valid)
                return n1 == n2;
            else throw new InvalidFormatException("Can not execute Equal function between two parameters : " + s1 + " " + s2);
        }

        public bool GreaterThan(string s1, string s2)
        {
            double n1, n2 = 0;
            bool valid = double.TryParse(s1, out n1) && double.TryParse(s2, out n2);
            if (valid)
                return n1 > n2;
            else throw new InvalidFormatException("Can not execute GreaterThan function between two parameters : " + s1 + " " + s2);
        }

        public bool LessThan(string s1, string s2)
        {
            double n1, n2 = 0;
            bool valid = double.TryParse(s1, out n1) && double.TryParse(s2, out n2);
            if (valid)
                return n1 < n2;
            else throw new InvalidFormatException("Can not execute LessThan function between two parameters : " + s1 + " " + s2);
        }

    }
}
