namespace SLExtensions.Test.Data
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SLExtensions.Data;

    [TestClass]
    public class ResourceSelectorTest
    {
        #region Enumerations

        public enum TestEnum
        {
            FirstValue,
            SecondValue
        }

        #endregion Enumerations

        #region Methods

        [TestMethod]
        public void ConvertTestFQN()
        {
            var testee = GetConvertTestsTestee();
            var expected = "Fully Qualified";
            object actual = testee.Convert(new CustomType(), typeof(object), null, System.Threading.Thread.CurrentThread.CurrentUICulture);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertTestFallBack()
        {
            var testee = GetConvertTestsTestee();
            var expected = "FallBack";
            object actual = testee.Convert("not existing key", typeof(object), null, System.Threading.Thread.CurrentThread.CurrentUICulture);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertTestInheritance()
        {
            var testee = GetConvertTestsTestee();
            var expected = "ClassA";
            object actual = testee.Convert(new ClassB(), typeof(object), null, System.Threading.Thread.CurrentThread.CurrentUICulture);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertTestInt32()
        {
            var testee = GetConvertTestsTestee();
            var expected = "ok";
            object actual = testee.Convert(12, typeof(object), null, System.Threading.Thread.CurrentThread.CurrentUICulture);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertTestNamespaceQualified()
        {
            var testee = GetConvertTestsTestee();
            var expected = "ok";
            object actual = testee.Convert(new Thread(() => { }), typeof(object), null, System.Threading.Thread.CurrentThread.CurrentUICulture);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertTestNull()
        {
            var testee = GetConvertTestsTestee();
            var expected = "ok";
            object actual = testee.Convert(null, typeof(object), null, System.Threading.Thread.CurrentThread.CurrentUICulture);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertTestShortName()
        {
            var testee = GetConvertTestsTestee();
            var expected = "ok";
            object actual = testee.Convert(this, typeof(object), null, System.Threading.Thread.CurrentThread.CurrentUICulture);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertTestString()
        {
            var testee = GetConvertTestsTestee();
            var expected = "ok";
            object actual = testee.Convert("test", typeof(object), null, System.Threading.Thread.CurrentThread.CurrentUICulture);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DetermineResourceKeyBehaviorTestCustomType()
        {
            var expected = ResourceSelectorBehavior.ValueTypeAsKey;
            var testee = new ResourceSelector();
            var actual = ResourceSelector.DetermineResourceKeyBehavior(new CustomType());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DetermineResourceKeyBehaviorTestDateTime()
        {
            var expected = ResourceSelectorBehavior.ValueAsKey;
            var testee = new ResourceSelector();
            var actual = ResourceSelector.DetermineResourceKeyBehavior(DateTime.Now);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DetermineResourceKeyBehaviorTestEnum()
        {
            var expected = ResourceSelectorBehavior.ValueAsKey;
            var testee = new ResourceSelector();
            var actual = ResourceSelector.DetermineResourceKeyBehavior(TestEnum.FirstValue);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DetermineResourceKeyBehaviorTestIProvideResourceKey()
        {
            var expected = ResourceSelectorBehavior.ValueAsKey;
            var testee = new ResourceSelector();
            var actual = ResourceSelector.DetermineResourceKeyBehavior(new TestImplementor());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DetermineResourceKeyBehaviorTestInt32()
        {
            var expected = ResourceSelectorBehavior.ValueAsKey;
            var testee = new ResourceSelector();
            var actual = ResourceSelector.DetermineResourceKeyBehavior(12);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DetermineResourceKeyBehaviorTestNull()
        {
            var expected = ResourceSelectorBehavior.ValueAsKey;
            var testee = new ResourceSelector();
            var actual = ResourceSelector.DetermineResourceKeyBehavior(null);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DetermineResourceKeyBehaviorTestString()
        {
            var expected = ResourceSelectorBehavior.ValueAsKey;
            var testee = new ResourceSelector();
            var actual = ResourceSelector.DetermineResourceKeyBehavior("test");
            Assert.AreEqual(expected, actual);
        }

        private ResourceSelector GetConvertTestsTestee()
        {
            ResourceDictionary testee = new ResourceDictionary();
            testee.Add(string.Empty, "ok");
            testee.Add("test", "ok");
            testee.Add("12", "ok");
            testee.Add("SLExtensions.Test.Data.ResourceSelectorTest+CustomType, SLExtensions.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                , "Fully Qualified");
            testee.Add("System.Threading.Thread", "ok");
            testee.Add("ResourceSelectorTest", "ok");
            testee.Add("SecondValue", "ok");
            testee.Add("ClassA", "ClassA");
            testee.Add("Object", "Object");
            testee.Add("FallBack", "FallBack");
            return new ResourceSelector { Resources = testee, FallbackResourceKey="FallBack" };
        }

        #endregion Methods

        #region Nested Types

        private class ClassA
        {
        }

        private class ClassB : ClassA
        {
        }

        private class CustomType
        {
        }

        private class TestImplementor : IProvideResourceKey
        {
            #region Properties

            public string ResourceKey
            {
                get { return "TestKey"; }
            }

            #endregion Properties
        }

        #endregion Nested Types
    }
}
