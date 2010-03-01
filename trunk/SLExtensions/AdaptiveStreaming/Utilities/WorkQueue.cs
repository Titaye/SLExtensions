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
    using System.Threading;

    /// <summary>
    /// Contains the private WorkQueue class used in the AdaptiveStreamingSource
    /// </summary>
    public partial class AdaptiveStreamingSource : System.Windows.Media.MediaStreamSource
    {
        /// <summary>
        /// This class defines a queue of events to be executed. It is only used in the main media stream
        /// source, hence we make it a private class to it. We can pull it out later if this
        /// needs to be used by other classes.
        /// </summary>
        private class WorkQueue : IDisposable
        {
            /// <summary>
            /// Our queue of work items
            /// </summary>
            private Queue<WorkQueueElement> m_queue;

            /// <summary>
            /// An event which fires whenever the queue has items in it
            /// (or rather, when the queue goes from empty to non-empty)
            /// </summary>
            private ManualResetEvent m_queueHasItemsEvent;

            /// <summary>
            /// Initializes a new instance of the WorkQueue class
            /// </summary>
            public WorkQueue()
            {
                m_queueHasItemsEvent = new ManualResetEvent(false);
                m_queue = new Queue<WorkQueueElement>();
            }

            /// <summary>
            /// Implements IDisposable.Dispose()
            /// </summary>
            public void Dispose()
            {
                if (m_queueHasItemsEvent != null)
                {
                    m_queueHasItemsEvent.Close();
                }

                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Enqueue a new work item
            /// </summary>
            /// <param name="elem">the item to add</param>
            public void Enqueue(WorkQueueElement elem)
            {
                lock (m_queue)
                {
                    m_queue.Enqueue(elem);
                    if (1 == m_queue.Count)
                    {
                        m_queueHasItemsEvent.Set();
                    }
                }
            }

            /// <summary>
            /// Remove and return an item from the queue
            /// </summary>
            /// <returns>next item from the queue</returns>
            public WorkQueueElement Dequeue()
            {
                WorkQueueElement elem = null;
                lock (m_queue)
                {
                    if (0 != m_queue.Count)
                    {
                        elem = m_queue.Dequeue();
                        if (0 == m_queue.Count)
                        {
                            m_queueHasItemsEvent.Reset();
                        }
                    }
                }

                return elem;
            }

            /// <summary>
            /// Clear the queue and add 1 item in the same operation. This is useful
            /// for operation that take precedence over all others (like closing and errors)
            /// </summary>
            /// <param name="elem">New item to add</param>
            public void ClearAndEnqueue(WorkQueueElement elem)
            {
                lock (m_queue)
                {
                    m_queue.Clear();
                    m_queue.Enqueue(elem);
                    m_queueHasItemsEvent.Set();
                }
            }

            /// <summary>
            /// Wait until the queue has an item in it
            /// </summary>
            public void WaitForWorkItem()
            {
                m_queueHasItemsEvent.WaitOne();
            }
        }
    }
}
