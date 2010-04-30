//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Parsing
{
    using System;
    using System.IO;
    using Microsoft.Expression.Encoder.AdaptiveStreaming;

    /// <summary>
    /// This class knows how to parse our adaptive streaming media bits
    /// </summary>
    internal class FragmentedMp4ParserImpl : IChunkParser
    {
        /// <summary>
        /// The base box size
        /// </summary>
        private const int BaseBoxSize = 8;

        /// <summary>
        /// The 'drIV' box header
        /// </summary>
        private static byte[] drmDataBoxId = { 0x64, 0x72, 0x49, 0x56 };

        /// <summary>
        /// The 'mdat' box header
        /// </summary>
        private static byte[] mdatBoxId = { 0x6d, 0x64, 0x61, 0x74 };

        /// <summary>
        /// The 'mfhd' box header
        /// </summary>
        private static byte[] mfhdBoxId = { 0x6d, 0x66, 0x68, 0x64 };

        /// <summary>
        /// The 'moof' box header
        /// </summary>
        private static byte[] moofBoxId = { 0x6d, 0x6f, 0x6f, 0x66 };

        /// <summary>
        /// The 'tfhf' box header
        /// </summary>
        private static byte[] tfhdBoxid = { 0x74, 0x66, 0x68, 0x64 };

        /// <summary>
        /// The 'traf' box header
        /// </summary>
        private static byte[] trafBoxId = { 0x74, 0x72, 0x61, 0x66 };

        /// <summary>
        /// The 'trun' box header
        /// </summary>
        private static byte[] trunBoxId = { 0x74, 0x72, 0x75, 0x6e };

        /// <summary>
        /// The current offset into the stream
        /// </summary>
        private long m_currentOffsetInBytes;

        /// <summary>
        /// The current offset, saved for rewinding
        /// </summary>
        private long m_currentOffsetStart;

        /// <summary>
        /// The number of bytes left in the curren subbox
        /// </summary>
        private int m_currentSubboxLeft;

        /// <summary>
        /// The position of the current subbox
        /// </summary>
        private int m_currentSubboxPos;

        /// <summary>
        /// The time of the current frame we are positioned at
        /// </summary>
        private long m_currentTime;

        /// <summary>
        /// The current presentation time, saved for rewinding
        /// </summary>
        private long m_currentTimeStart;

        /// <summary>
        /// The amount of data left in the stream
        /// </summary>
        private long m_dataLeftInBytes;

        /// <summary>
        /// The amount of data left, saved for rewinding
        /// </summary>
        private long m_dataLeftStart;

        /// <summary>
        /// The offsets into the header buffer of each drm blob
        /// </summary>
        private int[] m_drmIVOffsets;

        /// <summary>
        /// The size of each drm blob
        /// </summary>
        private int[] m_drmIVSizes;

        /// <summary>
        /// The fixed duration, if used
        /// </summary>
        private uint m_fixedDuration;

        /// <summary>
        /// The fixed frame size, if we have one
        /// </summary>
        private uint m_fixedFrameSizeInBytes;

        /// <summary>
        /// The duration of each frame
        /// </summary>
        private uint[] m_frameDurations;

        /// <summary>
        /// The index of the current frame we are positioned at
        /// </summary>
        private uint m_frameIndex;

        /// <summary>
        /// The current frame index, saved for rewinding
        /// </summary>
        private uint m_frameIndexStart;

        /// <summary>
        /// An array of frame sizes
        /// </summary>
        private uint[] m_frameSizesArray;

        /// <summary>
        /// Buffer which contains our header data
        /// </summary>
        private byte[] m_headerBuffer;

        /// <summary>
        /// The number of drm boxes
        /// </summary>
        private uint m_numDrmIVs;

        /// <summary>
        /// The number of frame sizes in the m_rgFrameSizes array
        /// </summary>
        private uint m_numFrameSizes;

        /// <summary>
        /// The number of durations in the m_frameDurations array
        /// </summary>
        private uint m_numTimes;

        /// <summary>
        /// The number of bytes left in the subbox
        /// </summary>
        private int m_subboxLeft;

        /// <summary>
        /// The position of the subbox
        /// </summary>
        private int m_subboxPos;

        /// <summary>
        /// The saved frame rate for this chunk.
        /// </summary>
        private double m_cachedFrameRate = -1.0;

        /// <summary>
        /// Gets the frame rate of this chunk.
        /// </summary>
        /// <returns>the frame rate, in fps</returns>
        public override double FrameRate
        {
            get
            {
                // Gets the duration of the first frame as a double
                if (m_frameDurations != null)
                {
                    if (m_cachedFrameRate == -1.0)
                    {
                        double duration = 0.0;
                        for (uint i = 0; i < m_numTimes; ++i)
                        {
                            duration += m_frameDurations[i];
                        }

                        m_cachedFrameRate = 10000000.0 * (double)m_numTimes / duration;
                    }

                    return m_cachedFrameRate;
                }

                // Default to fixed duration
                return 10000000.0 / (double)m_fixedDuration;
            }
        }

        /// <summary>
        /// Gets a frame of data. Returns null when there is no more data.
        /// </summary>
        /// <returns>the data for the frame, or null if a frame does not exist</returns>
        public override GetFrameData GetFrame()
        {
            long timestamp;
            uint startOffsetInBytes;
            uint frameSizeInBytes;
            byte[] drmDataArray = null;
            int drmDataArraySizeInBytes = 0;

            startOffsetInBytes = (uint)m_currentOffsetInBytes;
            frameSizeInBytes = m_fixedFrameSizeInBytes;
            if (0 == frameSizeInBytes)
            {
                if (m_frameIndex >= m_numFrameSizes)
                {
                    return null;
                }

                frameSizeInBytes = m_frameSizesArray[m_frameIndex];
            }

            if (m_dataLeftInBytes < frameSizeInBytes)
            {
                return null;
            }

            m_dataLeftInBytes -= frameSizeInBytes;
            m_currentOffsetInBytes += frameSizeInBytes;

            if (m_drmIVOffsets != null && m_numDrmIVs > m_frameIndex)
            {
                drmDataArraySizeInBytes = m_drmIVSizes[m_frameIndex];
                drmDataArray = new byte[drmDataArraySizeInBytes];
                Array.Copy(m_headerBuffer, m_drmIVOffsets[m_frameIndex], drmDataArray, 0, drmDataArraySizeInBytes);
            }

            timestamp = m_currentTime;
            uint duration = m_fixedDuration;
            if (m_frameDurations != null && m_numTimes > m_frameIndex)
            {
                duration = m_frameDurations[m_frameIndex];
            }

            // Cast duration to signed to facilitate sign expansion in case it is negative
            m_currentTime += (int)duration;
            m_frameIndex++;
            return new GetFrameData(timestamp, startOffsetInBytes, frameSizeInBytes, drmDataArray);
        }

        /// <summary>
        /// Implements IChunkParser.ParserHeader()
        /// </summary>
        /// <param name="stream">the stream to parse</param>
        /// <returns>returns false if we did not have enough data to parse the header</returns>
        public override bool ParseHeader(Stream stream)
        {
            bool isHeaderFound = false, isDataFound = false;

            // Reset the stream
            stream.Position = 0;

            // Do we have enough data to parse this stream?
            if ((stream.Length - stream.Position) < BaseBoxSize)
            {
                return false;
            }

            byte[] boxID;
            byte[] rawSize;
            uint sizeInBytes;

            // Pick off and ignore the 'moof' header
            rawSize = ReadBytes(4, stream);
            boxID = ReadBytes(4, stream);
            long moofSize = IntFromArray(4, rawSize);

            if (IntFromArray(4, moofBoxId) != IntFromArray(4, boxID))
            {
                throw new ChunkParserException("Unknown stream type");
            }

            // Check for large size
            if (moofSize == 1)
            {
                if ((moofSize = ReadLargeSize(stream)) < 0)
                {
                    return false;
                }
            }

            // Pick off and ignore 'mfhd'
            rawSize = ReadBytes(4, stream);
            boxID = ReadBytes(4, stream);
            long mfhdSize = IntFromArray(4, rawSize);

            // Make sure we are at the mfhd box
            if (IntFromArray(4, mfhdBoxId) != IntFromArray(4, boxID))
            {
                throw new ChunkParserException("Unknown stream type");
            }

            // Check for large size
            if (mfhdSize == 1)
            {
                if ((mfhdSize = ReadLargeSize(stream)) < 0)
                {
                    return false;
                }

                mfhdSize -= 8;
            }

            // Skip the rest of the mfhd field
            stream.Position += (mfhdSize - 8);

            do
            {
                // Great, now move onto the 'traf' box
                rawSize = ReadBytes(4, stream);
                boxID = ReadBytes(4, stream);
                sizeInBytes = (uint)IntFromArray(4, rawSize);
                if (sizeInBytes == 1)
                {
                    if ((stream.Length - stream.Position) < 8)
                    {
                        return false;
                    }

                    byte[] bigSize;
                    bigSize = ReadBytes(8, stream);
                    sizeInBytes = (uint)IntFromArray(8, bigSize);

                    if (sizeInBytes < 8)
                    {
                        throw new ChunkParserException();
                    }

                    sizeInBytes -= 8;
                }

                if (sizeInBytes < BaseBoxSize)
                {
                    throw new ChunkParserException();
                }

                sizeInBytes -= BaseBoxSize;

                if ((stream.Length - stream.Position) < sizeInBytes)
                {
                    // Don't have enough yet - can happen
                    return false;
                }

                if (Equal4cc(trafBoxId, boxID))
                {
                    bool haveTrun = false;
                    byte[] subboxType;

                    m_headerBuffer = new byte[sizeInBytes];
                    stream.Read(m_headerBuffer, 0, (int)sizeInBytes);
                    SubboxInit(0, (int)sizeInBytes);

                    // Indicate it's been read so we don't skip over it below
                    sizeInBytes = 0;
                    while (true == SubboxNext(out subboxType, out m_currentSubboxPos, out m_currentSubboxLeft))
                    {
                        uint flags;
                        byte version;

                        if (Equal4cc(tfhdBoxid, subboxType))
                        {
                            ParseVersionAndFlags(out version, out flags);

                            // Spec says unknown versions shall be ignored
                            if (0 == version)
                            {
                                // Track_ID
                                ReadIntFromSubbox(4);

                                if ((0x01 & flags) != 0)
                                {
                                    // Base data offset
                                    ReadIntFromSubbox(4);
                                }

                                if ((0x02 & flags) != 0)
                                {
                                    // Sample description index
                                    ReadIntFromSubbox(4);
                                }

                                if ((0x08 & flags) != 0)
                                {
                                    m_fixedDuration = (uint)ReadIntFromSubbox(4);
                                }

                                if ((0x10 & flags) != 0)
                                {
                                    m_fixedFrameSizeInBytes = (uint)ReadIntFromSubbox(4);
                                }

                                if ((0x20 & flags) != 0)
                                {
                                    // Sample flags
                                    ReadIntFromSubbox(4);
                                }
                            }
                        }
                        else if (Equal4cc(trunBoxId, subboxType))
                        {
                            ParseVersionAndFlags(out version, out flags);

                            // Spec says unknown versions shall be ignored
                            // Not dealing with multiple truns !
                            if (0 == version && !haveTrun)
                            {
                                ParseTrun(flags);
                                haveTrun = true;
                            }
                        }
                        else if (Equal4cc(drmDataBoxId, subboxType))
                        {
                            ParseArrayOfBlobs(out m_numDrmIVs, out m_drmIVSizes, out m_drmIVOffsets, ref m_currentSubboxPos, ref m_currentSubboxLeft);
                        }
                    }

                    isHeaderFound = true;
                }
                else if (Equal4cc(mdatBoxId, boxID))
                {
                    m_dataLeftInBytes = sizeInBytes;
                    m_currentOffsetInBytes = stream.Position;
                    isDataFound = true;
                    break;
                }

                if (isHeaderFound && isDataFound)
                {
                    break;
                }

                // Skip the rest of this box
                stream.Position += sizeInBytes;
            }
            while (true);

            // Saving for Seek()/Rewind()
            m_dataLeftStart = m_dataLeftInBytes;
            m_currentOffsetStart = m_currentOffsetInBytes;
            m_currentTimeStart = m_currentTime;
            m_frameIndexStart = m_frameIndex;

            return isHeaderFound && isDataFound && (m_fixedFrameSizeInBytes != 0 || m_frameSizesArray != null);
        }

        /// <summary>
        /// Rewinds the parser back to the last frame
        /// </summary>
        /// <returns>the new time we rewound to</returns>
        public override long Rewind()
        {
            m_dataLeftInBytes = m_dataLeftStart;
            m_currentOffsetInBytes = m_currentOffsetStart;
            m_currentTime = m_currentTimeStart;
            m_frameIndex = m_frameIndexStart;
            return m_currentTime;
        }

        /// <summary>
        /// Seeks to the new position
        /// </summary>
        /// <param name="position">position to seek to</param>
        /// <returns>time we actually seeked to</returns>
        public override long Seek(long position)
        {
            Rewind();

            uint duration = 0;
            uint lastDuration = 0;

            do
            {
                if (m_currentTime >= position)
                {
                    // Already past seek position or precisely at it
                    break;
                }

                // pos-cur >= 0 from here on
                duration = m_fixedDuration;
                if (m_frameDurations != null && m_numTimes > m_frameIndex)
                {
                    duration = m_frameDurations[m_frameIndex];
                }

                if (duration == 0)
                {
                    // For the last frame in the chunk for some media/chunking tool                
                    duration = lastDuration != 0 ? lastDuration : 200;
                }

                lastDuration = duration;
            }
            while ((position - m_currentTime > duration / 2) && GetFrame() != null);

            return m_currentTime;
        }

        /// <summary>
        /// Sets the presentation time on the parser
        /// </summary>
        /// <param name="time">the current presentation time</param>
        public override void SetPresentationTime(long time)
        {
            m_currentTime = time;
        }

        /// <summary>
        /// Converts an array to an integer
        /// </summary>
        /// <param name="numBytes">the size of the array</param>
        /// <param name="rb">the array to convert</param>
        /// <returns>the new integer</returns>
        internal static long IntFromArray(int numBytes, byte[] rb)
        {
            long tmp = 0;
            int c;

            for (c = 0; c < numBytes; c++)
            {
                tmp = (tmp << 8) | rb[c];
            }

            return tmp;
        }

        /// <summary>
        /// Are two 4cc's equal
        /// </summary>
        /// <param name="a">first 4CC to check</param>
        /// <param name="b">second 4CC to check</param>
        /// <returns>true if they are equal</returns>
        private static bool Equal4cc(byte[] a, byte[] b)
        {
            return a[0] == b[0] && a[1] == b[1] && a[2] == b[2] && a[3] == b[3];
        }

        /// <summary>
        /// Reads bytes from our stream
        /// </summary>
        /// <param name="numBytes">number of bytes to read</param>
        /// <param name="stream">stream to read from</param>
        /// <returns>array of bytes read</returns>
        private static byte[] ReadBytes(int numBytes, Stream stream)
        {
            byte[] p = new byte[numBytes];
            stream.Read(p, 0, numBytes);
            return p;
        }

        /// <summary>
        /// Read a large size from the stream
        /// </summary>
        /// <param name="stream">stream to read from</param>
        /// <returns>the large size read</returns>
        private static long ReadLargeSize(Stream stream)
        {
            // Suck out the box size
            ReadBytes(4, stream);

            // Do we have enough data to read?
            if ((stream.Length - stream.Position) < 8)
            {
                return -1;
            }

            byte[] bigSize;
            bigSize = ReadBytes(8, stream);
            long size = IntFromArray(8, bigSize);
            if (size < 8)
            {
                throw new ChunkParserException();
            }

            return size;
        }

        /// <summary>
        /// Parses an array of blobs
        /// </summary>
        /// <param name="numItems">number of items parsed</param>
        /// <param name="itemSizesArray">array of item sizes</param>
        /// <param name="itemOffsetsArray">array of item offsets</param>
        /// <param name="newPosition">new position after parsing</param>
        /// <param name="numBytesLeft">number of bytes left after parsing</param>
        private void ParseArrayOfBlobs(
            out uint numItems,
            out int[] itemSizesArray,
            out int[] itemOffsetsArray,
            ref int newPosition,
            ref int numBytesLeft)
        {
            int u, fixedSizeInBytes;
            fixedSizeInBytes = (int)ReadInt(4, ref newPosition, ref numBytesLeft);
            numItems = (uint)ReadInt(4, ref newPosition, ref numBytesLeft);

            if (numItems > 0x100000)
            {
                throw new ChunkParserException();
            }

            itemSizesArray = new int[numItems];
            itemOffsetsArray = new int[numItems];
            for (u = 0; u < numItems; u++)
            {
                int size = fixedSizeInBytes;

                // If size is 0, then it is variable
                if (0 == size)
                {
                    size = (int)ReadInt(4, ref newPosition, ref numBytesLeft);
                }

                if (size > numBytesLeft)
                {
                    throw new ChunkParserException();
                }

                itemSizesArray[u] = size;
                itemOffsetsArray[u] = newPosition;
                newPosition += size;
                numBytesLeft -= size;
            }
        }

        /// <summary>
        /// Parses the trun box
        /// </summary>
        /// <param name="flags">flags from the box</param>
        private void ParseTrun(uint flags)
        {
            uint numSamples = (uint)ReadIntFromSubbox(4);

            if (numSamples > 0)
            {
                // Spec says:
                // "The number of optional fields is
                //  determined from the number of bits
                //  set in the lower byte of the flags,
                //  and the size of a record from the
                //  bits set in the second byte of the
                //  flags. This procedure shall be followed,
                //  to allow for new fields to be defined."
                // So that's what this code below does.
                //
                // Note: this logic pattern feels like
                // it should be abstracted into a subroutine
                // so that it can be used for more things,
                // but I only found one instance of the above
                // language in the spec.  So keeping the code
                // inlined for now for readability.

                // read global (not per-sample) fields
                for (uint f = 1; f < 0x100; f <<= 1)
                {
                    if ((f & flags) != 0)
                    {
                        ReadIntFromSubbox(4);
                    }
                }

                // allcate space for per-sample fields we want
                if ((0x100 & flags) != 0)
                {
                    m_numTimes = numSamples;
                    m_frameDurations = new uint[numSamples];
                }

                if ((0x200 & flags) != 0)
                {
                    m_numFrameSizes = numSamples;
                    m_frameSizesArray = new uint[numSamples];
                }

                // Read per-sample fields.
                for (uint sample = 0; sample < numSamples; sample++)
                {
                    // I'm sure some day somebody will special-case
                    // this in the name of performance.
                    for (uint f = 0x100; f < 0x10000; f <<= 1)
                    {
                        if ((f & flags) != 0)
                        {
                            int i = (int)ReadIntFromSubbox(4);

                            if (0x100 == f)
                            {
                                m_frameDurations[sample] = (uint)i;
                            }

                            if (0x200 == f)
                            {
                                m_frameSizesArray[sample] = (uint)i;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parse the version and flags from a subbox
        /// </summary>
        /// <param name="version">the version we parsed</param>
        /// <param name="flags">the flags we parsed</param>
        private void ParseVersionAndFlags(out byte version, out uint flags)
        {
            if (m_currentSubboxLeft < 4)
            {
                throw new ChunkParserException();
            }

            version = m_headerBuffer[m_currentSubboxPos];

            byte[] p;
            p = new byte[3];
            Array.Copy(m_headerBuffer, m_currentSubboxPos + 1, p, 0, 3);
            flags = (uint)IntFromArray(3, p);

            m_currentSubboxPos += 4;
            m_currentSubboxLeft -= 4;
        }

        /// <summary>
        /// Reads some bytes from our data stream
        /// </summary>
        /// <param name="numBytes">number of bytes to read</param>
        /// <param name="newPosition">position to read</param>
        /// <param name="numBytesLeft">position left</param>
        /// <returns>the bytes we read</returns>
        private byte[] ReadBytes(int numBytes, ref int newPosition, ref int numBytesLeft)
        {
            byte[] p;

            if (numBytesLeft < numBytes)
            {
                throw new ChunkParserException();
            }

            p = new byte[numBytes];
            Array.Copy(m_headerBuffer, newPosition, p, 0, numBytes);
            newPosition += numBytes;
            numBytesLeft -= numBytes;
            return p;
        }

        /// <summary>
        /// Reads an integer from our data
        /// </summary>
        /// <param name="numBytes">number of bytes to read</param>
        /// <param name="newPosition">position to read</param>
        /// <param name="numBytesLeft">position left</param>
        /// <returns>the integer we read</returns>
        private long ReadInt(int numBytes, ref int newPosition, ref int numBytesLeft)
        {
            byte[] rb = ReadBytes(numBytes, ref newPosition, ref numBytesLeft);
            return IntFromArray(numBytes, rb);
        }

        /// <summary>
        /// Reads an integer from a subsize
        /// </summary>
        /// <param name="numBytes">size of the int to red</param>
        /// <returns>the int we read</returns>
        private long ReadIntFromSubbox(int numBytes)
        {
            byte[] rb = ReadBytes(numBytes, ref m_currentSubboxPos, ref m_currentSubboxLeft);
            return IntFromArray(numBytes, rb);
        }

        /// <summary>
        /// Inits a subbox for reading
        /// </summary>
        /// <param name="startPosition">the start of the subbox</param>
        /// <param name="numBytes">the size of the subbox</param>
        private void SubboxInit(int startPosition, int numBytes)
        {
            m_subboxPos = startPosition;
            m_subboxLeft = numBytes;
        }

        /// <summary>
        /// Go to the next subbox
        /// </summary>
        /// <param name="subboxType">the type of the subbox</param>
        /// <param name="subboxStartPosition">the start of the subbox</param>
        /// <param name="subboxSize">the size of the subbox</param>
        /// <returns>true if we found a subbox</returns>
        private bool SubboxNext(out byte[] subboxType, out int subboxStartPosition, out int subboxSize)
        {
            subboxType = null;
            subboxStartPosition = 0;
            subboxSize = 0;

            if (m_subboxLeft < 8)
            {
                return false;
            }

            subboxStartPosition = m_subboxPos;
            subboxSize = (int)ReadInt(4, ref m_subboxPos, ref m_subboxLeft);
            subboxType = ReadBytes(4, ref m_subboxPos, ref m_subboxLeft);

            if (subboxSize == 1)
            {
                if (m_subboxLeft < 8)
                {
                    return false;
                }

                subboxSize = (int)ReadInt(8, ref m_subboxPos, ref m_subboxLeft);

                if (subboxSize < 8)
                {
                    throw new ChunkParserException();
                }

                subboxSize -= 8;
            }

            if (subboxSize < 8)
            {
                throw new ChunkParserException();
            }

            subboxSize -= 8;

            if (subboxSize > m_subboxLeft)
            {
                // Containment violation
                throw new ChunkParserException();
            }

            subboxStartPosition = m_subboxPos;
            m_subboxPos += subboxSize;
            m_subboxLeft -= subboxSize;

            return true;
        }
    }
}
