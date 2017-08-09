using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.WebAPI.ViewModel
{
    public class AccessControlPolicyViewModel
    {
        public string CollectionName { get; set; }
        
        public string PolicyId { get; set; }
        
        public string Description { get; set; }
        
        public string RuleCombining { get; set; }
        
        public bool IsAttributeResourceRequired { get; set; }
        
        public string Action { get; set; }
        
        public string Target { get; set; }
    }
}
