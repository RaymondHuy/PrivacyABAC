using PrivacyABAC.DbInterfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using PrivacyABAC.DbInterfaces.Model;

namespace PrivacyABAC.MongoDb.Repository
{
    class PrivacyPolicyMongoDbRepository : IPrivacyPolicyRepository
    {
        public void Create(PrivacyPolicy entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public PrivacyPolicy[] GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(PrivacyPolicy entity, string id)
        {
            throw new NotImplementedException();
        }
    }
}
