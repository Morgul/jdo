using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

using JDO;
using JDO.Dynamic;

namespace JDO_Tests
{
    
    
    /// <summary>
    ///This is a test class for JDO and is intended
    ///to contain all JDO Unit Tests
    ///</summary>
    [TestClass()]
    public class JSONTest
    {
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

        // Our test string
        public string jsonTestString = "{\"foo\":1,\"bar\":\"test\",\"nested\":{\"nestedagain\":{\"bleh\":null},\"bar\":\"test\"},\"nestedList\":[1,\"foo\",[1,2],{\"bleh\":null}]}";
        public static dynamic jsonTestObject = null;

        #region Additional test attributes
        
        // You can use the following additional attributes as you write your tests:
        
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void ClassInitialize(TestContext testContext)
        {
            // Build the object representation of jsonTestString by hand.
            var testDict = new Dictionary<String, Object>();
            testDict["foo"] = 1;
            testDict["bar"] = "test";
            
            var nested = new Dictionary<String, Object>();
            testDict["nested"] = nested;

            var nestedAgain = new Dictionary<String, Object>();
            nestedAgain["bleh"] = null;

            nested["nestedagain"] = nestedAgain;
            nested["bar"] = "test";

            var nestedList = new List<Object>();
            nestedList.Add(1);
            nestedList.Add("foo");

            var subList = new List<Object>();
            subList.Add(1);
            subList.Add(2);
            nestedList.Add(subList);

            var subDict = new Dictionary<String, Object>();
            subDict["bleh"] = null;
            nestedList.Add(subDict);

            testDict["nestedList"] = nestedList;

            // Wrap our newly constructed hierarchy.
            jsonTestObject = DynObject.Wrap(testDict);
        } // end ClassInitialize
        
        // Use ClassCleanup to run code after all tests in a class have run
        /*
        [ClassCleanup()]
        //public static void MyClassCleanup()
        {
        }
        */
        // Use TestInitialize to run code before running each test
        /*
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }
        */
        // Use TestCleanup to run code after each test has run
        /*
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }
        */

        #endregion

        #region JSON Tests

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest()
        {
            dynamic actual = JSON.Load(jsonTestString);
            Assert.AreEqual(jsonTestObject.bar, actual.bar);
        } // end LoadTest

        /// <summary>
        ///A test for Dump
        ///</summary>
        [TestMethod()]
        public void DumpTest()
        {
            string actual = JSON.Dump(jsonTestObject);
            Assert.AreEqual(jsonTestString, actual);
        } // end DumpTest

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest()
        {
            string actual = JSON.Dump(jsonTestObject);
            Assert.AreEqual(jsonTestString, actual);
        } // end SerializeTest

        /// <summary>
        ///A test for Deserialize
        ///</summary>
        [TestMethod()]
        public void DeserializeTest()
        {
            dynamic actual = JSON.Load(jsonTestString);
            Assert.AreEqual(jsonTestObject.bar, actual.bar);
        } // end DeserializeTest

        #endregion

        #region DynObject Tests

        [TestMethod()]
        public void PrintingTest()
        {
            string actual = jsonTestObject.ToString();
            Assert.AreEqual(jsonTestString, actual);
        } // end PrintingTest

        [TestMethod()]
        public void ImplicitCastingStringTest()
        {
            string actual = jsonTestObject;
            Assert.AreEqual(jsonTestString, actual);
        } // end ImplicitCastingStringTest

        [TestMethod()]
        public void ExplicitCastingStringTest()
        {
            string actual = (string) jsonTestObject;
            Assert.AreEqual(jsonTestString, actual);
        } // end ExplicitCastingStringTest

        [TestMethod()]
        public void AccessorTest()
        {
            Assert.AreEqual("test", jsonTestObject.bar);
        } // end AccessorTest

        [TestMethod()]
        public void BasicSetTest()
        {
            dynamic dyn = new DynObject();
            dyn.foo = "bar";
            Assert.AreEqual("bar", dyn.foo);
        } // end BasicSetTest

        [TestMethod()]
        public void ExtendedSetTest()
        {
            dynamic dyn = new DynObject();
            dyn.foo.bar.baz = "bar";
            Assert.AreEqual("bar", dyn.foo.bar.baz);
        } // end ExtendedSetTest

        [TestMethod()]
        public void NestedTest()
        {
            Assert.AreEqual("test", jsonTestObject.nested.bar);
            Assert.AreEqual(null, jsonTestObject.nested.nestedagain.bleh);
        } // end ExtendedSetTest

        [TestMethod()]
        public void NestedListTest()
        {
            //[1,\"foo\",[1,2],{\"bleh\":null}]
            Assert.AreEqual(1, jsonTestObject.nestedList[0]);
            Assert.AreEqual(2, jsonTestObject.nestedList[2][1]);
            Assert.AreEqual(null, jsonTestObject.nestedList[3].bleh);
        } // end ExtendedSetTest

        [TestMethod()]
        public void PropertyTest()
        {
            Assert.AreEqual(jsonTestObject.wrappedObject.Count, jsonTestObject.Count);
        } // end PropertyTest

        [TestMethod()]
        public void MethodTest()
        {
            dynamic dyn = new DynObject();
            dyn.Add("foo", "bar");
            Assert.AreEqual("bar", dyn.foo);
        } // end AccessorTest

        [TestMethod()]
        public void CreateSerializeTest()
        {
            // "{\"foo\":1,\"bar\":\"test\",\"nested\":{\"nestedagain\":{\"bleh\":null},\"bar\":\"test\"},\"nestedList\":[1,\"foo\",[1,2],{\"bleh\":null}]}"

            dynamic dyn = new DynObject();
            dyn.foo = 1;
            dyn.bar = "test";
            dyn.nested.nestedagain.bleh = null;
            dyn.nested.bar = "test";
            dyn.nestedList = new DynList {1, "foo", new DynList {1, 2}, new DynObject {{"bleh", null}}};

            Assert.AreEqual(jsonTestString, JSON.Dump(dyn));
        } // end ExtendedSetTest
        #endregion

        #region DynList Tests

        /*
        [TestMethod()]
        public void DeserializeTest()
        {
            dynamic actual = JSON.Load(jsonTestString);
            Assert.AreEqual(jsonTestObject.bar, actual.bar);
        } // end DeserializeTest
        */
        #endregion
    } // end JSONTest
} // end namespace
