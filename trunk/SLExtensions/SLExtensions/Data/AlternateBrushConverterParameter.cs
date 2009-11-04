namespace SLExtensions.Data
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
    using System.Collections.Generic;

    public class AlternateBrushConverterParameter
    {
        #region Properties

        private Color color1;
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

        private Color color2;
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

        public AlternateBrushConverterParameter()
        {
            Brushes = new List<Brush>();
        }

        public List<Brush> Brushes { get; private set; }
    }
}