using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.DbInterfaces.Model
{
    public class PrivacyPolicy
    {
        public string CollectionName { get; set; }

        public string PolicyId { get; set; }

        public string Description { get; set; }

        public Function Target { get; set; }

        public bool IsAttributeResourceRequired { get; set; }

        public ICollection<FieldRule> Rules { get; set; }
        
    }
}
