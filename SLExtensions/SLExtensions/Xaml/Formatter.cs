namespace SLExtensions.Xaml
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Provides XAML formatting functions.
    /// </summary>
    public static class Formatter
    {
        #region Methods

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        public static string Format(this bool value)
        {
            return value ? "true" : "false";
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this short value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this double value)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:0.###}", value);
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this Enum value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this Point value)
        {
            return string.Format("{0},{1}", value.X.Format(), value.Y.Format());
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this Rect value)
        {
            return string.Format("{0},{1},{2},{3}",
                    value.X.Format(),
                    value.Y.Format(),
                    value.Width.Format(),
                    value.Height.Format());
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this Size value)
        {
            return string.Format("{0},{1}", value.Width.Format(), value.Height.Format());
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this Thickness value)
        {
            if (value.Left == value.Right && value.Right == value.Top && value.Top == value.Bottom) {
                return value.Left.Format();
            }
            else if (value.Left == value.Right && value.Top == value.Bottom) {
                return string.Format("{0},{1}", value.Left.Format(), value.Top.Format());
            }
            return string.Format("{0},{1},{2},{3}", value.Left.Format(), value.Top.Format(), value.Right.Format(), value.Bottom.Format());
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this CornerRadius value)
        {
            if (value.BottomLeft == value.BottomRight && value.BottomRight == value.TopLeft && value.TopLeft == value.TopRight) {
                return value.BottomLeft.Format();
            }
            return string.Format("{0},{1},{2},{3}", value.TopLeft.Format(), value.TopRight.Format(), value.BottomRight.Format(), value.BottomLeft.Format());
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this GridLength value)
        {
            string result;

            if (value.GridUnitType == GridUnitType.Auto) {
                result = "Auto";
            }
            else if (value.GridUnitType == GridUnitType.Pixel) {
                return value.Value.Format();
            }
            else {
                if (value.Value == 1) {
                    result = "*";
                }
                else {
                    result = string.Format("{0}*", value.Value.Format());
                }
            }
            return result;
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this Cursor value)
        {
            if (value == null) {
                return "Default";
            }
            return value.ToString();
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this Color value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this Uri value)
        {
            if (value == null) {
                throw new ArgumentNullException("value");
            }
            return value.ToString();
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this FontFamily value)
        {
            if (value == null) {
                throw new ArgumentNullException("value");
            }
            return value.Source.ToString();
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this FontStretch value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this FontStyle value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this FontWeight value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this TextDecorationCollection value)
        {
            if (value == null) {
                return "None";
            }
            else {
                return "Underline";
            }
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this DoubleCollection value)
        {
            if (value == null) {
                throw new ArgumentNullException("value");
            }
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < value.Count; i++) {
                if (i > 0) {
                    b.Append(",");
                }
                b.Append(value[i].Format());
            }
            return b.ToString();
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Format(this PointCollection value)
        {
            if (value == null) {
                throw new ArgumentNullException("value");
            }
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < value.Count; i++) {
                if (i > 0) {
                    b.Append(" ");
                }
                b.Append(value[i].Format());
            }
            return b.ToString();
        }

        #endregion Methods
    }
}