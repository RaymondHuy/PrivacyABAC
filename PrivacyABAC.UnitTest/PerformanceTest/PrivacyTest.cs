using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
using PrivacyABAC.Core.Model;
using System.IO;
using System.Diagnostics;

namespace PrivacyABAC.UnitTest.PerformanceTest
{
    /// <summary>
    /// Access control policy 1: Alice can read email if filename is not null or length >0 (giả sử request là Alice, read, email)
    /// Privacy policy 1: All body of emails is hidden from the 200th character
    /// Privacy polciy 2: All body of emails is hidden completely if filename is not null or length >0
    /// Privacy policy 3 (embedded): All names of To address of emails is replaced by x.Example abc@gmail.com ==> x @gmail.com
    /// Privacy policy 4 (embedded): All names of To address of emails is hidden completely  if the filename is not null or length > 0
    /// </summary>
    [TestClass]
    public class PrivacyTest
    {
        private string collectionName = "EnronEmail_500K";
        /// <summary>
        /// Case 1: AC1 
        /// </summary>
        [TestMethod]
        public void FirstCaseTest()
        {
            var container = TestConfiguration.GetContainer();
            var subjectRepository = container.Resolve<ISubjectRepository>();
            var resourceRepository = container.Resolve<IResourceRepository>();
            var service = container.Resolve<AccessControlService>();

            var user = JObject.Parse(@"
                {'name': 'Alice'}
            ");
            var environment = JObject.Parse("{}");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("headers.From", "Alice@gmail.com");
            var resource = resourceRepository.GetCollectionDataWithCustomFilter(collectionName, filter);
            var action = "read";

            var subject = new Subject(user);
            var data = new Resource(resource, collectionName);
            var env = new EnvironmentObject(environment);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = service.ExecuteProcess(subject, data, action, env);
            stopwatch.Stop();
            File.WriteAllText(@"C:\Users\ttqnguyet\Downloads\first_case.txt", stopwatch.ElapsedMilliseconds.ToString() + " " + result.Data.Count);
        }

        /// <summary>
        /// Case 2: PP1
        /// </summary>
        [TestMethod]
        public void SecondCaseTest()
        {
            var container = TestConfiguration.GetContainer();
            var subjectRepository = container.Resolve<ISubjectRepository>();
            var resourceRepository = container.Resolve<IResourceRepository>();
            var service = container.Resolve<PrivacyService>();

            var user = JObject.Parse(@"
                {'name': 'Alice'}
            ");
            var environment = JObject.Parse("{}");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("headers.From", "Alice@gmail.com");
            var resource = resourceRepository.GetCollectionDataWithCustomFilter(collectionName, filter);
            var action = "read";

            var subject = new Subject(user);
            var data = new Resource(resource, collectionName);
            var env = new EnvironmentObject(environment);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = service.ExecuteProcess(subject, data, action, env);
            stopwatch.Stop();
            File.WriteAllText(@"C:\Users\ttqnguyet\Downloads\test.txt", stopwatch.ElapsedMilliseconds.ToString() + " " + result.JsonObjects.Count + result.JsonObjects.ElementAt(0).ToString());
        }

        /// <summary>
        /// Case 3: PP2
        /// </summary>
        [TestMethod]
        public void ThirdCaseTest()
        {
            var container = TestConfiguration.GetContainer();
            var subjectRepository = container.Resolve<ISubjectRepository>();
            var resourceRepository = container.Resolve<IResourceRepository>();
            var service = container.Resolve<PrivacyService>();

            var user = JObject.Parse(@"
                {'name': 'Alice'}
            ");
            var environment = JObject.Parse("{}");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("headers.From", "Alice@gmail.com");
            var resource = resourceRepository.GetCollectionDataWithCustomFilter(collectionName, filter);
            var action = "read";

            var subject = new Subject(user);
            var data = new Resource(resource, collectionName);
            var env = new EnvironmentObject(environment);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = service.ExecuteProcess(subject, data, action, env);
            stopwatch.Stop();
            File.WriteAllText(@"C:\Users\ttqnguyet\Downloads\test.txt", stopwatch.ElapsedMilliseconds.ToString());
        }

        /// <summary>
        /// Case 4: PP3
        /// </summary>
        [TestMethod]
        public void FourthCaseTest()
        {
            var container = TestConfiguration.GetContainer();
            var subjectRepository = container.Resolve<ISubjectRepository>();
            var resourceRepository = container.Resolve<IResourceRepository>();
            var service = container.Resolve<PrivacyService>();

            var user = JObject.Parse(@"
                {'name': 'Alice'}
            ");
            var environment = JObject.Parse("{}");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("headers.From", "Alice@gmail.com");
            var resource = resourceRepository.GetCollectionDataWithCustomFilter(collectionName, filter);
            var action = "read";

            var subject = new Subject(user);
            var data = new Resource(resource, collectionName);
            var env = new EnvironmentObject(environment);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = service.ExecuteProcess(subject, data, action, env);
            stopwatch.Stop();
            File.WriteAllText(@"C:\Users\ttqnguyet\Downloads\first_case.txt", stopwatch.ElapsedMilliseconds.ToString());
        }

        /// <summary>
        /// Case 5: PP4
        /// </summary>
        [TestMethod]
        public void FifthCaseTest()
        {
            var container = TestConfiguration.GetContainer();
            var subjectRepository = container.Resolve<ISubjectRepository>();
            var resourceRepository = container.Resolve<IResourceRepository>();
            var service = container.Resolve<PrivacyService>();

            var user = JObject.Parse(@"
                {'name': 'Alice'}
            ");
            var environment = JObject.Parse("{}");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("headers.From", "Alice@gmail.com");
            var resource = resourceRepository.GetCollectionDataWithCustomFilter(collectionName, filter);
            var action = "read";

            var subject = new Subject(user);
            var data = new Resource(resource, collectionName);
            var env = new EnvironmentObject(environment);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = service.ExecuteProcess(subject, data, action, env);
            stopwatch.Stop();
            File.WriteAllText(@"C:\Users\ttqnguyet\Downloads\test.txt", stopwatch.ElapsedMilliseconds.ToString());
        }

        /// <summary>
        /// Case 6: AC1 + PP1 
        /// </summary>
        [TestMethod]
        public void SixthToTenthCaseTest()
        {
            var container = TestConfiguration.GetContainer();
            var subjectRepository = container.Resolve<ISubjectRepository>();
            var resourceRepository = container.Resolve<IResourceRepository>();
            var accessControlService = container.Resolve<AccessControlService>();
            var privacyService = container.Resolve<PrivacyService>();
            
            var user = JObject.Parse(@"
                {'name': 'Alice'}
            ");
            var environment = JObject.Parse("{}");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("headers.From", "Alice@gmail.com");
            var resource = resourceRepository.GetCollectionDataWithCustomFilter(collectionName, filter);
            var action = "read";

            var subject = new Subject(user);
            var data = new Resource(resource, collectionName);
            var env = new EnvironmentObject(environment);


            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var accessControlResult = accessControlService.ExecuteProcess(subject, data, action, env);
            //stopwatch.Stop();
            //var temp = accessControlResult.Data.ToArray();
            var data2 = new Resource(accessControlResult.Data.ToArray(), collectionName);
            //stopwatch.Start();
            var privacyResult = privacyService.ExecuteProcess(subject, data2, action, env);
            stopwatch.Stop();
            File.WriteAllText($@"C:\Users\ttqnguyet\Downloads\test.txt", stopwatch.ElapsedMilliseconds.ToString() + " " + data2.Data.ElementAt(0).ToString());
            
        }

    }
}
