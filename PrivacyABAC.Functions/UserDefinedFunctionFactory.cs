using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Functions
{
    public class UserDefinedFunctionFactory
    {
        private UserDefinedFunctionFactory() { }

        private static UserDefinedFunctionFactory _instance;

        public static UserDefinedFunctionFactory GetInstance()
        {
            return _instance;
        }

        public void RegisterFunction(IPluginFunction function)
        {
        }
    }
}
