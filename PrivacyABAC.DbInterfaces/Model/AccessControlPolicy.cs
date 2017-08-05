using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.DbInterfaces.Model
{
    public class AccessControlPolicy : IEntityBase
    {
        public string Id { get; set; }

        public string CollectionName { get; set; }

        public string Description { get; set; }

        public string RuleCombining { get; set; }

        public bool IsAttributeResourceRequired { get; set; }

        public string Action { get; set; }

        public Function Target { get; set; }

        public ICollection<AccessControlRule> Rules { get; set; }
    }
}
