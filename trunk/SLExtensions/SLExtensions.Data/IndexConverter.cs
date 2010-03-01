// <copyright file="IndexConverter.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Data
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Linq;
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
    /// Class retreiving an indexed value from a <see cref="System.Collection.IList"/> or enumerate a <see cref="System.Collection.IEnumerable"/> to the index
    /// </summary>
    public class IndexConverter : IValueConverter
    {
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
            if (value == null)
            {
                return null;
            }

            int prm;
            int? intPrm = parameter as int?;
            if (!intPrm.HasValue)
            {
                string strPrm = parameter as string;
                if (string.IsNullOrEmpty(strPrm)
                    || !int.TryParse(strPrm, out prm))
                {
                    throw new ArgumentException(Resource.IndexConverterMissingParameter);
                    // If no index found in parameter, prm is initialized at 0
                }
            }
            else
            {
                prm = intPrm.Value;
            }

            if(prm < 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resource.IndexConverterArgumentOutOfRange, prm, "?"));
            }

            IList col = value as IList;
            if (col != null)
            {
                return col[prm];
            }

            IEnumerable enumerable = value as IEnumerable;
            int cnt = 0;
            if (enumerable != null)
            {
                IEnumerator iterator = enumerable.GetEnumerator();
                while (iterator.MoveNext())
                {
                    if (cnt++ == prm)
                    {
                        return iterator.Current;
                    }
                }

                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resource.IndexConverterArgumentOutOfRange, prm, cnt));
            }

            throw new ArgumentException(Resource.IndexConverterExceptionValueNotIList);
        }

        /// <summary>
        /// Not supported
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
            throw new NotSupportedException();
        }

        #endregion Methods
    }
}