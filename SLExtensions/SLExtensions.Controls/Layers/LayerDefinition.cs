// <copyright file="LayerDefinition.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Layers
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Defines a layer
    /// </summary>
    public class LayerDefinition : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The name of the layer
        /// </summary>
        private string layerName;

        #endregion Fields

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the name of the layer.
        /// </summary>
        /// <value>The name of the layer.</value>
        public string LayerName
        {
            get
            {
                return this.layerName;
            }

            set
            {
                if (this.layerName != value)
                {
                    this.layerName = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.LayerName));
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Methods
    }
}