namespace SLExtensions.Test.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SLExtensions.Data;

    [TestClass]
    public class VisibilityConverterTest
    {
        #region Methods

        [TestMethod]
        public void TestConvertWithParameter()
        {
            VisibilityConverterParameter prm = new VisibilityConverterParameter();

            VisibilityConverter converter = new VisibilityConverter();
            Assert.AreEqual(converter.Convert("b", null, prm, null), Visibility.Collapsed);

            prm.Value = "a";

            Assert.AreEqual(converter.Convert("b", null, prm, null), Visibility.Collapsed);
            Assert.AreEqual(converter.Convert("a", null, prm, null), Visibility.Visible);

            prm.Condition = VisibilityCondition.IfValueCollapsed;
            Assert.AreEqual(converter.Convert("b", null, prm, null), Visibility.Visible);
            Assert.AreEqual(converter.Convert("a", null, prm, null), Visibility.Collapsed);

            Assert.AreEqual(converter.Convert("b", null, "a", null), Visibility.Collapsed);
            Assert.AreEqual(converter.Convert("a", null, "a", null), Visibility.Visible);

            Assert.AreEqual(converter.Convert(false, null, null, null), Visibility.Collapsed);
            Assert.AreEqual(converter.Convert(true, null, null, null), Visibility.Visible);

            Assert.AreEqual(converter.Convert(null, null, null, null), Visibility.Collapsed);
            Assert.AreEqual(converter.Convert("azerty", null, null, null), Visibility.Visible);
        }

        #endregion Methods
    }
}
