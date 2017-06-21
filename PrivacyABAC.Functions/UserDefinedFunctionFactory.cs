using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using PrivacyABAC.Infrastructure.Exceptions;

namespace PrivacyABAC.Functions
{
    public class UserDefinedFunctionFactory
    {
        private UserDefinedFunctionFactory() { }

        private SortedList<string, FunctionInfo> _sortedListFunctionInfo = new SortedList<string, FunctionInfo>();
        private SortedList<string, IPluginFunction> _sortedListPluginFunction = new SortedList<string, IPluginFunction>();

        private static UserDefinedFunctionFactory _instance;

        public static UserDefinedFunctionFactory GetInstance()
        {
            if (_instance == null)
                _instance = new UserDefinedFunctionFactory();
            return _instance;
        }

        public void RegisterFunction(IPluginFunction function)
        {
            foreach (var func in function.GetRegisteredFunctions())
            {
                string name = string.Format("{0}.{1}", function.GetClassName(), func.Name);
                _sortedListFunctionInfo.Add(name, func);
            }

            _sortedListPluginFunction.Add(function.GetClassName(), function);
        }

        public string ExecuteFunction(string name, params string[] parameters)
        {
            var arr = name.Split('.');
            string className = String.Empty;
            string functionName = string.Empty;
            if (arr.Length == 2)
            {
                className = arr[0];
                functionName = arr[1];
            }
            else throw new InvalidFormatException(ErrorFunctionMessage.InvalidFormatFunctionName, name);

            var plugin = _sortedListPluginFunction[className];
            if (plugin != null)
                return plugin.ExecuteFunction(functionName, parameters);
            else throw new FunctionNotFoundException(string.Format(ErrorFunctionMessage.NotFound, className));
        }
    }
}
