namespace SLExtensions.Controls.Animation
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class GoToStateCommandParameterTypeConverter : TypeConverter
    {
        #region Methods

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string text = value as string;

            if (string.IsNullOrEmpty(text))
                return null;

            string[] parts = text.Split(' ');
            if(parts.Length == 1)
                return new GoToStateCommandParameter { StateName = parts[0] };

            if (parts.Length != 2)
                return null;

            return new GoToStateCommandParameter { ElementName = parts[0], StateName = parts[1] };
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
                return null;

            GoToStateCommandParameter prm = value as GoToStateCommandParameter;
            if (prm == null)
                return null;

            return string.Concat(prm.ElementName, " ", prm.StateName);
        }

        #endregion Methods
    }
}