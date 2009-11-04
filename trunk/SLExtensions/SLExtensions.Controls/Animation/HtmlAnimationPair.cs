// <copyright file="HtmlAnimationPair.cs" company="Ucaya">
// Distributed under Microsoft Public License (Ms-PL)
// </copyright>
// <author>Thierry Bouquain</author>
namespace SLExtensions.Controls.Animation
{
    using System;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Link an html position to a silverlight element during an animation loop
    /// </summary>
    public class HtmlAnimationPair
    {
        #region Fields

        /// <summary>
        /// Html container offset relative to html document
        /// </summary>
        private Point htmlContainerOffset;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlAnimationPair"/> class.
        /// </summary>
        public HtmlAnimationPair()
        {
            this.htmlContainerOffset = new Point();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the HTML container.
        /// </summary>
        /// <value>The HTML container.</value>
        public HtmlElement HtmlContainer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the HTML container.
        /// </summary>
        /// <value>The name of the HTML container.</value>
        public string HtmlContainerName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the HTML element.
        /// </summary>
        /// <value>The HTML element.</value>
        public HtmlElement HtmlElement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the HTML element.
        /// </summary>
        /// <value>The name of the HTML element.</value>
        public string HtmlElementName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the last position during the animation.
        /// </summary>
        /// <value>The last position.</value>
        public Point? LastPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the last size during the animation.
        /// </summary>
        /// <value>The last size.</value>
        public Size? LastSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the SL element.
        /// </summary>
        /// <value>The SL element.</value>
        public FrameworkElement SLElement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the SL element.
        /// </summary>
        /// <value>The name of the SL element.</value>
        public string SLElementName
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Copies the silverlight properties to the HTML element.
        /// </summary>
        /// <param name="baseAnim">The base anim.</param>
        public void CopyToHtml(HtmlAnimation baseAnim)
        {
            if (baseAnim == null || baseAnim.ControlOwner == null)
            {
                return;
            }

            GeneralTransform transform = this.SLElement.TransformToVisual(Application.Current.RootVisual as UIElement);
            Point newLocation = transform.Transform(new Point());
            Point newBottomRight = transform.Transform(new Point(this.SLElement.ActualWidth, this.SLElement.ActualHeight));
            
            //Size newSize = new Size(this.SLElement.ActualWidth, this.SLElement.ActualHeight);
            Size newSize = new Size(newBottomRight.X - newLocation.X, newBottomRight.Y - newLocation.Y);

            if (newLocation != this.LastPosition || newSize != this.LastSize)
            {
                this.LastPosition = newLocation;
                this.LastSize = newSize;

                if (HtmlElement != null)
                {
                    HtmlElement.SetStyleAttribute("left", ((int)(newLocation.X - this.htmlContainerOffset.X)).ToString() + "px");
                    HtmlElement.SetStyleAttribute("top", ((int)(newLocation.Y - this.htmlContainerOffset.Y)).ToString() + "px");
                    HtmlElement.SetStyleAttribute("width", ((int)newSize.Width).ToString() + "px");
                    HtmlElement.SetStyleAttribute("height", ((int)newSize.Height).ToString() + "px");
                }
            }
        }

        /// <summary>
        /// Determines whether the specified base anim is valid.
        /// </summary>
        /// <param name="baseAnim">The base anim.</param>
        /// <returns>
        /// 	<c>true</c> if the specified base anim is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid(HtmlAnimation baseAnim)
        {
            if (baseAnim == null || baseAnim.ControlOwner == null)
            {
                return false;
            }

            if (this.HtmlElement == null)
            {
                this.HtmlElement = HtmlPage.Document.GetElementById(this.HtmlElementName);
            }

            if (this.HtmlContainer == null && !string.IsNullOrEmpty(this.HtmlContainerName))
            {
                this.HtmlContainer = HtmlPage.Document.GetElementById(this.HtmlContainerName);
                if (this.HtmlContainer != null)
                {
                    double ol = (double)this.HtmlContainer.GetProperty("offsetLeft");
                    double ot = (double)this.HtmlContainer.GetProperty("offsetTop");
                    this.htmlContainerOffset = new Point(ol, ot);
                }
            }

            if (this.SLElement == null)
            {
                this.SLElement = baseAnim.ControlOwner.FindName(this.SLElementName) as FrameworkElement;
            }

            if (this.HtmlElement != null && this.SLElement != null)
            {
                return true;
            }

            return false;
        }

        #endregion Methods
    }
}
