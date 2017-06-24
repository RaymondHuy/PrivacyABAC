using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.DbInterfaces.Model
{
    public class Function
    {
        public string FunctionName { get; set; }
        
        public List<Function> Parameters { get; set; }

        public string Value { get; set; }

        public string ResourceID { get; set; }
    }
}
