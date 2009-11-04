namespace SLExtensions.Showcase
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using SLExtensions.Controls;
    using SLExtensions.Imaging;
    using System.Windows.Media.Imaging;

    public partial class PageDynamicImaging : UserControl
    {
        public PageDynamicImaging()
        {
            InitializeComponent();

            EditableImage img = new EditableImage(255, 255);
            for (int i = 0; i < img.Height; ++i)
                for (int j = 0; j < img.Width; ++j)
                    img.SetPixel(i, j, (byte)i, 255, (byte)j, 255);

            BitmapImage bmp = new BitmapImage();
            bmp.SetSource(img.GetStream());
            imgTesting.Source = bmp;
        }

    }
}