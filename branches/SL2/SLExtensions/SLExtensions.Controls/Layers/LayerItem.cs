// <copyright file="LayerItem.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Layers
{
    using System;
    using System.Diagnostics;
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
    /// Item to be placed onto a layer
    /// </summary>
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    public class LayerItem : ContentControl
    {
        #region Fields

        /// <summary>
        /// Is fix size property. When <c>true</c>, the global zoom level doesn't affect this item. A scaletransform is applied.
        /// </summary>
        public static readonly DependencyProperty IsFixSizeProperty = 
            DependencyProperty.Register(
                "IsFixSize",
                typeof(bool),
                typeof(LayerItem),
                new PropertyMetadata((s, e) => ((LayerItem)s).OnIsFixSizeChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// Is mouse over dependency property
        /// </summary>
        public static readonly DependencyProperty IsMouseOverProperty = 
            DependencyProperty.Register(
                "IsMouseOver",
                typeof(bool),
                typeof(LayerItem),
                null);

        /// <summary>
        /// Is removing dependency property
        /// </summary>
        public static readonly DependencyProperty IsRemovingProperty = 
            DependencyProperty.Register(
                "IsRemoving",
                typeof(bool),
                typeof(LayerItem),
                null);

        /// <summary>
        /// Zoom dependency property
        /// </summary>
        public static readonly DependencyProperty ZoomProperty = 
            DependencyProperty.Register(
                "Zoom",
                typeof(double),
                typeof(LayerItem),
                new PropertyMetadata((s, e) => ((LayerItem)s).OnZoomChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// Template name of the element root
        /// </summary>
        private const string ElementRootName = "RootElement";

        /// <summary>
        /// Template name of the mouse over storyboard
        /// </summary>
        private const string StateMouseOverName = "MouseOver";

        /// <summary>
        /// Template name of the normal state storyboard
        /// </summary>
        private const string StateNormalName = "Normal";

        /// <summary>
        /// True if the control has been loaded; false otherwise.
        /// </summary> 
        private bool isloaded;

        /// <summary>
        /// scale transform applyed
        /// </summary>
        private ScaleTransform scaleTransform;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerItem"/> class.
        /// </summary>
        public LayerItem()
        {
            DefaultStyleKey = typeof(LayerItem);
            this.IsFixSize = true;
            this.Loaded += delegate
            {
                this.isloaded = true;
                this.UpdateVisualState();
            };
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fix size.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is fix size; otherwise, <c>false</c>.
        /// </value>
        public bool IsFixSize
        {
            get
            {
                return (bool)GetValue(IsFixSizeProperty);
            }

            set
            {
                SetValue(IsFixSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mouse over.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mouse over; otherwise, <c>false</c>.
        /// </value>
        public bool IsMouseOver
        {
            get
            {
                return (bool)GetValue(IsMouseOverProperty);
            }

            set
            {
                SetValue(IsMouseOverProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is removing.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is removing; otherwise, <c>false</c>.
        /// </value>
        public bool IsRemoving
        {
            get
            {
                return (bool)GetValue(IsRemovingProperty);
            }

            set
            {
                SetValue(IsRemovingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the layer source.
        /// </summary>
        /// <value>The layer source.</value>
        public LayerSource LayerSource
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the zoom.
        /// </summary>
        /// <value>The zoom value.</value>
        public double Zoom
        {
            get
            {
                return (double)GetValue(ZoomProperty);
            }

            set
            {
                SetValue(ZoomProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Get the elements
            object root = this.GetTemplateChild(ElementRootName);

            // Sync the logical and visual states of the control
            this.UpdateVisualState();
        }

        /// <summary>
        /// Responds to the MouseEnter event.
        /// </summary> 
        /// <param name="e">The event data for the MouseEnter event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            this.IsMouseOver = true;
            this.UpdateVisualState();
        }

        /// <summary> 
        /// Responds to the MouseLeave event.
        /// </summary>
        /// <param name="e">The event data for the MouseLeave event.</param> 
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            this.IsMouseOver = false;
            this.UpdateVisualState();
        }

        /// <summary>
        /// called when IsFixSize property has changed
        /// </summary>
        /// <param name="oldValue">old fix size value</param>
        /// <param name="newValue">new fix size value</param>
        private void OnIsFixSizeChanged(bool oldValue, bool newValue)
        {
            this.RefreshScale();
        }

        /// <summary>
        /// called when the zoom changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnZoomChanged(double oldValue, double newValue)
        {
            this.RefreshScale();
        }

        /// <summary>
        /// Refreshes the scale.
        /// </summary>
        private void RefreshScale()
        {
            if (this.scaleTransform == null)
            {
                this.scaleTransform = new ScaleTransform();
                if (this.RenderTransform != null)
                {
                    TransformGroup tg = new TransformGroup();
                    tg.Children.Add(this.scaleTransform);
                    tg.Children.Add(this.RenderTransform);
                    this.RenderTransform = tg;
                }
                else
                {
                    this.RenderTransform = this.scaleTransform;
                }
            }

            if (!this.IsFixSize || this.Zoom == 0)
            {
                this.scaleTransform.ScaleX = 1;
                this.scaleTransform.ScaleY = 1;
            }
            else
            {
                this.scaleTransform.ScaleX = (double)1 / this.Zoom;
                this.scaleTransform.ScaleY = (double)1 / this.Zoom;
            }
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        private void RemoveItem()
        {
            var item = (from kvp in this.LayerSource.LayerItems
                        where kvp.Value == this
                        select kvp.Key).First();
            this.LayerSource.Items.Remove(item);
        }

        /// <summary>
        /// Handles the Completed event of the removing control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Removing_Completed(object sender, EventArgs e)
        {
            this.RemoveItem();
        }

        /// <summary> 
        /// Update the current visual state of the button.
        /// </summary>
        private void UpdateVisualState()
        {
            if (!isloaded)
            {
                return;
            }

            if (IsMouseOver)
            {
                SLExtensions.Controls.Animation.VisualState.GoToState(this, true, StateMouseOverName, StateNormalName);
            }
            else
            {
                SLExtensions.Controls.Animation.VisualState.GoToState(this, true, StateNormalName);
            }
        }

        #endregion Methods
    }
}