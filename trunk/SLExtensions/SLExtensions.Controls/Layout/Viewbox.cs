namespace SLExtensions.Controls
{
    using System;
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

    //[TemplatePart(Name = "ContentControl", Type = typeof(ContentControl))]
    [TemplatePart(Name = "LayoutRoot", Type = typeof(Canvas))]
    [ContentProperty("Child")]
    public class Viewbox : Control
    {
        #region Fields

        /// <summary>
        /// Child depedency property.
        /// </summary>
        public static readonly DependencyProperty ChildProperty = 
            DependencyProperty.Register(
                "Child",
                typeof(FrameworkElement),
                typeof(Viewbox),
                new PropertyMetadata((d, e) => ((Viewbox)d).OnChildChanged((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue)));

        /// <summary>
        /// Stretch depedency property.
        /// </summary>
        public static readonly DependencyProperty StretchProperty = 
            DependencyProperty.Register(
                "Stretch",
                typeof(Stretch),
                typeof(Viewbox),
                new PropertyMetadata((d, e) => ((Viewbox)d).OnStretchChanged((Stretch)e.OldValue, (Stretch)e.NewValue)));

        private Canvas cvParent;
        private bool isLoaded = false;
        private Canvas layoutRoot;
        private ScaleTransform scaleTransform;
        private TransformGroup transformGroup;
        private TranslateTransform translateTransform;

        #endregion Fields

        #region Constructors

        public Viewbox()
        {
            DefaultStyleKey = typeof(Viewbox);

            transformGroup = new TransformGroup();
            scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;
            translateTransform = new TranslateTransform();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);

            cvParent = new Canvas();
            cvParent.RenderTransform = transformGroup;

            Stretch = Stretch.Uniform;
        }

        #endregion Constructors

        #region Properties

        public FrameworkElement Child
        {
            get
            {
                return (FrameworkElement)GetValue(ChildProperty);
            }

            set
            {
                SetValue(ChildProperty, value);
            }
        }

        public Stretch Stretch
        {
            get
            {
                return (Stretch)GetValue(StretchProperty);
            }

            set
            {
                SetValue(StretchProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        public override void OnApplyTemplate()
        {
            layoutRoot = GetTemplateChild("LayoutRoot") as Canvas;
            if (layoutRoot != null)
            {
                layoutRoot.Children.Add(cvParent);
                if (Child != null && Child.Parent == null)
                    cvParent.Children.Add(Child);
            }
            isLoaded = true;

            base.OnApplyTemplate();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Child != null)
            {
                double childWidth = (Child.Width + Child.Margin.Left + Child.Margin.Right);
                double childHeight = (Child.Height + Child.Margin.Top + Child.Margin.Bottom);
                if (childWidth.IsRational()
                    && childHeight.IsRational()
                    && childWidth > 0
                    && childHeight > 0)
                {

                    switch (Stretch)
                    {
                        case Stretch.Fill:
                            {
                                scaleTransform.ScaleX = finalSize.Width / childWidth;
                                scaleTransform.ScaleY = finalSize.Height / childHeight;

                                translateTransform.X = 0;
                                translateTransform.Y = 0;
                            } break;
                        case Stretch.None:
                            {
                                scaleTransform.ScaleX = 1;
                                scaleTransform.ScaleY = 1;
                                translateTransform.X = 0;
                                translateTransform.Y = 0;
                            } break;
                        case Stretch.Uniform:
                            {
                                double scale = Math.Min(finalSize.Width / childWidth, finalSize.Height / childHeight);
                                scaleTransform.ScaleX = scale;
                                scaleTransform.ScaleY = scale;

                                double left = (finalSize.Width - childWidth * scale) / 2;
                                double top = (finalSize.Height - childHeight * scale) / 2;
                                translateTransform.X = left;
                                translateTransform.Y = top;
                            } break;
                        case Stretch.UniformToFill:
                            {
                                double scale = Math.Max(finalSize.Width / childWidth, finalSize.Height / childHeight);
                                scaleTransform.ScaleX = scale;
                                scaleTransform.ScaleY = scale;

                                double left = (finalSize.Width - childWidth * scale) / 2;
                                double top = (finalSize.Height - childHeight * scale) / 2;
                                translateTransform.X = left;
                                translateTransform.Y = top;
                            } break;
                    }
                }
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Child != null
                && Child.Width.IsRational()
                && Child.Height.IsRational())
            {
                Child.Measure(availableSize);

                if (availableSize.Width.IsRational() && availableSize.Height.IsRational())
                {
                    return Child.DesiredSize;
                }
                if (availableSize.Height.IsRational() && double.IsPositiveInfinity(availableSize.Width))
                {
                    switch (Stretch)
                    {
                        case Stretch.Uniform:
                        case Stretch.UniformToFill:
                            {
                                double height = availableSize.Height;
                                double scale = Math.Abs((Child.Width + Child.Margin.Left + Child.Margin.Right) / (Child.Height + Child.Margin.Top + Child.Margin.Bottom));
                                double width = scale * height;
                                return new Size(width, height);
                            }
                    }
                }
                else if (availableSize.Width.IsRational() && double.IsPositiveInfinity(availableSize.Height))
                {
                    switch (Stretch)
                    {
                        case Stretch.Uniform:
                        case Stretch.UniformToFill:
                            {
                                double width = availableSize.Width;
                                double scale = Math.Abs((Child.Width + Child.Margin.Left + Child.Margin.Right) / (Child.Height + Child.Margin.Top + Child.Margin.Bottom));
                                double height = 0;
                                if (scale != 0)
                                    height = width / scale;
                                return new Size(width, height);
                            }
                    }
                }
            }
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// handles the ChildProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnChildChanged(FrameworkElement oldValue, FrameworkElement newValue)
        {
            InvalidateMeasure();

            if (isLoaded && oldValue != null)
            {
                cvParent.Children.Remove(oldValue);
            }

            if (isLoaded && newValue != null)
            {
                cvParent.Children.Add(newValue);
            }
        }

        /// <summary>
        /// handles the StretchProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnStretchChanged(Stretch oldValue, Stretch newValue)
        {
            InvalidateMeasure();
        }

        #endregion Methods
    }
}