using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrivacyABAC.DbInterfaces.Repository;
using PrivacyABAC.MongoDb.Repository;
using PrivacyABAC.Core.Service;
using Microsoft.Extensions.Logging;
using PrivacyABAC.MongoDb;
using PrivacyABAC.Functions;
using PrivacyABAC.Domains.Common;

namespace PrivacyABAC.UnitTest
{
    public class TestConfiguration
    {
        private static IContainer _container;

        public static IContainer GetContainer()
        {
            if (_container == null)
                _container = SetupContainer();
            return _container;
        }

        private static IContainer SetupContainer()
        {
            MongoDbContextProvider.Setup();

            var builder = new ContainerBuilder();

            builder.RegisterType<AccessControlPolicyMongoDbRepository>().As<IAccessControlPolicyRepository>();
            builder.RegisterType<PrivacyPolicyMongoDbRepository>().As<IPrivacyPolicyRepository>();
            builder.RegisterType<PolicyCombiningMongoDbRepository>().As<IPolicyCombiningRepository>();
            builder.RegisterType<PrivacyDomainMongoDbRepository>().As<IPrivacyDomainRepository>().SingleInstance();
            builder.RegisterType<AccessControlPolicyMongoDbRepository>().As<IAccessControlPolicyRepository>();
            builder.RegisterType<SubjectMongoDbRepository>().As<ISubjectRepository>();
            builder.RegisterType<ResourceMongoDbRepository>().As<IResourceRepository>();

            builder.RegisterType<LoggerFactory>().As<ILoggerFactory>().SingleInstance();
            builder.RegisterType<Logger<AccessControlService>>().As<ILogger<AccessControlService>>();
            builder.RegisterType<Logger<PrivacyService>>().As<ILogger<PrivacyService>>();
            builder.RegisterType<Logger<ConditionalExpressionService>>().As<ILogger<ConditionalExpressionService>>();

            builder.Register(c => new MongoDbContextProvider()
            {
                ConnectionString = "mongodb://localhost:27017",
                PolicyDatabaseName = "Policy",
                UserCollectionName = "UserDB",
                UserDatabaseName = "UserDB"
            }).SingleInstance();

            builder.RegisterType<AccessControlService>().SingleInstance();
            builder.RegisterType<PrivacyService>().SingleInstance();
            builder.RegisterType<ConditionalExpressionService>().SingleInstance();
            builder.RegisterType<SecurityService>().SingleInstance();

            var pluginFunctionFactory = UserDefinedFunctionFactory.GetInstance();
            pluginFunctionFactory.RegisterDefaultFunctions();

            var domainFactory = PrivacyDomainPluginFactory.GetInstance();
            domainFactory.RegisterDefaultPlugin();

            var container = builder.Build();
            return container;
        }
    }
}
