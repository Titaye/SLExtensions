namespace SLMedia.Core.Test
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SLMedia.Core;

    [TestClass]
    public class SrtParserTest
    {
        #region Fields

        private const string srtContent = @"1
        00:01:29,256 --> 00:01:31,859
        Listen to me, please.

        2
        00:01:32,059 --> 00:01:35,862
        You're like me, a homo sapiens,

        3
        00:01:36,063 --> 00:01:38,365
        a wise human.

        4
        00:01:38,565 --> 00:01:39,766
        Life,

        5
        00:01:39,866 --> 00:01:43,970
        a miracle in the universe,
        appeared around 4 billion years ago.

        ";

        #endregion Fields

        #region Methods

        [TestMethod]
        public void TestSrt()
        {
            var markers = SrtParser.ParseSrtFile(srtContent);
            Assert.AreEqual(5, markers.Length);

            Assert.AreEqual("00:01:29.2560000", Convert.ToString(markers[0].Position, new CultureInfo("en-us")));
            Assert.AreEqual("00:01:31.8590000", Convert.ToString((markers[0].Position + markers[0].Duration), new CultureInfo("en-us")));
            Assert.AreEqual("Listen to me, please.", markers[0].Content);

            Assert.AreEqual("00:01:32.0590000", Convert.ToString(markers[1].Position, new CultureInfo("en-us")));
            Assert.AreEqual("00:01:35.8620000", Convert.ToString((markers[1].Position + markers[1].Duration), new CultureInfo("en-us")));
            Assert.AreEqual("You're like me, a homo sapiens,", markers[1].Content);
        }

        #endregion Methods
    }
}