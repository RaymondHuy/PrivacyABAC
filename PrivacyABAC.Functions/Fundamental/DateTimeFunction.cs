using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Functions.Fundamental
{
    public class DateTimeFunction : IPluginFunction
    {
        public string ExecuteFunction(string functionName, params object[] parameters)
        {
            object result = null;

            if (string.Equals(functionName, "Equal"))
                result = Equal(parameters[0].ToString(), parameters[1].ToString());
            else if (string.Equals(functionName, "GreaterThan"))
                result = GreaterThan(parameters[0].ToString(), parameters[1].ToString());
            else if (string.Equals(functionName, "LessThan"))
                result = LessThan(parameters[0].ToString(), parameters[1].ToString());

            if (result == null) throw new FunctionNotFoundException(string.Format(ErrorFunctionMessage.NotFound, functionName + " function"));

            return result.ToString();
        }

        public string GetClassName()
        {
            return "DateTime";
        }

        public FunctionInfo[] GetRegisteredFunctions()
        {
            return new FunctionInfo[]
            {
                new FunctionInfo("Equal", 2),
                new FunctionInfo("GreaterThan", 2),
                new FunctionInfo("LessThan", 2)
            };
        }

        public bool Equal(string a, string b)
        {
            if (DateTime.TryParse(a, out DateTime d1) && DateTime.TryParse(b, out DateTime d2))
            {
                if (d1.Equals(d2)) return true;
                else return false;
            }
            else throw new InvalidFormatException(string.Format(ErrorFunctionMessage.InvalidFormatTwoParameter, a, b, "Equal"));
        }
        
        public bool GreaterThan(string a, string b)
        {
            if (DateTime.TryParse(a, out DateTime n1) && DateTime.TryParse(b, out DateTime n2))
            {
                if (n1 > n2) return true;
                else return false;
            }
            else throw new InvalidFormatException(string.Format(ErrorFunctionMessage.InvalidFormatTwoParameter, a, b, "GreaterThan"));
        }
        
        public bool LessThan(string a, string b)
        {
            if (DateTime.TryParse(a, out DateTime n1) && DateTime.TryParse(b, out DateTime n2))
            {
                if (n1 < n2) return true;
                else return false;
            }
            else throw new InvalidFormatException(string.Format(ErrorFunctionMessage.InvalidFormatTwoParameter, a, b, "LessThan"));
        }

    }
}
