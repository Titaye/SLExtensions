namespace SLExtensions.Sharepoint
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

    public class RequiredFieldException : Exception
    {
        #region Constructors

        public RequiredFieldException()
        {
        }

        public RequiredFieldException(string message)
            : base(message)
        {
        }

        public RequiredFieldException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion Constructors
    }
}