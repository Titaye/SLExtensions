namespace SLMedia.Core
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

    public static class MarkerExtensions
    {
        #region Methods

        public static bool? IsMarkerActiveAtPosition(this IMarker marker, TimeSpan position)
        {
            if (marker == null)
                throw new ArgumentNullException("marker");

            if (!marker.Duration.HasTimeSpan)
                return null;

            if (marker.Position < position
                && (marker.Position + marker.Duration.TimeSpan) > position)
            {
                return true;
            }

            return false;
        }

        #endregion Methods
    }
}