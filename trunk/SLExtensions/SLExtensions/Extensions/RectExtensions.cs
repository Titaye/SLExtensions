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
    public static class RectExtensions
    {
        public static Point Location(this Rect rect)
        {
            return new Point(rect.X, rect.Y);
        }

        public static Size Size(this Rect rect)
        {
            return new Size(rect.Width, rect.Height);
        }

    }
}
