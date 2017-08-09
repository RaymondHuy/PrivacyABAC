﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PrivacyABAC.DbInterfaces.Repository
{
    public interface IResourceRepository
    {
        JObject[] GetCollectionDataWithCustomFilter(string collectionName, dynamic filter);

        ICollection<string> GetArrayFieldName();
    }
}
