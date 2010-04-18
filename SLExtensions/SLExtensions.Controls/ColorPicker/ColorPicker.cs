namespace SLExtensions.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    using SLExtensions.Controls.Primitives;

    /// <summary>
    /// Represents a Color Picker control which allows a user to select a color.
    /// </summary>
    public class ColorPicker : Control
    {
        #region Fields

        /// <summary>
        /// Alpha depedency property.
        /// </summary>
        public static readonly DependencyProperty AlphaProperty = 
            DependencyProperty.Register(
                "Alpha",
                typeof(double),
                typeof(ColorPicker),
                new PropertyMetadata(1d, (d, e) => ((ColorPicker)d).OnAlphaChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// ColorXPosition depedency property.
        /// </summary>
        public static readonly DependencyProperty ColorXPositionProperty = 
            DependencyProperty.Register(
                "ColorXPosition",
                typeof(double),
                typeof(ColorPicker),
                new PropertyMetadata((d, e) => ((ColorPicker)d).OnColorXPositionChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// ColorYPosition depedency property.
        /// </summary>
        public static readonly DependencyProperty ColorYPositionProperty = 
            DependencyProperty.Register(
                "ColorYPosition",
                typeof(double),
                typeof(ColorPicker),
                new PropertyMetadata((d, e) => ((ColorPicker)d).OnColorYPositionChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// HueMonitorBrush depedency property.
        /// </summary>
        public static readonly DependencyProperty HueMonitorBrushProperty = 
            DependencyProperty.Register(
                "HueMonitorBrush",
                typeof(Brush),
                typeof(ColorPicker),
                new PropertyMetadata(DefaultHueMonitorBrush));

        /// <summary>
        /// HuePosition depedency property.
        /// </summary>
        public static readonly DependencyProperty HuePositionProperty = 
            DependencyProperty.Register(
                "HuePosition",
                typeof(double),
                typeof(ColorPicker),
                new PropertyMetadata((d, e) => ((ColorPicker)d).OnHuePositionChanged((double)e.OldValue, (double)e.NewValue)));

        /// <summary>
        /// RGBColorBrush depedency property.
        /// </summary>
        public static readonly DependencyProperty RGBColorBrushProperty = 
            DependencyProperty.Register(
                "RGBColorBrush",
                typeof(Brush),
                typeof(ColorPicker),
                null);

        /// <summary>
        /// ColorBrush depedency property.
        /// </summary>
        public static readonly DependencyProperty SatValBrushProperty = 
            DependencyProperty.Register(
                "SatValBrush",
                typeof(Brush),
                typeof(ColorPicker),
                null);

        /// <summary>
        /// SelectedColorBrush depedency property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorBrushProperty = 
            DependencyProperty.Register(
                "SelectedColorBrush",
                typeof(SolidColorBrush),
                typeof(ColorPicker),
                null);

        /// <summary>
        /// SelectedColor Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = 
            DependencyProperty.Register(
                "SelectedColor",
                typeof(Color),
                typeof(ColorPicker),
                new PropertyMetadata(new PropertyChangedCallback(SelectedColorPropertyChanged)));

        private static readonly Brush DefaultHueMonitorBrush = InitializeHueBrush();

        private readonly ColorSpace m_colorSpace;

        private FrameworkElement colorSampleVisual = XamlReader.Load(@"<Canvas xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
              xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
              Width='180' Height='180'>
            <Rectangle x:Name='ColorSample' Width='180' Height='180' Fill='Red'></Rectangle>
                <Rectangle x:Name='WhiteGradient' IsHitTestVisible='False' Width='180' Height='180'>
                    <Rectangle.Fill>
                        <LinearGradientBrush StartPoint='0,0' EndPoint='1,0'>
                            <GradientStop Offset='0' Color='#ffffffff'/>
                            <GradientStop Offset='1' Color='#00ffffff'/>
                        </LinearGradientBrush>
                    </Rectangle.Fill>
                </Rectangle>
                <Rectangle x:Name='BlackGradient' IsHitTestVisible='False' Width='180' Height='180'>
                    <Rectangle.Fill>
                        <LinearGradientBrush StartPoint='0,1' EndPoint='0, 0'>
                            <GradientStop Offset='0' Color='#ff000000'/>
                            <GradientStop Offset='1' Color='#00000000'/>
                        </LinearGradientBrush>
                    </Rectangle.Fill>
                </Rectangle>
                </Canvas>") as FrameworkElement;
        private bool inUpdatingSatValSelection = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Create a new instance of the ColorPicker control.
        /// </summary>
        public ColorPicker()
        {
            DefaultStyleKey = typeof(ColorPicker);
            m_colorSpace = new ColorSpace();
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Event fired when the selected color changes.  This event occurs when the 
        /// left-mouse button is lifted after clicking.
        /// </summary>
        public event SelectedColorChangedHandler SelectedColorChanged;

        /// <summary>
        /// Event fired when the selected color is changing.  This event occurs when the 
        /// left-mouse button is pressed and the user is moving the mouse.
        /// </summary>
        public event SelectedColorChangingHandler SelectedColorChanging;

        #endregion Events

        #region Properties

        public double Alpha
        {
            get
            {
                return (double)GetValue(AlphaProperty);
            }

            set
            {
                SetValue(AlphaProperty, value);
            }
        }

        public double ColorXPosition
        {
            get
            {
                return (double)GetValue(ColorXPositionProperty);
            }

            set
            {
                SetValue(ColorXPositionProperty, value);
            }
        }

        public double ColorYPosition
        {
            get
            {
                return (double)GetValue(ColorYPositionProperty);
            }

            set
            {
                SetValue(ColorYPositionProperty, value);
            }
        }

        public Brush HueMonitorBrush
        {
            get
            {
                return (Brush)GetValue(HueMonitorBrushProperty);
            }
        }

        public double HuePosition
        {
            get
            {
                return (double)GetValue(HuePositionProperty);
            }

            set
            {
                SetValue(HuePositionProperty, value);
            }
        }

        public Brush RGBColorBrush
        {
            get
            {
                return (Brush)GetValue(RGBColorBrushProperty);
            }

            set
            {
                SetValue(RGBColorBrushProperty, value);
            }
        }

        public Brush SatValBrush
        {
            get
            {
                return (Brush)GetValue(SatValBrushProperty);
            }

            set
            {
                SetValue(SatValBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the currently selected color in the Color Picker.
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set
            {
                SetValue(SelectedColorProperty, value);
                //this.UpdateVisuals();
            }
        }

        public SolidColorBrush SelectedColorBrush
        {
            get
            {
                return (SolidColorBrush)GetValue(SelectedColorBrushProperty);
            }

            set
            {
                SetValue(SelectedColorBrushProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Builds the visual tree for the ColorPicker control when the template is applied. 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateVisuals();
            SetSelectedColorBrush(SelectedColor);
        }

        private static Brush InitializeHueBrush()
        {
            var stops = new GradientStopCollection();
            stops.Add(new GradientStop { Color = Color.FromArgb(0xff, 0xff, 0x00, 0x00), Offset = 0 });
            stops.Add(new GradientStop { Color = Color.FromArgb(0xff, 0xff, 0xff, 0x00), Offset = 0.17 });
            stops.Add(new GradientStop { Color = Color.FromArgb(0xff, 0x00, 0xff, 0x00), Offset = 0.33 });
            stops.Add(new GradientStop { Color = Color.FromArgb(0xff, 0x00, 0xff, 0xff), Offset = 0.5 });
            stops.Add(new GradientStop { Color = Color.FromArgb(0xff, 0x00, 0x00, 0xff), Offset = 0.66 });
            stops.Add(new GradientStop { Color = Color.FromArgb(0xff, 0xff, 0x00, 0xff), Offset = 0.83 });
            stops.Add(new GradientStop { Color = Color.FromArgb(0xff, 0xff, 0x00, 0x00), Offset = 1 });
            return new LinearGradientBrush(stops, 90);
        }

        private static void SelectedColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker p = d as ColorPicker;
            if (p != null)
            {
                var newColor = (Color)e.NewValue;
                p.SetSelectedColorBrush(newColor);
                p.UpdateVisuals();
                if (p.SelectedColorChanged != null)
                {
                    SelectedColorEventArgs args = new SelectedColorEventArgs(newColor);
                    p.SelectedColorChanged(p, args);
                }
            }
        }

        private void FireSelectedColorChangingEvent(Color selectedColor)
        {
            if (SelectedColorChanging != null)
            {
                SelectedColorEventArgs args = new SelectedColorEventArgs(selectedColor);
                SelectedColorChanging(this, args);
            }
        }

        private byte GetAlphaByte()
        {
            return (byte)(Math.Max(Math.Min(1d, Alpha), 0) * 255);
        }

        private Color GetColor()
        {
            double yComponent = ColorYPosition;
            double xComponent = ColorXPosition;
            double hueComponent = HuePosition * 360;

            return m_colorSpace.ConvertHsvToRgb(GetAlphaByte(), hueComponent, xComponent, yComponent);
        }

        /// <summary>
        /// handles the AlphaProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnAlphaChanged(double oldValue, double newValue)
        {
            UpdateHueSelection();
        }

        /// <summary>
        /// handles the ColorXPositionProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnColorXPositionChanged(double oldValue, double newValue)
        {
            UpdateSatValSelection();
        }

        /// <summary>
        /// handles the ColorYPositionProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnColorYPositionChanged(double oldValue, double newValue)
        {
            UpdateSatValSelection();
        }

        /// <summary>
        /// handles the HuePositionProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnHuePositionChanged(double oldValue, double newValue)
        {
            UpdateHueSelection();
        }

        private void SetSelectedColorBrush(Color newColor)
        {
            SelectedColorBrush = new SolidColorBrush(newColor);
            RGBColorBrush = new SolidColorBrush(Color.FromArgb(255, newColor.R, newColor.G, newColor.B));
        }

        private void UpdateColorBrush()
        {
            Shape rectVisual = colorSampleVisual.FindName("ColorSample") as Shape;
            Color c = m_colorSpace.GetColorFromPosition(byte.MaxValue, HuePosition * byte.MaxValue);
            rectVisual.Fill = new SolidColorBrush(c);
            WriteableBitmap bitmap = new WriteableBitmap(colorSampleVisual, null);
            SatValBrush = new ImageBrush { ImageSource = bitmap };
        }

        private void UpdateHueSelection()
        {
            UpdateColorBrush();

            Color currColor = GetColor();
            FireSelectedColorChangingEvent(currColor);
            SelectedColor = currColor;
        }

        private void UpdateSatValSelection()
        {
            if (!inUpdatingSatValSelection)
            {
                inUpdatingSatValSelection = true;
                try
                {
                    Color currColor = GetColor();

                    FireSelectedColorChangingEvent(currColor);
                    SelectedColor = currColor;
                }
                finally
                {
                    inUpdatingSatValSelection = false;
                }
            }
        }

        private void UpdateVisuals()
        {
            if (inUpdatingSatValSelection)
                return;

            Color c = this.SelectedColor;
            Alpha = (double)c.A / byte.MaxValue;
            ColorSpace cs = new ColorSpace();
            HSV hsv = cs.ConvertRgbToHsv(c);

            HuePosition = hsv.Hue / 360;
            ColorYPosition = hsv.Value;
            ColorXPosition = hsv.Saturation;
            UpdateColorBrush();
        }

        #endregion Methods
    }
}