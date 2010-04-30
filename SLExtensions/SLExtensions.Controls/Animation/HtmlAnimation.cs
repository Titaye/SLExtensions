// <copyright file="HtmlAnimation.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Animation
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    /// <summary>
    /// Html animation manager
    /// </summary>
    public class HtmlAnimation
    {
        #region Fields

        private bool autoStart = true;

        /// <summary>
        /// timer for animation loop
        /// </summary>
        private DispatcherTimer timer;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Create a new HtmlAnimation
        /// </summary>
        public HtmlAnimation()
        {
            this.Pairs = new List<HtmlAnimationPair>();
            if (this.timer == null)
            {
                this.timer = new DispatcherTimer();

                this.timer.Interval = TimeSpan.FromMilliseconds(1000 / Application.Current.Host.Settings.MaxFrameRate);

                this.timer.Tick += this.Timer_Tick;
            }

            if (Application.Current.RootVisual != null && !System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                this.Start();
            }
        }

        #endregion Constructors

        #region Properties

        public bool AutoStart
        {
            get
            {
                return this.autoStart;
            }

            set
            {
                this.autoStart = value;
            }
        }

        /// <summary>
        /// Gets or sets ancestor for finding silverlight child
        /// </summary>
        public Control ControlOwner
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the pairs animated by this manager
        /// </summary>
        /// <value>The pairs.</value>
        public List<HtmlAnimationPair> Pairs
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public void Start()
        {
            if (!this.timer.IsEnabled)
                this.timer.Start();
        }

        public void Stop()
        {
            if (this.timer.IsEnabled)
                this.timer.Stop();
        }

        /// <summary>
        /// Animate pairs
        /// </summary>
        private void DoArrange()
        {
            foreach (HtmlAnimationPair item in this.Pairs)
            {
                if (item.IsValid(this))
                {
                    item.CopyToHtml(this);
                }
            }
        }

        /// <summary>
        ///  Call back for redrawing... it is the equivalent of every 'frame' 
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            this.DoArrange();
        }

        #endregion Methods
    }
}