// <copyright file="PlaylistCollection.cs" company="Microsoft">
//     Copyright © Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>Implements the PlaylistCollection class</summary>
// <author>Microsoft Expression Encoder Team</author>
namespace ExpressionMediaPlayer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// This class represents a collection of media items to play.
    /// </summary>
    public class PlaylistCollection : ObservableCollection<PlaylistItem>
    {
        /// <summary>
        /// Initializes a new instance of the PlaylistCollection class.
        /// </summary>
        public PlaylistCollection()
        {
            // Mock playlist
            //ParseXML("<playList><playListItems><playListItem title=\"SilverLight%20Sample%20Video%20File\" description=\"\" mediaSource=\"sl.wmv\" thumbSource=\"sl_Thumb.jpg\" ><chapters><chapter  position=\"0.000\" thumbnailSource=\"sl_0.000.jpg\" title=\"1\" /><chapter  position=\"1.000\" thumbnailSource=\"sl_1.000.jpg\" title=\"2\" /><chapter  position=\"2.000\" thumbnailSource=\"sl_2.000.jpg\" title=\"3\" /><chapter  position=\"3.000\" thumbnailSource=\"sl_3.000.jpg\" title=\"4\" /><chapter  position=\"4.000\" thumbnailSource=\"sl_4.000.jpg\" title=\"5\" /><chapter  position=\"5.000\" thumbnailSource=\"sl_5.000.jpg\" title=\"6\" /></chapters></playListItem></playListItems></playList>");
        }

        /// <summary>
        /// Initializes a new instance of the PlaylistCollection class.
        /// </summary>
        /// <param name="uriDocument">Source Uri of the media items.</param>
        /// <param name="playlistXml">Xml describing this playlist.</param>
        public PlaylistCollection(Uri uriDocument, String playlistXml)
        {
            ParseXml(uriDocument, playlistXml);
        }

        /// <summary>
        /// Parse a playlist item Xml.
        /// </summary>
        /// <param name="uriDocument">Source Uri of the media items.</param>
        /// <param name="playlistXml">Xml describing this playlist.</param>
        public void ParseXml(Uri uriDocument, String playlistXml)
        {
            XDocument xmlPlaylist = XDocument.Parse(playlistXml);
            for (IEnumerator<XElement> playlistEnumerator = xmlPlaylist.Descendants("playListItem").GetEnumerator(); playlistEnumerator.MoveNext(); )
            {
                Add(new PlaylistItem(uriDocument, this, playlistEnumerator.Current));
            }
        }

        /// <summary>
        /// Inserts an item into the collection.
        /// </summary>
        /// <param name="index">Index to insert the item at.</param>
        /// <param name="item">Item to insert.</param>
        protected override void InsertItem(int index, PlaylistItem item)
        {
            item.OwnerCollection = this;
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes an item at the given index.
        /// </summary>
        /// <param name="index">Index of the item to remove.</param>
        protected override void RemoveItem(int index)
        {
            this[index].OwnerCollection = null;
            base.RemoveItem(index);
        }
    }
}
