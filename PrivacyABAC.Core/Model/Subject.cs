using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Core.Model
{
    public class Subject
    {
        public JObject Data { get; private set; }

        public Subject(JObject data)
        {
            Data = data;
        }
    }
}
