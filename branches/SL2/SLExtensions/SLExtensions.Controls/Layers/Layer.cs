// <copyright file="Layer.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Layers
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Layer control
    /// </summary>
    public class Layer : Canvas
    {
        #region Fields

        /// <summary>
        /// Layer name dependency property
        /// </summary>
        public static readonly DependencyProperty LayerNameProperty = 
            DependencyProperty.Register(
                "LayerName",
                typeof(string),
                typeof(Layer),
                null);

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the name of the layer.
        /// </summary>
        /// <value>The name of the layer.</value>
        public string LayerName
        {
            get { return (string)GetValue(LayerNameProperty); }
            set { SetValue(LayerNameProperty, value); }
        }

        #endregion Properties
    }
}