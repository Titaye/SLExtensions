namespace SLExtensions
{
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

    public static class PointExtensions
    {
        #region Methods

        public static double Distance(this Point pt1, Point pt)
        {
            var num2 = pt1.X - pt.X;
            var num = pt1.Y - pt.Y;
            var num3 = (num2 * num2) + (num * num);
            return Math.Sqrt(num3);
        }

        public static bool IsRational(this Point value)
        {
            return value.X.IsRational() && value.Y.IsRational();
        }

        #endregion Methods
    }
}