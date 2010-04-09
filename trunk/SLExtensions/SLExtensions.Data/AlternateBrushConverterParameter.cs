namespace SLExtensions.Data
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class AlternateBrushConverterParameter
    {
        #region Fields

        private Color color1;
        private Color color2;

        #endregion Fields

        #region Constructors

        public AlternateBrushConverterParameter()
        {
            Brushes = new List<Brush>();
        }

        #endregion Constructors

        #region Properties

        public List<Brush> Brushes
        {
            get; private set;
        }

        public Color Color1
        {
            get { return color1; }
            set
            {
                color1 = value;
                if (Brushes.Count == 0)
                    Brushes.Add(new SolidColorBrush(value));
                else
                    Brushes[0] = new SolidColorBrush(value);
            }
        }

        public Color Color2
        {
            get { return color2; }
            set
            {
                color2 = value;
                while(Brushes.Count < 1)
                    Brushes.Add(new SolidColorBrush(Colors.Transparent));

                if (Brushes.Count == 1)
                    Brushes.Add(new SolidColorBrush(value));
                else
                    Brushes[1] = new SolidColorBrush(value);
            }
        }

        #endregion Properties
    }
}