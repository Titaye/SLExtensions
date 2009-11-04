namespace SLExtensions.Showcase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    // import slextensions extension methods
    using SLExtensions;
    using SLExtensions.Collections.ObjectModel;

    public partial class PageWebClient : UserControl
    {
        #region Fields

        private ObservableCollection<PageWebClientPostData> postData;

        #endregion Fields

        #region Constructors

        public PageWebClient()
        {
            InitializeComponent();
            postData = new ObservableCollection<PageWebClientPostData>();
            postDataGrid.ItemsSource = postData;
        }

        #endregion Constructors

        #region Methods

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            postData.Add(new PageWebClientPostData());
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            WebClient webclient = new WebClient();
            webclient.UploadStringCompleted += new UploadStringCompletedEventHandler(webclient_UploadStringCompleted);
            webclient.SendHtmlForm(
                new Uri(Application.Current.Host.Source, "../PostResult.aspx"),
                (from data in postData select new KeyValuePair<string, string>(data.Name, data.Value)));
        }

        private void upload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "all files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == true)
            {
                WebClient webclient = new WebClient();
                webclient.UploadStringCompleted += new UploadStringCompletedEventHandler(webclient_FileUploadStringCompleted);
                webclient.UploadProgressChanged += new UploadProgressChangedEventHandler(webclient_UploadProgressChanged);
                webclient.SendFile(
                    new Uri(Application.Current.Host.Source, "../PostFileResult.aspx"),
                    ofd.File.OpenRead(),
                    ofd.File.Name,
                    "uploadField",
                    null,
                    null);
            }
        }

        void webclient_FileUploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            WebClient webclient = (WebClient)sender;
            webclient.UploadStringCompleted -= new UploadStringCompletedEventHandler(webclient_FileUploadStringCompleted);
            webclient.UploadProgressChanged -= new UploadProgressChangedEventHandler(webclient_UploadProgressChanged);
            result2.Text = e.Result;
        }

        void webclient_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            progress.Text = "Progress : " + e.ProgressPercentage + " % (" + e.BytesSent + " / " + e.TotalBytesToSend + ")";
        }

        void webclient_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            WebClient webclient = (WebClient) sender;
            webclient.UploadStringCompleted -= new UploadStringCompletedEventHandler(webclient_UploadStringCompleted);
            result.Text = e.Result;
        }

        #endregion Methods
    }

    public class PageWebClientPostData
    {
        #region Properties

        public string Name
        {
            get; set;
        }

        public string Value
        {
            get; set;
        }

        #endregion Properties
    }
}