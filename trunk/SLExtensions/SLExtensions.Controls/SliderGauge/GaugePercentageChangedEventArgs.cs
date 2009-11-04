using System;

namespace SLExtensions.Controls
{
    /// <summary>
    /// Delegate for the PercentChanged event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void GaugePercentageChangedEventHandler(object sender, GaugePercentageChangedEventArgs e);

    /// <summary>
    /// Event data for the PercentChanged event.
    /// </summary>
    public class GaugePercentageChangedEventArgs : EventArgs
    {
        private readonly double _percentage;

        /// <summary>
        /// Creates a new instance of the GaugePercentageChangedEventArgs class.
        /// </summary>
        /// <param name="percentage">The current percentage.</param>
        public GaugePercentageChangedEventArgs(double percentage)
        {
            _percentage = percentage;
        }

        /// <summary>
        /// The newly selected percentage.
        /// </summary>
        public double Percentage
        {
            get { return _percentage; }
        }

    }
}
