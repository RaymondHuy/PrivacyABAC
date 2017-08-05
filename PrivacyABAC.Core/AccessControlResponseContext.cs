using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Core
{
    public class AccessControlResponseContext
    {
        public AccessControlEffect Effect { get; private set; }
        public ICollection<JObject> Data { get; private set; }

        public AccessControlResponseContext(AccessControlEffect effect, ICollection<JObject> data)
        {
            Effect = effect;
            Data = data;
        }
    }
}
