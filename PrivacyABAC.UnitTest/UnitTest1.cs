using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace PrivacyABAC.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string json = @"{
  'Name': 'Bad Boys',
  'ReleaseDate': '1995-4-7T00:00:00',
  'Genres': [
    'Action',
    'Comedy'
  ],
  'a': {'b': 'c'},
  'd': null
}";
            JObject j = JObject.Parse(json);
            j.TryGetValue("d", out JToken result);
            System.Console.WriteLine(result);
        }
    }
}
