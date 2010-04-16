//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Expression.Encoder.AdaptiveStreaming;

    /// <summary>
    /// This is a utility class for sending delegates to the UI thread. We store the
    /// root Dispatcher on class load (enforced from the UI thread, unless the MSS
    /// is created on a background thread, which would be bad, and quite likely dumb).
    /// </summary>
    internal class UIDispatcher
    {
        /// <summary>
        /// Stores the root visual
        /// </summary>
        private static System.Windows.Threading.Dispatcher sm_Dispatcher = null;

        /// <summary>
        /// Initializes static members of the UIDispatcher class. Stores the root dispatcher.
        /// </summary>
        static UIDispatcher()
        {
            sm_Dispatcher = Application.Current.RootVisual.Dispatcher;
        }

        /// <summary>
        /// Prevents a default instance of the UIDispatcher class from being created
        /// </summary>
        private UIDispatcher()
        {
        }

        /// <summary>
        /// Enforce calling of the static constructor
        /// </summary>
        public static void Load()
        {
            // Used to enforce the first use of Environment class from UI thread
        }

        /// <summary>
        /// Schedule a new action
        /// </summary>
        /// <param name="callback">action to schedule</param>
        public static void Schedule(Action callback)
        {
            sm_Dispatcher.BeginInvoke(callback);
        }

        /// <summary>
        /// Schedule a new download paused delegate
        /// </summary>
        /// <param name="callback">callback to schedule</param>
        /// <param name="sender">object that is sending this message</param>
        /// <param name="time">time of the callback</param>
        public static void Schedule(EventHandler<DownloadPausedEventArgs> callback, object sender, TimeSpan time)
        {
            sm_Dispatcher.BeginInvoke(callback, new object[] { sender, new DownloadPausedEventArgs(time) });
        }

        /// <summary>
        /// Schedule a new bitrate change callback
        /// </summary>
        /// <param name="callback">callback to schedule</param>
        /// <param name="sender">object that is sending this message</param>
        /// <param name="streamType">type of the stream</param>
        /// <param name="kbps">bitrate, in kbps</param>
        /// <param name="timestamp">timestamp of the change</param>
        public static void Schedule(EventHandler<BitrateChangedEventArgs> callback, object sender, MediaStreamType streamType, ulong kbps, DateTime timestamp)
        {
            sm_Dispatcher.BeginInvoke(callback, new object[] { sender, new BitrateChangedEventArgs(streamType, kbps, timestamp) });
        }
    }
}
