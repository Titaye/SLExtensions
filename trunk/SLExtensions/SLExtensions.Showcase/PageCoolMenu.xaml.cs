using System.Windows.Shapes;

namespace SLExtensions.Showcase
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using SLExtensions.Controls;

    public partial class PageCoolMenu : UserControl
    {
        private readonly Random m_random;

        public PageCoolMenu()
        {
            InitializeComponent();
            this.Loaded += CoolMenuSample_Loaded;
            m_random = new Random();
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddRectangle();
        }

        private void AddRectangle()
        {
            coolMenuRectangle.Items.Add(new Rectangle
                                            {
                                                Margin = new Thickness(5),
                                                StrokeThickness = 10,
                                                Stroke = new SolidColorBrush(GetRandomColor())
                                            });
        }

        private Color GetRandomColor()
        {
            byte[] colorBytes = new byte[3];
            m_random.NextBytes(colorBytes);
            return Color.FromArgb(255, colorBytes[0], colorBytes[1], colorBytes[2]);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (coolMenuRectangle.Items.Count > 0)
                coolMenuRectangle.Items.RemoveAt(coolMenuRectangle.Items.Count - 1);
        }
    }

    public class CustomCoolMenuBehavior : DefaultCoolMenuBehavior
    {
        public override void Initialize(CoolMenu parent, CoolMenuItem element)
        {
            base.Initialize(parent, element);
            element.Background = new SolidColorBrush(Colors.Yellow);
        }
    }
}