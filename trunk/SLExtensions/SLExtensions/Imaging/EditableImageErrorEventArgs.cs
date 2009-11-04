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

namespace SLExtensions.Imaging
{
    /// <summary>
    /// Event data for the ImageError event.
    /// </summary>
    public class EditableImageErrorEventArgs : EventArgs
    {
        /// Original Editable Image class courtesy Joe Stegman.
        /// http://blogs.msdn.com/jstegman

        private string _errorMessage = string.Empty;

        /// <summary>
        /// The error message indicating the error condition experienced by EditableImage.
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }
    }
}
