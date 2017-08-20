using PrivacyABAC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.WebAPI.ViewModel
{
    public class PrivacyCheckingViewModel
    {
        public AccessControlEffect Effect { get; set; }

        public string Data { get; set; }
    }
}
