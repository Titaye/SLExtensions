//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace Microsoft.Expression.Encoder.AdaptiveStreaming
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;

    /// <summary>
    /// An exception class that you can use to signal Parsing exceptions. These are
    /// non-fatal and the rest of the app will try to continue
    /// </summary>
    public class ChunkParserException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ChunkParserException class
        /// </summary>
        public ChunkParserException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ChunkParserException class
        /// </summary>
        /// <param name="message">exception message</param>
        public ChunkParserException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ChunkParserException class
        /// </summary>
        /// <param name="message">exception message</param>
        /// <param name="innerException">exception to wrap</param>
        public ChunkParserException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// This class is used as the return value to GetFrame()
    /// </summary>
    public class GetFrameData
    {
        /// <summary>
        /// Timestamp of the frame
        /// </summary>
        private long m_timestamp;

        /// <summary>
        /// Starting offset of the frame
        /// </summary>
        private uint m_startOffset;

        /// <summary>
        /// Size of the frame
        /// </summary>
        private uint m_frameSize;

        /// <summary>
        /// Array of drm data
        /// </summary>
        private byte[] m_drmData;

        /// <summary>
        /// Initializes a new instance of the GetFrameData class
        /// </summary>
        /// <param name="timestamp">timestamp of the frame</param>
        /// <param name="startOffset">offset into the data array</param>
        /// <param name="frameSize">size of the frame</param>
        /// <param name="data">drm data array</param>
        public GetFrameData(long timestamp, uint startOffset, uint frameSize, byte[] data)
        {
            m_timestamp = timestamp;
            m_startOffset = startOffset;
            m_frameSize = frameSize;
            m_drmData = data;
        }

        /// <summary>
        /// Gets the timestamp of the frame we are returning
        /// </summary>
        public long Timestamp
        {
            get
            {
                return m_timestamp;
            }
        }

        /// <summary>
        /// Gets the starting offset into the data array of the frame we are returning
        /// </summary>
        public uint StartOffset
        {
            get
            {
                return m_startOffset;
            }
        }

        /// <summary>
        /// Gets the size of the frame data
        /// </summary>
        public uint FrameSize
        {
            get
            {
                return m_frameSize;
            }
        }

        /// <summary>
        /// Gets the array of bytes for this frame
        /// </summary>
        public byte[] DrmData
        {
            get
            {
                return m_drmData;
            }
        }
    }

    /// <summary>
    /// This class defines the interface for chunk parsers. This interface can
    /// be implemented by external libraries to support file formats that are
    /// not supported here.
    /// </summary>
    public abstract class IChunkParser
    {
        /// <summary>
        /// Gets the frame rate of this chunk.
        /// </summary>
        /// <returns>the frame rate, in fps</returns>
        public abstract double FrameRate { get; }

        /// <summary>
        /// This function is called when we have fully received a chunk of data. The parser can set it self
        /// up for seubsequent calls by reading any header data here
        /// </summary>
        /// <param name="stream">the stream of data</param>
        /// <returns>true if the header was successfully parsed</returns>
        public abstract bool ParseHeader(Stream stream);

        /// <summary>
        /// Gets the next frame of data
        /// </summary>
        /// <returns>a structure describing this frame, or null if one was not available</returns>
        public abstract GetFrameData GetFrame();

        /// <summary>
        /// Rewind the parser beginning of the last frame
        /// </summary>
        /// <returns>the new time</returns>
        public abstract long Rewind();

        /// <summary>
        /// Seek the stream to the given position
        /// </summary>
        /// <param name="seekTime">the time to seek to</param>
        /// <returns>the time we actually seeked to</returns>
        public abstract long Seek(long seekTime);

        /// <summary>
        /// Sets the presentation time on the parser. Can be used as the base time
        /// for all subsequent samples. This is optional.
        /// </summary>
        /// <param name="time">the presentation time</param>
        public virtual void SetPresentationTime(long time) 
        { 
        }
    }
}
