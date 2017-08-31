using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Functions.Fundamental
{
    public class StringFunction : IPluginFunction
    {
        public string ExecuteFunction(string functionName, params object[] parameters)
        {
            object result = null;

            if (string.Equals(functionName, "Equal"))
                result = Equal(parameters[0].ToString(), parameters[1].ToString());
            else if (string.Equals(functionName, "EqualInsenitive"))
                result = EqualInsenitive(parameters[0].ToString(), parameters[1].ToString());
            else if (string.Equals(functionName, "IsNotNull"))
                result = IsNotNull(parameters[0]);

            if (result == null) throw new FunctionNotFoundException(string.Format(ErrorFunctionMessage.NotFound, functionName + " function"));

            return result.ToString();
        }

        public string GetClassName() => "String";

        public FunctionInfo[] GetRegisteredFunctions()
        {
            return new FunctionInfo[]
            {
                new FunctionInfo("Equal", 2),
                new FunctionInfo("EqualInsenitive", 2),
                new FunctionInfo("IsNotNull", 1)
            };
        }

        public bool EqualInsenitive(string a, string b)
        {
            if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase)) return true;
            else return false;
        }
        
        public bool Equal(string a, string b)
        {
            if (string.Equals(a, b)) return true;
            else return false;
        }

        public bool IsNotNull(object s)
        {
            return s != null;
        }
    }
}
