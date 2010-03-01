using System;
using System.Linq;
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
    public static class UriExtensions
    {
        public static Dictionary<string, string[]> QueryStringDictionary(this Uri uri)
        {
            var result = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

            if (uri == null)
                return result;
            
            if(!uri.IsAbsoluteUri)
                uri = new Uri( new Uri("http://localhost/"), uri); 

            var r = from parts in uri.Query.TrimStart('?').Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                    let keyvalue = parts.Split(new[] { '=' }, 2)
                    group keyvalue by keyvalue.First() into g
                    select g;
            foreach (var item in r.ToDictionary(g => g.Key, g => g.ToArray(), StringComparer.OrdinalIgnoreCase))
            {
                int count = item.Value.Sum(i => i.Length);
                string[] data = new string[count];
                int start = 0;
                foreach (var array in item.Value)
                {
                    Array.Copy(array, 0, data, start, array.Length);
                    start += array.Length;
                }
                result.Add(item.Key, data);
            }
            return result;
        }
    }
}
