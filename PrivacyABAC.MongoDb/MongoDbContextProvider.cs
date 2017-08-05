using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.MongoDb
{
    public class MongoDbContextProvider
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public void Setup()
        {
        }
    }
}
