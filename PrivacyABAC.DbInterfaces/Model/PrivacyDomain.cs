

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.DbInterfaces.Model
{
    public class PrivacyDomain : IEntityBase
    {
        public string Id { get; set; }

        public string DomainName { get; set; }

        public bool IsArrayFieldDomain { get; set; }

        public ICollection<string> Fields { get; set; }

        public ICollection<PriorityFunction> Functions { get; set; }
    }
}
