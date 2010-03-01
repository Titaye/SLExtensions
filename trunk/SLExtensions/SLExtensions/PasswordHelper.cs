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
    /// <summary>
    /// Provide attached property to enable DataBinding on PasswordBox
    /// </summary>
    public class PasswordHelper : DependencyObject
    {

        /// <summary>
        /// Indicates if a PasswordBox is DataBound
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetIsBound(PasswordBox obj)
        {
            return (bool)obj.GetValue(IsBoundProperty);
        }

        private static void SetIsBound(PasswordBox obj, bool value)
        {
            obj.SetValue(IsBoundProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsBound.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBoundProperty =
            DependencyProperty.RegisterAttached("IsBound", typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false));


        /// <summary>
        /// Text of the PasswordBox (bindable vision of Password property)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetText(PasswordBox obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        public static void SetText(PasswordBox obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(PasswordHelper), new PropertyMetadata(string.Empty,
                (sender, args) =>
                {
                    var pwdBox = sender as PasswordBox;
                    if (pwdBox == null)
                        return;
                    var nval = (args.NewValue as string) ?? string.Empty;
                    if(pwdBox.Password != nval)
                        pwdBox.Password = nval;
                    if (!GetIsBound(pwdBox))
                    {
                        pwdBox.PasswordChanged += delegate
                        {
                            SetText(pwdBox, pwdBox.Password);
                        };
                        SetIsBound(pwdBox, true);
                    }
                }));



    }
}
