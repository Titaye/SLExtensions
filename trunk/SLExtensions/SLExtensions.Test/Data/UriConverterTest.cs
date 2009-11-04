namespace SLExtensions.Test.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SLExtensions.Data;

    [TestClass]
    public class UriConverterTest
    {
        #region Methods

        [TestMethod]
        public void TestConvertBackAbsolute()
        {
            string absolute = "http://www.SLExtensions.Controls.com";
            Uri absoluteUri = new Uri(absolute);

            UriConverter converter = new UriConverter();

            object result = converter.ConvertBack(absoluteUri, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.IsInstanceOfType(result, typeof(string));

            Assert.AreEqual(absoluteUri.ToString(), (string)result);

            result = converter.ConvertBack(absoluteUri, typeof(Uri), null, CultureInfo.CurrentCulture);
            Assert.IsInstanceOfType(result, typeof(Uri));

            Assert.AreEqual(absoluteUri.ToString(), ((Uri)result).ToString());
        }

        [TestMethod]
        public void TestConvertBackRelative()
        {
            string relative = "default.aspx";
            Uri relativeUri = new Uri(relative, UriKind.RelativeOrAbsolute);
            UriConverter converter = new UriConverter();

            object result = converter.ConvertBack(relativeUri, typeof(string), null, CultureInfo.CurrentCulture);
            Assert.IsInstanceOfType(result, typeof(string));

            Assert.AreEqual(relativeUri.ToString(), (string)result);

            result = converter.ConvertBack(relativeUri, typeof(Uri), null, CultureInfo.CurrentCulture);
            Assert.IsInstanceOfType(result, typeof(Uri));

            Assert.AreEqual(relativeUri.ToString(), ((Uri)result).ToString());
        }

        [TestMethod]
        public void TestConvertBackRelativeWithParamString()
        {
            Uri resultUri = new Uri("default.aspx", UriKind.RelativeOrAbsolute);

            Uri sourceUri = new Uri("http://www.SLExtensions.Controls.com/default.aspx");

            UriConverter converter = new UriConverter();

            string paramString = "http://www.SLExtensions.Controls.com";

            object result = converter.ConvertBack(sourceUri, typeof(string), paramString, CultureInfo.CurrentCulture);
            Assert.IsInstanceOfType(result, typeof(string));

            Assert.AreEqual(resultUri.ToString(), (string)result);

            result = converter.ConvertBack(sourceUri, typeof(Uri), paramString, CultureInfo.CurrentCulture);
            Assert.IsInstanceOfType(result, typeof(Uri));

            Assert.AreEqual(resultUri.ToString(), ((Uri)result).ToString());
        }

        [TestMethod]
        public void TestConvertBackRelativeWithParamUri()
        {
            Uri resultUri = new Uri("default.aspx", UriKind.RelativeOrAbsolute);

            Uri sourceUri = new Uri("http://www.SLExtensions.Controls.com/default.aspx");

            UriConverter converter = new UriConverter();

            Uri param = new Uri("http://www.SLExtensions.Controls.com");

            object result = converter.ConvertBack(sourceUri, typeof(string), param, CultureInfo.CurrentCulture);
            Assert.IsInstanceOfType(result, typeof(string));

            Assert.AreEqual(resultUri.ToString(), (string)result);

            result = converter.ConvertBack(sourceUri, typeof(Uri), param, CultureInfo.CurrentCulture);
            Assert.IsInstanceOfType(result, typeof(Uri));

            Assert.AreEqual(resultUri.ToString(), ((Uri)result).ToString());
        }

        [TestMethod]
        public void TestConvertDirectAbsolute()
        {
            string absolute = "http://www.SLExtensions.Controls.com";
            Uri absoluteUri = new Uri(absolute);
            UriConverter converter = new UriConverter();

            object result = converter.Convert(absolute, typeof(Uri), null, CultureInfo.CurrentCulture);
            Assert.IsInstanceOfType(result, typeof(Uri));

            Assert.AreEqual(absoluteUri.ToString(), ((Uri)result).ToString());

            result = converter.Convert(absoluteUri, typeof(Uri), null, CultureInfo.CurrentCulture);
            Assert.IsInstanceOfType(result, typeof(Uri));

            Assert.AreEqual(absoluteUri.ToString(), ((Uri)result).ToString());
        }

        [TestMethod]
        public void TestConvertDirectRelative()
        {
            string relative = "default.aspx";
            Uri relativeUri = new Uri(relative, UriKind.RelativeOrAbsolute);
            UriConverter converter = new UriConverter();

            object result = converter.Convert(relative, typeof(Uri), null, CultureInfo.CurrentCulture);
            Assert.IsInstanceOfType(result, typeof(Uri));

            Assert.AreEqual(relativeUri.ToString(), ((Uri)result).ToString());

            result = converter.Convert(relativeUri, typeof(Uri), null, CultureInfo.CurrentCulture);
            Assert.IsInstanceOfType(result, typeof(Uri));

            Assert.AreEqual(relativeUri.ToString(), ((Uri)result).ToString());
        }

        [TestMethod]
        public void TestConvertDirectRelativeWithParamString()
        {
            string relative = "default.aspx";
            Uri relativeUri = new Uri(relative, UriKind.RelativeOrAbsolute);

            Uri resultUri = new Uri("http://www.SLExtensions.Controls.com/default.aspx");
            UriConverter converter = new UriConverter();

            string paramString = "http://www.SLExtensions.Controls.com";

            object result = converter.Convert(relative, typeof(Uri), paramString, CultureInfo.CurrentCulture);
            Assert.IsInstanceOfType(result, typeof(Uri));

            Assert.AreEqual(resultUri.ToString(), ((Uri)result).ToString());

            result = converter.Convert(relativeUri, typeof(Uri), paramString, CultureInfo.CurrentCulture);
            Assert.IsInstanceOfType(result, typeof(Uri));

            Assert.AreEqual(resultUri.ToString(), ((Uri)result).ToString());
        }

        [TestMethod]
        public void TestConvertDirectRelativeWithParamUri()
        {
            string relative = "default.aspx";
            Uri relativeUri = new Uri(relative, UriKind.RelativeOrAbsolute);

            Uri resultUri = new Uri("http://www.SLExtensions.Controls.com/default.aspx");
            UriConverter converter = new UriConverter();

            Uri param = new Uri("http://www.SLExtensions.Controls.com");

            object result = converter.Convert(relative, typeof(Uri), param, CultureInfo.CurrentCulture);
            Assert.IsInstanceOfType(result, typeof(Uri));

            Assert.AreEqual(resultUri.ToString(), ((Uri)result).ToString());

            result = converter.Convert(relativeUri, typeof(Uri), param, CultureInfo.CurrentCulture);
            Assert.IsInstanceOfType(result, typeof(Uri));

            Assert.AreEqual(resultUri.ToString(), ((Uri)result).ToString());
        }

        #endregion Methods
    }
}