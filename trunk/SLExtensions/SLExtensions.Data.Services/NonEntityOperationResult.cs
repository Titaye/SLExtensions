using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading;
using System.IO;
using System.Xml.Linq;

namespace SLExtensions.Data.Services
{
  internal class NonEntityOperationResult : IAsyncResult
  {
    ManualResetEvent asyncWait;
    bool completedSynchronously = true;
    internal bool userCompleted = false;
    readonly object source;
    HttpWebRequest request;
    AsyncCallback userCallback;
    object userState;
    internal Stream resultStream;
    Exception error;

    internal NonEntityOperationResult(object source, HttpWebRequest request, AsyncCallback callback, object state)
    {
      this.source = source;
      this.request = request;
      this.userCallback = callback;
      this.userState = state;
    }

    internal void BeginExecute()
    {
      // Start the response
      request.BeginGetResponse(new AsyncCallback(NonEntityOperationResult.AsyncEndGetResponse), this);
    }

    static void AsyncEndGetResponse(IAsyncResult asyncResult)
    {
      NonEntityOperationResult result = (NonEntityOperationResult)asyncResult.AsyncState;
      try
      {
        HttpWebResponse response = (HttpWebResponse)result.request.EndGetResponse(asyncResult);
        result.resultStream = response.GetResponseStream();

        // Set result state
        result.completedSynchronously = false;
      }
      catch (Exception ex)
      {
        // Let user know about exception condition
        result.error = ex;
        result.userCallback(result);
      }

      // Call the User
      result.userCallback(result);
    }


    #region IAsyncResult Members

    public object AsyncState
    {
      get { return userState; }
    }

    public System.Threading.WaitHandle AsyncWaitHandle
    {
      get
      {
        if (this.asyncWait == null)
        {
          Interlocked.CompareExchange<ManualResetEvent>(ref this.asyncWait, new ManualResetEvent(this.IsCompleted), null);
          if (this.IsCompleted)
          {
            this.asyncWait.Set();
          }
        }
        return this.asyncWait;
      }

    }

    public bool CompletedSynchronously
    {
      get { return completedSynchronously; }
    }

    public bool IsCompleted
    {
      get { return userCompleted; }
    }

    #endregion

    public Exception Error
    {
      get { return error; }
    }

  }
}
