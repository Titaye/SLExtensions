namespace SLExtensions
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

    public static class IDictionaryExtensions
    {
        #region Methods

        public static List<T> Add<U, T>(this IDictionary<U, List<T>> dic, U key, T value)
        {
            List<T> list;
            if (!dic.TryGetValue(key, out list))
            {
                list = new List<T>();
                dic.Add(key, list);
            }
            list.Add(value);
            return list;
        }

        /// <summary>
        /// Try to get a value from the dictionary
        /// </summary>
        /// <typeparam name="U">the dictionary key type</typeparam>
        /// <typeparam name="T">the dictionary value type</typeparam>
        /// <param name="dic">the dictionary</param>
        /// <param name="key">the key to look for</param>
        /// <returns>returns the value if found, null otherwise</returns>
        public static T TryGetValue<U, T>(this IDictionary<U, T> dic, U key)
        {
            T result = default(T);
            if (key != null)
            {
                dic.TryGetValue(key, out result);
            }

            return result;
        }

        #endregion Methods
    }
}