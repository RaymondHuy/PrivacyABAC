using MongoDB.Driver;
using PrivacyABAC.DbInterfaces.Model;
using PrivacyABAC.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.MongoDb
{
    public class MongoDbRepositoryBase<T> where T: IEntityBase
    {
        protected readonly IMongoDatabase dbContext;

        public MongoDbRepositoryBase(MongoDbContextProvider mongoDbContextProvider)
        {
            if (dbContext == null)
            {
                Check.NotEmpty(mongoDbContextProvider.PolicyDatabaseName, "MongoDb PolicyDatabaseName");

                var client = new MongoClient(mongoDbContextProvider.ConnectionString);
                dbContext = client.GetDatabase(mongoDbContextProvider.PolicyDatabaseName);
            }
        }

        public virtual IEnumerable<T> GetAll()
        {
            var result = dbContext.GetCollection<T>(typeof(T).Name).Find(_ => true);
            return result.ToList();
        }

        public virtual T GetById(string id)
        {
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("_id", id);

            var result = dbContext.GetCollection<T>(typeof(T).Name).Find(filter);
            return result.FirstOrDefault();
        }

        public virtual void Add(T entity)
        {
            dbContext.GetCollection<T>(typeof(T).Name).InsertOne(entity);
        }

        public virtual void Delete(string id)
        {
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("_id", id);

            dbContext.GetCollection<T>(typeof(T).Name).DeleteOne(filter);
        }

        public virtual void Update(T entity)
        {
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("_id", entity.Id);

            dbContext.GetCollection<T>(typeof(T).Name).ReplaceOne(filter, entity);
        }
        
    }
}
