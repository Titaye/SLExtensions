namespace SLExtensions.Xaml.Emf
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Represents a EMF context.
    /// </summary>
    internal class EmfContext
    {
        #region Fields

        /// <summary>
        /// ALTERNATE
        /// </summary>
        public const short ALTERNATE = 1;

        /// <summary>
        /// MM_ANISOTROPIC
        /// </summary>
        public const short MM_ANISOTROPIC = 8;

        /// <summary>
        /// MM_HIENGLISH
        /// </summary>
        public const short MM_HIENGLISH = 5;

        /// <summary>
        /// MM_HIMETRIC
        /// </summary>
        public const short MM_HIMETRIC = 3;

        /// <summary>
        /// MM_ISOTROPIC
        /// </summary>
        public const short MM_ISOTROPIC = 7;

        /// <summary>
        /// MM_LOENGLISH
        /// </summary>
        public const short MM_LOENGLISH = 4;

        /// <summary>
        /// MM_LOMETRIC
        /// </summary>
        public const short MM_LOMETRIC = 2;

        /// <summary>
        /// MM_TEXT
        /// </summary>
        public const short MM_TEXT = 1;

        /// <summary>
        /// MM_TWIPS
        /// </summary>
        public const short MM_TWIPS = 6;

        /// <summary>
        /// OPAQUE
        /// </summary>
        public const short OPAQUE = 2;

        /// <summary>
        /// TRANSPARENT
        /// </summary>
        public const short TRANSPARENT = 1;

        /// <summary>
        /// WINDING
        /// </summary>
        public const short WINDING = 2;

        private List<object> objects = new List<object>();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        /// <value>The color of the background.</value>
        public Color BackgroundColor
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the background mode.
        /// </summary>
        /// <value>The background mode.</value>
        public short BackgroundMode
        {
            get; set;
        }

        /// <summary>
        /// Gets the current selected brush.
        /// </summary>
        /// <value>The brush.</value>
        public Brush Brush
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        /// <value>The current position.</value>
        public Point CurrentPosition
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the mapping mode.
        /// </summary>
        /// <value>The mapping mode.</value>
        public short MappingMode
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the offset view port org.
        /// </summary>
        /// <value>The offset view port org.</value>
        public Point OffsetViewPortOrg
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the offset window org.
        /// </summary>
        /// <value>The offset window org.</value>
        public Point OffsetWindowOrg
        {
            get; set;
        }

        /// <summary>
        /// Gets the current selected pen.
        /// </summary>
        /// <value>The pen.</value>
        public Pen Pen
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the poly fill mode.
        /// </summary>
        /// <value>The poly fill mode.</value>
        public short PolyFillMode
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>The scale X.</value>
        public double Scale
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>The color of the text.</value>
        public Color TextColor
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the view port ext.
        /// </summary>
        /// <value>The view port ext.</value>
        public Point ViewPortExt
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the view port org.
        /// </summary>
        /// <value>The view port ext.</value>
        public Point ViewPortOrg
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the window ext.
        /// </summary>
        /// <value>The window ext.</value>
        public Point WindowExt
        {
            get; set;
        }

        /// <summary>
        /// Gets the window org.
        /// </summary>
        /// <value>The window org.</value>
        public Point WindowOrg
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds an object.
        /// </summary>
        /// <param name="o">The o.</param>
        public void AddObject(object o)
        {
            for (int i = 0; i < this.objects.Count; i++) {
                if (this.objects[i] == null) {
                    this.objects[i] = o;
                    return;
                }
            }
            this.objects.Add(o);
        }

        /// <summary>
        /// Removes the object.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveObject(int index)
        {
            if (index >= 0 && index < this.objects.Count) {
                object o = this.objects[index];
                this.objects[index] = null;

                if (o == this.Pen) {
                    this.Pen = null;
                }
                else if (o == this.Brush) {
                    this.Brush = null;
                }
            }
            else {
            }
        }

        /// <summary>
        /// Selects the object.
        /// </summary>
        /// <param name="index">The index.</param>
        public void SelectObject(int index)
        {
            if (index >= 0 && index < this.objects.Count) {
                object o = this.objects[index];

                Pen p = o as Pen;
                if (p != null) {
                    this.Pen = p;
                }
                else {
                    Brush b = o as Brush;
                    if (b != null) {
                        this.Brush = b;
                    }
                }
            }
            else {
            }
        }

        /// <summary>
        /// Translates specified coordinates.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public Point Translate(int x, int y)
        {
            //xD = (xL - xWindowOrg)*(xViewportExt/xWindowExt) + xViewportOrg

            double width = (double)this.ViewPortExt.X / (double)this.WindowExt.X;
            double height = (double)this.ViewPortExt.Y / (double)this.WindowExt.Y;

            double resultX = (x - this.WindowOrg.X) * width + this.ViewPortOrg.X;
            double resultY = (y - this.WindowOrg.Y) * height + this.ViewPortOrg.Y;

            return new Point(resultX * Scale, resultY * Scale);
        }

        #endregion Methods
    }
}