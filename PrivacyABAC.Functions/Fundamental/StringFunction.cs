using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Functions.Fundamental
{
    class StringFunction : IPluginFunction
    {
        public string ExecuteFunction(string functionName, params string[] parameters)
        {
            object result = null;

            if (string.Equals(functionName, "Equal"))
                result = Equal(parameters[0], parameters[1]);
            else if (string.Equals(functionName, "EqualInsenitive"))
                result = EqualInsenitive(parameters[0], parameters[1]);

            if (result == null) throw new FunctionNotFoundException(string.Format(ErrorFunctionMessage.NotFound(), functionName + " function"));

            return result.ToString();
        }

        public string GetClassName()
        {
            return "String";
        }

        [FunctionInfo("EqualInsenitive", 2)]
        public bool EqualInsenitive(string a, string b)
        {
            if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase)) return true;
            else return false;
        }

        [FunctionInfo("Equal", 2)]
        public bool Equal(string a, string b)
        {
            if (string.Equals(a, b)) return true;
            else return false;
        }
    }
}
