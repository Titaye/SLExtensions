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
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Advertising;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Heuristic;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Logging;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Manifest;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Network;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Parsing;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Url;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities;

    /// <summary>
    /// This class is the main implementation of the Silverlight MediaStreamSource.
    /// It supports reading in a manifest file that describes the various audio/video
    /// bitrates that we are capable of streaming. It automatically switches between
    /// the various bitrates based on several different variables as implemented
    /// in the Heuristics module. We provide a default Heuristics implementation which
    /// can be overriden for more fine tuned control of which bitrates are streamed.
    /// </summary>
    public partial class AdaptiveStreamingSource : MediaStreamSource, IDisposable
    {
        /// <summary>
        /// Constant for 1 second in hundred nanosecond units
        /// </summary>
        private const ulong OneSecondInHns = 1000 * 1000 * 10;

        /// <summary>
        /// Our list of advertising insertion points
        /// </summary>
        private AdInsertionPoints m_adInsertionPoints = new AdInsertionPoints();

        /// <summary>
        /// Our current buffering state
        /// </summary>
        private int m_bufferingStateMask;

        /// <summary>
        /// An array to keep track of how many chunks per stream we miss in a row
        /// </summary>
        private int[] m_consecutiveMissedChunks = new int[Configuration.Streams.MaxStreams];

        /// <summary>
        /// Similar to the way we keep track of the current playing bitrate to track changes,
        /// we have another array to keep track of the current downloading bitrate. This can
        /// then be used when we get a bitrate changed event to determine if it really changed.
        /// </summary>
        private ulong[] m_downloadBitrate = new ulong[Configuration.Streams.MaxStreams];

        /// <summary>
        /// The current Heuristics algorithms that we are using. This module contains
        /// all of the logic for choosing a bitrate to download and stream
        /// </summary>
        private Heuristics m_heuristics = new HeuristicsImpl();

        /// <summary>
        /// We use this flag to skip doing anything when we get our first seek to 0
        /// </summary>
        private bool m_isFirstSeekFlag = true;

        /// <summary>
        /// This flag keeps track of whether or not we have started the
        /// work queue thread.
        /// </summary>
        private bool m_isWorkQueueThreadStarted;

        /// <summary>
        /// This is the absolute Url to the manifest file we are streaming. It differs from m_ManifestSourceUrl
        /// only when the latter is a relative url
        /// </summary>
        private Uri m_manifestAbsoluteUrl;

        /// <summary>
        /// This variable keeps track of the information in the manifest.
        /// Through it, you can access all the information you need about
        /// each stream we are serving up.
        /// </summary>        
        private ManifestInfo m_manifestInfo;

        /// <summary>
        /// This is the Url to the manifest file that we are streaming. If this is an absolute
        /// Url (http://XXX) then m_ManifestSourceUrl is equal to m_ManifestAbsoluteUrl
        /// </summary>
        private Uri m_manifestSourceUrl;

        /// <summary>
        /// Used for the media sample attributes when we can't get a sample
        /// </summary>
        private Dictionary<MediaSampleAttributeKeys, string> m_nullAttributes = new Dictionary<MediaSampleAttributeKeys, string>(0);

        /// <summary>
        /// This member is a wrapper for the MediaElement control. We can use
        /// it to obtain playback information like frames dropped, frames per second,
        /// etc
        /// </summary>
        private PlaybackInfo m_playbackInfo;

        /// <summary>
        /// Internally, bitrate messages are fired for each. We only want to notify any clients
        /// when the actual playing bitrate has changed. Therefore, we remember each bitrate that
        /// we have seen and only fire changed events if the bitrate does indeed change.
        /// </summary>
        private ulong[] m_playBitrate = new ulong[Configuration.Streams.MaxStreams];

        /// <summary>
        /// The current state of our stream
        /// </summary>
        private State m_state = State.None;

        /// <summary>
        /// This is the queue of pending commands. These commands are run on a background
        /// thread so that we do not block the UI.
        /// </summary>
        private WorkQueue m_workQueue;

        /// <summary>
        /// This is the background thread that handles all of the commands in the work queue
        /// </summary>
        private Thread m_workQueueThread;

        /// <summary>
        /// Provide an object with strong identity to lock the work queue thread call on
        /// </summary>
        private object m_workQueueThreadLock = new object();

        /// <summary>
        /// Initializes a new instance of the AdaptiveStreamingSource class
        /// </summary>
        /// <param name="mediaElement">The media element that we are sending samples to. We use it internally to keep track of playback statistics</param>
        /// <param name="url">The url of the manifest for the stream we are serving</param>
        public AdaptiveStreamingSource(MediaElement mediaElement, Uri url)
        {
            // Make sure our Url is not null
            if (null == url)
            {
                throw new ArgumentNullException("url", Errors.NullUrlOnMSSError);
            }

            // Also check the mediaElement parameter
            if (null == mediaElement)
            {
                throw new ArgumentNullException("mediaElement", Errors.NullMediaElementOnMSSError);
            }

            // Remember the Url to the manifest we are streaming
            m_manifestSourceUrl = url;

            // Create our default manifest parser
            ManifestParser = new ManifestParserImpl();

            // Create our default chunk parser factory
            ChunkParserFactory = new ChunkParserFactoryImpl();

            // Create our default url generator
            UrlGenerator = new UrlGeneratorImpl();

            // Create a new queue for processing commands. All work is done on a background thread,
            // which will shuttle events back to the UI thread in case something needs to be displayed.
            m_workQueue = new WorkQueue();

            // Create the thread that we are going to run background commands on
            m_workQueueThread = new Thread(WorkerThread);

            // Make sure we remember the Dispatcher class for the UI thread
            UIDispatcher.Load();

            // Playback info is a wrapper on around media element
            m_playbackInfo = new PlaybackInfo(mediaElement);

            // Hook our heuristics events
            m_heuristics.ChunkReplacementSuggested += HeuristicsChunkReplacementSuggested;
        }

        /// <summary>
        /// Prevents a default instance of the AdaptiveStreamingSource class from being created
        /// </summary>
        private AdaptiveStreamingSource()
        {
        }

        /// <summary>
        /// This event is fired at the moment when the media source gets out of buffering 
        /// state and starts serving samples
        /// </summary>
        public event EventHandler<EventArgs> BufferingDone;

        /// <summary>
        /// This event is fired at the moment when media source finds itself out of 
        /// media to serve with outstanding requests
        /// </summary>
        public event EventHandler<EventArgs> BufferingStarted;

        /// <summary>
        /// This event is fired whenever we start downloading a new bitrate
        /// </summary>
        public event EventHandler<BitrateChangedEventArgs> DownloadBitrateChange;

        /// <summary>
        /// Called after AdPauseExpectedAt() when application has full bandwidth to start downloading ads content
        /// </summary>
        public event EventHandler<DownloadPausedEventArgs> DownloadsPaused;

        /// <summary>
        /// This is a public event which is fired whenever a media chunk has finished
        /// downloading. It is exposed so that external callers can keep track of what's
        /// going on, and is useful for tracking state and regression testing. This event
        /// does not need to be handled and can safely be ignored.
        /// </summary>
        public event EventHandler<MediaChunkDownloadedEventArgs> MediaChunkDownloaded;

        /// <summary>
        /// This event is fired whenever the bitrate of the playing video is changed.
        /// </summary>
        public event EventHandler<BitrateChangedEventArgs> PlayBitrateChange;

        /// <summary>
        /// The GetSampleCompleted event, which is fired every time we get a sample
        /// </summary>
        protected event EventHandler<GetSampleCompletedEventArgs> GetSampleCompleted;

        /// <summary>
        /// This enumeration is used for keeping track of the state of our
        /// stream.
        /// </summary>
        private enum State
        {
            /// <summary>
            /// Stream is neither open nor closed
            /// </summary>
            None,

            /// <summary>
            /// Stream is opened
            /// </summary>
            Opened,

            /// <summary>
            /// Stream is closed
            /// </summary>
            Closed
        }

        /// <summary>
        /// Gets or sets the factory class that creates new chunk parsers. We use 1 parser
        /// object for each chunk that we receive.
        /// </summary>
        public IChunkParserFactory ChunkParserFactory { get; set; }

        /// <summary>
        /// Gets or sets the module that performs our network buffering and bitrate throttling heuristics
        /// </summary>
        public Heuristics Heuristics
        {
            get
            {
                return m_heuristics;
            }

            set
            {
                // Unhook our old events
                m_heuristics.ChunkReplacementSuggested -= HeuristicsChunkReplacementSuggested;

                // Set the new value
                m_heuristics = value;

                // Rehook our new ones
                m_heuristics.ChunkReplacementSuggested += HeuristicsChunkReplacementSuggested;
            }
        }

        /// <summary>
        /// Gets or sets the parser for a manifest file. It is set to our default parser, but can
        /// be changed to support custom manifest files.
        /// </summary>
        public IManifestParser ManifestParser { get; set; }

        /// <summary>
        /// Gets or sets the url generator for each chunk based on the base url in the manifest.
        /// You can set your own generator here for custom url generation.
        /// </summary>
        public IUrlGenerator UrlGenerator { get; set; }

        /// <summary>
        /// Gets the current manifest used in this source.
        /// </summary>
        public ManifestInfo Manifest
        {
            get
            {
                return m_manifestInfo;
            }
        }

        /// <summary>
        /// Called by an application ahead of time to make media source to free download bandwidth for ads content
        /// </summary>
        /// <param name="timestamp">the time to pause for an ad</param>
        /// <param name="duration">the duration to pause for</param>
        public void AdPauseExpectedAt(TimeSpan timestamp, TimeSpan duration)
        {
            m_adInsertionPoints.Add(new AdInsertionPoint(timestamp, duration));
        }

        /// <summary>
        /// Called by an application ahead of time to make media source to free download bandwidth for ads content
        /// </summary>
        /// <param name="adInsertPoints">the points to pause at</param>
        public void AdPauseExpectedAt(AdInsertionPoint[] adInsertPoints)
        {
            foreach (AdInsertionPoint point in adInsertPoints)
            {
                this.m_adInsertionPoints.Add(point);
            }
        }

        /// <summary>
        /// Get the bitrates that the given stream type supports
        /// </summary>
        /// <param name="streamType">the stream type to query</param>
        /// <returns>an array of bitrates that we support</returns>
        public ulong[] Bitrates(MediaStreamType streamType)
        {
            if (m_manifestInfo == null)
            {
                return null;
            }

            StreamInfo stream = m_manifestInfo.GetStreamInfoForStreamType(streamType);
            if (stream == null)
            {
                return null;
            }

            return stream.GetBitrateArray();
        }

        /// <summary>
        /// Gets the size of data in our buffer for the given stream type
        /// </summary>
        /// <param name="streamType">the stream type to query</param>
        /// <returns>the size of data in the buffer</returns>
        public ulong BufferSize(MediaStreamType streamType)
        {
            if (m_manifestInfo == null)
            {
                return 0;
            }

            StreamInfo stream = m_manifestInfo.GetStreamInfoForStreamType(streamType);
            if (stream == null)
            {
                return 0;
            }

            return stream.Queue.BufferSize;
        }

        /// <summary>
        /// Gets the amount of data we have in our buffer for the given stream type
        /// </summary>
        /// <param name="streamType">stream type to query</param>
        /// <returns>the amount of data in the buffer</returns>
        public TimeSpan BufferTime(MediaStreamType streamType)
        {
            if (m_manifestInfo == null)
            {
                return new TimeSpan(0);
            }

            StreamInfo stream = m_manifestInfo.GetStreamInfoForStreamType(streamType);
            if (stream == null)
            {
                return new TimeSpan(0);
            }

            return new TimeSpan((long)stream.Queue.BufferTime);
        }

        /// <summary>
        /// Implements IDisposable.Dispose()
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Call this to start resuming downloads again after a pause has been reached
        /// </summary>
        public void DownloadsResume()
        {
            if (m_heuristics != null)
            {
                m_heuristics.ResumeDownloads();
            }
        }

        /// <summary>
        /// Tells this media source to only use a limited range of the available bitrates
        /// </summary>
        /// <param name="streamType">the stream type to set</param>
        /// <param name="minBitrate">the new minimum bitrate, in bps</param>
        /// <param name="maxBitrate">the new maximum bitrate, in bps</param>
        public void SetBitrateRange(MediaStreamType streamType, long minBitrate, long maxBitrate)
        {
            m_heuristics.SetBitrateRange(streamType, minBitrate, maxBitrate);
        }

        /// <summary>
        /// Overrides MediaStreamSource.CloseMedia
        /// </summary>
        protected override void CloseMedia()
        {
            try
            {
                // Mark our state as closed
                m_state = State.Closed;

                // Detach our playback wrapper from the media element
                if (m_playbackInfo != null)
                {
                    m_playbackInfo.Detach();
                }

                // No need to respond to anything because we are shutting down
                if (m_workQueue != null)
                {
                    m_workQueue.ClearAndEnqueue(new WorkQueueElement(WorkQueueElement.Command.Close, null));
                }
            }
            catch (AdaptiveStreamingException e)
            {
                RaiseError(e);
            }
            catch (Exception e)
            {
                RaiseError(e);
                throw;
            }
        }

        /// <summary>
        /// Disposes both managed and unmanaged resources
        /// </summary>
        /// <param name="isDisposing">true to clean up both managed and unmanaged resources, false for managed only</param>
        protected virtual void Dispose(bool isDisposing)
        {
            // Detach our playback wrapper from the media element
            if (m_playbackInfo != null)
            {
                m_playbackInfo.Dispose();
            }

            // If we have a work queue, post a close message to it
            if (m_workQueueThread != null && m_workQueue != null)
            {
                // No need to respond to anything because we are shutting down
                m_workQueue.ClearAndEnqueue(new WorkQueueElement(WorkQueueElement.Command.Close, null));

                // Wait for the thread to close
                m_workQueueThread.Join(3000);

                // Dispose the work queue
                m_workQueue.Dispose();
            }
        }

        /// <summary>
        /// Overrides MediaStreamSource.GetDiagnosticAsync
        /// </summary>
        /// <param name="diagnosticKind">describes the kind of information to get</param>
        protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
        {
            try
            {
                // Shuffle this command off to a worker thread
                m_workQueue.Enqueue(new WorkQueueElement(WorkQueueElement.Command.Diagnostics, diagnosticKind));
            }
            catch (AdaptiveStreamingException e)
            {
                RaiseError(e);
            }
            catch (Exception e)
            {
                RaiseError(e);
                throw;
            }
        }

        /// <summary>
        /// Overrides MediaStreamSource.GetSampleAsync
        /// </summary>
        /// <param name="mediaStreamType">the type of media stream to retrieve a sample for</param>
        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            try
            {
                // Setup some tracing information
                DateTime enter = DateTime.Now;

                // Tell our heurisitics module that we are requesting a new sample type
                m_heuristics.OnSampleRequested(mediaStreamType);

                // Do the actual work on our background thread
                m_workQueue.Enqueue(new WorkQueueElement(WorkQueueElement.Command.Sample, mediaStreamType));

                DateTime exit = DateTime.Now;
                if ((exit - enter).TotalSeconds > 20e-3)
                {
                    Tracer.Trace(TraceChannel.Timing, "GetSampleAsync: long time: {0}", (exit - enter).TotalSeconds);
                }
            }
            catch (AdaptiveStreamingException e)
            {
                RaiseError(e);
            }
            catch (Exception e)
            {
                RaiseError(e);
                throw;
            }
        }

        /// <summary>
        /// Overrides MediaStreamSource.OpenMediaAsync
        /// </summary>
        protected override void OpenMediaAsync()
        {
            try
            {
                // If called from multiple threads, make sure that we only do this one at a time
                lock (m_workQueueThreadLock)
                {
                    // If we have not started the work queue thread, then start it now
                    if (!m_isWorkQueueThreadStarted)
                    {
                        // Create a new WebClient to open the manifest file
                        WebClient client = new WebClient();
                        client.OpenReadCompleted += new OpenReadCompletedEventHandler(OnManifestReadCompleted);

                        // Our manifest Url can either be absolute or relative
                        Uri source = m_manifestSourceUrl;

                        // Construct and absolute uri
                        Uri absoluteUri = source;
                        if (absoluteUri.IsAbsoluteUri == false)
                        {
                            absoluteUri = new Uri(new Uri(client.BaseAddress), absoluteUri);
                        }

                        // Asynchronously open the manifest
                        client.OpenReadAsync(source, absoluteUri);

                        // Start our work queue thread to handle commands
                        m_workQueueThread.Start();

                        // make sure we never start it twice
                        m_isWorkQueueThreadStarted = true;
                    }
                }
            }
            catch (AdaptiveStreamingException e)
            {
                RaiseError(e);
            }
            catch (Exception e)
            {
                RaiseError(e);
                throw;
            }
        }

        /// <summary>
        /// Overrides MediaStreamSource.SeekAsync
        /// </summary>
        /// <param name="seekToTime">the time to seek to</param>
        protected override void SeekAsync(long seekToTime)
        {
            try
            {
                // Send this request to our worker thread
                m_workQueue.Enqueue(new WorkQueueElement(WorkQueueElement.Command.Seek, seekToTime));
            }
            catch (AdaptiveStreamingException e)
            {
                RaiseError(e);
            }
            catch (Exception e)
            {
                RaiseError(e);
                throw;
            }
        }

        /// <summary>
        /// Overrides MediaStreamSource.SwitchMediaStreamAsync
        /// </summary>
        /// <param name="mediaStreamDescription">the stream to switch to</param>
        protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
        {
            try
            {
                // Send this request to our worker thread
                m_workQueue.Enqueue(new WorkQueueElement(WorkQueueElement.Command.SwitchMedia, mediaStreamDescription));
            }
            catch (AdaptiveStreamingException e)
            {
                RaiseError(e);
            }
            catch (Exception e)
            {
                RaiseError(e);
                throw;
            }
        }

        /// <summary>
        /// Event handler function for when a new chunk is requested.
        /// </summary>
        /// <param name="sender">The object sending this request</param>
        /// <param name="args">Event args for this request</param>
        private void HeuristicsChunkReplacementSuggested(object sender, EventArgs args)
        {
            try
            {
                m_workQueue.Enqueue(new WorkQueueElement(WorkQueueElement.Command.ReplaceMedia, null));
            }
            catch (AdaptiveStreamingException e)
            {
                RaiseError(e.ToString());
            }
            catch (Exception e)
            {
                RaiseError(e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Handle a close command
        /// </summary>
        private void DoClose()
        {
            Tracer.Trace(TraceChannel.MSS, "Media closed");
            if (m_heuristics != null)
            {
                m_heuristics.Shutdown();
            }

            if (m_manifestInfo != null)
            {
                m_manifestInfo.Shutdown();
            }

            m_workQueueThread = null;
            m_isWorkQueueThreadStarted = false;
            m_workQueue = null;
            m_playbackInfo = null;
        }

        /// <summary>
        /// Do a diagnostics command request
        /// </summary>
        /// <param name="diagnosticsKind">the kind of diagnostics to get</param>
        private void DoDiagnostics(MediaStreamSourceDiagnosticKind diagnosticsKind)
        {
            StreamInfo audioStreamInfo = m_manifestInfo.GetStreamInfoForStreamType(MediaStreamType.Audio);
            ulong result = 0;
            switch (diagnosticsKind)
            {
                case MediaStreamSourceDiagnosticKind.BufferLevelInBytes:
                    result = m_manifestInfo.GetStreamInfoForStreamType(MediaStreamType.Video).Queue.BufferSize;
                                        
                    if (audioStreamInfo != null)
                    {
                        result += audioStreamInfo.Queue.BufferSize;
                    }

                    break;
                case MediaStreamSourceDiagnosticKind.BufferLevelInMilliseconds:
                    ulong video = m_manifestInfo.GetStreamInfoForStreamType(MediaStreamType.Video).Queue.BufferTime;
                    ulong audio = 0;
                    if (audioStreamInfo != null)
                    {
                        audio = audioStreamInfo.Queue.BufferTime;
                    }

                    result = (video > audio ? audio : video) / 10000;
                    break;
            }

            ReportGetDiagnosticCompleted(diagnosticsKind, (long)result);
        }

        /// <summary>
        /// This function reads the manfiest file from the given stream
        /// </summary>
        /// <param name="manifestStream">The manifest file to read</param>
        private void DoOpen(Stream manifestStream)
        {
            // Check for errors
            if (null == manifestStream)
            {
                RaiseError(string.Format(CultureInfo.InvariantCulture, Errors.ManifestFailureError, m_manifestSourceUrl));
                return;
            }

            // Parse the manifest file using our current parser
            m_manifestInfo = ManifestParser.ParseManifest(manifestStream, m_manifestAbsoluteUrl);
            manifestStream.Close();

            // Heuristics modules can tell us to either request a new download, or to pause
            // all of our current downloads. Either way, we need to listen for those events.
            m_heuristics.RequestDownload += new EventHandler<Heuristics.RequestDownloadEventArgs>(StartChunkDownload);
            m_heuristics.DownloadsPaused += new EventHandler<Heuristics.DownloadsPausedEventArgs>(FireDownloadsPaused);

            // Initialize our heuristics module. Convert our ad points into an array
            AdInsertionPoint[] adArray = m_adInsertionPoints.ToArray();
            m_heuristics.Initialize(m_manifestInfo, m_playbackInfo, adArray);

            // Set our state to opened
            m_state = State.Opened;

            // Report back to our media element that we have completed opening the media
            ReportOpenMediaCompleted(m_manifestInfo.MediaAttributes, m_manifestInfo.MediaStreamDescriptions);

            // Attach our playback info to the media element
            if (m_playbackInfo != null)
            {
                m_playbackInfo.Attach();
            }
        }

        /// <summary>
        /// Handle a ParseChunk command
        /// </summary>
        /// <param name="chunk">the media chunk we are parsing</param>
        private void DoParseChunk(MediaChunk chunk)
        {
            // Try to parse the chunk
            if (!chunk.ParseHeader(ChunkParserFactory))
            {
                Tracer.Trace(TraceChannel.Error, "Media data chunk {0} ({2} kbps) has a bad format, chunk state is {1}.", chunk.Sid, chunk.State, chunk.Bitrate);
            }

            // We work with chunks inplace, but need to update queue pointers
            m_manifestInfo.GetStreamInfoForStreamType(chunk.MediaType).Queue.Add(chunk);
        }

        /// <summary>
        /// Do the actual ReportGetSampleCompleted work. Fires an event in case derived classes
        /// want to listen in.
        /// </summary>
        /// <param name="chunk">the chunk with our sample</param>
        /// <param name="mediaStreamSample">the sample we are reporting</param>
        private void DoReportGetSampleCompleted(MediaChunk chunk, MediaStreamSample mediaStreamSample)
        {
            ReportGetSampleCompleted(mediaStreamSample);
            GetSampleCompletedEventArgs args = new GetSampleCompletedEventArgs();
            args.Sample = mediaStreamSample;
            args.ChunkId = (chunk != null) ? chunk.ChunkId : -1;
            args.Bitrate = (chunk != null) ? chunk.Bitrate : 0;
            args.StreamId = (chunk != null) ? chunk.StreamId : -1;

            if (GetSampleCompleted != null && chunk != null)
            {
                GetSampleCompleted(this, args);
            }
        }

        /// <summary>
        /// Handle a sample request
        /// </summary>
        /// <param name="elem">the work command we are handling</param>
        private void DoSample(WorkQueueElement elem)
        {
            DateTime enter = DateTime.Now;

            // Give the sample if available, nothing otherwise
            MediaStreamType mediaStreamType = (MediaStreamType)elem.CommandParameter;
            int mediaTypeIndex = (int)mediaStreamType;

            // Get the queue of chunks for this stream
            MediaChunkQueue mediaQueue = m_manifestInfo.GetStreamInfoForStreamType(mediaStreamType).Queue;
            int mediaTypeMask = mediaStreamType == MediaStreamType.Video ? 0x01 : 0x02;

            MediaChunk chunk = null;
            GetFrameData frameData = null;

            bool flag = true;
            while (flag)
            {
                // Get the current chunk from our media queue
                flag = false;
                chunk = mediaQueue.Current;
                if (chunk == null)
                {
                    // This is a redudant code in a case native side calls twice after the end of media
                    // It should not happen but because we played with serialization on/off we keep it in place as a defensive code
                    DoReportGetSampleCompleted(null, new MediaStreamSample(m_manifestInfo.GetStreamInfoForStreamType(mediaStreamType).Description, null, 0, 0, 0, m_nullAttributes));
                    return;
                }

                // If we have not finished parsing the chunk yet, then we can't send the sample
                if (chunk.State == MediaChunk.ChunkState.Pending || chunk.State == MediaChunk.ChunkState.Loaded)
                {
                    // If the chunk is pending but hasn't been downloaded yet, then force a download
                    if (chunk.State == MediaChunk.ChunkState.Pending && chunk.Downloader == null)
                    {
                        if (chunk.Bitrate == 0)
                        {
                            // Silverlight failed to load previous chunk or inform us about failure and as a result we did not even started a new one
                            chunk.Bitrate = m_manifestInfo.GetStreamInfoForStreamType(chunk.MediaType).Bitrates[0];
                        }

                        Tracer.Trace(TraceChannel.Error, "Lost {0} in state {1} trying to load again.", chunk.Sid, chunk.State);
                        m_heuristics.ForceNextDownload(chunk.StreamId, chunk.ChunkId);
                        m_heuristics.ScheduleDownloads();
                    }

                    // Media chunk is not yet available, try again later
                    m_workQueue.Enqueue(elem);
                    ReportGetSampleProgress(chunk.DownloadPercent / 100.0f);

                    if ((m_bufferingStateMask & mediaTypeMask) == 0)
                    {
                        if (m_bufferingStateMask == 0)
                        {
                            FireBufferingStarted();
                        }

                        m_bufferingStateMask |= mediaTypeMask;
                    }

                    // Take a nap to give us some time to download. If it's already downloaded, 
                    // then it just hasn't been parsed yet
                    if (chunk.DownloadPercent < 100)
                    {
                        Thread.Sleep(10);
                    }

                    if (chunk.SampleRequestsMissed++ == 0)
                    {
                        Tracer.Trace(
                            TraceChannel.Error, 
                            "Chunk {0} is not available on sample request, chunk state {1}, downloader is {2}",
                            chunk.Sid, 
                            chunk.State, 
                            chunk.Downloader == null ? "null" : "not null");
                    }
                    else if (chunk.SampleRequestsMissed % 100 == 0)
                    {
                        Tracer.Trace(
                            TraceChannel.Error, 
                            "Chunk {0} is not available for {3} seconds, chunk state {1}, downloader is {2}",
                            chunk.Sid, 
                            chunk.State, 
                            chunk.Downloader == null ? "null" : "not null", 
                            chunk.SampleRequestsMissed / 100);
                    }
                    else if (chunk.SampleRequestsMissed >= (m_playbackInfo.IsPlaying ? 500 : 1500))
                    {
                        // After 5 seconds delay during play or 15 seconds while paused or stopped, move on to the next chunk.
                        if (chunk.Downloader != null)
                        {
                            chunk.Downloader.CancelDownload();
                        }

                        chunk.SampleRequestsMissed = 0;

                        m_consecutiveMissedChunks[mediaTypeIndex]++;
                        string msg = String.Format(CultureInfo.InvariantCulture, "Failed to load in time media chunk {0} ({1},{2}, #{3} in a row)", chunk.Sid, chunk.Bitrate, chunk.State, m_consecutiveMissedChunks[mediaTypeIndex]);

                        // If we have missed to many, then throw an error
                        if (m_consecutiveMissedChunks[mediaTypeIndex] >= Configuration.Playback.MaxMissingOrCorruptedChunks)
                        {
                            throw new AdaptiveStreamingException(msg);
                        }
                        else
                        {
                            Tracer.Trace(TraceChannel.Error, msg);
                        }

                        mediaQueue.MoveNext();

                        // No need to verify flag, if we hit end of stream we'll know in 10 milliseconds
                    }

                    DateTime exit2 = DateTime.Now;
                    if ((exit2 - enter).TotalSeconds > 20e-3)
                    {
                        Tracer.Trace(TraceChannel.Timing, "DoSample: long time: {0}", (exit2 - enter).TotalSeconds);
                    }

                    return;
                }
                else if (chunk.State != MediaChunk.ChunkState.Parsed)
                {
                    // We are not parsed, so flag us as missed
                    m_consecutiveMissedChunks[mediaTypeIndex]++;
                    string msg = String.Format(
                        CultureInfo.InvariantCulture, 
                        "Failed to {0} media chunk {1} ({2}), #{3} in a row.",
                        chunk.State == MediaChunk.ChunkState.Error ? "download" : "parse", 
                        chunk.Sid, 
                        chunk.Bitrate, 
                        m_consecutiveMissedChunks[mediaTypeIndex]);
                    
                    if (m_consecutiveMissedChunks[mediaTypeIndex] >= Configuration.Playback.MaxMissingOrCorruptedChunks)
                    {
                        throw new AdaptiveStreamingException(msg);
                    }
                    else
                    {
                        Tracer.Trace(TraceChannel.Error, msg);
                    }

                    mediaQueue.MoveNext();
                    m_workQueue.Enqueue(elem);
                    return;
                }

                // If we get here, then we should have a frame. Try to get it from our parser
                try
                {
                    if ((frameData = chunk.GetNextFrame()) == null)
                    {
                        // We could not get a frame from our parser, so move to the next chunk
                        flag = mediaQueue.MoveNext();
                        if (!flag)
                        {
                            // Signal end of the stream
                            DoReportGetSampleCompleted(null, new MediaStreamSample(m_manifestInfo.GetStreamInfoForStreamType(mediaStreamType).Description, null, 0, 0, 0, m_nullAttributes)); 
                            return;
                        }
                    }
                }
                catch (ChunkNotParsedException)
                {
                    // We could not get a frame from our parser, so move to the next chunk
                    flag = mediaQueue.MoveNext();
                    if (!flag)
                    {
                        // Signal end of the stream
                        DoReportGetSampleCompleted(null, new MediaStreamSample(m_manifestInfo.GetStreamInfoForStreamType(mediaStreamType).Description, null, 0, 0, 0, m_nullAttributes)); 
                        return;
                    }
                }
            }

            if (chunk.SampleRequestsMissed > 0)
            {
                Tracer.Trace(TraceChannel.Error, "Chunk {0} was not available for {1} milliseconds", chunk.Sid, chunk.SampleRequestsMissed * 10);
            }

            // Since we have a chunk here, we can reset are missed requests
            chunk.SampleRequestsMissed = 0;
            m_consecutiveMissedChunks[mediaTypeIndex] = 0;

            // Update our buffering state
            if ((m_bufferingStateMask & mediaTypeMask) != 0)
            {
                m_bufferingStateMask &= ~mediaTypeMask;

                if (m_bufferingStateMask == 0)
                {
                    FireBufferingDone();
                }
            }

            // Notify everyone about the bitrate we are using
            FireOnPlayBitrateChange(chunk.MediaType, chunk.Bitrate, DateTime.Now);

            // Check to see if we have any DRM attributes
            Dictionary<MediaSampleAttributeKeys, string> sampleAttributes = new Dictionary<MediaSampleAttributeKeys, string>();
            if (frameData.DrmData != null)
            {
                sampleAttributes.Add(/*"XCP_MS_SAMPLE_DRM"*/ MediaSampleAttributeKeys.DRMInitializationVector, Convert.ToBase64String(frameData.DrmData));
            }

            // Create the sample that we send to the media element
            MediaStreamSample sample = new MediaStreamSample(
                m_manifestInfo.GetStreamInfoForStreamType(mediaStreamType).Description,
                chunk.DownloadedPiece,
                frameData.StartOffset,
                frameData.FrameSize,
                frameData.Timestamp,
                sampleAttributes);

            // Must call if chunk.GetNextFrame is called, which happens only here above
            mediaQueue.UpdateBufferSizes();

            // Report this sample to our heuristics module
            if (mediaStreamType == MediaStreamType.Video)
            {
                m_playbackInfo.SourceFramesPerSecond = chunk.FrameRate;
            }

            m_heuristics.OnSampleDelivered(mediaStreamType, chunk.ChunkId, chunk.Bitrate, frameData.Timestamp);

            // Respond to the media element
            DoReportGetSampleCompleted(chunk, sample);

            DateTime exit = DateTime.Now;
            if ((exit - enter).TotalSeconds > 20e-3)
            {
                Tracer.Trace(TraceChannel.Timing, "DoSample: long time: {0}", (exit - enter).TotalSeconds);
            }
        }

        /// <summary>
        /// Handle the Seek command
        /// </summary>
        /// <param name="seekPosition">the position to seek to</param>
        private void DoSeek(long seekPosition)
        {
            Tracer.Trace(TraceChannel.Download, "+Seek {0}", seekPosition / 10000);

            // If this is our first seek, and our position is 0, then
            // we don't have to do anything
            if (m_isFirstSeekFlag)
            {
                m_isFirstSeekFlag = false;
                if (seekPosition == 0)
                {
                    ReportSeekCompleted((long)seekPosition);
                    return;
                }
            }

            // Don't seek beyond the beginning
            if (seekPosition < 0)
            {
                throw new AdaptiveStreamingException("Seek beyond the beginning of the stream");
            }

            ulong position = (ulong)seekPosition;
            StreamInfo stream;
            ulong audiopos = 0, videopos = 0;

            // Tell our heuristics what we are doing
            m_heuristics.StartSeek((ulong)seekPosition);

            // Get our stream video stream info
            stream = m_manifestInfo.GetStreamInfoForStreamType(MediaStreamType.Video);
            if (stream != null)
            {
                videopos = position = m_manifestInfo.GetStreamInfoForStreamType(MediaStreamType.Video).Queue.DiscardUntil(position);
                MediaChunk cur = stream.Queue.Current;

                // We always discard all content on Seek for now, hence:
                Tracer.Assert(cur != null, "Positioned beyond the end of stream after (video)");
                m_heuristics.ForceNextDownload(m_manifestInfo.GetStreamInfoForStreamType(MediaStreamType.Video).StreamId, cur.ChunkId);
            }

            // Same for audio
            stream = m_manifestInfo.GetStreamInfoForStreamType(MediaStreamType.Audio);
            if (stream != null)
            {
                audiopos = m_manifestInfo.GetStreamInfoForStreamType(MediaStreamType.Audio).Queue.DiscardUntil(position);

                if (position < audiopos)
                {
                    position = audiopos;
                }

                MediaChunk cur = stream.Queue.Current;

                // We always discard all content on Seek for now, hence:
                Tracer.Assert(cur != null, "Positioned beyond the end of stream after (audio)");
                m_heuristics.ForceNextDownload(m_manifestInfo.GetStreamInfoForStreamType(MediaStreamType.Audio).StreamId, cur.ChunkId);
            }

            // Now cancel all downloads from our previous instance
            Downloader.CancelAllDownloads(m_heuristics.InstanceId);
            m_heuristics.EndSeek((ulong)seekPosition);
            Tracer.Trace(TraceChannel.SeekPos, " A-V={0}", ((long)audiopos - (long)videopos) / 10000);
            ReportSeekCompleted((long)position);
            Tracer.Trace(TraceChannel.Download, "-Seek {0}", seekPosition / 10000);
        }

        /// <summary>
        /// Fire a buffering done event
        /// </summary>
        private void FireBufferingDone()
        {
            if (BufferingDone != null)
            {
                BufferingDone(this, new EventArgs());
            }
        }

        /// <summary>
        /// Fire a buffering started event
        /// </summary>
        private void FireBufferingStarted()
        {
            if (BufferingStarted != null)
            {
                BufferingStarted(this, new EventArgs());
            }
        }

        /// <summary>
        /// Fire a downloads paused event
        /// </summary>
        /// <param name="sender">object which is sending this event</param>
        /// <param name="args">downloads paused event args</param>
        private void FireDownloadsPaused(object sender, Heuristics.DownloadsPausedEventArgs args)
        {
            if (DownloadsPaused != null)
            {
                UIDispatcher.Schedule(new EventHandler<DownloadPausedEventArgs>(DownloadsPaused), this, args.StartTime);
            }
        }

        /// <summary>
        /// Fires an OnDownloadBitrateChange event. This function is always called from a background thread,
        /// so it delegates this call to the UI thread.
        /// </summary>
        /// <param name="streamType">the type of the stream that is changing</param>
        /// <param name="kbps">the bitrate of the stream that is changing, in kbps</param>
        /// <param name="timestamp">the timestamp that the change occurred</param>
        private void FireOnDownloadBitrateChange(MediaStreamType streamType, ulong kbps, DateTime timestamp)
        {
            if (null != DownloadBitrateChange)
            {
                if (kbps != m_downloadBitrate[(int)streamType])
                {
                    m_downloadBitrate[(int)streamType] = kbps;
                    UIDispatcher.Schedule(FireOnDownloadBitrateChangeFromUIThread, this, streamType, kbps, timestamp);
                }
            }
        }

        /// <summary>
        /// Fires the actual OnDownloadBitrateChange event. This must always be called from the UI thread.
        /// </summary>
        /// <param name="sender">object that sent this</param>
        /// <param name="args">bitrate changed event args</param>
        private void FireOnDownloadBitrateChangeFromUIThread(object sender, BitrateChangedEventArgs args)
        {
            if (null != DownloadBitrateChange)
            {
                DownloadBitrateChange(sender, args);
            }
        }

        /// <summary>
        /// Fires a OnPlayBitrateChange event. This function is always called from a background,
        /// so it will send this event to the UI thread.
        /// </summary>
        /// <param name="streamType">the stream type that is changing</param>
        /// <param name="kbps">the bitrate the stream has changed to, in kbps</param>
        /// <param name="timestamp">the timestamp the change occurred</param>
        private void FireOnPlayBitrateChange(MediaStreamType streamType, ulong kbps, DateTime timestamp)
        {
            if (null != PlayBitrateChange)
            {
                if (kbps != m_playBitrate[(int)streamType])
                {
                    m_playBitrate[(int)streamType] = kbps;
                    UIDispatcher.Schedule(FireOnPlayBitrateChangeFromUIThread, this, streamType, kbps, timestamp);
                }
            }
        }

        /// <summary>
        /// Fires a OnPlayBitrateChange event. This function assumes we are on the UI thread. Once Silverlight
        /// gives us a better way to determine that we are on the UI thread, we can merge this and
        /// FireOnBitrateChange into one function.
        /// </summary>
        /// <param name="sender">object that sent this</param>
        /// <param name="args">bitrate changed event args</param>
        private void FireOnPlayBitrateChangeFromUIThread(object sender, BitrateChangedEventArgs args)
        {
            if (null != PlayBitrateChange)
            {
                PlayBitrateChange(this, args);
            }
        }

        /// <summary>
        /// Delegate function which is called from the WebClient when the manifest
        /// has been read completely
        /// </summary>
        /// <param name="sender">object that sent this event</param>
        /// <param name="e">event args</param>
        private void OnManifestReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                // Check for error conditions
                if (e.Cancelled || null != e.Error)
                {
                    // Not much we can do without a manifest
                    RaiseError(String.Format(CultureInfo.InvariantCulture, Errors.CannotLoadManifestError, m_manifestSourceUrl, e.Error == null ? "Cancelled" : e.Error.Message));
                    return;
                }

                // Get the source url
                Uri absoluteUrl = (Uri)e.UserState;

                // Stick to original absolute URL if provided to avoid redirection trap 
                // for chunks address, but use actual URL otherwise
                m_manifestAbsoluteUrl = m_manifestSourceUrl.AbsoluteUri.StartsWith("HTTP://", StringComparison.OrdinalIgnoreCase) ? m_manifestSourceUrl : absoluteUrl;

                // Now read in the actual manifest file stream. Since this call is already
                // on a background thread, we do not need to send it to our worker thread
                Stream manifestStream = e.Result;
                DoOpen(manifestStream);
            }
            catch (AdaptiveStreamingException ex)
            {
                RaiseError(ex);
            }
            catch (Exception ex)
            {
                RaiseError(ex);
                throw;
            }
        }

        /// <summary>
        /// Raises an error to the media element. Shuts everything
        /// down and puts us in a closed state.
        /// </summary>
        /// <param name="message">message to raise</param>
        private void RaiseError(string message)
        {
            if (m_workQueue != null)
            {
                m_workQueue.ClearAndEnqueue(new WorkQueueElement(WorkQueueElement.Command.Close, null)); // No need to respond to anything, we are shutting down
            }

            if (m_playbackInfo != null)
            {
                m_playbackInfo.Detach();
            }

            Tracer.Trace(TraceChannel.Error, "Error raised: " + message);
            if (m_state != State.Closed)
            {
                ErrorOccurred(message);
            }

            m_state = State.Closed;
        }

        /// <summary>
        /// Raises an error to the media element. Shuts everything
        /// down and puts us in a closed state.
        /// </summary>
        /// <param name="e">exception message to raise</param>
        private void RaiseError(Exception e)
        {
            RaiseError(e.Message);
        }

        /// <summary>
        /// Fired when a download is complete
        /// </summary>
        /// <param name="sender">object that sent it to us</param>
        /// <param name="args">download complete evet args</param>
        private void ReportDownloadComplete(object sender, Downloader.DownloadCompleteEventArgs args)
        {
            MediaChunk chunk = args.Chunk;
            try
            {
                if (chunk.State == MediaChunk.ChunkState.Error)
                {
                    Tracer.Trace(TraceChannel.Error, "Media data chunk {0} ({2} kbps) was not loaded, chunk state is {1}.", chunk.Sid, chunk.State, chunk.Bitrate);
                }
                else if (chunk.State == MediaChunk.ChunkState.Loaded)
                {
                    m_workQueue.Enqueue(new WorkQueueElement(WorkQueueElement.Command.ParseChunk, chunk));
                    m_manifestInfo.GetStreamInfoForStreamType(chunk.MediaType).Queue.UpdateBufferSizes();
                }

                // Send this message to our heuristics module
                m_heuristics.OnDownloadCompleted(chunk.StreamId, chunk.ChunkId, chunk.Bitrate, chunk.DownloadStartTime, chunk.DownloadCompleteTime, chunk.Length);

                // Fire our public chunk download event for anyone listening
                if (MediaChunkDownloaded != null)
                {
                    MediaChunkDownloaded(this, new MediaChunkDownloadedEventArgs(chunk.StreamId, chunk.ChunkId, chunk.Bitrate));
                }
            }
            catch (AdaptiveStreamingException e)
            {
                RaiseError(e);
            }
            catch (Exception e)
            {
                RaiseError(e);
                throw;
            }
        }

        /// <summary>
        /// Starts a chunk download
        /// </summary>
        /// <param name="sender">the object that sent this event</param>
        /// <param name="args">event args for the download event</param>        
        private void StartChunkDownload(object sender, Heuristics.RequestDownloadEventArgs args)
        {
            StreamInfo streamInfo = m_manifestInfo.GetStreamInfoForStream(args.StreamId);
            FireOnDownloadBitrateChange(streamInfo.MediaType, args.Bitrate, DateTime.Now);
            MediaChunk chunk = streamInfo.Queue[args.ChunkId];
            if (chunk.Bitrate != args.Bitrate)
            {
                chunk.Bitrate = args.Bitrate;
            }

            Downloader.Start(streamInfo.BaseUrl, UrlGenerator, chunk, ReportDownloadComplete, m_heuristics.InstanceId);
        }

        /// <summary>
        /// Perform the actual chunk replacement through a call to ForceNextDownload
        /// </summary>
        private void DoChunkReplacementSuggested()
        {
            StreamInfo streamInfo = m_manifestInfo.GetStreamInfoForStreamType(MediaStreamType.Video);

            // It's only for video
            if (streamInfo == null)
            {
                // No media replacement for audio only media
                return;
            }

            m_heuristics.ForceNextDownload(streamInfo.StreamId, streamInfo.Queue.Prune(4 * OneSecondInHns));
        }

        /// <summary>
        /// This is our worker thread which dispatches all our commands
        /// </summary>
        private void WorkerThread()
        {
            try
            {
                while (true)
                {
                    m_workQueue.WaitForWorkItem();
                    WorkQueueElement elem = m_workQueue.Dequeue();

                    if (elem == null)
                    {
                        // Clear was called on queue before elem was retrieved: it's either Close or Error
                        continue;
                    }

                    DateTime enter = DateTime.Now;

                    switch (elem.CommandToPerform)
                    {
                        case WorkQueueElement.Command.Close:
                            // Abort/close means that we should just exit, this object should be discarded and never used again
                            DoClose();

                            // Terminate worker thread
                            return;

                        case WorkQueueElement.Command.Diagnostics:
                            DoDiagnostics((MediaStreamSourceDiagnosticKind)elem.CommandParameter);
                            break;

                        case WorkQueueElement.Command.Open:
                            // Open does not happens on worker thread, it's done on callback for manifest download to start loading new chunks
                            break;

                        case WorkQueueElement.Command.ParseChunk:
                            DoParseChunk((MediaChunk)elem.CommandParameter);
                            break;

                        case WorkQueueElement.Command.Sample:
                            if (m_state == State.Opened)
                            {
                                DoSample(elem);
                            }

                            break;

                        case WorkQueueElement.Command.Seek:
                            if (m_state == State.Opened)
                            {
                                DoSeek((long)elem.CommandParameter);
                            }

                            break;

                        case WorkQueueElement.Command.SwitchMedia:
                            // We ignore SwitchMedia command for now, just pretending that everything is fine
                            ReportSwitchMediaStreamCompleted((MediaStreamDescription)elem.CommandParameter);
                            break;

                        case WorkQueueElement.Command.ReplaceMedia:
                            DoChunkReplacementSuggested();
                            break;
                    }

                    DateTime exit = DateTime.Now;
                    if ((exit - enter).TotalSeconds > 20e-3)
                    {
                        Tracer.Trace(TraceChannel.Timing, "WorkerThread: long time: {0} cmd:{1}", (exit - enter).TotalSeconds, elem.CommandToPerform.ToString());
                    }
                }
            }
            catch (AdaptiveStreamingException e)
            {
                RaiseError(e);
            }
        }

        /// <summary>
        /// Event args for when a sample has been retrieved
        /// </summary>
        protected class GetSampleCompletedEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets the bitrate of this sample
            /// </summary>
            public ulong Bitrate { get; set; }

            /// <summary>
            /// Gets or sets the id of the chunk that this sample came from
            /// </summary>
            public int ChunkId { get; set; }

            /// <summary>
            /// Gets or sets the sample that was retrieved
            /// </summary>
            public MediaStreamSample Sample { get; set; }

            /// <summary>
            /// Gets or sets the stream that this sample came from
            /// </summary>
            public int StreamId { get; set; }
        }
    }
}
