namespace SLExtensions.Sharepoint.Data
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class ChoiceValuesFromListConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var columnName = parameter as string;
            var columns = value as IEnumerable;
            if (columns == null || string.IsNullOrEmpty(columnName))
                return null;

            ChoiceColumnInfo choiceCol = columns.OfType<ChoiceColumnInfo>().FirstOrDefault(c => StringComparer.OrdinalIgnoreCase.Compare(c.StaticName, columnName) == 0);
            if (choiceCol == null)
                return null;

            return choiceCol.Choices;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}