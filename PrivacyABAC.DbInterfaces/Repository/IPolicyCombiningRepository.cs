using PrivacyABAC.DbInterfaces.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.DbInterfaces.Repository
{
    public interface IPolicyCombiningRepository : IRepository<AccessControlPolicyCombining>
    {
        string GetRuleCombining(ICollection<AccessControlPolicy> policies);
    }
}
