using PrivacyABAC.DbInterfaces.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.DbInterfaces.Repository
{
    public interface IPrivacyPolicyRepository : IRepository<PrivacyPolicy>
    {
        ICollection<PrivacyPolicy> GetPolicies(string collectionName, bool? isAttributeResourceRequired);
    }
}
