// <copyright file="URIConverter.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Data
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
    /// Convert data to Uri objects as it passes through the binding engine.
    /// </summary>
    public class UriConverter : IValueConverter
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
        /// The Uri to be passed to the target dependency property.
        /// </returns>
        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            Uri sourceUri = value as Uri;
            if (sourceUri == null)
            {
                // Create sourceUri
                string stringValue = value as string;
                if(!string.IsNullOrEmpty(stringValue))
                    sourceUri = new Uri(stringValue, UriKind.RelativeOrAbsolute);
            }

            if (sourceUri == null)
            {
                throw new NotSupportedException(Resource.UriConverterExceptionInvalidSource);
            }

            // We got an abosulte Uri we do not need to modify it.
            if (sourceUri.IsAbsoluteUri)
            {
                return sourceUri;
            }

            // No parameter return the sourceUri
            if (parameter == null)
            {
                return sourceUri;
            }

            string strPrm;
            Uri uriPrm;
            if (!string.IsNullOrEmpty(strPrm = parameter as string))
            {
                // There a string as a parameter
                Uri uriParameter;

                // Check if it's an Uri
                if (Uri.TryCreate(strPrm, UriKind.RelativeOrAbsolute, out uriParameter))
                {
                    // Apply our parameter Uri to the sourceUri
                    return new Uri(uriParameter, sourceUri);
                }
            }
            else if ((uriPrm = parameter as Uri) != null)
            {
                // Apply our parameter Uri to the sourceUri
                return new Uri(uriPrm, sourceUri);
            }

            throw new NotSupportedException(Resource.UriConverterExceptionInvalidParameter);
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Uri uri = value as Uri;
            if (uri == null)
            {
                throw new ArgumentException(Resource.UriConverterExceptionInvalidConvertBackSource);
            }

            if (targetType != typeof(string)
                && targetType != typeof(Uri))
            {
                throw new NotSupportedException(Resource.UriConverterExceptionInvalidConvertBackTargetType);
            }

            // No parameter return the sourceUri
            if (parameter == null)
            {
                return ConvertBackType(targetType, uri);
            }

            string strPrm;
            Uri uriPrm;
            if (!string.IsNullOrEmpty(strPrm = parameter as string))
            {
                // There a string as a parameter
                Uri uriParameter;

                // Check if it's an Uri
                if (Uri.TryCreate(strPrm, UriKind.RelativeOrAbsolute, out uriParameter))
                {
                    // Apply our parameter Uri
                    return ConvertBackType(targetType, uriParameter.MakeRelativeUri(uri));
                }
            }
            else if ((uriPrm = parameter as Uri) != null)
            {
                // Apply our parameter Uri to the sourceUri
                return ConvertBackType(targetType, uriPrm.MakeRelativeUri(uri));
            }

            throw new NotSupportedException(Resource.UriConverterExceptionInvalidConvertBackParameter);
        }

        /// <summary>
        /// Handle convert back type convertion
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="uri">The URI to convert.</param>
        /// <returns>the converted value</returns>
        private static object ConvertBackType(Type targetType, Uri uri)
        {
            if (targetType == typeof(Uri))
            {
                return uri;
            }

            return uri.ToString();
        }

        #endregion Methods
    }
}