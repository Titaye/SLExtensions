namespace SLMedia.Core
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

    //: FrameworkElement
    public class MarkerSelectorCommandParameter
    {
        #region Properties

        public bool AllowMultipleActive
        {
            get; set;
        }

        public string Key
        {
            get; set;
        }

        public object Value
        {
            get; set;
        }

        #endregion Properties

        #region Other

        //#region Key
        //public string Key
        //{
        //    get
        //    {
        //        return (string)GetValue(KeyProperty);
        //    }
        //    set
        //    {
        //        SetValue(KeyProperty, value);
        //    }
        //}
        ///// <summary>
        ///// Key depedency property.
        ///// </summary>
        //public static readonly DependencyProperty KeyProperty =
        //    DependencyProperty.Register(
        //        "Key",
        //        typeof(string),
        //        typeof(MarkerSelectorCommandParameter), null);
        //#endregion Key
        //public object Value
        //{
        //    get { return GetValue(ValueProperty); }
        //    set { SetValue(ValueProperty, value); }
        //}
        //// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ValueProperty =
        //    DependencyProperty.Register("Value", typeof(object), typeof(MarkerSelectorCommandParameter), null);
        //#region AllowMultipleActive
        //public bool AllowMultipleActive
        //{
        //    get
        //    {
        //        return (bool)GetValue(AllowMultipleActiveProperty);
        //    }
        //    set
        //    {
        //        SetValue(AllowMultipleActiveProperty, value);
        //    }
        //}
        ///// <summary>
        ///// AllowMultipleActive depedency property.
        ///// </summary>
        //public static readonly DependencyProperty AllowMultipleActiveProperty =
        //    DependencyProperty.Register(
        //        "AllowMultipleActive",
        //        typeof(bool),
        //        typeof(MarkerSelectorCommandParameter),
        //        null);
        //#endregion AllowMultipleActive

        #endregion Other
    }
}