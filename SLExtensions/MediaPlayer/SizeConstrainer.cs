// <copyright file="SizeConstrainer.cs" company="Microsoft">
//     Copyright © Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>Implements the SizeConstrainer class</summary>
// <author>Microsoft Expression Encoder Team</author>using System;
namespace ExpressionMediaPlayer
{
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Wrapper Control that prevents the content it contains from expanding when 
    /// offered infinity eg when inside a table cell with * height.
    /// </summary>
    [ContentProperty("Child")]
    public class SizeConstrainer : Control
    { 
        /// <summary>
        /// A child dependency property.
        /// </summary>
        public static readonly DependencyProperty ChildProperty = DependencyProperty.Register
           ("Child", typeof(UIElement), typeof(SizeConstrainer), new PropertyMetadata(new PropertyChangedCallback(SizeConstrainer.OnChildPropertyChanged)));

        /// <summary>
        /// The content presenter.
        /// </summary>
        private ContentPresenter m_presenter;

        /// <summary>
        /// Initializes a new instance of the SizeConstrainer class.
        /// </summary>
        public SizeConstrainer()
        {
            DefaultStyleKey = typeof(SizeConstrainer);
        }

        /// <summary>
        /// Gets or sets the child UI element.
        /// </summary>
        public UIElement Child
        {
            get { return GetValue(ChildProperty) as UIElement; }
            set { SetValue(ChildProperty, value); }
        }

        /// <summary>
        /// Event handler for the child dependency property.
        /// </summary>
        /// <param name="dobj">Dependency object that is changing.</param>
        /// <param name="args">Event args.</param>
        private static void OnChildPropertyChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs args)
        {
            (dobj as SizeConstrainer).OnChildChanged();
        }

        /// <summary>
        /// Event handler for the on child changed event.
        /// </summary>
        private void OnChildChanged()
        {
            if (m_presenter != null)
            {
                m_presenter.Content = Child;
            }
        }

        /// <summary>
        /// Overrides MeasureOverride().
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>The size measured.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            //Important that we allow the child to measure to the true available size rather than 0,0..
            Child.Measure(availableSize);
            
            //But we return a zero size requirement
            //rather than the result of the measure operation
            //(The crucial line of code in this class)
            return new Size(0, 0);
        }

        /// <summary>
        /// Overrides ArrangeOverride().
        /// </summary>
        /// <param name="finalSize">The final size.</param>
        /// <returns>The arranged size.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Overrides OnApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_presenter = GetTemplateChild("ChildPresenter") as ContentPresenter;
            OnChildChanged();
        }
    }
}
