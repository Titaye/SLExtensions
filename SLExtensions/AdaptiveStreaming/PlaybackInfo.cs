//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace Microsoft.Expression.Encoder.AdaptiveStreaming
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities;

    /// <summary>
    /// This class wraps the Silverlight MediaElement control. We use it to report playback
    /// information like frames per second, dropped frames, etc. It is only used by the
    /// MediaStreamSource class, so it is private to it. We implement IDisposable
    /// because we create a Timer inside.
    /// </summary>
    public sealed class PlaybackInfo : IDisposable
    {
        /// <summary>
        /// Number of samples we will track
        /// </summary>
        internal const long DroppedFpsHistorySize = 8;

        /// <summary>
        /// The number of times we have refreshed the stats from the media element.
        /// Cumulative tics is also an index to the last sample stored,
        /// the counter of tics used would be that +1
        /// </summary>
        private long m_cumulativeTics = -1;

        /// <summary>
        /// History of frames we have dropped.
        /// </summary>
        private long[] m_droppedFpsHistory;

        /// <summary>
        /// The number of frames we are dropping per second
        /// </summary>
        private double m_droppedFramesPerSecond;

        /// <summary>
        /// Has the browser been minimized
        /// </summary>
        private bool m_isMinimizedBrowser;

        /// <summary>
        /// The media element we are tracking
        /// </summary>
        private MediaElement m_mediaElement;

        /// <summary>
        /// The current state of the media element we are tracking
        /// </summary>
        private MediaElementState m_mediaElementState = MediaElementState.Closed;

        /// <summary>
        /// The number of times we have been minimized
        /// </summary>
        private int m_minimizedCounter;

        /// <summary>
        /// An event handler which runs on the UI thread which refreshes the 
        /// stats from the media element
        /// </summary>
        private Action m_refreshFromUIThread;

        /// <summary>
        /// The number of frames we are rendering per second
        /// </summary>
        private double m_renderedFramesPerSecond;

        /// <summary>
        /// A timer which tracks the stats of the media element
        /// </summary>
        private Timer m_timer;

        /// <summary>
        /// Initializes a new instance of the PlaybackInfo class
        /// </summary>
        /// <param name="me">the MediaElement we are tracking</param>
        public PlaybackInfo(MediaElement me)
        {
            m_mediaElement = me;
            m_droppedFpsHistory = new long[DroppedFpsHistorySize];
            m_refreshFromUIThread = new Action(RefreshFromUIThread);
            me.CurrentStateChanged += new RoutedEventHandler(MediaElementCurrentStateChanged);
        }

        /// <summary>
        /// Gets the number of frames dropped per second
        /// </summary>
        public double DroppedFramesPerSecond
        {
            get
            {
                return m_droppedFramesPerSecond;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we are playing minimized
        /// </summary>
        public bool IsMinimizedBrowser
        {
            get
            {
                return m_isMinimizedBrowser;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the MediaElement is paused or stopped.
        /// </summary>
        public bool IsPausedOrStopped
        {
            get
            {
                return m_mediaElementState == MediaElementState.Paused || m_mediaElementState == MediaElementState.Stopped;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the MediaElement is currently in a playing state
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return m_mediaElementState == MediaElementState.Playing;
            }
        }

        /// <summary>
        /// Gets the number of frames rendered per second
        /// </summary>
        public double RenderedFramesPerSecond
        {
            get
            {
                return m_renderedFramesPerSecond;
            }
        }

        /// <summary>
        /// Gets the cumulative tics since we have started.
        /// </summary>
        public long CumulativeTics
        { 
            get 
            { 
                return m_cumulativeTics; 
            } 
        }

        /// <summary>
        /// Gets or sets the source frame rate.
        /// </summary>
        public double SourceFramesPerSecond
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the history of dropped frames.
        /// </summary>
        /// <returns>History of dropped frames</returns>
        public long[] GetDroppedFpsHistory()
        {
            return m_droppedFpsHistory;
        }

        /// <summary>
        /// Attach to the media element. Spawns a Timer which keeps pinging the media element.
        /// </summary>
        public void Attach()
        {
            if (m_timer != null)
            {
                m_timer.Dispose();
                m_timer = null;
            }

            m_timer = new Timer(new TimerCallback(Refresh), null, new TimeSpan(0, 0, 2), new TimeSpan(0, 0, 1));
        }

        /// <summary>
        /// Stop pinging the media element
        /// </summary>
        public void Detach()
        {
            Timer tm = m_timer;
            m_timer = null;

            if (tm != null)
            {
                tm.Dispose();
            }

            m_mediaElement = null;
        }

        /// <summary>
        /// Implements IDisposable.Dispose()
        /// </summary>
        public void Dispose()
        {
            Detach();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Delegate function which tracks the state of the media element
        /// </summary>
        /// <param name="sender">the object which sent this event</param>
        /// <param name="e">event args</param>
        private void MediaElementCurrentStateChanged(object sender, RoutedEventArgs e)
        {
            m_mediaElementState = (sender as MediaElement).CurrentState;
        }

        /// <summary>
        /// Schedule a refresh call on the UI thread
        /// </summary>
        /// <param name="notUsed">parameter is ignored</param>
        private void Refresh(object notUsed)
        {
            UIDispatcher.Schedule(m_refreshFromUIThread);
        }

        /// <summary>
        /// Query the MediaElement for its stats. This must be done on the UI thread.
        /// </summary>
        private void RefreshFromUIThread()
        {
            MediaElement me = m_mediaElement;
            if (me != null)
            {
                m_droppedFramesPerSecond = me.DroppedFramesPerSecond;
                m_renderedFramesPerSecond = me.RenderedFramesPerSecond;

                long dfps = (long)m_droppedFramesPerSecond;
                long rfps = (long)m_renderedFramesPerSecond;

                long displayTic;

                // CPU monitoring heuristic code
                if (rfps == 0 && dfps >= 15)
                {
                    m_minimizedCounter++;
                }
                else
                {
                    m_minimizedCounter = 0;
                }

                if (m_minimizedCounter >= 2)
                {
                    // Browser is minimized, don't change counters
                    m_isMinimizedBrowser = true;
                    displayTic = -1;
                }
                else
                {
                    m_isMinimizedBrowser = false;

                    m_cumulativeTics++;
                    m_droppedFpsHistory[m_cumulativeTics % DroppedFpsHistorySize] = dfps;
                    displayTic = m_cumulativeTics;
                }

                // A tic == -1 means it was not stored to be used by
                // frame rate heuristics
                MS.Internal.Expression.Encoder.AdaptiveStreaming.Heuristic.HeuristicsImpl.NhTrace(
                    "FRMO",
                    "FPS tic:{0} rendered:{1} dropped:{2} minimized:{3}",
                    displayTic,
                    rfps,
                    dfps,
                    m_isMinimizedBrowser ? 1 : 0);
            }
        }
    }
}
