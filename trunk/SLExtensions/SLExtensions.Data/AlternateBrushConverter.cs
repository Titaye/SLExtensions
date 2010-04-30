namespace SLExtensions.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class AlternateBrushConverter : IValueConverter
    {
        #region Fields

        private static Dictionary<string, SolidColorBrush> defaultBrushes;

        /// <summary>
        /// ARGB regular expression
        /// </summary>
        private static Regex regexArgb = new Regex(@"#(?<a>[a-fA-F\d]{2})?(?<r>[a-fA-F\d]{2})(?<g>[a-fA-F\d]{2})(?<b>[a-fA-F\d]{2})");
        private static SolidColorBrush transparentBrush;

        private int counter = 0;

        #endregion Fields

        #region Constructors

        static AlternateBrushConverter()
        {
            // Fill default brushes with static SolidColorBrush
            defaultBrushes = new Dictionary<string, SolidColorBrush>(StringComparer.OrdinalIgnoreCase);

            // Create a brush per Colors property
            foreach (var colorProperty in typeof(Colors).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
            {
                if (colorProperty.PropertyType == typeof(Color))
                {
                    // Delay SolidColorBrush creation until first call
                    defaultBrushes[colorProperty.Name] = null;

                }
            }

            // Store transparent brush as it will be used at each AlternateBrushConverter creation
            transparentBrush = GetColorFromCache("Transparent", null);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AlternateBrushConverter()
        {
            DefaultBrush = transparentBrush;
            Brushes = new List<SolidColorBrush>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Default brush
        /// </summary>
        public SolidColorBrush DefaultBrush
        {
            get;
            set;
        }


        /// <summary>
        /// brushes
        /// </summary>
        public List<SolidColorBrush> Brushes
        {
            get;
            set;
        }


        #endregion Properties

        #region Methods

        public object Convert(object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            AlternateBrushConverterParameter param;
            string colors = parameter as string;
            List<SolidColorBrush> brushes = new List<SolidColorBrush>();

            if (Brushes != null && Brushes.Count > 0)
            {
                brushes = Brushes;
            }
            else if (!string.IsNullOrEmpty(colors))
            {
                // parse color string like #FFFFFFFF Red #FFFFFF Blue
                param = new AlternateBrushConverterParameter();
                string[] colorCollection = colors.Split();
                foreach (var item in colorCollection)
                {
                    Match m = regexArgb.Match(item);
                    if (m.Success)
                    {
                        // Parameter entry is argb pattern

                        byte a = 255;
                        byte r;
                        byte g;
                        byte b;

                        if (m.Groups["a"].Success)
                            a = (byte)int.Parse(m.Groups["a"].Value, System.Globalization.NumberStyles.HexNumber);

                        r = (byte)int.Parse(m.Groups["r"].Value, System.Globalization.NumberStyles.HexNumber);
                        g = (byte)int.Parse(m.Groups["g"].Value, System.Globalization.NumberStyles.HexNumber);
                        b = (byte)int.Parse(m.Groups["b"].Value, System.Globalization.NumberStyles.HexNumber);

                        param.Brushes.Add(new SolidColorBrush(Color.FromArgb(a, r, g, b)));
                    }
                    else
                    {
                        param.Brushes.Add(GetColorFromCache(item, DefaultBrush));
                    }
                }
                brushes = param.Brushes;
            }
            else
            {
                //use given AlternateBrushConverterParameter
                param = (AlternateBrushConverterParameter)parameter;
                if (param == null)
                {
                    return null;
                }
            }



            // no brushes
            //if (param.Brushes.Count == 0)
            if (brushes.Count == 0)
                return null;

            int? intValue = value as int?;
            
            //int idx = counter % param.Brushes.Count;
            int idx = intValue.HasValue && intValue.Value > -1 ? intValue.Value % brushes.Count : counter % brushes.Count;
            counter++;

            return brushes[idx];
            //return param.Brushes[idx];
        }

        public object ConvertBack(object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static SolidColorBrush GetColorFromCache(string name, SolidColorBrush defaultBrush)
        {
            SolidColorBrush brush = null;
            if (defaultBrushes.TryGetValue(name, out brush))
            {
                if (brush == null)
                {
                    var colorProp = (from p in typeof(Colors).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                                     where StringComparer.OrdinalIgnoreCase.Compare(name, p.Name) == 0
                                     select p).First();
                    brush = new SolidColorBrush((Color)colorProp.GetValue(null, null));
                    defaultBrushes[colorProp.Name] = brush;
                }
            }

            return brush ?? defaultBrush ?? transparentBrush;
        }

        #endregion Methods
    }
}