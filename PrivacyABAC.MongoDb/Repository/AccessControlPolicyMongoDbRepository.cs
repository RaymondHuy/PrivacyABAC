using PrivacyABAC.DbInterfaces.Repository;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using PrivacyABAC.DbInterfaces.Model;
using MongoDB.Driver;

namespace PrivacyABAC.MongoDb.Repository
{
    public class AccessControlPolicyMongoDbRepository : 
        MongoDbRepositoryBase<AccessControlPolicy>, IAccessControlPolicyRepository
    {
        public AccessControlPolicyMongoDbRepository(MongoDbContextProvider mongoDbContextProvider)
            :base(mongoDbContextProvider)
        {
        }

        public ICollection<AccessControlPolicy> Get(string collectionName, string action, bool? isAttributeResourceRequired)
        {
            var builder = Builders<AccessControlPolicy>.Filter;
            var filter = builder.Eq("collection_name", collectionName)
                       & builder.Eq("action", action);

            if (isAttributeResourceRequired != null)
                filter = filter & builder.Eq("is_attribute_resource_required", isAttributeResourceRequired);

            var data = dbContext.GetCollection<AccessControlPolicy>("AccessControlPolicy")
                                   .Find(filter)
                                   .ToList();
            return data;
        }
    }
}
