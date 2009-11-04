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
    /// Contains the WorkQueueElement private to the AdaptiveStreamingSource class
    /// </summary>
    public partial class AdaptiveStreamingSource : System.Windows.Media.MediaStreamSource
    {
        /// <summary>
        /// An individual element stored in our work queue. Describes the type of work
        /// to perform.
        /// </summary>
        private class WorkQueueElement
        {
            /// <summary>
            /// The command we are performing
            /// </summary>
            private Command m_commandToPerform;

            /// <summary>
            /// A command specific parameter
            /// </summary>
            private object m_commandParameter;

            /// <summary>
            /// Initializes a new instance of the WorkQueueElement class
            /// </summary>
            /// <param name="cmd">the command to perform</param>
            /// <param name="prm">parameter for the command</param>
            public WorkQueueElement(Command cmd, object prm)
            {
                m_commandToPerform = cmd;
                m_commandParameter = prm;
            }

            /// <summary>
            /// The type of work to perform
            /// </summary>
            public enum Command
            {
                /// <summary>
                /// Closes the media stream source
                /// </summary>
                Close,

                /// <summary>
                /// Report diagnostics back to the media element
                /// </summary>
                Diagnostics,

                /// <summary>
                /// Open a new manifest
                /// </summary>
                Open,

                /// <summary>
                /// Parse a chunk that we have received
                /// </summary>
                ParseChunk,

                /// <summary>
                /// Pause the media stream
                /// </summary>
                Pause,

                /// <summary>
                /// Handle a new sample request
                /// </summary>
                Sample,

                /// <summary>
                /// Perform a seek
                /// </summary>
                Seek,

                /// <summary>
                /// Stop the media stream
                /// </summary>
                Stop,

                /// <summary>
                /// Switch to a different media stream
                /// </summary>
                SwitchMedia,

                /// <summary>
                /// A chunk replacement suggested
                /// </summary>
                ReplaceMedia
            }

            /// <summary>
            /// Gets the command we are performing
            /// </summary>
            public Command CommandToPerform
            {
                get
                {
                    return m_commandToPerform;
                }
            }

            /// <summary>
            /// Gets a command specific parameter
            /// </summary>
            public object CommandParameter
            {
                get
                {
                    return m_commandParameter;
                }
            }
        }
    }
}
