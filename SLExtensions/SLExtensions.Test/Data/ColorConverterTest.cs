namespace SLExtensions.Test.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SLExtensions.Data;

    [TestClass]
    public class ColorConverterTest
    {
        #region Fields

        private ColorConverter converter = new ColorConverter();

        #endregion Fields

        #region Methods

        [TestMethod]
        public void TestColorReflection()
        {
            Color color = (Color)converter.Convert("Transparent", typeof(Color), null, CultureInfo.CurrentCulture);
            Assert.Equals(color, Colors.Transparent);
        }

        [TestMethod]
        public void TestHexa()
        {
            Color color = (Color) converter.Convert("#1F2F3F4F", typeof(Color), null, CultureInfo.CurrentCulture);
            Assert.Equals(color.A, 0x1F);
            Assert.Equals(color.R, 0x2F);
            Assert.Equals(color.G, 0x3F);
            Assert.Equals(color.B, 0x4F);
        }

        [TestMethod]
        public void TestHtmlColorName()
        {
            Color color = (Color)converter.Convert("MintCream", typeof(Color), null, CultureInfo.CurrentCulture);
            Assert.Equals(color.A, 0xFF);
            Assert.Equals(color.R, 0xF5);
            Assert.Equals(color.G, 0xFF);
            Assert.Equals(color.B, 0xFA);
        }

        #endregion Methods
    }
}