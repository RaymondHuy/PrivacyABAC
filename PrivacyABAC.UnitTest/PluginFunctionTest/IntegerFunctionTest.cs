using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrivacyABAC.Functions.Fundamental;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.UnitTest
{
    [TestClass]
    public class IntegerFunctionTest
    {
        [TestMethod]
        public void TestIntegerEqual()
        {
            var function = new IntegerFunction();
            Assert.AreEqual(function.ExecuteFunction("Equal", "1", "1"), "True");
        }
        [TestMethod]
        public void TestIntegerGreaterThan()
        {
            var function = new IntegerFunction();
            Assert.AreEqual(function.ExecuteFunction("GreaterThan", "2", "1"), "True");
        }

        [TestMethod]
        public void TestIntegerLessThan()
        {
            var function = new IntegerFunction();
            Assert.AreEqual(function.ExecuteFunction("LessThan", "2", "5"), "True");
        }
    }
}
