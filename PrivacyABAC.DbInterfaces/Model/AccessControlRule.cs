using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.DbInterfaces.Model
{
    public class AccessControlRule : IEntityBase
    {
        public string Id { get; set; }

        public string Effect { get; set; }

        public Function Condition { get; set; }
    }
}
