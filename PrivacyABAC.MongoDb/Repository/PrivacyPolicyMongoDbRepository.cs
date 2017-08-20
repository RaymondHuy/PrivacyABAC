using PrivacyABAC.DbInterfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using PrivacyABAC.DbInterfaces.Model;
using MongoDB.Driver;

namespace PrivacyABAC.MongoDb.Repository
{
    public class PrivacyPolicyMongoDbRepository : MongoDbRepositoryBase<PrivacyPolicy>, IPrivacyPolicyRepository
    {
        public PrivacyPolicyMongoDbRepository(MongoDbContextProvider mongoDbContextProvider)
            : base(mongoDbContextProvider)
        {

        }

        public ICollection<PrivacyPolicy> GetPolicies(string collectionName, bool? isAttributeResourceRequired)
        {
            var builder = Builders<PrivacyPolicy>.Filter;
            var filter = builder.Eq("collection_name", collectionName);

            if (isAttributeResourceRequired != null)
                filter = filter & builder.Eq("is_attribute_resource_required", isAttributeResourceRequired);


            var data = dbContext.GetCollection<PrivacyPolicy>("PrivacyPolicy")
                                   .Find(filter)
                                   .ToList();
            return data;
        }
    }
}
