using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using PrivacyABAC.Infrastructure.Exceptions;
using PrivacyABAC.Functions.Fundamental;

namespace PrivacyABAC.Functions
{
    public class UserDefinedFunctionFactory
    {
        private UserDefinedFunctionFactory() { }

        private Dictionary<string, FunctionInfo> _sortedListFunctionInfo = new Dictionary<string, FunctionInfo>();
        private Dictionary<string, IPluginFunction> _sortedListPluginFunction = new Dictionary<string, IPluginFunction>();

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

        public string ExecuteFunction(string name, params object[] parameters)
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

        public FunctionInfo GetFunction(string keyword)
        {
            return _sortedListFunctionInfo[keyword];
        }

        public IEnumerable<string> GetAllFunctionNames()
        {
            return _sortedListFunctionInfo.Keys;
        }

        public void RegisterDefaultFunctions()
        {
            RegisterFunction(new BooleanFunction());
            RegisterFunction(new DateTimeFunction());
            RegisterFunction(new DoubleFunction());
            RegisterFunction(new IntegerFunction());
            RegisterFunction(new LogicalOperatorFunction());
            RegisterFunction(new StringFunction());
        }
    }
}
