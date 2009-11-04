//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows.Media;
    using Microsoft.Expression.Encoder.AdaptiveStreaming;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Logging;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Parsing;

    /// <summary>
    ///  This class contains all of the state related to a chunk of media.
    ///  These are either audio or video chunks. This class contains the
    ///  downloader that is responsible for downloading this chunk.
    /// </summary>
    internal class MediaChunk
    {
        /// <summary>
        /// The download completed time
        /// </summary>
        private DateTime m_downloadCompleteTime = DateTime.MinValue;

        /// <summary>
        /// The start time of the download
        /// </summary>
        private DateTime m_downloadStartTime = DateTime.MinValue;

        /// <summary>
        /// This variable keeps track of a seek position, in case we get a Seek call
        /// before this chunk has been parsed. It is set in the Seek() call and used
        /// in the GetNextFrame() call.
        /// </summary>
        private long m_lazySeek = -1;

        /// <summary>
        /// The type of media in this chunk
        /// </summary>
        private MediaStreamType m_mediaStreamType = MediaStreamType.Script;

        /// <summary>
        /// The parser that knows how to parse this chunk
        /// </summary>
        private IChunkParser m_parser;

        /// <summary>
        /// The current state of this chunk
        /// </summary>
        private ChunkState m_state = ChunkState.Pending;

        /// <summary>
        /// Initializes a new instance of the MediaChunk class
        /// </summary>
        /// <param name="chunkId">the id of this chunk</param>
        /// <param name="mediaType">the media type of this chunk</param>
        /// <param name="streamId">the id of the stream this chunk is contained in</param>
        public MediaChunk(int chunkId, MediaStreamType mediaType, int streamId)
        {
            ChunkId = chunkId;
            MediaType = mediaType;
            StreamId = streamId;
            DownloadedPiece = null;
        }

        /// <summary>
        /// The internal state of this chunk
        /// </summary>
        public enum ChunkState
        {
            /// <summary>
            /// This chunk is pending downloaded
            /// </summary>
            Pending,

            /// <summary>
            /// This chunk has been fully downloaded but not parsed yet
            /// </summary>
            Loaded,

            /// <summary>
            /// This chunk has been downloaded and parsed
            /// </summary>
            Parsed,

            /// <summary>
            /// This chunk contains an error
            /// </summary>
            Error
        }

        /// <summary>
        /// Gets or sets the Bitrate, in kbps, of this chunk
        /// </summary>
        public ulong Bitrate { get; set; }

        /// <summary>
        /// Gets or sets the Id of this chunk
        /// </summary>
        public int ChunkId { get; set; }

        /// <summary>
        /// Gets or sets the current offset into the stream of what's not read yet
        /// </summary>
        public ulong CurrentOffset { get; set; }

        /// <summary>
        /// Gets or sets the time that this chunk finished downloading
        /// </summary>
        public DateTime DownloadCompleteTime
        {
            get
            {
                return m_downloadCompleteTime;
            }

            set
            {
                m_downloadCompleteTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the actual stream of data downloaded
        /// </summary>
        public Stream DownloadedPiece { get; set; }

        /// <summary>
        /// Gets or sets the current object that is downloading this chunk
        /// </summary>
        public Network.Downloader Downloader { get; set; }

        /// <summary>
        /// Gets or sets the percentage of the file that has been downloaded so far
        /// </summary>
        public int DownloadPercent { get; set; }

        /// <summary>
        /// Gets or sets the time this chunk started downloading
        /// </summary>
        public DateTime DownloadStartTime
        {
            get
            {
                return m_downloadStartTime;
            }

            set
            {
                m_downloadStartTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the duration of this chunk, in 100ns units
        /// </summary>
        public ulong Duration { get; set; }

        /// <summary>
        /// Gets or sets the duration left to send to the media element, in 100ns units
        /// </summary>
        public ulong DurationLeft { get; set; }

        /// <summary>
        /// Gets or sets a message if this chunk hits an error
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the length of data in this chunk
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Gets or sets the type of data contained in this chunk
        /// </summary>
        public MediaStreamType MediaType
        {
            get
            {
                return m_mediaStreamType;
            }

            set
            {
                m_mediaStreamType = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of times we requested a sample from this chunk
        /// but did not have it downloaded yet.
        /// </summary>
        public int SampleRequestsMissed { get; set; }

        /// <summary>
        /// Gets a string Id for this chunk, useful for tracing
        /// </summary>
        public string Sid
        {
            get
            {
                return (MediaType == MediaStreamType.Video ? "V" : "A") + ChunkId.ToString("D4", CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets or sets the start time of this chunk, in 100ns units
        /// </summary>
        public long StartTime { get; set; }

        /// <summary>
        /// Gets or sets the current state of this chunk
        /// </summary>
        public ChunkState State
        {
            get
            {
                return m_state;
            }

            set
            {
                m_state = value;
            }
        }

        /// <summary>
        /// Gets or sets the Id of the stream that this chunk belongs to
        /// </summary>
        public int StreamId { get; set; }

        /// <summary>
        /// Gets or sets the url that this chunk is downloaded from
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets the frame rate of the chunk
        /// </summary>
        public double FrameRate
        {
            get
            {
                if (null == m_parser || State != ChunkState.Parsed)
                {
                    throw new ChunkNotParsedException();
                }

                return m_parser.FrameRate;
            }
        }

        /// <summary>
        /// Gets the next frame of data. Will throw a ChunkNotParsedException if we have not
        /// parsed this chunk yet.
        /// </summary>
        /// <returns>null if no more frames</returns>
        public GetFrameData GetNextFrame()
        {
            // Make sure this chunk has been parsed
            if (null == m_parser || State != ChunkState.Parsed)
            {
                // Actually, it's not end of data, it's somebody forgot to call ParseHeader() first.
                throw new ChunkNotParsedException();
            }

            // Do we have a pending seek that needs to be performed?
            if (m_lazySeek >= 0)
            {
                // Do the seek now
                Seek(m_lazySeek);

                // Reset our seek
                m_lazySeek = -1;
            }

            // Get a frame from the parser
            GetFrameData data = m_parser.GetFrame();
            bool result = data != null;

            if (result)
            {
                Tracer.Assert(CurrentOffset <= (ulong)Length, String.Format(CultureInfo.InvariantCulture, "{0} CFF Parser return current offset beyond the chunk stream: {1} in a stream of {2} bytes", Sid, CurrentOffset, Length));
                CurrentOffset = data.StartOffset;

                // rtTimestamp == 0 in the end of the chunk
                DurationLeft = Duration == 0 || !result ? 0 : Duration - (ulong)(data.Timestamp - StartTime);
            }
            else
            {
                CurrentOffset = 0;
                DurationLeft = 0;
            }

            return data;
        }

        /// <summary>
        /// Parse the header data of this chunk
        /// </summary>
        /// <param name="parserFactory">the parser factory to use for this chunk</param>
        /// <returns>true if the chunk header was header</returns>
        public bool ParseHeader(IChunkParserFactory parserFactory)
        {
            bool isParsed = false;
            lock (this)
            {
                // We can only attempt to parse this chunk when it has been
                // completely loaded
                if (ChunkState.Loaded == State)
                {
                    bool result = false;
                    try
                    {
                        Tracer.Assert(DownloadedPiece != null);

                        // Create a new parser for this chunk
                        m_parser = parserFactory.CreateParserForStream(DownloadedPiece);
                        m_parser.SetPresentationTime(StartTime);
                        result = m_parser.ParseHeader(DownloadedPiece);
                        if (!result)
                        {
                            ErrorMessage = String.Format(CultureInfo.InvariantCulture, "{0} Failed to parse, not enough data (state {1}, {2}, {3} kbps)", Sid, State, DateTime.Now.ToUniversalTime().ToString(), Bitrate);
                            Tracer.Trace(TraceChannel.Error, ErrorMessage);
                        }
                    }
                    catch (ChunkParserException ex)
                    {
                        ErrorMessage = String.Format(CultureInfo.InvariantCulture, "{0} Failed to parse (state {1}, exception {2}, {3}, {4} kbps)", Sid, State, ex.Message, DateTime.Now.ToUniversalTime().ToString(), Bitrate);
                        Tracer.Trace(TraceChannel.Error, ErrorMessage);
                        Tracer.Assert(!result);
                    }

                    if (!result)
                    {
                        State = ChunkState.Error;
                        return false;
                    }

                    DurationLeft = Duration;
                    State = ChunkState.Parsed;
                }

                isParsed = ChunkState.Parsed == State || ChunkState.Pending == State;

                if (ChunkState.Parsed != State)
                {
                    Tracer.Trace(TraceChannel.Error, "{0} state after parsing header: {1}", Sid, State);
                }
            }

            return isParsed;
        }

        /// <summary>
        /// Discard chunk content and prepare the chunk object for possible reuse (in a case of Seek())
        /// </summary>
        public void Reset()
        {
            // This lock is against simultaneous change by Downloader
            lock (this)
            {
                Downloader = null;

                // Close our stream if it exists
                if (null != DownloadedPiece)
                {
                    DownloadedPiece.Close();
                    DownloadedPiece = null;
                }

                m_parser = null;
                DownloadPercent = 0;
                DurationLeft = 0;
                CurrentOffset = 0;
                Length = 0;
                SampleRequestsMissed = 0;
                State = ChunkState.Pending;
            }
        }

        /// <summary>
        /// Seek this chunk to the given position
        /// </summary>
        /// <param name="position">the position to seek to, in 100ns units</param>
        /// <returns>the time we actually seeked to</returns>
        public long Seek(long position)
        {
            long result = StartTime;

            // Check to see if this time in is this chunk
            if (position != 0 && (position < StartTime || StartTime + (long)Duration < position))
            {
                // This position is not in this chunk
                return StartTime;
            }

            // If we haven't parsed this chunk yet, then flag the seek for later
            if (null == m_parser || State != ChunkState.Parsed)
            {
                // Lazy Seek
                result = m_lazySeek = position; // We may err within one frame
            }
            else
            {
                // If seeking to the start, rewind the parser
                if (position == 0)
                {
                    m_parser.Rewind();
                }
                else
                {
                    result = m_parser.Seek(position);
                }

                Tracer.Trace(TraceChannel.Seek, " ={0}{1} ", MediaType == MediaStreamType.Video ? "V" : "A", result / 10000);

                if (MediaType == MediaStreamType.Audio)
                {
                    Tracer.Trace(TraceChannel.SeekPos, " *AA={0}", (position - result) / 10000);
                }
            }

            return result;
        }
    }
}
