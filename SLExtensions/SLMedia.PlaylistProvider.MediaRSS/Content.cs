namespace SLMedia.PlaylistProvider.MediaRSS
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
    using System.Xml;
    using System.Xml.Linq;

    using SLExtensions;

    /// <summary>
    /// <media:content 
    ///            url="http://www.foo.com/movie.mov" 
    ///            fileSize="12216320" 
    ///            type="video/quicktime"
    ///            medium="video"
    ///            isDefault="true" 
    ///            expression="full" 
    ///            bitrate="128" 
    ///            framerate="25"
    ///            samplingrate="44.1"
    ///            channels="2"
    ///            duration="185" 
    ///            height="200"
    ///            width="300" 
    ///            lang="en" />
    /// </summary>
    public class Content
    {
        #region Properties

        /// <summary>
        /// is the kilobits per second rate of media. It is an optional attribute.
        /// </summary>
        public int? Bitrate
        {
            get; set;
        }

        /// <summary>
        /// is number of audio channels in the media object. It is an optional attribute. 
        /// </summary>
        public int? Channels
        {
            get; set;
        }

        /// <summary>
        /// is the number of seconds the media object plays. It is an optional attribute.
        /// </summary>
        public int? Duration
        {
            get; set;
        }

        /// <summary>
        /// determines if the object is a sample or the full version of the object, or even if it is a continuous stream (sample | full | nonstop). Default value is 'full'. It is an optional attribute.
        /// </summary>
        public Expression? Expression
        {
            get; set;
        }

        /// <summary>
        /// is the number of bytes of the media object. It is an optional attribute.
        /// </summary>
        public int? FileSize
        {
            get; set;
        }

        /// <summary>
        /// is the number of frames per second for the media object. It is an optional attribute.
        /// </summary>
        public int? Framerate
        {
            get; set;
        }

        /// <summary>
        /// is the height of the media object. It is an optional attribute.
        /// </summary>
        public int? Height
        {
            get; set;
        }

        /// <summary>
        /// determines if this is the default object that should be used for the &lt;media:group&gt;. There should only be one default object per &lt;media:group&gt;. It is an optional attribute.
        /// </summary>
        public bool? IsDefault
        {
            get; set;
        }

        /// <summary>
        /// is the primary language encapsulated in the media object. Language codes possible are detailed in RFC 3066. This attribute is used similar to the xml:lang attribute detailed in the XML 1.0 Specification (Third Edition). It is an optional attribute.
        /// </summary>
        public string Lang
        {
            get; set;
        }

        /// <summary>
        /// is the type of object (image | audio | video | document | executable). While this attribute can at times seem redundant if type is supplied, it is included because it simplifies decision making on the reader side, as well as flushes out any ambiguities between MIME type and object type. It is an optional attribute.
        /// </summary>
        public Medium? Medium
        {
            get; set;
        }

        /// <summary>
        /// is the number of samples per second taken to create the media object. It is expressed in thousands of samples per second (kHz). It is an optional attribute.
        /// </summary>
        public double? SamplingRate
        {
            get; set;
        }

        /// <summary>
        /// is the standard MIME type of the object. It is an optional attribute.
        /// </summary>
        public string Type
        {
            get; set;
        }

        /// <summary>
        /// should specify the direct url to the media object. If not included, a <media:player> element must be specified.
        /// </summary>
        public string Url
        {
            get; set;
        }

        /// <summary>
        /// is the width of the media object. It is an optional attribute.
        /// </summary>
        public int? Width
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public static Content FromXml(XElement elem)
        {
            Content content = new Content();
            content.Url = elem.GetAttribute("url");
            content.FileSize = elem.GetIntAttribute("fileSize");
            content.Type = elem.GetAttribute("type");
            string medium = elem.GetAttribute("medium");
            if (!string.IsNullOrEmpty(medium))
                content.Medium = (Medium)Enum.Parse(typeof(Medium), medium, true);
            content.IsDefault = elem.GetBoolAttribute("isDefault");

            string expression = elem.GetAttribute("expression");
            if (!string.IsNullOrEmpty(expression))
                content.Expression = (Expression)Enum.Parse(typeof(Expression), expression, true);

            content.Bitrate = elem.GetIntAttribute("bitrate");
            content.Framerate = elem.GetIntAttribute("framerate");
            content.SamplingRate = elem.GetDoubleAttribute("samplingrate");
            content.Channels = elem.GetIntAttribute("channels");
            content.Duration = elem.GetIntAttribute("duration");
            content.Height = elem.GetIntAttribute("height");
            content.Width = elem.GetIntAttribute("width");
            content.Lang = elem.GetAttribute("lang");

            return content;
        }

        #endregion Methods
    }
}