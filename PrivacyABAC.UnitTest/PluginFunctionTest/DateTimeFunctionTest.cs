using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrivacyABAC.Functions.Fundamental;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.UnitTest
{
    [TestClass]
    public class DateTimeFunctionTest
    {
        [TestMethod]
        public void TestDateTimeEqual()
        {
            var function = new DateTimeFunction();
            Assert.AreEqual(function.ExecuteFunction("Equal", "1/1/2013", "1/1/2013"), "True");
        }

        [TestMethod]
        public void TestDateTimeGreaterThan()
        {
            var function = new DateTimeFunction();
            Assert.AreEqual(function.ExecuteFunction("GreaterThan", "2/2/2013", "1/1/2013"), "True");
            Assert.AreEqual(function.ExecuteFunction("GreaterThan", "2:00 PM", "1: 15 PM"), "True");
        }

        [TestMethod]
        public void TestDateTimeLessThan()
        {
            var function = new DateTimeFunction();
            Assert.AreEqual(function.ExecuteFunction("LessThan", "2/2/2003", "1/1/2013"), "True");
            Assert.AreEqual(function.ExecuteFunction("LessThan", "2:00 PM", "4: 15 PM"), "True");
        }
    }
}
