namespace SLExtensions.Controls
{
    using System;

    #region Delegates

    /// <summary>
    /// Delegate for the PercentChanged event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void GaugePercentageChangedEventHandler(object sender, GaugePercentageChangedEventArgs e);

    #endregion Delegates

    /// <summary>
    /// Event data for the PercentChanged event.
    /// </summary>
    public class GaugePercentageChangedEventArgs : EventArgs
    {
        #region Fields

        private readonly double _percentage;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a new instance of the GaugePercentageChangedEventArgs class.
        /// </summary>
        /// <param name="percentage">The current percentage.</param>
        public GaugePercentageChangedEventArgs(double percentage)
        {
            _percentage = percentage;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The newly selected percentage.
        /// </summary>
        public double Percentage
        {
            get { return _percentage; }
        }

        #endregion Properties
    }
}