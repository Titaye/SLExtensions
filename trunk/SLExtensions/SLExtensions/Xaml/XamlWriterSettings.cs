using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace SLExtensions.Xaml
{
    /// <summary>
    /// Specifies the features for the XAML writer.
    /// </summary>
    public class XamlWriterSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XamlWriterSettings"/> class.
        /// </summary>
        public XamlWriterSettings()
        {
            this.WriteDefaultValues = false;
            this.MaxUIElements = int.MaxValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to write default values.
        /// </summary>
        /// <value><c>true</c> if write default values; otherwise, <c>false</c>.</value>
        public bool WriteDefaultValues { get; set; }
        /// <summary>
        /// Gets or sets the maximum number of UI elements to write.
        /// </summary>
        /// <value>The max UI elements.</value>
        public int MaxUIElements { get; set; }
    }
}
