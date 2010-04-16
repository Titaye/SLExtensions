namespace SLExtensions.Test.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SLExtensions.Data;

    [TestClass]
    public class BoolConverterTest
    {
        #region Fields

        BoolConverter converter = new BoolConverter();

        #endregion Fields

        #region Enumerations

        public enum TestEnum
        {
            Default,
            EnumValue = 1
        }

        #endregion Enumerations

        #region Methods

        [TestMethod]
        public void TestDecimalArithmetics()
        {
            //Test basic decimal arithmetics
            Assert.AreEqual(converter.Convert(10, null, ">10", null), false);
            Assert.AreEqual(converter.Convert(10, null, "<10", null), false);

            Assert.AreEqual(converter.Convert(10, null, ">=10", null), true);
            Assert.AreEqual(converter.Convert(10, null, "<=10", null), true);

            Assert.AreEqual(converter.Convert(11, null, ">10", null), true);
            Assert.AreEqual(converter.Convert(9, null, "<10", null), true);

            Assert.AreEqual(converter.Convert(11, null, "!10", null), true);
            Assert.AreEqual(converter.Convert(11, null, "!=10", null), true);
        }

        [TestMethod]
        public void TestDecimalDefault()
        {
            //Test default decimal convertion. >= 1: true
            Assert.AreEqual(converter.Convert(1, null, null, null), true);
            Assert.AreEqual(converter.Convert(0, null, null, null), false);
        }

        [TestMethod]
        public void TestEnumEquality()
        {
            //Test equality and string convertion
            Assert.AreEqual(converter.Convert(TestEnum.EnumValue, null, "EnumValue", null), true);
            Assert.AreEqual(converter.Convert(TestEnum.EnumValue, null, "1", null), true);
            Assert.AreEqual(converter.Convert(TestEnum.Default, null, "EnumValue", null), false);
        }

        [TestMethod]
        public void TestNotNullity()
        {
            var obj = new object();

            //Test null and not parameter
            Assert.AreEqual(converter.Convert(obj, null, "!", null), false);
            Assert.AreEqual(converter.Convert(null, null, "!", null), true);
        }

        [TestMethod]
        public void TestNullity()
        {
            var obj = new object();

            //Test null
            Assert.AreEqual(converter.Convert(obj, null, null, null), true);
            Assert.AreEqual(converter.Convert(null, null, null, null), false);
        }

        [TestMethod]
        public void TestObjectEquality()
        {
            var obj = new object();
            var obj2 = new object();

            //Test object equality
            Assert.AreEqual(converter.Convert(obj, null, obj, null), true);
            Assert.AreEqual(converter.Convert(obj, null, obj2, null), false);
        }

        [TestMethod]
        public void TestObjectEqualityAndConvertionFromString()
        {
            //Test equality and string convertion
            Assert.AreEqual(converter.Convert(1, null, "1", null), true);
            Assert.AreEqual(converter.Convert(1, null, "2", null), false);
        }

        [TestMethod]
        public void TestStringIsNotNull()
        {
            //Test default string convertion. !String.IsNullOrEmpty : true
            Assert.AreEqual(converter.Convert("aaa", null, null, null), true);
            Assert.AreEqual(converter.Convert("", null, null, null), false);
        }

        #endregion Methods
    }
}