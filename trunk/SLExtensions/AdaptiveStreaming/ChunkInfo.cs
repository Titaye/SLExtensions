//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace Microsoft.Expression.Encoder.AdaptiveStreaming
{
    using System;

    /// <summary>
    /// This class describes an individual chunk of data.
    /// </summary>
    public class ChunkInfo
    {
        /// <summary>
        /// the chunk id
        /// </summary>
        private int m_id;

        /// <summary>
        /// Chunk duration
        /// </summary>
        private ulong m_duration;

        /// <summary>
        /// Chunk start time
        /// </summary>
        private ulong m_startTime;

        /// <summary>
        /// Initializes a new instance of the ChunkInfo class
        /// </summary>
        /// <param name="id">the chunk id</param>
        /// <param name="duration">chunk duration</param>
        /// <param name="startTime">start time of the chunk</param>
        public ChunkInfo(int id, ulong duration, ulong startTime)
        {
            m_id = id;
            m_duration = duration;
            m_startTime = startTime;
        }

        /// <summary>
        /// Gets the id of this chunk
        /// </summary>
        public int Id
        {
            get
            {
                return m_id;
            }
        }

        /// <summary>
        /// Gets the duration of this chunk
        /// </summary>
        public ulong Duration
        {
            get
            {
                return m_duration;
            }
        }

        /// <summary>
        /// Gets the starting presentation time of this chunk
        /// </summary>
        public ulong StartTime
        {
            get
            {
                return m_startTime;
            }
        }
    }
}
