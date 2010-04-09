namespace SLExtensions.Showcase
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using SLExtensions.Controls;

    public class CustomCoolMenuBehavior : DefaultCoolMenuBehavior
    {
        #region Methods

        public override void Initialize(CoolMenu parent, CoolMenuItem element)
        {
            base.Initialize(parent, element);
            element.Background = new SolidColorBrush(Colors.Yellow);
        }

        #endregion Methods
    }

    public partial class PageCoolMenu : UserControl
    {
        #region Fields

        private readonly Random m_random;

        #endregion Fields

        #region Constructors

        public PageCoolMenu()
        {
            InitializeComponent();
            this.Loaded += CoolMenuSample_Loaded;
            m_random = new Random();
        }

        #endregion Constructors

        #region Methods

        private void AddRectangle()
        {
            coolMenuRectangle.Items.Add(new Rectangle
                                            {
                                                Margin = new Thickness(5),
                                                StrokeThickness = 10,
                                                Stroke = new SolidColorBrush(GetRandomColor())
                                            });
        }

        void CoolMenuSample_Loaded(object sender, RoutedEventArgs e)
        {
            var col = new System.Collections.ObjectModel.ObservableCollection<string>
                {
                    "/Assets/CoolMenuImages/Icons/box.png",
                    "/Assets/CoolMenuImages/Icons/bomb.png",
                    "/Assets/CoolMenuImages/Icons/calc.png",
                    "/Assets/CoolMenuImages/Icons/fish.png",
                    "/Assets/CoolMenuImages/Icons/star.png"
                };
            // CoolMenuBinding.ItemsSource = col;

            for (int i = 0; i < 6; ++i)
                AddRectangle();
        }

        private Color GetRandomColor()
        {
            byte[] colorBytes = new byte[3];
            m_random.NextBytes(colorBytes);
            return Color.FromArgb(255, colorBytes[0], colorBytes[1], colorBytes[2]);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddRectangle();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (coolMenuRectangle.Items.Count > 0)
                coolMenuRectangle.Items.RemoveAt(coolMenuRectangle.Items.Count - 1);
        }

        #endregion Methods
    }
}