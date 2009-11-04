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

    using SLExtensions.Input;

    public static class DisposeHelper
    {
        #region Methods

        public static void DisposeRecursive(this UIElement element)
        {
            Panel panel = element as Panel;
            if (panel != null)
            {
                foreach (var item in panel.Children)
                {
                    item.DisposeRecursive();
                }
            }

            Border bd = element as Border;
            if (bd != null && bd.Child != null)
            {
                bd.Child.DisposeRecursive();
            }

            ContentControl cc = element as ContentControl;
            if (cc != null && cc.Content != null)
            {
                UIElement ccElem = cc.Content as UIElement;
                if (ccElem != null)
                    ccElem.DisposeRecursive();

                IDisposable ccDisp = cc.Content as IDisposable;
                if (ccDisp != null)
                    ccDisp.Dispose();
            }

            ContentPresenter cp = element as ContentPresenter;
            if (cp != null && cp.Content != null)
            {
                UIElement cpElem = cp.Content as UIElement;
                if (cpElem != null)
                    cpElem.DisposeRecursive();

                IDisposable cpDisp = cp.Content as IDisposable;
                if (cpDisp != null)
                    cpDisp.Dispose();
            }

            ToolTipService.SetToolTip(element, null);
            CommandService.UnregisterCommands(element as FrameworkElement);

            //FrameworkElement fe = element as FrameworkElement;
            //if (fe != null)
            //{
            //    foreach (var item in fe.Resources)
            //    {
            //        UIElement elemRes = item.Value as UIElement;
            //        if (elemRes != null)
            //            elemRes.DisposeRecursive();

            //        IDisposable dispRes = item.Value as IDisposable;
            //        if (dispRes != null)
            //        {
            //            dispRes.Dispose();
            //        }
            //    }
            //}

            IDisposable disposable = element as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        #endregion Methods
    }
}