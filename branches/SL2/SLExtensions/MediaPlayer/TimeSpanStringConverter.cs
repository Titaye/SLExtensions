// <copyright file="TimeSpanStringConverter.cs" company="Microsoft">
//     Copyright © Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>Implements the TimeSpanStringConverter class</summary>
// <author>Microsoft Expression Encoder Team</author>
namespace ExpressionMediaPlayer
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Enumeration of modes for this converter.
    /// </summary>
    [Flags]
    public enum ConverterModes
    {
        /// <summary>
        /// Don't use milliseconds.
        /// </summary>
        NoMilliseconds = 1,

        /// <summary>
        /// Don't use leading zeros.
        /// </summary>
        NoLeadingZeros = 2,

        /// <summary>
        /// Use tenth of a seconds.
        /// </summary>
        TenthSecond = 4,
    }

    /// <summary>
    /// Converts seconds to a time string and back (TimeSpan-to-string) .
    /// </summary>
    public sealed class TimeSpanStringConverter
    {
        /// <summary>
        /// Padding format.
        /// </summary>
        private const string PaddedFormat = "00";

        /// <summary>
        /// Prevents a default instance of the TimeSpanStringConverter class from being created.
        /// </summary>
        private TimeSpanStringConverter()
        {
        }

        /// <summary>
        /// Converts the value to a string.
        /// </summary>
        /// <param name="timeValue">The timespan value.</param>
        /// <param name="mode">The converter mode.</param>
        /// <returns>The string representation of the timespan.</returns>
        public static string ConvertToString(TimeSpan timeValue, ConverterModes mode)
        {
            String strTimeSep = ":";// SL2 doesn't support CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator;
            bool negative = timeValue < TimeSpan.Zero;
            if (negative)
            {
                timeValue = TimeSpan.Zero - timeValue;
            }

            double milliseconds = timeValue.TotalMilliseconds % 1000;
            long seconds = (long)timeValue.TotalMilliseconds / 1000;

            // If we're not showing millseconds then round up if necessary
            if (mode == ConverterModes.NoMilliseconds)
            {
                if (milliseconds >= 500)
                {
                    seconds++;
                }
            }
            else
            {
                if (milliseconds > 999.5)
                {
                    seconds++;
                    milliseconds = 0;
                }
            }

            long minutes = seconds / 60; ;
            long hours = minutes / 60;

            seconds -= minutes * 60;
            minutes -= hours * 60;

            bool wroteSomething = false;
            string timeString = "";
            string strToStringFormat = "";
            if (!EnumUtils.IsBitSet(mode, ConverterModes.NoLeadingZeros))
            {
                strToStringFormat = PaddedFormat;
            }

            if (!EnumUtils.IsBitSet(mode, ConverterModes.NoLeadingZeros) || hours != 0 || wroteSomething)
            {
                timeString += hours.ToString(strToStringFormat, CultureInfo.CurrentCulture) + strTimeSep;
                wroteSomething = true;
                strToStringFormat = PaddedFormat;
            }

            if (!EnumUtils.IsBitSet(mode, ConverterModes.NoLeadingZeros) || minutes != 0 || wroteSomething)
            {
                timeString += minutes.ToString(strToStringFormat, CultureInfo.CurrentCulture) + strTimeSep;
                wroteSomething = true;
                strToStringFormat = PaddedFormat;
            }

            timeString += seconds.ToString(strToStringFormat, CultureInfo.CurrentCulture);
            if (!EnumUtils.IsBitSet(mode, ConverterModes.NoMilliseconds))
            {
                if (EnumUtils.IsBitSet(mode, ConverterModes.TenthSecond))
                {
                    long wholeMilliseconds = System.Math.Min(9,(long)((milliseconds / 100) + 0.5));// using Min to avoid displaying ".10"
                    timeString += CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator +
                                wholeMilliseconds.ToString("0", CultureInfo.CurrentCulture);
                }
                else
                {
                    long wholeMilliseconds = (long)(milliseconds + 0.5);
                    timeString += CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator +
                                wholeMilliseconds.ToString("000", CultureInfo.CurrentCulture);
                }
            }

            if (negative)
            {
                timeString = "-" + timeString;
            }

            return timeString;
        }

        /// <summary>
        /// Enumeration utilities class.
        /// </summary>
        static class EnumUtils
        {
            /// <summary>
            /// Checks to see if a bit is set in a flags enumeration.
            /// </summary>
            /// <typeparam name="T">The enumeration to check.</typeparam>
            /// <param name="value">The value of the enumeration.</param>
            /// <param name="check">The bit to check.</param>
            /// <returns>True if the bit is set; false otherwise.</returns>
            public static bool IsBitSet<T>(T value, T check) where T : IConvertible
            {
                int localValue = value.ToInt32(CultureInfo.InvariantCulture);
                int localCheck = check.ToInt32(CultureInfo.InvariantCulture);
                return (localValue & localCheck) == localCheck;
            }
        }
    }
}
