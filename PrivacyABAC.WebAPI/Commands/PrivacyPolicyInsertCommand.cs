using PrivacyABAC.DbInterfaces.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.WebAPI.Commands
{

    public class PrivacyPolicyInsertCommand
    {
        public string PolicyID { get; set; }

        public string CollectionName { get; set; }

        public string Description { get; set; }
        

        public string Target { get; set; }

        public ICollection<PrivacyRuleViewModel> Rules { get; set; }
    }

    public class SubPrivacyPolicyInsertCommand
    {
        public string PolicyID { get; set; }

        public string CollectionName { get; set; }

        public string Description { get; set; }
        

        public ICollection<PrivacyRuleViewModel> Rules { get; set; }

        public string DomainName { get; set; }

        public int Priority { get; set; }
    }

    public class PrivacyRuleViewModel
    {
        public string Condition { get; set; }

        public string RuleID { get; set; }

        public ICollection<FieldEffect> FieldEffects { get; set; }
    }
}
