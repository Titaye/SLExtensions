// <copyright file="IMediaStreamSourceFactory.cs" company="Microsoft">
//     Copyright © Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>Implements the download preogress bar class</summary>
// <author>Microsoft Expression Encoder Team</author>
namespace ExpressionMediaPlayer
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

    /// <summary>
    /// This interface defines a factory creation method for custom media stream sources.
    /// </summary>
    public interface IMediaStreamSourceFactory
    {
        /// <summary>
        /// Factory method to create a custom media stream source.
        /// </summary>
        /// <param name="mediaElement">The hosting MediaElement.</param>
        /// <param name="uri">The Uri to the source content.</param>
        /// <returns>A new MediaStreamSource.</returns>
        MediaStreamSource Create(MediaElement mediaElement, Uri uri);
    }
}
