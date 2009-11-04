// <copyright file="DownloadProgressBar.cs" company="Microsoft">
//     Copyright © Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>Implements the download preogress bar class</summary>
// <author>Microsoft Expression Encoder Team</author>
namespace ExpressionMediaPlayer
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

    /// <summary>
    /// This class represents a progress bar for downloading a media item. Extends ProgressBar 
    /// by allowing start offset indicating DownloadProgressOffset.
    /// </summary>
    [TemplatePart(Name = DownloadProgressBar.DeterminateRoot, Type = typeof(FrameworkElement))]
    public class DownloadProgressBar : ProgressBar
    {
        /// <summary>
        /// String for the visual element of the progress bar.
        /// </summary>
        private const string DeterminateRoot = "DeterminateRoot";

        /// <summary>
        /// Visual element representing progress bar indicator.
        /// </summary>
        private FrameworkElement m_determinateRoot;

        /// <summary>
        /// Dependancy property for Download Offset.
        /// </summary>
        public static readonly DependencyProperty DownloadOffsetProperty = DependencyProperty.Register("DownloadOffset", typeof(Double), typeof(DownloadProgressBar), new PropertyMetadata(new PropertyChangedCallback(DownloadProgressBar.OnDownloadProgressPropertyChanged)));                

        /// <summary>
        /// Initializes a new instance of the DownloadProgressBar class.
        /// </summary>
        public DownloadProgressBar()
        {
            DownloadOffset = 0.0;            
        }

        /// <summary>
        /// Gets or sets the amount downloaded, ie. offset to the start of the downloaded progress indicator.
        /// </summary>
        public Double DownloadOffset
        {
            get
            {
                return (Double)this.GetValue(DownloadOffsetProperty);
            }

            set
            {
                this.SetValue(DownloadOffsetProperty, value);
            }
        }

        /// <summary>
        /// Overridden OnApplyTemplate, sets the DeterminateRoot member, which is the visual element for the progress indicator.
        /// </summary>
        public override void OnApplyTemplate()
        {
 	        base.OnApplyTemplate();
            m_determinateRoot = GetTemplateChild(DeterminateRoot) as FrameworkElement;
        }

        /// <summary>
        /// Update the visual for the offset.
        /// </summary>
        /// <param name="offset">Amount of offset (gets bounded between Minimum and Maximum).</param>
        public void SetOffsetVisual(double offset)
        {
            offset = Math.Max(Math.Min(offset, Maximum), Minimum);
            Double left = ((offset - Minimum) / (Maximum - Minimum)) * ActualWidth;
            if (m_determinateRoot != null)
            {
                m_determinateRoot.Margin = new Thickness(left, m_determinateRoot.Margin.Top, m_determinateRoot.Margin.Right, m_determinateRoot.Margin.Bottom);
            }
        }

        /// <summary>
        /// Called when "DownloadProgressOffset" is set.
        /// </summary>
        /// <param name="obj">DownloadProgressBar source.</param>
        /// <param name="args">New value etc.</param>
        protected static void OnDownloadProgressPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DownloadProgressBar downloadProgressBar = obj as DownloadProgressBar;
            if (downloadProgressBar != null)
            {
                downloadProgressBar.SetOffsetVisual((Double)args.NewValue);
            }
        } 
    }
}
