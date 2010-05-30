namespace SLMedia.Video
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Xml.Linq;

    using SLExtensions;

    using SLMedia.Core;

    public class XItemParserVideo : IXItemParser
    {
        #region Methods

        public bool TryParseItem(XElement element, out IMediaItem item)
        {
            item = null;
            if (element.Name != "video")
            {
                return false;
            }

            VideoItem videoItem = new VideoItem();
            item = videoItem;
            XItemParserMediaItem.ReadItem(videoItem, element);

            var audioTracks = (from ats in element.Elements("audioTracks")
                               from at in ats.Elements("audioTrack")
                               select new AudioTrack { Title = (string)at.Attribute("title") }).ToArray();

            for (int i = 0; i < audioTracks.Length; i++)
            {
                audioTracks[i].Index = i;
            }
            videoItem.AudioTracks = audioTracks;
            videoItem.ItemType = "Video";
            return true;
        }

        #endregion Methods
    }
}