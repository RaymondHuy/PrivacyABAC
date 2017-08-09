using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.WebAPI.Commands
{
    public class PrivacyCheckingCommand
    {
        public string UserID { get; set; }

        public string ResourceName { get; set; }

        public string ResourceCondition { get; set; }

        public string Environment { get; set; }

        public string Action { get; set; }
    }
}
