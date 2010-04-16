//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace Microsoft.Expression.Encoder.AdaptiveStreaming
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Media;

    /// <summary>
    /// This class defines the basic information that needs to be exposed by
    /// any manifest parser. You can extend this class with your own private
    /// data if a custom parser needs more information.
    /// </summary>
    public class ManifestInfo
    {
        /// <summary>
        /// List of markers in the header of this manifest
        /// </summary>
        private TimelineMarkerCollection m_timelineMarkers = new TimelineMarkerCollection();

        /// <summary>
        /// This list contains the information about all of the streams that are in this manifest.
        /// This is artificially limited to 2 right now since that is all we support.
        /// </summary>
        private List<StreamInfo> m_streams = new List<StreamInfo>(MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities.Configuration.Streams.MaxStreams);

        /// <summary>
        /// This keeps track of which streams are active. We are limited to 1 video
        /// and 1 audio stream. If we have multiple audio and video streams in the manifest,
        /// then this keeps track of the first audio and video found.
        /// </summary>
        private StreamInfo[] m_activeStreams = new StreamInfo[MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities.Configuration.Streams.MaxStreams];

        /// <summary>
        /// This contains a list of MediaStreamDescription objects as required by the MediaElement
        /// when we call MediaStreamSource.ReportOpenMediaCompleted()
        /// </summary>
        private List<MediaStreamDescription> m_streamDescriptions = new List<MediaStreamDescription>();

        /// <summary>
        /// This contains our dictionary of media attributes, as required by the MediaElement
        /// when we call MediaStreamSource.ReportOpenMediaCompleted()
        /// </summary>
        private Dictionary<MediaSourceAttributesKeys, string> m_mediaAttributes = new Dictionary<MediaSourceAttributesKeys, string>(MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities.Configuration.Streams.MaxStreams);

        /// <summary>
        /// Keeps track of the major version of the manifest. This is not used internally
        /// however it is useful for version tracking.
        /// </summary>
        private int m_manifestMajorVersion = -1;

        /// <summary>
        /// Keeps track of the minor version of the manifest. This is not used internally
        /// however it is useful for version tracking.
        /// </summary>
        private int m_manifestMinorVersion = -1;

        /// <summary>
        /// Gets or sets the manifest major version. Not used internall, but useful for
        /// version tracking.
        /// </summary>
        public int ManifestMajorVersion
        {
            get
            {
                return m_manifestMajorVersion;
            }

            set
            {
                m_manifestMajorVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the manifest minor version. Not used internall, but useful for
        /// version tracking.
        /// </summary>
        public int ManifestMinorVersion
        {
            get
            {
                return m_manifestMinorVersion;
            }

            set
            {
                m_manifestMinorVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is a valid manifest, false otherwise. A manifest that is invalid
        /// is one that was parsed but does not conform to the specfication.
        /// </summary>
        public bool Valid { get; set; }

        /// <summary>
        /// Gets the number of streams that are contained in this manifest. This
        /// includes all streams, not just the active ones.
        /// </summary>
        public int NumberOfStreams 
        { 
            get 
            { 
                return m_streams.Count; 
            } 
        }

        /// <summary>
        /// Gets a list of MediaStreamDescription objects for each of the streams in our manifest.
        /// This is used in our call to ReportOpenMediaCompleted back to our hosting MediaElement.
        /// </summary>
        public IEnumerable<MediaStreamDescription> MediaStreamDescriptions 
        { 
            get 
            { 
                return m_streamDescriptions; 
            } 
        }

        /// <summary>
        /// Gets the collection of markers in the manifest
        /// </summary>
        public TimelineMarkerCollection Markers
        {
            get
            {
                return m_timelineMarkers;
            }
        }

        /// <summary>
        /// Gets a dictionary of MediaSourceAttributeKeys which describe attributes
        /// of our streams. These attributes are used in our call to ReportOpenMediaCompleted 
        /// back to our hosting MediaElement
        /// </summary>
        public IDictionary<MediaSourceAttributesKeys, string> MediaAttributes 
        { 
            get 
            { 
                return m_mediaAttributes; 
            } 
        }

        /// <summary>
        /// Retrieves the information for a particular stream
        /// </summary>
        /// <param name="streamId">the id of the stream to retrieve</param>
        /// <returns>A StreamInfo containing the information for the given stream</returns>
        public StreamInfo GetStreamInfoForStream(int streamId) 
        { 
            return m_streams[streamId]; 
        }

        /// <summary>
        /// Gets the information for the particular stream type. If there are multiple
        /// streams of the given type, it will return the first one it finds.
        /// </summary>
        /// <param name="type">the type of stream to retrieve</param>
        /// <returns>A StreamInfo object with the information for the stream</returns>
        public StreamInfo GetStreamInfoForStreamType(MediaStreamType type)
        {
            foreach (StreamInfo info in m_activeStreams)
            {
                if (info != null)
                {
                    if (info.MediaType == type)
                    {
                        return info;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a stream to this manifest
        /// </summary>
        /// <param name="streamIndexInfo">the stream to add</param>
        public void AddStream(StreamInfo streamIndexInfo)
        {
            // Let's go through and fixup some of the info in the
            // stream and make sure that it is all valid
            streamIndexInfo.CalculateStartTimes();

            // Now add it to our list of streams
            m_streams.Add(streamIndexInfo);

            // We can only have 1 audio and 1 video stream active, so let's keep track
            // of which ones are active if we have more than one
            if (m_activeStreams[(int)streamIndexInfo.MediaType] == null)
            {
                m_activeStreams[(int)streamIndexInfo.MediaType] = streamIndexInfo;
            }

            // Add our media stream descriptor
            m_streamDescriptions.Add(streamIndexInfo.Description);
        }

        /// <summary>
        /// Shutdowns this manifest object
        /// </summary>
        internal void Shutdown()
        {
            foreach (StreamInfo stream in m_streams)
            {
                stream.Queue.Shutdown();
            }
        }
    }
}
