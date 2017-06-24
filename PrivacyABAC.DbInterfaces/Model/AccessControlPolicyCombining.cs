

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.DbInterfaces.Model
{
    public class AccessControlPolicyCombining
    {
        
        public ICollection<string> PolicyIds { get; set; }
        
        public string Algorithm { get; set; }
    }
}
