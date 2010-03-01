// <copyright file="VisibilityConverterParameter.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Data
{
    using System;
    using System.Net;
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
    /// Optional parameter for a VisibilityConverter
    /// </summary>
    public class VisibilityConverterParameter
    {
        #region Properties

        /// <summary>
        /// Gets or sets how to handle the visibility when the converter value equals the parameter reference value
        /// </summary>
        public VisibilityCondition Condition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets reference convertion value
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        #endregion Properties
    }
}