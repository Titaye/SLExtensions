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

    using SLMedia.Core;
    using SLMedia.PlaylistProvider.MSNVideo;
    using SLMedia.Video;

    public partial class PagePlayerVideo : UserControl
    {
        #region Constructors

        public PagePlayerVideo()
        {
            InitializeComponent();

            MSNVideoProvider msnProvider = new MSNVideoProvider();
            Uri uri = new Uri("http://edge1.catalog.video.msn.com/videoByTag.aspx?tag=frfr_cinetv&ns=MSNVideo_Top_Cat&mk=fr-fr&sf=ActiveStartDate&sd=-1&vs=0&ind=&ps=&rct=&ff=99");
            msnProvider.ContentSource = uri;
            player.Controller.PlaylistSource = msnProvider;
        }

        #endregion Constructors
    }
}