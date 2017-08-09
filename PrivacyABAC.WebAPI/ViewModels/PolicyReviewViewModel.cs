using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.WebAPI.ViewModel
{
    public class PolicyReviewViewModel
    {
        public string PolicyID { get; set; }

        public string Target { get; set; }

        public ICollection<string> Rules { get; }
        
    }
}
