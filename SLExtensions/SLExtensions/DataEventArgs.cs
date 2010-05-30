namespace SLExtensions
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

    public class DataEventArgs<T> : EventArgs
    {
        #region Constructors

        public DataEventArgs(T data)
        {
            this.Data = data;
        }

        #endregion Constructors

        #region Properties

        public T Data
        {
            get; private set;
        }

        #endregion Properties
    }

    public static class DataEventArgs
    {
        #region Methods

        public static DataEventArgs<T> Create<T>(T data)
        {
            return new DataEventArgs<T>(data);
        }

        #endregion Methods
    }
}