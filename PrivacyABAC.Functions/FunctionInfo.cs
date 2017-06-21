using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Functions
{
    public class FunctionInfo
    {
        public string Description { get; private set; }

        public string Example { get; private set; }

        public int NumberParameters { get; private set; }

        public string Name { get; private set; }

        public FunctionInfo(string name, int numberParameters, string description, string example)
        {
            Name = name;
            NumberParameters = numberParameters;
            Description = description;
            Example = example;
        }

        public FunctionInfo(string name, int numberParameters)
        {
            Name = name;
            NumberParameters = numberParameters;
            Description = string.Empty;
            Example = string.Empty;
        }
    }
}
