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
using Newtonsoft.Json.Linq;
using MongoDB.Driver;
using MongoDB.Bson;

namespace PrivacyABAC.UnitTest.UseCaseTest
{
    [TestClass]
    public class PrivacyTest
    {
        [TestMethod]
        public void PrivacyChecking()
        {
            var container = TestConfiguration.GetContainer();
            var subjectRepository = container.Resolve<ISubjectRepository>();
            var resourceRepository = container.Resolve<IResourceRepository>();
            var service = container.Resolve<SecurityService>();

            Console.WriteLine("Hi");
            return;
            var subject = subjectRepository.GetUniqueUser("_id", "");
            var environment =  JObject.Parse("");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("", "");
            var resource = resourceRepository.GetCollectionDataWithCustomFilter("", filter);
            var action = "read";

            var result = service.ExecuteProcess(subject, resource, action, "", environment);

            
            //var data = JObject.Parse(jsonData);
            //Console.WriteLine(expressionService.IsAccessControlPolicyRelateToContext(policy, data, data, data));
        }
    }
}
