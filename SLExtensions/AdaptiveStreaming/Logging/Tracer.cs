//*@@@*************************************************************************
//
// Microsoft Expression Encoder
// Copyright © Microsoft Corporation. All rights reserved.
//
//*@@@*************************************************************************
namespace MS.Internal.Expression.Encoder.AdaptiveStreaming.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using Microsoft.Expression.Encoder.AdaptiveStreaming;
    using MS.Internal.Expression.Encoder.AdaptiveStreaming.Utilities;

    /// <summary>
    /// The channel that a trace message belongs to
    /// </summary>
    internal enum TraceChannel
    {
        /// <summary>
        /// Error level messages, pass always
        /// </summary>
        Error,

        /// <summary>
        /// Library level messsages, pass always
        /// </summary>
        MSS,

        /// <summary>
        /// Download related
        /// </summary>
        Download,

        /// <summary>
        /// More verbose download
        /// </summary>
        DownloadVerbose,

        /// <summary>
        /// Seeking messages
        /// </summary>
        Seek,

        /// <summary>
        /// Seeking position messages
        /// </summary>
        SeekPos,

        /// <summary>
        /// Network heuristics related
        /// </summary>
        NetHeur,

        /// <summary>
        /// Ad insertion API
        /// </summary>
        AdsInsert,

        /// <summary>
        /// Checking for operations taking longer time than they should
        /// </summary>
        Timing,

        /// <summary>
        /// Bandwidth statistics reporting messages
        /// </summary>
        Stats,

        /// <summary>
        /// Quality monitoring API
        /// </summary>
        QualityMonitor,

        /// <summary>
        /// XML configuration
        /// </summary>
        Config,

        /// <summary>
        /// Targets download url only
        /// </summary>
        DownloadUrl
    }

    /// <summary>
    /// This class contains utilities to provide tracing and debugging routines
    /// </summary>
    internal class Tracer
    {
        /// <summary>
        /// The destination our trace calls go to
        /// </summary>
        private static TraceDestination sm_Destination = TraceDestination.Memory;

        /// <summary>
        /// Our active tracing channels
        /// </summary>
        private static List<TraceChannel> sm_ActiveChannels = new List<TraceChannel>(10);

        /// <summary>
        /// The intial size of the trace log. Use that much at the beginning of playback, 
        /// if Memory or Browser is the trace desctination
        /// </summary>
        private static int sm_InitMemoryCapacity = 100 * 1024;

        /// <summary>
        /// The max size of the trace log.
        /// </summary>
        private static int sm_MaxMemoryCapacity = 1024 * 1024;

        /// <summary>
        /// Our in memory trace log. Start with 100Kb, do not exceed 1Mb for trace
        /// </summary>
        private static StringBuilder sm_Trace = new StringBuilder(sm_InitMemoryCapacity, sm_MaxMemoryCapacity);

        /// <summary>
        /// Initializes static members of the Tracer class.
        /// </summary>
        static Tracer()
        {
            Tracer.SetDestination(Tracer.TraceDestination.Debug);
            Tracer.ActivateChannel(TraceChannel.Error);
            Tracer.ActivateChannel(TraceChannel.MSS);
            Tracer.ActivateChannel(TraceChannel.NetHeur);
        }

        /// <summary>
        /// Prevents a default instance of the Tracer class from being created
        /// </summary>
        private Tracer()
        {
        }

        /// <summary>
        /// Enumeration of the possible destination for the tracing data
        /// </summary>
        public enum TraceDestination
        {
            /// <summary>
            /// Output to a memory string
            /// </summary>
            Memory,

            /// <summary>
            /// Output to the console
            /// </summary>
            Console,

            /// <summary>
            /// Output to the debug classes
            /// </summary>
            Debug,

            /// <summary>
            /// Output to the browser
            /// </summary>
            Browser
        }

        /// <summary>
        /// Gets or sets the maximum memory capacity of our log
        /// </summary>
        public static int MaxMemoryCapacity
        {
            get
            {
                return sm_MaxMemoryCapacity;
            }

            set
            {
                // Don't let set it to anything less than 10K
                if (value < 1024 * 10)
                {
                    return;
                }

                Trace(TraceChannel.Error, "Changing in memory trace buffer capacity from {0} to {1}.", sm_Trace.Capacity, value);
                lock (sm_Trace)
                {
                    sm_MaxMemoryCapacity = value;
                    if (sm_MaxMemoryCapacity < sm_InitMemoryCapacity)
                    {
                        sm_InitMemoryCapacity = sm_MaxMemoryCapacity;
                    }

                    StringBuilder old = sm_Trace;
                    sm_Trace = new StringBuilder(sm_InitMemoryCapacity, sm_MaxMemoryCapacity);

                    if (old.Length >= sm_MaxMemoryCapacity)
                    {
                        Debug.WriteLine(old.ToString());
                        Trace(TraceChannel.Error, "Changing in memory trace buffer resulted in trace flush to debug output.");
                    }
                    else
                    {
                        sm_Trace.Append(old.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Gets the current tracing text
        /// </summary>
        public static string Text
        {
            get
            {
                string res; 
                
                lock (sm_Trace)
                {
                    res = sm_Trace.ToString();
                }

                return res;
            }
        }

        /// <summary>
        /// Asserts a particular condition
        /// </summary>
        /// <param name="condition">condition to assert</param>
        public static void Assert(bool condition)
        {
            if (!condition)
            {
                Trace(TraceChannel.Error, "Assert failure!");
#if DEBUG
                    throw new AdaptiveStreamingException("Assert failed");
#endif
            }
        }

        /// <summary>
        /// Asserts a particular condition
        /// </summary>
        /// <param name="condition">condition to assert</param>
        /// <param name="message">message to assert</param>
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                Trace(TraceChannel.Error, "Assert failure! " + message);
#if DEBUG
                    throw new AdaptiveStreamingException("Assert failed:" + message);
#endif
            }
        }

        /// <summary>
        /// Trace a message
        /// </summary>
        /// <param name="channel">channel to send message to</param>
        /// <param name="line">message to trace</param>
        public static void Trace(TraceChannel channel, string line)
        {
            try
            {
                bool isActive = sm_ActiveChannels.Contains(channel);
                if (isActive)
                {
                    switch (sm_Destination)
                    {
                        case TraceDestination.Memory:
                            Append(line);
                            break;
                        case TraceDestination.Console:
                            Console.WriteLine(line);
                            break;
                        case TraceDestination.Debug:
                            Debug.WriteLine(line);
                            break;
                        case TraceDestination.Browser:
                            Append(line + "<br>");
                            break;
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // Exceeded maximum memory trace capacity
                ResetMemory();
            }
        }

        /// <summary>
        /// Format a message and trace
        /// </summary>
        /// <param name="channel">channel to send to</param>
        /// <param name="format">format string</param>
        /// <param name="list">format string parameters</param>
        public static void Trace(TraceChannel channel, string format, params object[] list)
        {
            string line = String.Format(CultureInfo.InvariantCulture, format, list);
            Trace(channel, line);
        }

        /// <summary>
        /// Activates the given channel
        /// </summary>
        /// <param name="channel">channel to activate</param>
        private static void ActivateChannel(TraceChannel channel)
        {
            sm_ActiveChannels.Add(channel);
        }

        /// <summary>
        /// Appends a new message to the trace log
        /// </summary>
        /// <param name="line">line to append</param>
        private static void Append(string line)
        {
            lock (sm_Trace)
            {
                if (sm_Trace.Length + line.Length >= sm_MaxMemoryCapacity)
                {
                    ResetMemory();
                }

                sm_Trace.AppendLine(line);
            }
        }

        /// <summary>
        /// Resets our tracing memory
        /// </summary>
        private static void ResetMemory()
        {
            Debug.WriteLine(sm_Trace.ToString());
            sm_Trace.Remove(0, sm_Trace.Length); // No sense to cut back, it would be just one more memory allocation
        }

        /// <summary>
        /// Set's the tracing destination
        /// </summary>
        /// <param name="dest">trace destination</param>
        private static void SetDestination(TraceDestination dest)
        {
            sm_Destination = dest;
        }
    }
}
