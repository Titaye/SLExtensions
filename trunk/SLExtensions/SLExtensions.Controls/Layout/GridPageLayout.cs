namespace SLExtensions.Controls
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

    public class GridPageLayout : Panel
    {
        #region Fields

        /// <summary>
        /// ColumnCount depedency property.
        /// </summary>
        public static readonly DependencyProperty ColumnCountProperty = 
            DependencyProperty.Register(
                "ColumnCount",
                typeof(double),
                typeof(GridPageLayout),
                new PropertyMetadata(1d, (d, e) => ((GridPageLayout)d).OnColumnCountChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// Orientation depedency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = 
            DependencyProperty.Register(
                "Orientation",
                typeof(Orientation),
                typeof(GridPageLayout),
                new PropertyMetadata((d, e) => ((GridPageLayout)d).OnOrientationChanged((Orientation)e.OldValue, (Orientation)e.NewValue)));

        /// <summary>
        /// PageHeight depedency property.
        /// </summary>
        public static readonly DependencyProperty PageHeightProperty = 
            DependencyProperty.Register(
                "PageHeight",
                typeof(double),
                typeof(GridPageLayout),
                new PropertyMetadata((d, e) => ((GridPageLayout)d).OnPageHeightChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// PageOrientation depedency property.
        /// </summary>
        public static readonly DependencyProperty PageOrientationProperty = 
            DependencyProperty.Register(
                "PageOrientation",
                typeof(Orientation),
                typeof(GridPageLayout),
                new PropertyMetadata((d, e) => ((GridPageLayout)d).OnPageOrientationChanged((Orientation)e.OldValue, (Orientation)e.NewValue)));

        /// <summary>
        /// PageWidth depedency property.
        /// </summary>
        public static readonly DependencyProperty PageWidthProperty = 
            DependencyProperty.Register(
                "PageWidth",
                typeof(double),
                typeof(GridPageLayout),
                new PropertyMetadata((d, e) => ((GridPageLayout)d).OnPageWidthChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// RowCount depedency property.
        /// </summary>
        public static readonly DependencyProperty RowCountProperty = 
            DependencyProperty.Register(
                "RowCount",
                typeof(double),
                typeof(GridPageLayout),
                new PropertyMetadata(double.NaN, (d, e) => ((GridPageLayout)d).OnRowCountChanged((double)e.OldValue, (double)e.NewValue)));

        #endregion Fields

        #region Constructors

        public GridPageLayout()
        {
            Orientation = Orientation.Horizontal;
            PageOrientation = Orientation.Horizontal;
            PageWidth = double.NaN;
            PageHeight = double.NaN;
        }

        #endregion Constructors

        #region Properties

        public double ColumnCount
        {
            get
            {
                return (double)GetValue(ColumnCountProperty);
            }

            set
            {
                SetValue(ColumnCountProperty, value);
            }
        }

        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        public double PageHeight
        {
            get
            {
                return (double)GetValue(PageHeightProperty);
            }

            set
            {
                SetValue(PageHeightProperty, value);
            }
        }

        public Orientation PageOrientation
        {
            get
            {
                return (Orientation)GetValue(PageOrientationProperty);
            }

            set
            {
                SetValue(PageOrientationProperty, value);
            }
        }

        public double PageWidth
        {
            get
            {
                return (double)GetValue(PageWidthProperty);
            }

            set
            {
                SetValue(PageWidthProperty, value);
            }
        }

        public double RowCount
        {
            get
            {
                return (double)GetValue(RowCountProperty);
            }

            set
            {
                SetValue(RowCountProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (ColumnCount <= 0 || RowCount <= 0)
                return finalSize;

            double pageWidth = PageWidth;
            double pageHeight = PageHeight;

            if (double.IsNaN(pageWidth))
                pageWidth = finalSize.Width;
            if (double.IsNaN(pageHeight))
                pageHeight = finalSize.Height;

            if (double.IsNaN(RowCount)
                && double.IsNaN(ColumnCount))
            {
                return finalSize;
            }

            int rowCount;
            int columnCount;
            if (double.IsNaN(RowCount))
            {
                rowCount = (int)Math.Ceiling((double)Children.Count / (int)ColumnCount);
            }
            else
            {
                rowCount = (int)RowCount;
            }

            if (double.IsNaN(ColumnCount))
            {
                columnCount = (int)Math.Ceiling((double)Children.Count / (int)RowCount);
            }
            else
            {
                columnCount = (int)ColumnCount;
            }

            double colWidth = pageWidth / columnCount;
            double colHeight = pageHeight / rowCount;

            int pageItemCount = columnCount * rowCount;
            int nbPages = 0;

            if (columnCount * rowCount != 0)
                nbPages = (int)Math.Ceiling(((double)Children.Count) / (columnCount * rowCount));

            double totalColHeight = 0;
            double currentColHeight = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                FrameworkElement fe = Children[i] as FrameworkElement;

                if (double.IsInfinity(pageHeight)
                    && Orientation == Orientation.Horizontal)
                {
                    if (i % columnCount == 0)
                    {
                        totalColHeight += currentColHeight;
                        currentColHeight = fe.DesiredSize.Height;
                        for (int j = 0; j < Children.Count && j < j + columnCount; j++)
                        {
                            var item = Children[j];
                            currentColHeight = Math.Max(item.DesiredSize.Height, currentColHeight);
                        }
                        colHeight = currentColHeight;
                    }
                }

                int pageIndex = (int)Math.Floor((double)i / pageItemCount);

                int columnIdx = Orientation == Orientation.Horizontal ? (i % pageItemCount) % columnCount : (i % pageItemCount) / rowCount;
                int rowIdx = Orientation == Orientation.Horizontal ? (i % pageItemCount) / columnCount : (i % pageItemCount) % rowCount;

                double pageOffsetX = PageOrientation == Orientation.Horizontal ? pageWidth * pageIndex : 0;
                double pageOffsetY = PageOrientation == Orientation.Horizontal ? 0 : pageHeight * pageIndex;

                Rect rect = new Rect(pageOffsetX + colWidth * columnIdx, pageOffsetY + colHeight * rowIdx, colWidth, colHeight);
                fe.Arrange(rect);
            }

            if (PageOrientation == Orientation.Horizontal)
                return new Size(nbPages * pageWidth, pageHeight);
            else
                return new Size(pageWidth, nbPages * pageHeight);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (ColumnCount <= 0 || RowCount <= 0)
                return availableSize;

            double pageWidth = PageWidth;
            double pageHeight = PageHeight;

            if (double.IsNaN(pageWidth))
                pageWidth = availableSize.Width;
            if (double.IsNaN(pageHeight))
                pageHeight = availableSize.Height;

            int rowCount;
            int columnCount;
            if (double.IsNaN(RowCount))
            {
                rowCount = (int)Math.Ceiling((double)Children.Count / (int)ColumnCount);
            }
            else
            {
                rowCount = (int)RowCount;
            }

            if (double.IsNaN(ColumnCount))
            {
                columnCount = (int)Math.Ceiling((double)Children.Count / (int)RowCount);
            }
            else
            {
                columnCount = (int)ColumnCount;
            }

            double colWidth = pageWidth / columnCount;
            double colHeight = pageHeight / rowCount;

            double totalColHeight = 0;
            double currentColHeight = 0;
            int i = 0;
            foreach (var item in Children)
            {
                item.Measure(new Size(colWidth, colHeight));

                if (double.IsInfinity(colHeight)
                    && Orientation == Orientation.Horizontal)
                {
                    if (i % columnCount == 0)
                    {
                        totalColHeight += currentColHeight;
                        currentColHeight = item.DesiredSize.Height;
                    }
                    else
                    {
                        currentColHeight = Math.Max(item.DesiredSize.Height, currentColHeight);
                    }
                }
                i++;
            }

            totalColHeight += currentColHeight;

            if (double.IsInfinity(pageHeight))
                pageHeight = totalColHeight;

            int nbPages = 0;

            if (columnCount * rowCount != 0)
                nbPages = (int)Math.Ceiling(((double)Children.Count) / (columnCount * rowCount));

            if (PageOrientation == Orientation.Horizontal)
            {
                return new Size(Math.Min(nbPages * pageWidth, availableSize.Width), pageHeight);
            }
            else
                return new Size(pageWidth, Math.Min(nbPages * pageHeight, availableSize.Height));
            //return availableSize;
        }

        /// <summary>
        /// handles the ColumnCountProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnColumnCountChanged(double oldValue, double newValue)
        {
            InvalidateMeasure();
        }

        /// <summary>
        /// handles the OrientationProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnOrientationChanged(Orientation oldValue, Orientation newValue)
        {
            InvalidateMeasure();
        }

        /// <summary>
        /// handles the PageHeightProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnPageHeightChanged(double oldValue, double newValue)
        {
            InvalidateMeasure();
        }

        /// <summary>
        /// handles the PageOrientationProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnPageOrientationChanged(Orientation oldValue, Orientation newValue)
        {
            InvalidateMeasure();
        }

        /// <summary>
        /// handles the PageWidthProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnPageWidthChanged(double oldValue, double newValue)
        {
            InvalidateMeasure();
        }

        /// <summary>
        /// handles the RowCountProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnRowCountChanged(double oldValue, double newValue)
        {
            InvalidateMeasure();
        }

        #endregion Methods
    }
}