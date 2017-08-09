using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.WebAPI.ViewModel
{
    public class PrivacyPolicyViewModel
    {
        public string CollectionName { get; set; }
        
        public string PolicyId { get; set; }
        
        public string Description { get; set; }
        
        public string Target { get; set; }
    }
}
