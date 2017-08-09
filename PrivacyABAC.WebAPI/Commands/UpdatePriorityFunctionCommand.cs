using PrivacyABAC.DbInterfaces.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.WebAPI.Commands
{
    public class UpdatePriorityFunctionCommand
    {
        public string DomainName { get; set; }

        public ICollection<PriorityFunction> PriorityFunctions { get; set; }
    }

    public class InsertPriorityFunctionCommand
    {
        public string DomainName { get; set; }

        public PriorityFunction Priority { get; set; }
    }
}
