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

namespace SLExtensions
{
    public class DataEventArgs<T> : EventArgs
    {
        public DataEventArgs(T data)
        {
            this.Data = data;
        }

        public T Data { get; private set; }
    }
}
