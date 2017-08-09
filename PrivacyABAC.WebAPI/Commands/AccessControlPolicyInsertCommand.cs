using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.WebAPI.Commands
{
    public class AccessControlPolicyInsertCommand
    {
        public string PolicyID { get; set; }

        public string CollectionName { get; set; }

        public string Description { get; set; }

        public string Action { get; set; }

        public string RuleCombining { get; set; }

        public string Target { get; set; }

        public bool IsAttributeResourceRequired { get; set; }

        public ICollection<AccessControlRuleViewModel> Rules { get; set; }
    }

    public class AccessControlRuleViewModel
    {
        public string RuleID { get; set; }

        public string Condition { get; set; }

        public string Effect { get; set; }
    }
}
