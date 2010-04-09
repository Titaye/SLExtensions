namespace SLExtensions.Showcase
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using SLExtensions.Controls;

    public partial class PageColorPicker : UserControl
    {
        #region Fields

        private readonly Random m_rand;

        #endregion Fields

        #region Constructors

        public PageColorPicker()
        {
            InitializeComponent();
            m_rand = new Random();

            UpdateCurrentColor();
        }

        #endregion Constructors

        #region Methods

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            byte[] buff = new byte[3];
            m_rand.NextBytes(buff);
            ColorPicker1.SelectedColor = Color.FromArgb(255, buff[0], buff[1], buff[2]);
            UpdateCurrentColor();
        }

        private void ColorPicker1_SelectedColorChanged(object sender, SelectedColorEventArgs args)
        {
            UpdateCurrentColor();
        }

        private void ColorPicker1_SelectedColorChanging(object sender, SelectedColorEventArgs e)
        {
            tbCurrColor1.Text = string.Format("(Changing Event) Selected color: {0}", e.SelectedColor);
        }

        private void UpdateCurrentColor()
        {
            if (tbCurrColor != null)
            {
                tbCurrColor.Text = string.Format("(Changed Event) Selected color: {0}", ColorPicker1.SelectedColor);
            }
        }

        #endregion Methods
    }
}