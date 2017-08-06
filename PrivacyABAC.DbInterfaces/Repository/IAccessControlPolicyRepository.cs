using PrivacyABAC.DbInterfaces.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.DbInterfaces.Repository
{
    public interface IAccessControlPolicyRepository : IRepository<AccessControlPolicy>
    {
        ICollection<AccessControlPolicy> Get(string collectionName, string action, bool? isAttributeResourceRequired);   
    }
}
