using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Core.Model
{
    public class EnvironmentObject
    {
        public JObject Data { get; private set; }

        public EnvironmentObject(JObject data)
        {
            Data = data;
        }
    }
}
