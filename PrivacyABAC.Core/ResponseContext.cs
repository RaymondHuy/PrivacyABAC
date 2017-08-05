using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Core
{
    public class ResponseContext
    {
        public JArray Data { get; set; }

        public ICollection<JObject> JsonObjects { get; set; }

        public string Message { get; set; }

        public AccessControlEffect Effect { get; private set; }

        public ResponseContext(AccessControlEffect effect, JArray data, string message = null)
        {
            Effect = effect;
            Data = data;
            Message = message;
        }
        public ResponseContext(AccessControlEffect effect, ICollection<JObject> data)
        {
            Effect = effect;
            JsonObjects = data;
        }
    }
}
