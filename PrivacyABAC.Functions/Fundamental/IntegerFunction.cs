using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Functions.Fundamental
{
    class IntegerFunction : IPluginFunction
    {
        public string ExecuteFunction(string functionName, params string[] parameters)
        {
            object result = null;

            if (string.Equals(functionName, "Equal"))
                result = Equal(parameters[0], parameters[1]);
            else if (string.Equals(functionName, "GreaterThan"))
                result = GreaterThan(parameters[0], parameters[1]);
            else if (string.Equals(functionName, "LessThan"))
                result = LessThan(parameters[0], parameters[1]);

            if (result == null) throw new FunctionNotFoundException(string.Format(ErrorFunctionMessage.NotFound(), functionName + " function"));

            return result.ToString();
        }

        public string GetClassName()
        {
            return "Integer";
        }

        [FunctionInfo("Equal", 2)]
        public bool Equal(string a, string b)
        {
            if (int.TryParse(a, out int n1) && int.TryParse(b, out int n2))
            {
                if (n1 == n2) return true;
                else return false;
            }
            else throw new InvalidFormatException(string.Format(ErrorFunctionMessage.InvalidFormatTwoParameter(), a, b, "Equal"));
        }

        [FunctionInfo("GreaterThan", 2)]
        public bool GreaterThan(string a, string b)
        {
            if (int.TryParse(a, out int n1) && int.TryParse(b, out int n2))
            {
                if (n1 > n2) return true;
                else return false;
            }
            else throw new InvalidFormatException(string.Format(ErrorFunctionMessage.InvalidFormatTwoParameter(), a, b, "GreaterThan"));
        }

        [FunctionInfo("LessThan", 2)]
        public bool LessThan(string a, string b)
        {
            if (int.TryParse(a, out int n1) && int.TryParse(b, out int n2))
            {
                if (n1 < n2) return true;
                else return false;
            }
            else throw new InvalidFormatException(string.Format(ErrorFunctionMessage.InvalidFormatTwoParameter(), a, b, "LessThan"));
        }
    }
}
