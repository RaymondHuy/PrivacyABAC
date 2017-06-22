using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Domains
{
    public class PrivacyDomainFactory
    {
        private Dictionary<string, IPluginDomain> _domainDictionay = new Dictionary<string, IPluginDomain>();

        private PrivacyDomainFactory() { }

        private static PrivacyDomainFactory _instance;

        public static PrivacyDomainFactory GetInstance()
        {
            if (_instance != null)
                _instance = new PrivacyDomainFactory();
            return _instance;
        }

        public void RegisterPlugin(IPluginDomain pluginDomain)
        {
            _domainDictionay.Add(pluginDomain.GetName(), pluginDomain);
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
            else throw new InvalidFormatException(ErrorMessage.NotFound, name);

            var plugin = _domainDictionay[className];
            if (plugin != null)
                return plugin.ExecuteFunction(functionName, parameters);
            else throw new FunctionNotFoundException(string.Format(ErrorMessage.NotFound, className));
        }
    }
}
