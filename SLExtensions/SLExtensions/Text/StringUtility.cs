namespace SLExtensions.Text
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    using SLExtensions.IO;

    /// <summary>
    /// Provides several string utility methods.
    /// </summary>
    public static class StringUtility
    {
        #region Methods

        /// <summary>
        /// Formats the specified message using the invariant culture.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public static string Format(string message, params object[] o)
        {
            try {
                return string.Format(CultureInfo.InvariantCulture, message, o);
            }
            catch (FormatException) {
                return message;     // leave message as-is
            }
        }

        /// <summary>
        /// Return the camel-case representation of the specified string value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ToCamelCase(string value)
        {
            if (value == null) {
                throw new ArgumentNullException("value");
            }

            char[] chars = value.ToCharArray();
            int i = 0;

            while (i < chars.Length && char.IsUpper(chars[i])) {
                chars[i] = char.ToLower(chars[i++], CultureInfo.InvariantCulture);
            }

            return new string(chars);
        }

        /// <summary>
        /// Tries to parse a byte from given string value.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool TryParseByte(string s, out byte value)
        {
            value = 0;
            bool result = false;

            try {
                value = byte.Parse(s);
            }
            catch (Exception) {
            }
            return result;
        }

        /// <summary>
        /// Tries to parse an Int32 from given string value.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool TryParseInt32(string s, out int value)
        {
            value = 0;
            bool result = false;

            try {
                value = int.Parse(s);
            }
            catch (Exception) {
            }
            return result;
        }

        /// <summary>
        /// Validates the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="nullAllowed">if set to <c>true</c> [null allowed].</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="minLength">The minimum number of bytes when encoded using specified encoding.</param>
        /// <param name="maxLength">The maximum number of bytes when encoded using specified encoding.</param>
        public static void ValidateValue(string value, bool nullAllowed, Encoding encoding, int minLength, int maxLength)
        {
            if (!nullAllowed || value != null) {
                int length = ByteUtility.GetByteCount(encoding, value);

                if (length < minLength || length > maxLength) {
                    throw new ArgumentOutOfRangeException(StringUtility.Format(Resource.ExceptionStringLength, value, minLength, maxLength));
                }
               }
        }

        #endregion Methods
    }
}