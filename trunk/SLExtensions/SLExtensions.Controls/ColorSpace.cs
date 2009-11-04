namespace SLExtensions.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    /// <summary>
    /// Data structure that represents a HSV value.
    /// </summary>
    internal struct HSV
    {
        #region Fields

        private readonly double m_hue;
        private readonly double m_saturation;
        private readonly double m_value;

        #endregion Fields

        #region Constructors

        public HSV(double hue, double saturation, double value)
        {
            m_hue = hue;
            m_saturation = saturation;
            m_value = value;
        }

        #endregion Constructors

        #region Properties

        public double Hue
        {
            get { return m_hue; }
        }

        public double Saturation
        {
            get { return m_saturation; }
        }

        public double Value
        {
            get { return m_value; }
        }

        #endregion Properties
    }

    /// <summary>
    /// Contains helper methods for use by the ColorPicker control.
    /// </summary>
    internal class ColorSpace
    {
        #region Fields

        private const byte MAX = 255;
        private const byte MIN = 0;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Converts from Hue/Sat/Val (HSV) color space to Red/Green/Blue color space.
        /// Algorithm ported from: http://www.colorjack.com/software/dhtml+color+picker.html
        /// </summary>
        /// <param name="h">The Hue value.</param>
        /// <param name="s">The Saturation value.</param>
        /// <param name="v">The Value value.</param>
        /// <returns></returns>
        public Color ConvertHsvToRgb(double h, double s, double v)
        {
            h = h / 360;
            if (s > 0)
            {
                if (h >= 1)
                    h = 0;
                h = 6 * h;
                int hueFloor = (int)Math.Floor(h);
                byte a = (byte)Math.Round(MAX * v * (1.0 - s));
                byte b = (byte)Math.Round(MAX * v * (1.0 - (s * (h - hueFloor))));
                byte c = (byte)Math.Round(MAX * v * (1.0 - (s * (1.0 - (h - hueFloor)))));
                byte d = (byte)Math.Round(MAX * v);

                switch (hueFloor)
                {
                    case 0: return Color.FromArgb(MAX, d, c, a);
                    case 1: return Color.FromArgb(MAX, b, d, a);
                    case 2: return Color.FromArgb(MAX, a, d, c);
                    case 3: return Color.FromArgb(MAX, a, b, d);
                    case 4: return Color.FromArgb(MAX, c, a, d);
                    case 5: return Color.FromArgb(MAX, d, a, b);
                    default: return Color.FromArgb(0, 0, 0, 0);
                }
            }
            else
            {
                byte d = (byte)(v * MAX);
                return Color.FromArgb(255, d, d, d);
            }
        }

        /// <summary>
        /// Converts from the Red/Green/Blue color space to the Hue/Sat/Val (HSV) color space.
        /// Algorithm ported from: http://www.codeproject.com/KB/recipes/colorspace1.aspx
        /// </summary>
        /// <param name="c">The color to convert.</param>
        /// <returns></returns>
        public HSV ConvertRgbToHsv(Color c)
        {
            // normalize red, green and blue values

            double r = (c.R / 255.0);
            double g = (c.G / 255.0);
            double b = (c.B / 255.0);

            // conversion start

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            double h = 0.0;
            if (max == r && g >= b)
            {
               h = 60 * (g - b) / (max - min);
            }
            else if (max == r && g < b)
            {
                h = 60 * (g - b) / (max - min) + 360;
            }
            else if (max == g)
            {
                h = 60 * (b - r) / (max - min) + 120;
            }
            else if (max == b)
            {
                h = 60 * (r - g) / (max - min) + 240;
            }

            double s = (max == 0) ? 0.0 : (1.0 - (min / max));

            return new HSV(h, s, max);
        }

        public Color GetColorFromPosition(double position)
        {
            int gradientStops = 6;
            position *= gradientStops;
            byte mod = (byte)(position % MAX);
            byte diff = (byte)(MAX - mod);

            switch ((int)(position / MAX))
            {
                case 0: return Color.FromArgb(MAX, MAX, mod, MIN);
                case 1: return Color.FromArgb(MAX, diff, MAX, MIN);
                case 2: return Color.FromArgb(MAX, MIN, MAX, mod);
                case 3: return Color.FromArgb(MAX, MIN, diff, MAX);
                case 4: return Color.FromArgb(MAX, mod, MIN, MAX);
                case 5: return Color.FromArgb(MAX, MAX, MIN, diff);
                case 6: return Color.FromArgb(MAX, MAX, mod, MIN);
                default: return Colors.Black;
            }
        }

        public string GetHexCode(Color c)
        {
            return string.Format("#{0}{1}{2}",
                c.R.ToString("X2"),
                c.G.ToString("X2"),
                c.B.ToString("X2"));
        }

        #endregion Methods
    }
}