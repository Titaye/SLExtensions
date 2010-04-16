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

namespace SLExtensions
{
    public static class NumericExtensions
    {
        /// <summary>
        /// Check if a number is zero.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <returns>True if the number is zero, false otherwise.</returns>
        public static bool IsZero(this double value)
        {
            // We actually consider anything within an order of magnitude of
            // epsilon to be zero

            return Math.Abs(value) < 2.2204460492503131E-15;
        }

        public static bool IsRational(this double value)
        {
            return !double.IsInfinity(value) && !double.IsNaN(value);
        }

    }
}
