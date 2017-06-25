using PrivacyABAC.DbInterfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using PrivacyABAC.DbInterfaces.Model;

namespace PrivacyABAC.MongoDb.Repository
{
    public class AccessControlPolicyMongoDbRepository : IAccessControlPolicyRepository
    {
        public void Create(AccessControlPolicy entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public AccessControlPolicy[] GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(AccessControlPolicy entity, string id)
        {
            throw new NotImplementedException();
        }
    }
}
