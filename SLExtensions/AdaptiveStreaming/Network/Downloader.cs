//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Network
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Windows.Media;
    using Microsoft.Expression.Encoder.AdaptiveStreaming;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Logging;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Url;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities;

    /// <summary>
    /// This class handles all of our chunk downloading logic. You can start a download,
    /// stop a download, cancel all downloads, etc.
    /// </summary>
    internal class Downloader
    {
        /// <summary>
        /// The number of packets which we force to always bypass any caching
        /// </summary>
        public const int PacketPairPacketCount = 2;

        /// <summary>
        /// A table of all downloads we are handling. Hashed on InstanceId.
        /// </summary>
        private static Dictionary<Guid, List<Downloader>> sm_allDownloads = new Dictionary<Guid, List<Downloader>>(MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities.Configuration.Streams.MaxStreams);

        /// <summary>
        /// The queue of pending chunks to download.
        /// </summary>
        private static Queue<Downloader> sm_downloadQueue = new Queue<Downloader>(MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities.Configuration.Streams.MaxDownloadQueue);

        /// <summary>
        /// The chunk we are downloading
        /// </summary>
        private MediaChunk m_chunk;

        /// <summary>
        /// The event handler which is called when a download is complete
        /// </summary>
        private EventHandler<DownloadCompleteEventArgs> m_downloadCompleteEventHandler;

        /// <summary>
        /// The HttpWebRequest we are using to handle this download
        /// </summary>
        private HttpWebRequest m_httpWebRequest;

        /// <summary>
        /// A unique identifier for this download
        /// </summary>
        private Guid m_instanceId;

        /// <summary>
        /// A flag which track whether we have been aborted or not
        /// </summary>
        private bool m_isAborted;

        /// <summary>
        /// A flag which tracks if we have already been used. Used in the
        /// HttpWebRequest callback.
        /// </summary>
        private bool m_isUsed;

        /// <summary>
        /// Internal variable we use to track how many times we have retried the download
        /// before bailing
        /// </summary>
        private int m_retryCount;

        /// <summary>
        /// The Url of the file we are downloading. This is the full Url, after it has
        /// been obfuscated and filled out (if obfuscation exists)
        /// </summary>
        private string m_url;

        /// <summary>
        /// Initializes a new instance of the Downloader class
        /// </summary>
        /// <param name="baseUrl">the base url of the file to download</param>
        /// <param name="urlGenerator">the module that generates our url</param>
        /// <param name="mediaChunk">the chunk we are trying to download</param>
        /// <param name="completeCallback">the event handler to call when we are complete</param>
        /// <param name="instance">A unique id to group downloads. Can be used to group audio and video downloads separately</param>
        private Downloader(
            string baseUrl,
            IUrlGenerator urlGenerator,
            MediaChunk mediaChunk,
            EventHandler<DownloadCompleteEventArgs> completeCallback,
            Guid instance)
        {
            m_instanceId = instance;
            m_chunk = mediaChunk;

            m_url = urlGenerator.GenerateUrlStringForChunk(baseUrl, mediaChunk.StreamId, mediaChunk.ChunkId, mediaChunk.Bitrate, mediaChunk.StartTime, (long)mediaChunk.Duration);
            m_url = m_url.Replace('\\', '/');

            // Assumption: ChunkId always start from 0
            // Add random URL modifier for the first 2 video chunks to make sure they are not in cache.
            // We use the first two chunks to get reliable bandwidth estimates.
            if (mediaChunk.ChunkId < PacketPairPacketCount && mediaChunk.MediaType == MediaStreamType.Video)
            {
                m_url = m_url + "?packetpair=" + DateTime.Now.Ticks;
            }

            if (mediaChunk.Url != m_url)
            {
                mediaChunk.Url = m_url;
            }

            m_downloadCompleteEventHandler = completeCallback;

            // Add this download object to our table of objects
            lock (sm_allDownloads)
            {
                if (!sm_allDownloads.ContainsKey(m_instanceId))
                {
                    sm_allDownloads.Add(m_instanceId, new List<Downloader>(4));
                }

                List<Downloader> allMyDownloads = sm_allDownloads[m_instanceId];
                allMyDownloads.Add(this);
            }
        }

        /// <summary>
        /// Cancel all downloads for the given instance.
        /// </summary>
        /// <param name="instanceId">the instance to cancel</param>
        public static void CancelAllDownloads(Guid instanceId)
        {
            lock (sm_allDownloads)
            {
                if (sm_allDownloads.ContainsKey(instanceId))
                {
                    List<Downloader> allMyDownloads = sm_allDownloads[instanceId];

                    foreach (Downloader dl in allMyDownloads)
                    {
                        dl.CancelDownload();
                    }

                    allMyDownloads.Clear();
                }
            }
        }

        /// <summary>
        /// Downloads a new chunk
        /// </summary>
        /// <param name="baseUrl">the base url of the file to download</param>
        /// <param name="urlGenerator">the class that will generator the url for this chunk</param>
        /// <param name="chunk">the chunk we are trying to download</param>
        /// <param name="completeCallback">the event handler to call when we are complete</param>
        /// <param name="instance">A unique id to group downloads. Can be used to group audio and video downloads separately</param>
        public static void Start(
            string baseUrl, 
            IUrlGenerator urlGenerator, 
            MediaChunk chunk,
            EventHandler<DownloadCompleteEventArgs> completeCallback, 
            Guid instance)
        {
            Tracer.Assert(chunk.Bitrate > 0, String.Format(CultureInfo.InvariantCulture, "Cannot load chunk {0} with zero bitrate.", chunk.Sid));
            Downloader downloader = new Downloader(baseUrl, urlGenerator, chunk, completeCallback, instance);
            downloader.StartDownload();
        }

        /// <summary>
        /// Cancels the current download associated with this object.
        /// </summary>
        public void CancelDownload()
        {
            bool alreadyAborted;
            lock (m_chunk)
            {
                alreadyAborted = m_isAborted;
                m_isAborted = true;
                if (m_chunk.Downloader == this)
                {
                    m_chunk.Downloader = null;
                    m_chunk.State = MediaChunk.ChunkState.Pending;
                }
            }

            // Notify the web request that we are aborting
            if (m_httpWebRequest != null && !alreadyAborted)
            {
                UIDispatcher.Schedule(new Action(m_httpWebRequest.Abort));
            }
            else
            {
                // If this guy had not been started yet, then we still need
                // to fire the download completed event otherwise the heuristics
                // module will still think we are downloading it, thus hitting
                // our limit on simultaneous downloads
                if (m_downloadCompleteEventHandler != null && !alreadyAborted)
                {
                    m_downloadCompleteEventHandler(this, new DownloadCompleteEventArgs(m_chunk));
                }
            }
        }

        /// <summary>
        /// Start any queued downloads. This should be called only from UI thread, never from anywhere else.
        /// </summary>
        private static void StartPendingDownloads()
        {
            try
            {
                Downloader dl;
                do
                {
                    dl = null;

                    // Must keep this lock very fast, because media serving thread may be delayed by it
                    lock (sm_downloadQueue)
                    {
                        if (sm_downloadQueue.Count > 0)
                        {
                            dl = sm_downloadQueue.Dequeue();
                        }
                    }

                    if (dl != null)
                    {
                        dl.StartDownloadForReal();
                    }
                }
                while (dl != null);
            }
            catch (Exception e)
            {
                Tracer.Trace(TraceChannel.Error, "Exception in StartPendingDownloads: {0}", e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Delegate we use for the HttpWebRequest read callback
        /// </summary>
        /// <param name="result">the request result</param>
        private void HttpWebRequestReadCallback(IAsyncResult result)
        {
            try
            {
                // We expect exactly one callback when the stream is available
                if (m_isUsed)
                {
                    Tracer.Trace(TraceChannel.Error, "Multiple read callbacks from HttpWebRequest object for chunk {0}", m_chunk.Sid);
                    return;
                }

                m_isUsed = true;

                lock (sm_allDownloads)
                {
                    if (sm_allDownloads.ContainsKey(m_instanceId))
                    {
                        List<Downloader> allMyDownloads = sm_allDownloads[m_instanceId];
                        if (allMyDownloads.Contains(this))
                        {
                            allMyDownloads.Remove(this);
                        }
                    }
                }

                Stream stream = null;
                HttpWebResponse clientResponse = null;
                string track = m_isAborted ? "(aborted)" : "(no response or not completed)";
                if (m_httpWebRequest.HaveResponse && result.IsCompleted && !m_isAborted)
                {
                    track = "(problem)";
                    try
                    {
                        clientResponse = (HttpWebResponse)m_httpWebRequest.EndGetResponse(result);
                        if (clientResponse.StatusCode == HttpStatusCode.OK)
                        {
                            stream = clientResponse.GetResponseStream();
                            track = "(ok)";
                        }
                        else
                        {
                            m_chunk.ErrorMessage = String.Format(
                                CultureInfo.InvariantCulture,
                                "{2} download failure @ {4} with code {0}: {1} ({3})",
                                clientResponse.StatusCode,
                                clientResponse.StatusDescription,
                                m_chunk.Sid,
                                m_url,
                                DateTime.Now.ToUniversalTime().ToString());
                            Tracer.Trace(TraceChannel.Error, m_chunk.ErrorMessage);
                        }
                    }
                    catch (WebException e)
                    {
                        clientResponse = null;
                        stream = null;
                        m_chunk.ErrorMessage = String.Format(CultureInfo.InvariantCulture, "{0} SL download race, chunk was not loaded ({1})", m_chunk.Sid, e.ToString());
                        Tracer.Trace(TraceChannel.Error, m_chunk.ErrorMessage);
                    }
                    catch (ProtocolViolationException e)
                    {
                        clientResponse = null;
                        stream = null;
                        m_chunk.ErrorMessage = String.Format(CultureInfo.InvariantCulture, "{0} SL download race, chunk was not loaded ({1})", m_chunk.Sid, e.ToString());
                        Tracer.Trace(TraceChannel.Error, m_chunk.ErrorMessage);
                    }
                    catch (ObjectDisposedException e)
                    {
                        clientResponse = null;
                        stream = null;
                        m_chunk.ErrorMessage = String.Format(CultureInfo.InvariantCulture, "{0} SL download race, chunk was not loaded ({1})", m_chunk.Sid, e.ToString());
                        Tracer.Trace(TraceChannel.Error, m_chunk.ErrorMessage);
                    }
                    catch (ArgumentNullException e)
                    {
                        clientResponse = null;
                        stream = null;
                        m_chunk.ErrorMessage = String.Format(CultureInfo.InvariantCulture, "{0} SL download race, chunk was not loaded ({1})", m_chunk.Sid, e.ToString());
                        Tracer.Trace(TraceChannel.Error, m_chunk.ErrorMessage);
                    }
                    catch (InvalidOperationException e)
                    {
                        clientResponse = null;
                        stream = null;
                        m_chunk.ErrorMessage = String.Format(CultureInfo.InvariantCulture, "{0} SL download race, chunk was not loaded ({1})", m_chunk.Sid, e.ToString());
                        Tracer.Trace(TraceChannel.Error, m_chunk.ErrorMessage);
                    }
                    catch (ArgumentException e)
                    {
                        clientResponse = null;
                        stream = null;
                        m_chunk.ErrorMessage = String.Format(CultureInfo.InvariantCulture, "{0} SL download race, chunk was not loaded ({1})", m_chunk.Sid, e.ToString());
                        Tracer.Trace(TraceChannel.Error, m_chunk.ErrorMessage);
                    }
                    catch (Exception e)
                    {
                        clientResponse = null;
                        stream = null;
                        m_chunk.ErrorMessage = String.Format(CultureInfo.InvariantCulture, "{0} SL download race, chunk was not loaded ({1})", m_chunk.Sid, e.ToString());
                        Tracer.Trace(TraceChannel.Error, m_chunk.ErrorMessage);

                        // An rethrow because we do not know what we have
                        throw;
                    }
                }

                Tracer.Trace(TraceChannel.DownloadVerbose, "{0}- loaded {1}", m_chunk.Sid, track);

                if (stream != null)
                {
                    lock (m_chunk)
                    {
                        if (m_chunk.Downloader == this && !m_isAborted)
                        {
                            m_chunk.DownloadedPiece = stream;
                            m_chunk.Downloader = null;
                            m_chunk.Length = stream.Length;
                            m_chunk.DownloadCompleteTime = DateTime.Now;
                            m_chunk.State = MediaChunk.ChunkState.Loaded;
                            m_chunk.DurationLeft = m_chunk.Duration;
                        }
                        else
                        {
                            stream.Close();
                        }
                    }
                }
                else
                {
                    lock (m_chunk)
                    {
                        if (m_chunk.Downloader == this)
                        {
                            // Otherwise chunk.Downloader must be null, if not, we missed a race
                            Tracer.Assert(!m_isAborted);
                            m_chunk.ErrorMessage = String.Format(
                                CultureInfo.InvariantCulture, 
                                "{0} {2} download ERROR {3} {4}, state {1}, bitrate {5}, url {6}",
                                m_chunk.Sid, 
                                m_chunk.State, 
                                DateTime.Now.ToUniversalTime().ToString(),
                                clientResponse != null ? clientResponse.StatusCode.ToString() : "?",
                                clientResponse != null ? clientResponse.StatusDescription : string.Empty, 
                                m_chunk.Bitrate, 
                                m_url);
                            Tracer.Trace(TraceChannel.Error, m_chunk.ErrorMessage);

                            // Try once more
                            if (++m_retryCount < 2)
                            {
                                m_isUsed = false;
                                Tracer.Trace(TraceChannel.DownloadVerbose, "{0} Retrying {1} time", m_chunk.Sid, m_retryCount);
                                Utilities.UIDispatcher.Schedule(new Action(StartDownloadForReal));
                                return; // Don't complete call, try again
                            }

                            m_chunk.Downloader = null;
                            m_chunk.State = MediaChunk.ChunkState.Error;
                        }
                    }
                }

                if (m_downloadCompleteEventHandler != null)
                {
                    m_downloadCompleteEventHandler(this, new DownloadCompleteEventArgs(m_chunk));
                }
            }
            catch (Exception e)
            {
                Tracer.Trace(TraceChannel.Error, "Exception in request_ReadCallback: {0}", e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Start a new download. Just enqueues the download request to be 
        /// called when possibility opens
        /// </summary>
        private void StartDownload()
        {
            bool proceed = false;
            lock (m_chunk)
            {
                proceed = m_chunk.Downloader == null;
                if (proceed)
                {
                    m_chunk.Downloader = this;
                }
            }

            if (proceed)
            {
                lock (sm_downloadQueue)
                {
                    sm_downloadQueue.Enqueue(this);
                }

                Utilities.UIDispatcher.Schedule(new Action(StartPendingDownloads));
            }
        }

        /// <summary>
        /// Perform the actual download
        /// </summary>
        private void StartDownloadForReal()
        {
            DateTime enter = DateTime.Now;
            Tracer.Trace(TraceChannel.DownloadUrl, "Downloading: " + m_url);
            
            try
            {
                // If our chunk has been aborted, then do not start the download. Chunk abort
                // status is set under the sm_allDownloads lock, so we need to check it under that as well
                lock (sm_allDownloads)
                {
                    if (m_isAborted)
                    {
                        // No need to flag completed since we have already done it
                        return;
                    }
                }

                m_chunk.DownloadStartTime = DateTime.Now;
                m_httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(new Uri(m_url, UriKind.Absolute));
                m_httpWebRequest.BeginGetResponse(new AsyncCallback(HttpWebRequestReadCallback), null);
                Tracer.Trace(TraceChannel.DownloadVerbose, "{0}+", m_chunk.Sid);
            }
            catch (InvalidOperationException e)
            {
                m_chunk.State = MediaChunk.ChunkState.Error;
                Tracer.Trace(TraceChannel.Error, "{0}* cannot start downloading because of {1}", m_chunk.Sid, e.ToString());
            }
            catch (NotSupportedException e)
            {
                m_chunk.State = MediaChunk.ChunkState.Error;
                Tracer.Trace(TraceChannel.Error, "{0}* cannot start downloading because of {1}", m_chunk.Sid, e.ToString());
            }
            catch (Exception e)
            {
                m_chunk.State = MediaChunk.ChunkState.Error;
                Tracer.Trace(TraceChannel.Error, "{0}* cannot start downloading because of {1}", m_chunk.Sid, e.ToString());

                // We have to rethrow this error here, even though there is nobody to catch it,
                // because we do not know what it is.
                throw;
            }

            DateTime exit = DateTime.Now;
            if ((exit - enter).TotalSeconds > 20e-3)
            {
                Tracer.Trace(TraceChannel.Timing, "{0} StartDownloadForReal: long time: {1}", m_chunk.Sid, (exit - enter).TotalSeconds);
            }
        }

        /// <summary>
        /// Event handler arguments for the Download Complete event
        /// </summary>
        internal class DownloadCompleteEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the DownloadCompleteEventArgs class
            /// </summary>
            /// <param name="chunk">the media chunk we are downloading</param>
            public DownloadCompleteEventArgs(MediaChunk chunk)
            {
                Chunk = chunk;
            }

            /// <summary>
            /// Gets or sets the MediaChunk we are downloading
            /// </summary>
            public MediaChunk Chunk { get; set; }
        }
    }
}
