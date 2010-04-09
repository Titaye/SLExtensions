namespace SLExtensions.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Xml;
    using System.Xml.Linq;

    public static class DataServiceContextExtensions
    {
        #region Methods

        /// <summary>
        /// Extension method to support retrieving Data Service Operations that support
        /// primitive values.  
        /// </summary>
        /// <typeparam name="T">The expected data type.</typeparam>
        /// <param name="ctx">The Type to be extended (DataServiceContext).</param>
        /// <param name="uri">The address of the operation (with parameters if used)</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">State to be communicated back to the user.</param>
        /// <returns>An IAsyncResult.</returns>
        public static IAsyncResult BeginExecuteNonEntityOperation<T>(this DataServiceContext ctx, Uri uri, AsyncCallback callback, object state)
        {
            // Since I can't figure out how to limit the type to primitives, i'll do it with a runtime check
              if (!typeof(T).IsPrimitive && typeof(T) != typeof(string))
              {
            throw new DataServiceClientException("BeginExecuteForPrimitives cannot be used with types that are not primatives or String type.");
              }

              // Convert the Uri to use the Context's URI Base
              uri = BuildUri(ctx.BaseUri, uri);

              // Build the request to make our specific request
              HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(uri);
              request.Method = "GET";
              request.Accept = "application/atom+xml,application/xml";
              //request.Headers[HttpRequestHeader.AcceptCharset] = "UTF-8";
              request.Headers["DataServiceVersion"] = "1.0;Silverlight";
              request.Headers["MaxDataServiceVersion"] = "1.0;Silverlight";

              NonEntityOperationResult result = new NonEntityOperationResult(ctx, request, callback, state);
              result.BeginExecute();

              return result;
        }

        /// <summary>
        /// Ends the execute non entity operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx">The DataServiceContext object.</param>
        /// <param name="asyncResult">The async result.</param>
        /// <returns>The results of the non-entity operation.</returns>
        public static IEnumerable<T> EndExecuteNonEntityOperation<T>(this DataServiceContext ctx, IAsyncResult asyncResult)
        {
            NonEntityOperationResult result = (NonEntityOperationResult)asyncResult;

              // Mark it as complete
              result.userCompleted = true;

              // If we got an error on background thread, throw it here.
              if (result.Error != null) throw result.Error;

              // Load the results as an XML Doc
              StreamReader rdr = new StreamReader(result.resultStream, true);
              XDocument doc = XDocument.Load(rdr);

              // Convert to a Enumerable list of values
              try
              {
            var qry = from x in doc.Root.Descendants()
                  select Convert.ChangeType(x.Value, typeof(T), null);

            return qry.Cast<T>();
              }
              catch (Exception ex)
              {
            throw new InvalidCastException("Could not cast results into expected type", ex);
              }
        }

        /// <summary>
        /// Resets the specified DataServiceContext object to remove all the entities.
        /// </summary>
        /// <param name="ctx">The DataServiceContext object.</param>
        public static void Reset(this DataServiceContext ctx)
        {
            foreach (LinkDescriptor link in ctx.Links) ctx.DetachLink(link.Source, link.SourceProperty, link.Target);
              foreach (EntityDescriptor entity in ctx.Entities) ctx.Detach(entity.Entity);
        }

        // Borrowed from System.Data.Services.Client.Util.CreateUri from MS Code
        internal static Uri BuildUri(Uri baseUri, Uri requestUri)
        {
            const char ForwardSlash = '/';

              if (!requestUri.IsAbsoluteUri)
              {
            if (baseUri.OriginalString.EndsWith("/", StringComparison.Ordinal))
            {
              if (requestUri.OriginalString.StartsWith("/", StringComparison.Ordinal))
              {
            requestUri = new Uri(baseUri, new Uri(requestUri.OriginalString.TrimStart(ForwardSlash), UriKind.Relative));
              }
              else
              {
            requestUri = new Uri(baseUri, requestUri);
              }
            }
            else
            {
              requestUri = new Uri(baseUri.OriginalString + "/" + requestUri.OriginalString.TrimStart(ForwardSlash), UriKind.Absolute);
            }
              }

              return requestUri;
        }

        #endregion Methods
    }
}