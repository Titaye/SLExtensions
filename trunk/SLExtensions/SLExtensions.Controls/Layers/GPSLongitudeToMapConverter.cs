// <copyright file="GPSLongitudeToMapConverter.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Layers
{
    using System;
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
    /// Convert Gps longitude to pixel point
    /// </summary>
    public class GPSLongitudeToMapConverter : IValueConverter
    {
        #region Constructors

        /// <summary>
        /// Initialize static behavior
        /// </summary>
        static GPSLongitudeToMapConverter()
        {
            Instance = new GPSLongitudeToMapConverter();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a default GPSLongitudeToMapConverter instance
        /// </summary>
        public static GPSLongitudeToMapConverter Instance
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IMapProjection projection = parameter as IMapProjection;
            if (projection == null)
            {
                return value;
            }

            return projection.LatLongToPixelXY(0, (double) value, 1).X;
        }

        /// <summary>
        /// Not supported exception
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}