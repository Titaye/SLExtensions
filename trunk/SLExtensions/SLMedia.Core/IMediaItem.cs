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

    public interface IMediaItem
    {
        #region Properties

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
    }
}