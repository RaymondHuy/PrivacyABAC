using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.WebAPI.Controllers
{
    public class ResourceController : Controller
    {
        private readonly IMongoClient _mongoClient;

        public ResourceController(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }

        [HttpGet]
        [Route("api/structure")]
        public string GetCollectionStructure(string collectionName)
        {
            var exampleStructure = _mongoClient.GetDatabase(JsonAccessControlSetting.UserDefaultDatabaseName)
                                   .GetCollection<BsonDocument>(collectionName)
                                   .Find(_ => true)
                                   .First();
            var jsonSetting = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            return exampleStructure.ToJson(jsonSetting);
        }

        [HttpGet]
        [Route("api/SubStructure")]
        public string GetFieldStructure(string fieldName)
        {
            var array = fieldName.Split('.');
            var exampleStructure = _mongoClient.GetDatabase(JsonAccessControlSetting.UserDefaultDatabaseName)
                                   .GetCollection<BsonDocument>(array[0])
                                   .Find(_ => true)
                                   .First();
            var jsonSetting = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = exampleStructure.ToJson(jsonSetting);
            string result = JObject.Parse(json).SelectToken("projects").ToString();
            return result;
        }

        [HttpGet]
        [Route("api/collections")]
        public IEnumerable<string> GetAllCollections()
        {
            var client = new MongoClient();
            MongoServer server = client.GetServer();
            MongoDatabase database = server.GetDatabase(JsonAccessControlSetting.UserDefaultDatabaseName);
            return database.GetCollectionNames();
        }

        [HttpGet]
        [Route("api/subject/fields")]
        public string GetSubjectFields()
        {
            var exampleStructure = _mongoClient.GetDatabase(JsonAccessControlSetting.UserDefaultDatabaseName)
                                   .GetCollection<BsonDocument>(JsonAccessControlSetting.UserDefaultCollectionName)
                                   .Find(_ => true)
                                   .First();
            var jsonSetting = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            return exampleStructure.ToJson(jsonSetting);
        }
    }
}
