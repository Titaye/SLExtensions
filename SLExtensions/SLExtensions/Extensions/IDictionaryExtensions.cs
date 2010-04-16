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
using System.Collections.Generic;

namespace SLExtensions
{
    public static class IDictionaryExtensions
    {
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
    }
}
