//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Media;
    using Microsoft.Expression.Encoder.AdaptiveStreaming;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Logging;

    /// <summary>
    /// This class holds the queue of chunks that we send to the media element. We have 1 MediaChunkQueue
    /// for each stream in our manifest.
    /// </summary>
    internal class MediaChunkQueue
    {
        /// <summary>
        /// The maximum number of chunks we can buffer ahead of time.
        /// Players won't be able to buffer more than this number of chunks
        /// </summary>
        private const long MaxChunksBuffered = 1024;

        /// <summary>
        /// The total size of the data in the queue, in bytes
        /// </summary>
        private ulong m_bufferSize;

        /// <summary>
        /// The total duration of the data in the queue, in 100ns units
        /// </summary>
        private ulong m_bufferTime;

        /// <summary>
        /// The array of data objects in our queue
        /// </summary>
        private MediaChunk[] m_dataQueue;

        /// <summary>
        /// The index of the first item in the queue
        /// </summary>
        private int m_indexFirst;

        /// <summary>
        /// The index of the last item in the queue
        /// </summary>
        private int m_indexLast;

        /// <summary>
        /// Initializes a new instance of the MediaChunkQueue class
        /// </summary>
        /// <param name="count">the number of chunks in the queue</param>
        /// <param name="mediaType">the media type of the queue</param>
        /// <param name="streamId">the id of the stream this queue belongs to</param>
        public MediaChunkQueue(int count, MediaStreamType mediaType, int streamId)
        {
            m_dataQueue = new MediaChunk[count];
            for (int i = 0; i < count; i++)
            {
                m_dataQueue[i] = new MediaChunk(i, mediaType, streamId);
            }
        }

        /// <summary>
        /// Gets the total size, in bytes, of all data in the queue
        /// </summary>
        public ulong BufferSize
        {
            get
            {
                return m_bufferSize;
            }
        }

        /// <summary>
        /// Gets the total time, in 100ns units, of all data in the queue
        /// </summary>
        public ulong BufferTime
        {
            get
            {
                return m_bufferTime;
            }
        }

        /// <summary>
        /// Gets the current media chunk in the queue
        /// </summary>
        public MediaChunk Current
        {
            get
            {
                return m_indexFirst < m_dataQueue.Length ? m_dataQueue[m_indexFirst] : null;
            }
        }

        /// <summary>
        /// Gets the last media chunk in the queue
        /// </summary>
        public MediaChunk Last
        {
            get
            {
                Tracer.Assert(m_indexLast <= m_dataQueue.Length);
                return m_indexFirst >= m_indexLast ? null : // Right after Open or Seek, no data
                    m_dataQueue[m_indexLast - 1].DownloadedPiece == null ? null : // No data yet
                    m_dataQueue[m_indexLast - 1];
            }
        }

        /// <summary>
        /// Gets the media chunk with the given id
        /// </summary>
        /// <param name="chunkId">id of the chunk to get</param>
        /// <returns>media chunk with id == chunkId</returns>
        public MediaChunk this[int chunkId]
        {
            get
            {
                if (chunkId < 0 || chunkId >= m_dataQueue.Length)
                {
                    throw new AdaptiveStreamingException(String.Format(CultureInfo.InvariantCulture, "Trying to access chunk {0} while only 0 to {1} are available", chunkId, m_dataQueue.Length - 1));
                }

                return m_dataQueue[chunkId];
            }
        }

        /// <summary>
        /// Add a chunk to the queue
        /// </summary>
        /// <param name="chunk">the chunk to add</param>
        public void Add(MediaChunk chunk)
        {
            lock (m_dataQueue)
            {
                if (chunk.ChunkId >= m_dataQueue.Length)
                {
                    throw new AdaptiveStreamingException(String.Format(CultureInfo.InvariantCulture, "Chunk ID is too big, ID={0}, media length is {1} chunks.", chunk.ChunkId, m_dataQueue.Length));
                }
                else if (chunk.ChunkId < m_indexFirst || chunk.ChunkId >= m_indexFirst + MaxChunksBuffered)
                {
                    // Silently ignore this chunk because we cannot buffer any more
                    Tracer.Trace(TraceChannel.Seek, "Arrived chunk {2}{0}, while current is {2}{1}", chunk.ChunkId, m_indexFirst, chunk.MediaType == MediaStreamType.Video ? "V" : "A");
                }
                else if (chunk.StreamId != m_dataQueue[chunk.ChunkId].StreamId)
                {
                    // We switched streams and were not able to cancel this download, 
                    // don't replace audio from one stream with audio from another
                    chunk.Reset();
                }
                else
                {
                    // Tack it to the end
                    if (chunk.ChunkId >= m_indexLast)
                    {
                        m_indexLast = chunk.ChunkId + 1;
                    }

                    // Get the current chunk
                    MediaChunk cur = m_dataQueue[chunk.ChunkId];

                    // In the current implementation this is never hit because chunks are updated in place, 
                    // i.e. chunk == cur always
                    if (chunk != cur)
                    {
                        if (chunk.Bitrate > cur.Bitrate || cur.DownloadedPiece == null)
                        {
                            m_dataQueue[chunk.ChunkId] = chunk;
                            cur.Reset();
                        }
                        else
                        {
                            chunk.Reset();
                        }
                    }
                }

                UpdateBufferSizes();
            }
        }

        /// <summary>
        /// Clears the queue of all data up until the given time. This happens in a Seek usually.
        /// </summary>
        /// <param name="seekTime">the time to discard until</param>
        /// <returns>the new time we are positioned to</returns>
        public ulong DiscardUntil(ulong seekTime)
        {
            int newCur = 0;
            ulong timeBack = 0;

            Tracer.Trace(TraceChannel.Seek, "U={0} ", seekTime / 10000000);

            lock (m_dataQueue)
            {
                for (; newCur < m_dataQueue.Length; newCur++)
                {
                    timeBack += m_dataQueue[newCur].Duration;

                    // - 1000000 (100 milliseconds) is a heuristic constant to avoid poitioning to the very end of the chunk, 
                    // when the first frame from the next one is a better match + avoiding double download.
                    // Don't put too high value, because it will result in a visible audio-video gap
                    if (timeBack - 1000000 > seekTime)
                    {
                        break;
                    }
                }

                // Did we pass the end?
                if (newCur >= m_dataQueue.Length)
                {
                    // Seek beyond buffer, no error, just get to the last seekable position
                    newCur = m_dataQueue.Length - 1;
                }

                timeBack -= m_dataQueue[newCur].Duration;

                Tracer.Trace(TraceChannel.Seek, "{0}{1}->{2} ", m_dataQueue[0].MediaType == MediaStreamType.Video ? "V" : "A", m_indexFirst, newCur);

                // Discard all even if we can use them: HM workaround
                for (int i = m_indexFirst; i < m_indexLast; i++) 
                {
                    m_dataQueue[i].Reset();
                    Tracer.Trace(TraceChannel.Seek, "{0}{1} ", m_dataQueue[0].MediaType == MediaStreamType.Video ? "V" : "A", i);
                }

                // Re-position the first and last indices
                m_indexFirst = newCur;
                for (m_indexLast = m_indexFirst; m_indexLast < m_dataQueue.Length && m_dataQueue[m_indexLast].State == MediaChunk.ChunkState.Parsed; m_indexLast++)
                {
                }
            }

            // If we are audio, then we can seek into the chunk
            if (m_dataQueue[m_indexFirst].MediaType == MediaStreamType.Audio)
            {
                timeBack = (ulong)m_dataQueue[m_indexFirst].Seek((long)seekTime);
                Tracer.Trace(TraceChannel.Seek, " A" + (timeBack / 10000));
            }
            else
            {
                // Rewind in a case it's already loaded
                m_dataQueue[m_indexFirst].Seek(0);
                Tracer.Trace(TraceChannel.Seek, " V" + (timeBack / 10000));
            }

            // Recalculate all our buffer info
            UpdateBufferSizes();
            return timeBack;
        }

        /// <summary>
        /// Prunes media buffer leaving only toKeep amount round up to chunk boundary, 
        /// used for faster recovery when CPU cannot handle current bitrate
        /// </summary>
        /// <param name="toKeep">amount to leave</param>
        /// <returns>index of the chunk to start at</returns>
        public int Prune(ulong toKeep) 
        {
            Tracer.Trace(TraceChannel.Seek, "Pruning, {0} sec to keep, {1} sec present. Queue[{2}:{3}]", toKeep / 10000000, m_bufferTime / 10000000, m_indexFirst, m_indexLast);

            lock (m_dataQueue)
            {
                int cur;
                ulong timeBack = 0;
                for (cur = m_indexFirst; (cur < m_dataQueue.Length) && (timeBack < toKeep); cur++)
                {
                    timeBack += m_dataQueue[cur].DurationLeft;
                }

                Tracer.Trace(TraceChannel.Seek, "{0} sec @ {1}", timeBack / 10000000, cur);

                if (timeBack >= toKeep)
                {
                    for (int k = cur; (k < m_dataQueue.Length) && (k < m_indexLast); k++)
                    {
                        m_dataQueue[k].Reset();
                        Tracer.Trace(TraceChannel.Seek, "Prune discarding {0}{1} ", m_dataQueue[0].MediaType == MediaStreamType.Video ? "V" : "A", k);
                    }

                    m_indexLast = cur;
                }
            }

            UpdateBufferSizes();
            Tracer.Trace(TraceChannel.Seek, "Pruned, {0} sec kept. Queue[{1}:{2}]", m_bufferTime / 10000000, m_indexFirst, m_indexLast);
            return m_indexLast;
        }

        /// <summary>
        /// Move the start of the queue to the next chunk
        /// </summary>
        /// <returns>true if we still have items left</returns>
        public bool MoveNext()
        {
            bool result = false;
            lock (m_dataQueue)
            {
                if (m_indexFirst < m_dataQueue.Length)
                {
                    m_dataQueue[m_indexFirst].Reset();
                    m_indexFirst++;
                    result = m_indexFirst < m_dataQueue.Length;
                }

                UpdateBufferSizes();
            }

            return result;
        }

        /// <summary>
        /// Resets all the chunks in the queue
        /// </summary>
        public void Shutdown()
        {
            lock (m_dataQueue)
            {
                foreach (MediaChunk chunk in m_dataQueue)
                {
                    chunk.Reset();
                }
            }
        }

        /// <summary>
        /// Recalculates the total time and size of chunks in this queue
        /// </summary>
        public void UpdateBufferSizes()
        {
            lock (m_dataQueue)
            {
                ulong bytes = 0;
                ulong hns = 0;
                bool doLoop = true;
                int i = m_indexFirst;

                while (doLoop)
                {
                    doLoop = false;

                    Tracer.Assert(m_indexLast <= m_dataQueue.Length);
                    for (; (i < m_dataQueue.Length)
                        && (m_dataQueue[i].Bitrate > 0)
                        && (m_dataQueue[i].DownloadedPiece != null)
                        && (m_dataQueue[i].State != MediaChunk.ChunkState.Pending)
                        && (m_dataQueue[i].State != MediaChunk.ChunkState.Error);
                        i++)
                    {
                        hns += m_dataQueue[i].DurationLeft;
                        Tracer.Assert(m_dataQueue[i].Length >= 0, String.Format(CultureInfo.InvariantCulture, "Negative chunk length {0}", m_dataQueue[i].Length));
                        Tracer.Assert(
                            ((ulong)m_dataQueue[i].Length) >= m_dataQueue[i].CurrentOffset,
                            String.Format(CultureInfo.InvariantCulture, "Wrong length left, length {0}, currrent offset {1}", m_dataQueue[i].Length, m_dataQueue[i].CurrentOffset));
                        bytes += (ulong)m_dataQueue[i].Length - m_dataQueue[i].CurrentOffset;
                    }

                    // Skip missing chunks
                    for (; (i < m_dataQueue.Length) && (m_dataQueue[i].State == MediaChunk.ChunkState.Error); i++)
                    {
                        doLoop = true;
                    }
                }

                m_bufferTime = hns;
                m_bufferSize = bytes;
                Tracer.Assert(m_bufferSize < 50000000, String.Format(CultureInfo.InvariantCulture, "Buffer size overflow: {0}/{1}", m_bufferSize, bytes));
            }
        }
    }
}
