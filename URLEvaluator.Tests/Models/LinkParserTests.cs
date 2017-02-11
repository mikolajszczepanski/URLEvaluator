using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using URLEvaluator.Models;
using System.Linq;
using System.Collections;

namespace URLEvaluator.Tests.Models
{

    class LinkParserTestComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return StringComparer.CurrentCulture.Compare(x, ((Link)y).Url);
        }
    }


    /// <summary>
    /// Summary description for LinkParserTests
    /// </summary>
    [TestClass]
    public class LinkParserTests
    {
        public LinkParserTests()
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
        public void ExtractAbsoluteLinks()
        {
            var rowData = "<p>Test</p>\n <h1>Test</h1>\n <ul>\n <li><a href=\"http://test.web/x/y/1\">test</a></li>\n\t\t\t\t\t<li><a class=\"xyz\" href=\"http://test.web/x/2\">test</a></li>\n\t\t\t\t\t<li><a class=\"xyz\" href = \"http://test.web/x-y-z/x/3/t\">test</a></li>\n\t\t\t\t\t<li><a class=\"x y z\" HREF =\"http://test.web/x-y-z/4\">test</a></li>\n </ul>\n\t\t\t\t<link rel=\"stylesheet\" type=\"text/css\" href=\"theme.css\">";
            var expectedLinks = new List<string>() {
                @"http://test.web/x/y/1",@"http://test.web/x/2",@"http://test.web/x-y-z/x/3/t",@"http://test.web/x-y-z/4"
            };
            TestLinkParser(rowData, expectedLinks);
        }

        [TestMethod]
        public void ExtractRelativeLinks()
        {
            var rowData = "\t\t\t\t<div>\r\n\t\t\t\t<p>Test</p>\r\n                <h1>Test</h1>\r\n                <ul>\r\n                    <li><a href=\"x/1\" class=\"x\">test1</a></li>\r\n\t\t\t\t\t<li><a placeholder=\"text\" href=\"x/y/2\">test2</a></li>\r\n\t\t\t\t\t<li><a class=\"xyz\" href = \"x/y/x-y-z/t-qty/3\">test3</a></li>\r\n                </ul>\r\n\t\t\t\t</div>";
            var expectedLinks = new List<string>() {
                @"x/1",@"x/y/2",@"x/y/x-y-z/t-qty/3"
            };
            TestLinkParser(rowData, expectedLinks);
        }

        private void TestLinkParser(string rowData, List<string> expectedLinks)
        {
            var linkParser = new LinkParser();
            var extractedLinks = linkParser.ExtractLinksFromData(rowData);
            CollectionAssert.AreEqual(expectedLinks, extractedLinks.ToList(), new LinkParserTestComparer());
        }
    }
}
