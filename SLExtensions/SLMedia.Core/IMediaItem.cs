namespace SLMedia.Core
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions.Collections.ObjectModel;

    public interface IMediaItem
    {
        #region Properties

        ObservableCollection<IMarkerSelector> ActiveMarkerSelectors
        {
            get;
        }

        [ScriptableMemberAttribute]
        IEnumerable<Category> Categories
        {
            get;
        }

        [ScriptableMemberAttribute]
        string Description
        {
            get;
        }

        [ScriptableMemberAttribute]
        string Id
        {
            get;
        }

        ObservableCollection<IMarkerSelector> MarkerSelectors
        {
            get;
        }

        IEnumerable<IMarkerSource> MarkerSources
        {
            get;
        }

        [ScriptableMemberAttribute]
        string Source
        {
            get;
        }

        [ScriptableMemberAttribute]
        Uri SourceUri
        {
            get;
        }

        [ScriptableMemberAttribute]
        string Thumbnail
        {
            get;
        }

        [ScriptableMemberAttribute]
        Uri ThumbnailUri
        {
            get;
        }

        [ScriptableMemberAttribute]
        string Title
        {
            get;
        }

        #endregion Properties

        #region Methods

        void LoadMarkers(IDictionary<string, object> autoactivatedMarkers);

        #endregion Methods
    }
}