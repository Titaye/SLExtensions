// <copyright file="VisibilityConvertionType.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Data
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

    #region Enumerations

    /// <summary>
    /// VisibilityConvertionParameter value converting behavior
    /// </summary>
    public enum VisibilityCondition
    {
        /// <summary>
        /// If the value of the VisibilityConvertionParameter equals the value of the source, the result is Visibility.Visible
        /// </summary>
        IfValueVisible,

        /// <summary>
        /// If the value of the VisibilityConvertionParameter equals the value of the source, the result is Visibility.Collapsed
        /// </summary>
        IfValueCollapsed
    }

    #endregion Enumerations
}