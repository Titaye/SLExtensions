//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace Microsoft.Expression.Encoder.AdaptiveStreaming
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows.Media;
    using System.Xml;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities;

    /// <summary>
    /// This class defines the basic information that needs to be exposed 
    /// for each stream in the manifest. You can extend this class if
    /// you want to add any private per stream data.
    /// </summary>
    public class StreamInfo
    {
        /// <summary>
        /// This is our list of attributes for each bitrate.
        /// The media stream attributes, as used by MediaElement. There are only a max of 4 of them,
        /// (4 for video and 1 for audio). They include: XCP_MS_UINT32_4CC, XCP_MS_UINT32_WIDTH,
        /// XCP_MS_UINT32_HEIGHT, XCP_MS_BLOB_VIDEO_CODEC for Video and XCP_MS_BLOB_WAVEFORMATEX
        /// for audio
        /// </summary>
        private List<BitrateAttributeData> m_attributes = new List<BitrateAttributeData>(MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities.Configuration.Streams.MaxBitratesPerStream);

        /// <summary>
        /// The base Url for chunks in this stream
        /// </summary>
        private string m_baseUrl = string.Empty;

        /// <summary>
        /// This is the list of bitrates that are in this stream, in bps
        /// </summary>
        private List<ulong> m_bitrates = new List<ulong>(MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities.Configuration.Streams.MaxBitratesPerStream);

        /// <summary>
        /// This is our queue of chunks in this stream
        /// </summary>
        private MediaChunkQueue m_chunksQueue;

        /// <summary>
        /// The language of the media in this stream
        /// </summary>
        private string m_language = string.Empty;

        /// <summary>
        /// The MediaStreamType of this stream. We initialize to Script, which doubles
        /// as our invalid valid since we do not support Script streams
        /// </summary>
        private MediaStreamType m_mediaType = MediaStreamType.Script;

        /// <summary>
        /// The number of chunks in this stream
        /// </summary>
        private int m_numberOfChunksInStream;

        /// <summary>
        /// Initializes a new instance of the StreamInfo class
        /// </summary>
        /// <param name="baseUrl">the base url for chunks in this stream</param>
        /// <param name="language">the language of this stream</param>
        /// <param name="numberOfChunks">the number of chunks in this stream</param>
        /// <param name="mediaType">the MediaStreamType of this stream</param>
        /// <param name="streamId">the id of this stream</param>
        public StreamInfo(string baseUrl, string language, int numberOfChunks, MediaStreamType mediaType, int streamId)
        {
            m_baseUrl = baseUrl;
            m_mediaType = mediaType;
            m_language = language;
            m_numberOfChunksInStream = numberOfChunks;

            // Initialize our queue of chunks
            m_chunksQueue = new MediaChunkQueue(numberOfChunks, MediaType, streamId);
        }

        /// <summary>
        /// Gets the base Url for chunks in this stream
        /// </summary>
        public string BaseUrl
        {
            get
            {
                return m_baseUrl;
            }
        }

        /// <summary>
        /// Gets the list of bitrates in this stream, in bps
        /// </summary>
        public ReadOnlyCollection<ulong> Bitrates
        {
            get
            {
                return new ReadOnlyCollection<ulong>(m_bitrates);
            }
        }

        /// <summary>
        /// Gets or sets the description of this media stream as used by MediaElement
        /// </summary>
        public MediaStreamDescription Description { get; set; }

        /// <summary>
        /// Gets the language of the media in this stream
        /// </summary>
        public string Language
        {
            get
            {
                return m_language;
            }
        }

        /// <summary>
        /// Gets or sets the MediaStreamType of the stream represented by this object
        /// </summary>
        public MediaStreamType MediaType
        {
            get
            {
                return m_mediaType;
            }

            set
            {
                m_mediaType = value;
            }
        }

        /// <summary>
        /// Gets the total number of chunks in this stream
        /// </summary>
        public int NumberOfChunksInStream
        {
            get
            {
                return m_numberOfChunksInStream;
            }
        }

        /// <summary>
        /// Gets the StreamId for this stream
        /// </summary>
        public int StreamId
        {
            get
            {
                // Use the first chunk in the stream as our Id
                return m_chunksQueue[0].StreamId;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this StreamInfo object contains valid data.
        /// An invalid StreamInfo object is one which did not parse correctly from the manifest.
        /// </summary>
        public bool Valid { get; set; }

        /// <summary>
        /// Gets our queue of chunk information.
        /// </summary>
        internal MediaChunkQueue Queue
        {
            get
            {
                return m_chunksQueue;
            }
        }

        /// <summary>
        /// Adds a new bitrate and its associated attributes to this class
        /// </summary>
        /// <param name="bitrate">the bitrate to add</param>
        /// <param name="attributes">the media attributes for this bitrate</param>
        public void AddBitrate(ulong bitrate, IDictionary<MediaStreamAttributeKeys, string> attributes)
        {
            m_bitrates.Add(bitrate);
            m_attributes.Add(new BitrateAttributeData(bitrate, attributes));
        }

        /// <summary>
        /// Adds a media chunk to this stream object.
        /// </summary>
        /// <param name="chunkId">The id of the chunk we are adding</param>
        /// <param name="chunkDuration">The duration of the chunk we are adding</param>
        public void AddMediaChunk(int chunkId, ulong chunkDuration)
        {
            // Our media chunk queue gets created and size in our constructor,
            // so we can check here to see if we have already added a chunk with this
            // id
            MediaChunk chunk = m_chunksQueue[chunkId];
            if (chunk.Duration > 0)
            {
                throw new AdaptiveStreamingException(String.Format(System.Globalization.CultureInfo.InvariantCulture, "Bad manifest format: repeated chunk ID {0}", chunkId));
            }

            chunk.Duration = chunkDuration;
        }

        /// <summary>
        /// Gets the MediaStreamAttributes associated with this stream and bitrate 
        /// as used by MediaElement
        /// </summary>
        /// <param name="bitrate">the bitrate we are looking for</param>
        /// <returns>the attributes for this bitrate</returns>
        public IDictionary<MediaStreamAttributeKeys, string> GetAttributesForBitrate(ulong bitrate)
        {
            foreach (BitrateAttributeData data in m_attributes)
            {
                if (data.Bitrate == bitrate)
                {
                    return data.Attributes;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets an array of our available bitrates
        /// </summary>
        /// <returns>the array of available bitrates</returns>
        public ulong[] GetBitrateArray()
        {
            return m_bitrates.ToArray();
        }

        /// <summary>
        /// Get the information for a particular media chunk
        /// </summary>
        /// <param name="chunkId">the id of the chunk to retrieve</param>
        /// <returns>a ChunkInfo object describing the chunk</returns>
        public ChunkInfo GetChunkInfoForId(int chunkId)
        {
            return new ChunkInfo(m_chunksQueue[chunkId].ChunkId, m_chunksQueue[chunkId].Duration, (ulong)m_chunksQueue[chunkId].StartTime);
        }

        /// <summary>
        /// Go through our chunks and make sure that our start times correspond to
        /// our durations. This is called by our manifest code before we add this stream to our manifest.
        /// </summary>
        internal void CalculateStartTimes()
        {
            long chunkStartTime = 0;
            for (int id = 0; id < NumberOfChunksInStream; id++)
            {
                // Last chunk MAY HAVE zero length
                if (m_chunksQueue[id].Duration == 0 && id + 1 != NumberOfChunksInStream)
                {
                    throw new AdaptiveStreamingException(String.Format(System.Globalization.CultureInfo.InvariantCulture, "Chunk #{0} duration is not specified.", id));
                }

                m_chunksQueue[id].StartTime = chunkStartTime;
                chunkStartTime += (long)m_chunksQueue[id].Duration;
            }

            // Now sort the bitrates while we are it
            m_bitrates.Sort();
        }

        /// <summary>
        /// Private struct which links bitrate to attributes
        /// </summary>
        private class BitrateAttributeData
        {
            /// <summary>
            /// Initializes a new instance of the BitrateAttributeData class
            /// </summary>
            /// <param name="bitrate">the bitrate for this class</param>
            /// <param name="attrs">the attributes for this class</param>
            public BitrateAttributeData(ulong bitrate, IDictionary<MediaStreamAttributeKeys, string> attrs)
            {
                Bitrate = bitrate;
                Attributes = attrs;
            }

            /// <summary>
            /// Gets or sets our attributes
            /// </summary>
            public IDictionary<MediaStreamAttributeKeys, string> Attributes { get; set; }

            /// <summary>
            /// Gets or sets our bitrate
            /// </summary>
            public ulong Bitrate { get; set; }
        }
    }
}
