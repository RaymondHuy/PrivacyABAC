using PrivacyABAC.DbInterfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using PrivacyABAC.DbInterfaces.Model;

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
            throw new NotImplementedException();
        }
    }
}
