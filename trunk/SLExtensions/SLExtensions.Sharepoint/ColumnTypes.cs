namespace SLExtensions.Sharepoint
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class ColumnTypes
    {
        #region Constructors

        static ColumnTypes()
        {
            RegisteredColumnTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            RegisteredColumnTypes.Add("URL", typeof(UrlColumnInfo));
            RegisteredColumnTypes.Add("Boolean", typeof(BoolColumnInfo));
            RegisteredColumnTypes.Add("MultiChoice", typeof(ChoiceColumnInfo));
            RegisteredColumnTypes.Add("DateTime", typeof(DateTimeColumnInfo));
            RegisteredColumnTypes.Add("Choice", typeof(ChoiceColumnInfo));
            RegisteredColumnTypes.Add("Number", typeof(NumberColumnInfo));
            RegisteredColumnTypes.Add("Lookup", typeof(LookupColumnInfo));
            RegisteredColumnTypes.Add("LookupMulti", typeof(LookupColumnInfo));
            FallbackColumnType = typeof(TextColumnInfo);
        }

        #endregion Constructors

        #region Properties

        public static Type FallbackColumnType
        {
            get; set;
        }

        public static Dictionary<string, Type> RegisteredColumnTypes
        {
            get; set;
        }

        #endregion Properties
    }
}