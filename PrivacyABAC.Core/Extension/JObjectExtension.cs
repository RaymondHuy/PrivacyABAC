using Newtonsoft.Json.Linq;
using PrivacyABAC.Domains.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Core.Extension
{
    public static class JObjectExtension
    {
        public static void AddNewField(this JObject jObject, string pathField, JObject rawObject, string privacyFunction)
        {
            var nestedFields = pathField.Split(new char[] { '.' });
            jObject.AddNewFieldFromPath(rawObject, nestedFields, 0, privacyFunction);
        }
        private static void AddNewFieldFromPath(this JObject privacyObject, JObject rawObject, string[] pathField, int startIndex, string privacyFunction)
        {
            var field = pathField[startIndex];
            var token = rawObject.SelectToken(field);

            if (token is JArray)
            {
                privacyObject[field] = privacyObject[field] ?? new JArray();

                var rawArray = (JArray)rawObject.SelectToken(field);
                var privacyArray = (JArray)privacyObject.SelectToken(field);
                int currentIndex = 0;

                foreach (var element in rawArray)
                {
                    if (element is JObject)
                    {
                        var nextRawObject = (JObject)element;
                        var pathIndexObject = field + "[" + currentIndex + "]";

                        if (privacyObject.SelectToken(pathIndexObject) == null)
                            privacyArray.Add(new JObject());

                        var nextPrivacyObject = (JObject)privacyObject.SelectToken(pathIndexObject);
                        nextPrivacyObject.AddNewFieldFromPath(nextRawObject, pathField, startIndex + 1, privacyFunction);

                    }
                    else
                    {
                        string className = privacyFunction.Split('.')[0];
                        string functionName = privacyFunction.Split('.')[1];
                        var privacyDomainFactory = PrivacyDomainPluginFactory.GetInstance();
                        string param = rawObject[field][currentIndex].ToString();
                        string result = privacyDomainFactory.ExecuteFunction(className, functionName, new string[] { param });
                        privacyArray.Add(result);
                    }
                    ++currentIndex;
                }
            }
            else if (token is JObject)
            {
                privacyObject[field] = privacyObject[field] ?? new JObject();

                var nextPrivacyObject = (JObject)privacyObject[field];
                var nextRawObject = (JObject)rawObject[field];

                nextPrivacyObject.AddNewFieldFromPath(nextRawObject, pathField, startIndex + 1, privacyFunction);
            }
            else
            {
                string className = privacyFunction.Split('.')[0];
                string functionName = privacyFunction.Split('.')[1];
                var privacyDomainFactory = PrivacyDomainPluginFactory.GetInstance();
                string param = rawObject[field].ToString();
                string result = privacyDomainFactory.ExecuteFunction(className, functionName, new string[] { param });
                privacyObject[field] = result;
            }
        }
    }
}
