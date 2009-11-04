//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace Microsoft.Expression.Encoder.AdaptiveStreaming
{
    using System;
    using System.Windows.Media;

    /// <summary>
    /// Event args for the bitrate changed event
    /// </summary>
    public class BitrateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the BitrateChangedEventArgs class
        /// </summary>
        /// <param name="streamType">the type that is changing</param>
        /// <param name="bitrate">the bitrate we changed to, in kbps</param>
        /// <param name="timestamp">the timestamp of the change</param>
        public BitrateChangedEventArgs(MediaStreamType streamType, ulong bitrate, DateTime timestamp)
        {
            StreamType = streamType;
            Bitrate = bitrate;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Gets or sets the stream type that changed
        /// </summary>
        public MediaStreamType StreamType { get; set; }

        /// <summary>
        /// Gets or sets the new bitrate, in kbps
        /// </summary>
        public ulong Bitrate { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the bitrate change
        /// </summary>
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// These are the event arguments for the public MediaChunkDownloaded event
    /// </summary>
    public class MediaChunkDownloadedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the MediaChunkDownloadedEventArgs class
        /// </summary>
        /// <param name="streamId">stream id of this chunk</param>
        /// <param name="chunkId">id of this chunk</param>
        /// <param name="bitrate">bitrate of this chunk</param>
        public MediaChunkDownloadedEventArgs(int streamId, int chunkId, ulong bitrate)
        {
            StreamId = streamId;
            ChunkId = chunkId;
            Bitrate = bitrate;
        }

        /// <summary>
        /// Gets or sets the Id of the stream from which the chunk was downloaded
        /// </summary>
        public int StreamId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the chunk that was downloaded
        /// </summary>
        public int ChunkId { get; set; }

        /// <summary>
        /// Gets or sets the bitrate of the chunk that was downloaded
        /// </summary>
        public ulong Bitrate { get; set; }
    }

    /// <summary>
    /// Event args class for DownloadPaused event
    /// </summary>
    public class DownloadPausedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DownloadPausedEventArgs class
        /// </summary>
        /// <param name="startTime">time the ad started</param>
        public DownloadPausedEventArgs(TimeSpan startTime)
        {
            StartTime = startTime;
        }

        /// <summary>
        /// Gets or sets the time the pause started
        /// </summary>
        public TimeSpan StartTime { get; set; }
    }
}
