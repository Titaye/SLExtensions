namespace SLExtensions.Imaging
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
    /// Event data for the ImageError event.
    /// </summary>
    public class EditableImageErrorEventArgs : EventArgs
    {
        #region Fields

        /// Original Editable Image class courtesy Joe Stegman.
        /// http://blogs.msdn.com/jstegman
        private string _errorMessage = string.Empty;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The error message indicating the error condition experienced by EditableImage.
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        #endregion Properties
    }
}