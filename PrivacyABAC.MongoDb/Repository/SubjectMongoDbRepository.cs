using PrivacyABAC.DbInterfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using MongoDB.Driver;
using PrivacyABAC.Infrastructure.Validation;
using MongoDB.Bson.IO;
using MongoDB.Bson;

namespace PrivacyABAC.MongoDb.Repository
{
    public class SubjectMongoDbRepository : ISubjectRepository
    {
        private readonly IMongoDatabase dbContext;
        private string _userCollectionName;

        public SubjectMongoDbRepository(MongoDbContextProvider mongoDbContextProvider)
        {
            if (dbContext == null)
            {
                Check.NotEmpty(mongoDbContextProvider.UserDatabaseName, "MongoDb UserDatabaseName");

                var client = new MongoClient(mongoDbContextProvider.ConnectionString);
                dbContext = client.GetDatabase(mongoDbContextProvider.UserDatabaseName);
                _userCollectionName = mongoDbContextProvider.UserCollectionName;
            }
        }

        public JArray GetAllUsers()
        {
            var user = dbContext.GetCollection<BsonDocument>(_userCollectionName)
                                .Find(_ => true)
                                .ToList();
            var jsonSetting = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            return JArray.Parse(user.ToJson(jsonSetting));
        }

        public JObject GetUniqueUser(string fieldName, string value)
        {
            FilterDefinition<BsonDocument> filter;
            if(fieldName.Equals("_id"))
                filter = Builders<BsonDocument>.Filter.Eq(fieldName, ObjectId.Parse(value));
            else filter = Builders<BsonDocument>.Filter.Eq(fieldName, value);

            var user = dbContext.GetCollection<BsonDocument>(_userCollectionName)
                                .Find(filter)
                                .FirstOrDefault();
            var jsonSetting = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            return JObject.Parse(user.ToJson(jsonSetting));
        }
    }
}
