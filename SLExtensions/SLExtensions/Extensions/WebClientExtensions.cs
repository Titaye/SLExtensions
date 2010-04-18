namespace SLExtensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public static class WebClientExtensions
    {
        #region Methods

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
        public static void UploadFile(
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

                    while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
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
        public static void UploadValues(this WebClient webclient, Uri uri, IEnumerable<KeyValuePair<string, string>> data)
        {
            UploadValues(webclient, uri, data, null);
        }

        /// <summary>
        /// Sends an HTML form. The request is sent using POST method
        /// </summary>
        /// <param name="webclient">The webclient.</param>
        /// <param name="uri">The URI of the resource to receive the file. </param>
        /// <param name="data">The form data to upload.</param>
        /// <param name="userToken">user state</param>
        public static void UploadValues(this WebClient webclient, Uri uri, IEnumerable<KeyValuePair<string, string>> data, object userToken)
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

        #endregion Methods
    }
}