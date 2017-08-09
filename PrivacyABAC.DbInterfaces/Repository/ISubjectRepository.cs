using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.DbInterfaces.Repository
{
    public interface ISubjectRepository
    {
        JArray GetAllUsers();
        JObject GetUniqueUser(string fieldName, string value);
    }
}
