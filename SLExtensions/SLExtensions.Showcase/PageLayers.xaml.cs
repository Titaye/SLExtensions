namespace SLExtensions.Showcase
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions.Controls.Layers;

    public partial class PageLayers : UserControl
    {
        #region Fields

        private int idx = 3;
        Random rnd = new Random();

        #endregion Fields

        #region Constructors

        public PageLayers()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Page_Loaded);
        }

        #endregion Constructors

        #region Methods

        private void btnItems_Click(object sender, RoutedEventArgs e)
        {
            TestData datasource = (TestData)Resources["datasource"];
            datasource.Data.Add(new TestData() { GPSPoint = new Point((double)rnd.Next(-18000, +18000) / 100, (double)rnd.Next(-8000, +8000) / 100) });
            //<!--<map:LayerSource ItemsSource="{Binding Data, Source={StaticResource datasource}}" ItemTemplate="{StaticResource itemTemplate}" map:LayerHost.Layer="pointsLayer"  />-->
            //ptSource.ItemsSource = new TestData().Data;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            idx++;

            int viewIdx = idx % 7;
            loadView(viewIdx);
        }

        private void loadView(int viewIdx)
        {
            List<LayerSource> newSources = new List<LayerSource>();

            foreach (var item in map.GetLayerContents("tilesLayer"))
            {
                map.LayerSources.Remove((LayerSource)item);
            }

            switch (viewIdx)
            {
                case 0:
                    newSources.Add(new SLExtensions.Controls.Layers.VirtualEarth.AerialView());
                    viewName.Text = "Virtual Earth Aerial";
                    break;
                case 1:
                    newSources.Add(new SLExtensions.Controls.Layers.VirtualEarth.RoadView());
                    viewName.Text = "Virtual Earth Road";
                    break;
                case 2:
                    newSources.Add(new SLExtensions.Controls.Layers.VirtualEarth.HybridView());
                    viewName.Text = "Virtual Earth Satellite";
                    break;
                case 3:
                    newSources.Add(new SLExtensions.Controls.Layers.GoogleMaps.AerialView());
                    viewName.Text = "GoogleMap Aerial";
                    break;
                case 4:
                    newSources.Add(new SLExtensions.Controls.Layers.GoogleMaps.AerialView());
                    newSources.Add(new SLExtensions.Controls.Layers.GoogleMaps.RoadView() { RoadViewType = SLExtensions.Controls.Layers.GoogleMaps.RoadViewType.RoadOnly });
                    viewName.Text = "GoogleMap Aerial + road";
                    break;
                case 5:
                    newSources.Add(new SLExtensions.Controls.Layers.GoogleMaps.RoadView() { RoadViewType = SLExtensions.Controls.Layers.GoogleMaps.RoadViewType.Terrain });
                    viewName.Text = "GoogleMap Terrain";
                    break;
                case 6:
                    newSources.Add(new SLExtensions.Controls.Layers.GoogleMaps.RoadView());
                    viewName.Text = "GoogleMap Road";
                    break;
            }
            idx = viewIdx;

            foreach (var item in newSources)
            {
                LayerHost.SetLayer(item, "tilesLayer");
                map.LayerSources.Add(item);
            }
        }

        private void moveRight_Click(object sender, RoutedEventArgs e)
        {
            // Set the point at x:250px at the current zoom level as the new top left corner
            map.SetTopLeftPoint(new Point(250, 0), true);
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadView(0);
            //LayoutRoot.Children.Add(new SLExtensions.Controls.Layers.GoogleMaps.AerialView());
        }

        private void zoomIn_Click(object sender, RoutedEventArgs e)
        {
            map.ZoomAndInvariantLocal(map.Zoom * 1.1, new Point(), true);
        }

        private void zoomOut_Click(object sender, RoutedEventArgs e)
        {
            map.ZoomAndInvariantLocal(map.Zoom / 1.1, new Point(), true);
        }

        #endregion Methods
    }

    public class TestData
    {
        #region Fields

        private ObservableCollection<TestData> data = new ObservableCollection<TestData>();

        #endregion Fields

        #region Properties

        public ObservableCollection<TestData> Data
        {
            get
            {
                return data;
                //return new TestData[] {
                //    new TestData() { GPSPoint = new Point(90, 45) },
                //    new TestData() { GPSPoint = new Point(90, 0) },
                //    new TestData() { GPSPoint = new Point(90, -45) },
                //    new TestData() { GPSPoint = new Point(0, 45) },
                //    new TestData() { GPSPoint = new Point(0, 0) },
                //    new TestData() { GPSPoint = new Point(0, -45) },
                //    new TestData() { GPSPoint = new Point(-90, 45) },
                //    new TestData() { GPSPoint = new Point(-90, 0) },
                //    new TestData() { GPSPoint = new Point(-90, -45) },
                //};
            }
        }

        public Point GPSPoint
        {
            get; set;
        }

        #endregion Properties
    }
}