// <copyright file="SLExtension.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Method extensions for SLExtension framework
    /// </summary>
    public static class SLExtension
    {
        #region Methods

        public static T FirstVisualAncestorOfType<T>(this FrameworkElement element)
            where T : FrameworkElement
        {
            var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
            while (parent != null)
            {
                if (parent is T)
                    return (T)parent;
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }
            return null;
        }

        /// <summary>
        /// Gets a cookie from its name.
        /// </summary>
        /// <param name="document">The <c>HtmlDocument</c>.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        /// <returns>The cookie content</returns>
        public static string GetCookie(this HtmlDocument document, string cookieName)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(Application.Current.RootVisual))
            {
                return string.Empty;
            }

            // Get cookie login token
            string cookies = document.Cookies;
            int idx = cookies.IndexOf(cookieName + "=");
            string val = "";
            if (idx == 0 || (idx > 0 && (cookies[idx - 1] == ';' || cookies[idx - 1] == ' ')))
            {
                int lastIdx = cookies.IndexOf(';', idx);
                if (lastIdx == -1)
                {
                    lastIdx = cookies.Length;
                }

                int start = idx + cookieName.Length + 1;
                val = cookies.Substring(start, lastIdx - start);
                return HttpUtility.UrlDecode(val);
            }
            else
            {
                return string.Empty;
            }
        }

        public static int IndexOf<TSource>(this IEnumerable<TSource> source, TSource item)
        {
            int i = 0;
            foreach (var itemInSource in source)
            {
                if (object.Equals(item, itemInSource))
                    return i;
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Determines whether an element is in the visual tree of the current application.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// 	<c>true</c> if element paramter is in visual tree otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInVisualTree(this FrameworkElement element)
        {
            return IsInVisualTree(element, Application.Current.RootVisual as FrameworkElement);
        }

        /// <summary>
        /// Determines whether an element is in the visual tree of a given ancestor.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="ancestor">The ancestor.</param>
        /// <returns>
        /// 	<c>true</c> if element paramter is in visual tree otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInVisualTree(this FrameworkElement element, FrameworkElement ancestor)
        {
            FrameworkElement fe = element;
            while (fe != null)
            {
                if (fe == ancestor)
                {
                    return true;
                }

                fe = VisualTreeHelper.GetParent(fe) as FrameworkElement;
            }

            return false;
        }

        public static bool IsRational(this double value)
        {
            return !double.IsInfinity(value) && !double.IsNaN(value);
        }

        public static Point Location(this Rect rect)
        {
            return new Point(rect.X, rect.Y);
        }

        public static void RemoveAll<T>(this List<T> list, Predicate<T> predicate)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (predicate(list[i]))
                {
                    list.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Sends a file in multipart HTML form. The request is sent using POST method
        /// </summary>
        /// <param name="webclient">The webclient.</param>
        /// <param name="uri">The URI of the resource to receive the file. </param>
        /// <param name="fileStream">The file content to upload</param>
        /// <param name="fileName">The file name</param>
        /// <param name="fileFieldName">The form field to receive the file</param>
        /// <param name="data">The form data to upload.</param>
        /// <param name="userToken">user state</param>
        public static void SendFile(
            this WebClient webclient,
            Uri uri,
            Stream fileStream,
            string fileName,
            string fileFieldName,
            IEnumerable<KeyValuePair<string, string>> data,
            object userToken)
        {
            string boundary = Guid.NewGuid().ToString("N").PadLeft(40, '-');
            webclient.Headers[HttpRequestHeader.ContentType] = "multipart/form-data; boundary=" + boundary;

            webclient.OpenWriteCompleted += (s, e) =>
            {
                using (StreamWriter writer = new StreamWriter(e.Result))
                {
                    writer.WriteLine("--" + boundary);
                    writer.WriteLine("Content-Disposition: form-data; name=\"" + fileFieldName + "\"; filename=\"" + fileName + "\"");
                    writer.WriteLine("Content-Type: application/octet-stream");
                    writer.WriteLine();
                    writer.Flush();

                    byte[] buffer = new byte[4096];
                    int read = 0;

                    while ((read = fileStream.Read(buffer, 0, buffer.Length)) == buffer.Length)
                    {
                        e.Result.Write(buffer, 0, read);
                        e.Result.Flush();
                    }

                    writer.WriteLine();

                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            writer.WriteLine("--" + boundary);
                            writer.WriteLine("Content-Disposition: form-data; name=\"" + item.Key + "\"");
                            writer.WriteLine();
                            writer.WriteLine(item.Value);
                        }
                    }

                    writer.WriteLine("--" + boundary + "--");
                    writer.Flush();
                    writer.Close();
                }
            };

            webclient.OpenWriteAsync(uri, "POST", userToken);
        }

        /// <summary>
        /// Sends an HTML form. The request is sent using POST method
        /// </summary>
        /// <param name="webclient">The webclient.</param>
        /// <param name="uri">The URI of the resource to receive the file. </param>
        /// <param name="data">The form data to upload.</param>
        public static void SendHtmlForm(this WebClient webclient, Uri uri, IEnumerable<KeyValuePair<string, string>> data)
        {
            StringBuilder dataBuilder = new StringBuilder();
            int cnt = 0;
            foreach (var item in data)
            {
                if (cnt > 0)
                {
                    dataBuilder.Append('&');
                }

                dataBuilder.Append(item.Key);
                dataBuilder.Append('=');
                dataBuilder.Append(HttpUtility.UrlEncode(item.Value));
                cnt++;
            }

            webclient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            webclient.UploadStringAsync(uri, dataBuilder.ToString());
        }

        /// <summary>
        /// Sends an HTML form. The request is sent using POST method
        /// </summary>
        /// <param name="webclient">The webclient.</param>
        /// <param name="uri">The URI of the resource to receive the file. </param>
        /// <param name="data">The form data to upload.</param>
        /// <param name="userToken">user state</param>
        public static void SendHtmlForm(this WebClient webclient, Uri uri, IEnumerable<KeyValuePair<string, string>> data, object userToken)
        {
            StringBuilder dataBuilder = new StringBuilder();
            int cnt = 0;
            foreach (var item in data)
            {
                if (cnt > 0)
                {
                    dataBuilder.Append('&');
                }

                dataBuilder.Append(item.Key);
                dataBuilder.Append('=');
                dataBuilder.Append(HttpUtility.UrlEncode(item.Value));
                cnt++;
            }

            webclient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            webclient.UploadStringAsync(uri, "POST", dataBuilder.ToString(), userToken);
        }

        /// <summary>
        /// Sets a cookie.
        /// </summary>
        /// <param name="document">The <c>HtmlDocument</c> holding the cookies.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        /// <param name="value">The cookie value.</param>
        /// <param name="expireDays">Expiration in days.</param>
        public static void SetCookie(this HtmlDocument document, string cookieName, string value, int? expireDays)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(Application.Current.RootVisual))
            {
                return;
            }

            StringBuilder cookieDefinition = new StringBuilder(cookieName);
            cookieDefinition.Append('=');
            cookieDefinition.Append(HttpUtility.UrlEncode(value));
            if (expireDays.HasValue)
            {
                cookieDefinition.AppendFormat(";expires={0:R}", DateTime.Now.AddDays(expireDays.Value).ToUniversalTime());
            }

            document.Cookies = cookieDefinition.ToString();
        }

        public static Size Size(this Rect rect)
        {
            return new Size(rect.Width, rect.Height);
        }

        /// <summary>
        /// Try to get a value from the dictionary
        /// </summary>
        /// <typeparam name="U">the dictionary key type</typeparam>
        /// <typeparam name="T">the dictionary value type</typeparam>
        /// <param name="dic">the dictionary</param>
        /// <param name="key">the key to look for</param>
        /// <returns>returns the value if found, null otherwise</returns>
        public static T TryGetValue<U, T>(this IDictionary<U, T> dic, U key)
        {
            T result = default(T);
            if (key != null)
            {
                dic.TryGetValue(key, out result);
            }

            return result;
        }

        #endregion Methods
    }
}
