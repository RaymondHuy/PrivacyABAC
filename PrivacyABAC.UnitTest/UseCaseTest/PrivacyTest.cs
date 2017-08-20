using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrivacyABAC.DbInterfaces.Repository;
using PrivacyABAC.MongoDb.Repository;
using PrivacyABAC.Core.Service;
using Microsoft.Extensions.Logging;

namespace PrivacyABAC.UnitTest.UseCaseTest
{
    [TestClass]
    public class PrivacyTest
    {
        [TestMethod]
        public void PrivacyChecking()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<AccessControlPolicyMongoDbRepository>().As<IAccessControlPolicyRepository>();
            builder.RegisterType<PrivacyPolicyMongoDbRepository>().As<IPrivacyPolicyRepository>();
            builder.RegisterType<PolicyCombiningMongoDbRepository>().As<IPolicyCombiningRepository>();
            builder.RegisterType<PrivacyDomainMongoDbRepository>().As<IPrivacyDomainRepository>().SingleInstance();
            builder.RegisterType<AccessControlPolicyMongoDbRepository>().As<IAccessControlPolicyRepository>();
            builder.RegisterType<SubjectMongoDbRepository>().As<ISubjectRepository>();
            builder.RegisterType<ResourceMongoDbRepository>().As<IResourceRepository>();

            builder.RegisterType<Logger<AccessControlService>>().As<ILogger<AccessControlService>>();
            builder.RegisterType<Logger<PrivacyService>>().As<ILogger<PrivacyService>>();
            builder.RegisterType<Logger<ConditionalExpressionService>>().As<ILogger<ConditionalExpressionService>>();

            builder.RegisterType<AccessControlService>().SingleInstance();
            builder.RegisterType<PrivacyService>().SingleInstance();
            builder.RegisterType<ConditionalExpressionService>().SingleInstance();
            builder.RegisterType<SecurityService>().SingleInstance();

            var container = builder.Build();

            var service = container.Resolve<SecurityService>();
            //var data = JObject.Parse(jsonData);
            //Console.WriteLine(expressionService.IsAccessControlPolicyRelateToContext(policy, data, data, data));
        }
    }
}
