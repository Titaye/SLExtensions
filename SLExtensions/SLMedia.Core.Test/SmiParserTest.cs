namespace SLMedia.Core.Test
{
    using System;
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

    [TestClass]
    public class SmiParserTest
    {
        #region Fields

        private const string smiContent = @"<SAMI>  <HEAD>    <STYLE TYPE='text/css'>      <!--        P {          font-size: 11pt;          font-family: Arial,SansSerif;          font-weight: normal;          color: #FFFFFF;          background: #000000;          text-align: center;          padding: 5px;        }        .Captions { Name: Captions; lang: EN; }      -->    </STYLE>  </HEAD>  <BODY>    <SYNC Start='900'>      <P Class='Captions'>aaaa</P>    </SYNC>    <SYNC Start='3960'>      <P Class='Captions'>bbb</P>    </SYNC>  </BODY></SAMI>";
        private const string smiContentHtml = @"<SAMI>  <HEAD>    <STYLE TYPE='text/css'>      <!--        P {          font-size: 11pt;          font-family: Arial,SansSerif;          font-weight: normal;          color: #FFFFFF;          background: #000000;          text-align: center;          padding: 5px;        }        .Captions { Name: Captions; lang: EN; }      -->    </STYLE>  </HEAD>  <body>    <SYNC Start='900'>      <P Class='Captions'>aaaa<br>bbb</P>    </SYNC>    <SYNC Start='2000'>      <P Class='Captions'>&nbsp;</P>    </SYNC>    <SYNC Start='3960'>      <P Class='Captions'>bbb<br/><br><br/>aaa</P>    </SYNC>    <SYNC Start='4000'>        <P Class='Captions'>qui &oelig;uvre &agrave; l'acc&egrave;s</P>    </SYNC><SYNC Start='4500'>        <P Class='Captions'>qui<span>test</span> a</P>    </SYNC>  </body></SAMI>";

        #endregion Fields

        #region Methods

        [TestMethod]
        public void TestBasicSmi()
        {
            var markersByLng = SmiParser.ParseSmiFile(smiContent);

            Assert.IsTrue(markersByLng.ContainsKey("EN"));

            var markers = markersByLng["EN"];
            Assert.AreEqual(2, markers.Length);

            Assert.AreEqual(900, markers[0].Position.TotalMilliseconds);
            Assert.AreEqual(3960, markers[1].Position.TotalMilliseconds);
            Assert.AreEqual("aaaa", markers[0].Content);
            Assert.AreEqual("bbb", markers[1].Content);
        }

        [TestMethod]
        public void TestSmiHtmlEncoded()
        {
            var markersByLng = SmiParser.ParseSmiFile(smiContentHtml);

            Assert.IsTrue(markersByLng.ContainsKey("EN"));

            var markers = markersByLng["EN"];
            Assert.AreEqual(4, markers.Length);

            Assert.AreEqual(900, markers[0].Position.TotalMilliseconds);
            Assert.IsTrue(markers[0].Duration.HasTimeSpan);
            Assert.AreEqual(1100, markers[0].Duration.TimeSpan.TotalMilliseconds);
            Assert.AreEqual(3960, markers[1].Position.TotalMilliseconds);
            Assert.AreEqual(4000, markers[2].Position.TotalMilliseconds);
            Assert.AreEqual(4500, markers[3].Position.TotalMilliseconds);
            Assert.AreEqual("aaaa\nbbb", markers[0].Content);
            Assert.AreEqual("bbb\n\n\naaa", markers[1].Content);
            Assert.AreEqual("qui œuvre à l'accès", markers[2].Content);
            Assert.AreEqual("qui test a", markers[3].Content);
        }

        #endregion Methods
    }
}