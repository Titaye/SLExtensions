﻿namespace SLMedia.Video
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public interface IVideoItem
    {
        //#region Properties

        //VideoAdapter VideoAdapter
        //{
        //    get;
        //}

        //#endregion Properties

        VideoAdapter GetVideoAdapter(VideoAdapter videoAdapter);
    }
}