namespace SLExtensions.Showcase.Controllers
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

    public class PageHtmlConverterController : NotifyingObject
    {
        #region Fields

        private string html = @"Sample text showing how <b>HtmlTextBlock</b> makes it <i>*easy*</i> to display <strong>rich</strong> text in a Silverlight application by using a <em>simple</em> subset of <u>well-formed</u> HTML markup and a custom control.
        <p>Experiment by typing <i>your own</i> markup into the text box at the bottom to see how it gets rendered by Silverlight's <b>TextBlock</b>, by the custom <b>HtmlTextBlock</b>, and by the browser.</p>
        <p>Note: <b>HtmlTextBlock</b> displays invalid <a href='http://en.wikipedia.org/wiki/Xhtml'>XHTML</a> as-is.</p>";

        #endregion Fields

        #region Properties

        public string Html
        {
            get { return html; }
            set
            {
                if (html != value)
                {
                    html = value;
                    this.OnPropertyChanged(this.GetPropertyName(n => n.Html));
                }
            }
        }

        #endregion Properties
    }
}
