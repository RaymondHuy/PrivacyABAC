using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Core.Model
{
    public class Resource
    {
        public JObject[] Data { get; private set; }

        public string Name { get; private set; }

        public Resource(JObject[] data, string name)
        {
            Name = name;
            Data = data;
        }
    }
}
