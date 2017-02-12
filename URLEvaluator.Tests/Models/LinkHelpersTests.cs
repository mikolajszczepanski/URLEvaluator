using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using URLEvaluator.Models;

namespace URLEvaluator.Tests.Models
{
    /// <summary>
    /// Summary description for LinkHelpersTests
    /// </summary>
    [TestClass]
    public class LinkHelpersTests
    {
        public LinkHelpersTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CleanUrlFromProtocolTest()
        {
            var url = @"http://www.test.website";
            var cleanedUrl = LinkHelpers.CleanUrlFromProtocol(url);
            Assert.AreEqual(cleanedUrl, "test.website");
        }

        [TestMethod]
        public void MergeRootUrlAndSubpageUrlToAbsoluteTest()
        {
            var rootUrl = "http://websiteroot.com";
            var subpage = "x/y/z/123";
            var merged = LinkHelpers.MergeRootUrlAndSubpageUrlToAbsolute(rootUrl, subpage);
            Assert.AreEqual("http://websiteroot.com/x/y/z/123", merged);
        }

        [TestMethod]
        public void MergeRootUrlAndSubpageUrlToAbsoluteTest2()
        {
            var rootUrl = "http://websiteroot.com";
            var subpage = "/x/y/z/123";
            var merged = LinkHelpers.MergeRootUrlAndSubpageUrlToAbsolute(rootUrl, subpage);
            Assert.AreEqual("http://websiteroot.com/x/y/z/123", merged);
        }

        [TestMethod]
        public void ValidateCorrectUrl()
        {
            var url = "http://test.web";
            var result = LinkHelpers.ValidateUrl(url);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateIncorrectUrl()
        {
            var url = "test.web";
            var result = LinkHelpers.ValidateUrl(url);
            Assert.IsFalse(result);
        }

    }
}
