namespace SLExtensions
{
    using System;
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

    public static class HtmlDocumentExtensions
    {
        #region Methods

        /// <summary>
        /// Gets a cookie from its name.
        /// </summary>
        /// <param name="document">The <c>HtmlDocument</c>.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        /// <returns>The cookie content</returns>
        public static string GetCookie(this HtmlDocument document, string cookieName)
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
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

        /// <summary>
        /// Sets a cookie.
        /// </summary>
        /// <param name="document">The <c>HtmlDocument</c> holding the cookies.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        /// <param name="value">The cookie value.</param>
        /// <param name="expireDays">Expiration in days.</param>
        public static void SetCookie(this HtmlDocument document, string cookieName, string value, int? expireDays)
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
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

        #endregion Methods
    }
}