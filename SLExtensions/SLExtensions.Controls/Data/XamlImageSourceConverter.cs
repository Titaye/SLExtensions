namespace SLExtensions.Data
{
    using System;
    using System.IO;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    using SLExtensions.Data;
    using SLExtensions.Imaging;

    public class XamlImageSourceConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte[] content = value as byte[];
            if (content != null)
            {
                BitmapImage source = new BitmapImage();
                source.SetSource(new MemoryStream(content));
                return source;
            }

            Uri uri = new UriConverter().Convert(value, targetType, parameter, culture) as Uri;
            if (uri != null)
            {
                string extensions = uri.ToString();
                if (StringComparer.OrdinalIgnoreCase.Compare(System.IO.Path.GetExtension(extensions), ".xaml") == 0)
                {
                    var brush = new BitmapImage();
                    var stream = Application.GetResourceStream(uri);
                    if (stream != null)
                    {
                        // Load from application resources
                        content = new byte[stream.Stream.Length];
                        stream.Stream.Read(content, 0, content.Length);
                        SetSourceFromXaml(brush, System.Text.Encoding.UTF8.GetString(content, 0, content.Length));
                    }
                    else
                    {
                        // Load from web
                        WebClient wc = new WebClient();
                        wc.DownloadStringCompleted += (s, e) =>
                        {
                            if (e.Error == null && !e.Cancelled)
                            {
                                SetSourceFromXaml(brush, e.Result);
                            }
                        };
                        wc.DownloadStringAsync(uri);
                    }
                    return brush;
                }
                else
                {
                    return new BitmapImage(uri);
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private void SetSourceFromXaml(BitmapImage brush, string xaml)
        {
            try
            {
                var fe = XamlReader.Load(xaml) as FrameworkElement;
                fe.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                fe.Arrange(new Rect(0, 0, fe.DesiredSize.Width, fe.DesiredSize.Height));
                WriteableBitmap bmp = new WriteableBitmap(fe, null);
                var stream = bmp.SaveAsPng();
                brush.SetSource(stream);
            }
            catch
            {
            }
        }

        #endregion Methods
    }
}