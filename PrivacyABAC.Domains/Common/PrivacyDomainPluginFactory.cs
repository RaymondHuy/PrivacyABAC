
using PrivacyABAC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PrivacyABAC.Domains.Common
{
    public class PrivacyDomainPluginFactory
    {
        private PrivacyDomainPluginFactory() { }

        private static PrivacyDomainPluginFactory _instance = new PrivacyDomainPluginFactory();

        public static PrivacyDomainPluginFactory GetInstance()
        {
            return _instance;
        }

        private SortedList<string, Type> _container = new SortedList<string, Type>();
        private SortedList<string, IPluginDomain> _pluginContainer = new SortedList<string, IPluginDomain>();

        public void RegisterDefaultPlugin()
        {
            RegisterPlugin(new DateTimeDomainPrivacy());
            RegisterPlugin(new DefaultDomainPrivacy());
            RegisterPlugin(new SSNDomainPrivacy());
            RegisterPlugin(new PhoneDomainPrivacy());
            RegisterPlugin(new AddressDomainPrivacy());
            RegisterPlugin(new EmailDomain());
        }
        public void RegisterPlugin(IPluginDomain plugin)
        {
            _pluginContainer.Add(plugin.GetName(), plugin);
        }

        public Type GetDomainType(string name)
        {
            return _container[name];
        }

        public ICollection<string> GetAllDomainType()
        {
            return _container.Select(n => n.Key).ToList();
        }

        public string ExecuteFunction(string domainName, string functionName, string[] param)
        {
            var plugin = _pluginContainer[domainName];
            if (plugin != null)
            {
                return plugin.ExecuteFunction(functionName, param);
            }
            else throw new FunctionNotFoundException(string.Format("Can not find {0}", domainName));
        }
    }
}
