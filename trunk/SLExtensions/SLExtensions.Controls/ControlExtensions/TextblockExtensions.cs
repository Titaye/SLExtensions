namespace SLExtensions.Controls
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class TextblockExtensions
    {
        #region Fields

        // Using a DependencyProperty as the backing store for MultilineText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MultilineTextProperty = 
            DependencyProperty.RegisterAttached("MultilineText", typeof(string), typeof(TextblockExtensions), new PropertyMetadata(MultilineChanged));

        #endregion Fields

        #region Methods

        public static string GetMultilineText(DependencyObject obj)
        {
            return (string)obj.GetValue(MultilineTextProperty);
        }

        public static void SetMultilineText(DependencyObject obj, string value)
        {
            obj.SetValue(MultilineTextProperty, value);
        }

        private static void MultilineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlock tb = d as TextBlock;
            if (tb == null)
                return;

            string newVal = e.NewValue as string;
            if (!string.IsNullOrEmpty(newVal))
            {
                string[] lines = Regex.Split(newVal, "\r?\n", RegexOptions.Multiline);
                tb.Inlines.Clear();
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i > 0)
                        tb.Inlines.Add(new LineBreak());
                    tb.Inlines.Add(new Run() { Text = lines[0] });
                }
            }
        }

        #endregion Methods
    }
}