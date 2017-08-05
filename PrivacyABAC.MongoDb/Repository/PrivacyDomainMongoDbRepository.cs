using PrivacyABAC.DbInterfaces.Model;
using PrivacyABAC.DbInterfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.MongoDb.Repository
{
    public class PrivacyDomainMongoDbRepository : MongoDbRepositoryBase<PrivacyDomain>, IPrivacyDomainRepository
    {
        public PrivacyDomainMongoDbRepository(MongoDbContextProvider mongoDbContextProvider)
            : base(mongoDbContextProvider)
        {

        }
    }
}
