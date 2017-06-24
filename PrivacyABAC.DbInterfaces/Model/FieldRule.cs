using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.DbInterfaces.Model
{
    public class FieldRule
    {
        public ICollection<FieldEffect> FieldEffects { get; set; }
        
        public Function Condition { get; set; }

        public string Identifer { get; set; }
    }
}
