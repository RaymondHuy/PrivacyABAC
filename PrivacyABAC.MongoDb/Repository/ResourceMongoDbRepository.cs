using PrivacyABAC.DbInterfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using PrivacyABAC.Infrastructure.Validation;

namespace PrivacyABAC.MongoDb.Repository
{
    public class ResourceMongoDbRepository : IResourceRepository
    {
        private readonly IMongoDatabase dbContext;

        public ResourceMongoDbRepository(MongoDbContextProvider mongoDbContextProvider)
        {
            if (dbContext == null)
            {
                Check.NotEmpty(mongoDbContextProvider.UserDatabaseName, "MongoDb UserDatabaseName");

                var client = new MongoClient(mongoDbContextProvider.ConnectionString);
                dbContext = client.GetDatabase(mongoDbContextProvider.UserDatabaseName);
            }
        }

        public IEnumerable<string> GetAllCollectionNames()
        {
            var collections = new List<string>();

            foreach (BsonDocument collection in dbContext.ListCollectionsAsync().Result.ToListAsync<BsonDocument>().Result)
            {
                string name = collection["name"].AsString;
                collections.Add(name);
            }

            return collections;
        }

        public ICollection<string> GetArrayFieldName()
        {
            throw new NotImplementedException();
        }

        public JObject[] GetCollectionDataWithCustomFilter(string collectionName, dynamic filter)
        {
            var data = filter == null 
                ? dbContext.GetCollection<BsonDocument>(collectionName)
                                                 .Find(_ => true)
                                                 .ToList()

                : dbContext.GetCollection<BsonDocument>(collectionName)
                          .Find((FilterDefinition<BsonDocument>)filter)
                          .ToList();
            var jsonSetting = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            return Newtonsoft.Json.JsonConvert.DeserializeObject<JObject[]>(data.ToJson(jsonSetting));
        }

        public string GetJsonStructureOfCollection(string collectionName)
        {
            var exampleStructure = dbContext.GetCollection<BsonDocument>(collectionName)
                                   .Find(_ => true)
                                   .First();
            var jsonSetting = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            return exampleStructure.ToJson(jsonSetting);
        }

        public string GetJsonStructureOfEmbeddedArray(string fieldName)
        {
            var arrayTokens = fieldName.Split('.');
            //var collectionName = arrayTokens[0];
            //var pathToField = string.Empty;
            //for (int i = 1; i < arrayTokens.Count(); i++)
            //{

            //}
            var exampleStructure = dbContext.GetCollection<BsonDocument>(arrayTokens[0])
                                   .Find(_ => true)
                                   .First();
            var jsonSetting = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = exampleStructure.ToJson(jsonSetting);
            string result = JObject.Parse(json).SelectToken(arrayTokens[1]).ToString();
            return result;
        }
    }
}
