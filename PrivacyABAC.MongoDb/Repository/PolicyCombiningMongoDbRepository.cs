using MongoDB.Driver;
using PrivacyABAC.DbInterfaces.Model;
using PrivacyABAC.DbInterfaces.Repository;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.MongoDb.Repository
{
    public class PolicyCombiningMongoDbRepository : MongoDbRepositoryBase<AccessControlPolicyCombining>, 
                                                    IPolicyCombiningRepository
    {
        public PolicyCombiningMongoDbRepository(MongoDbContextProvider mongoDbContextProvider)
            : base(mongoDbContextProvider)
        { }
        
        public string GetRuleCombining(ICollection<AccessControlPolicy> policies)
        {
            if (policies.Count == 0)
                return AlgorithmCombining.PERMIT_OVERRIDES;

            var builder = Builders<AccessControlPolicyCombining>.Filter;
            var id = policies.ElementAt(0).Id;
            var filter = builder.AnyEq("policies_id", id);

            var data = dbContext.GetCollection<AccessControlPolicyCombining>("AccessControlPolicyCombining")
                                   .Find(filter)
                                   .FirstOrDefault();

            return data == null ? AlgorithmCombining.PERMIT_OVERRIDES : data.Algorithm;
        }
    }
}
