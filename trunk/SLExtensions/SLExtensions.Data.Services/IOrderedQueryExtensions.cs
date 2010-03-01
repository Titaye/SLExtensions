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
using System.Linq;
using System.Data.Services.Client;
using System.Collections.Generic;

namespace SLExtensions.Data.Services.Extensions
{
  public static class IOrderedQueryExtensions
  {
    public static DataServiceQuery<T> AsDataServiceQuery<T>(this IOrderedQueryable<T> qry)
    {
      return (DataServiceQuery<T>)qry;
    }

    public static IAsyncResult BeginExecute<T>(this IOrderedQueryable<T> qry, AsyncCallback callback, object state)
    {
      return ((DataServiceQuery<T>)qry).BeginExecute(callback, state);
    }

    public static IEnumerable<T> EndExecute<T>(this IOrderedQueryable<T> qry, IAsyncResult result)
    {
      return ((DataServiceQuery<T>)qry).EndExecute(result);
    }
  }
}
