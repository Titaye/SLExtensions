namespace SLExtensions
{
    using System;
    using System.Linq.Expressions;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// From Michael Sync blog http://michaelsync.net/2009/04/09/silverlightwpf-implementing-propertychanged-with-expression-tree
    /// </summary>
    public static class NotifyingObjectExtensions
    {
        #region Methods

        public static string GetPropertyName<T, TProperty>(this T owner, Expression<Func<T, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                if (unaryExpression != null)
                {
                    memberExpression = unaryExpression.Operand as MemberExpression;
                    if (memberExpression == null)
                        throw new NotImplementedException();
                }
                else
                    throw new NotImplementedException();
            }

            var propertyName = memberExpression.Member.Name;
            return propertyName;
        }

        public static void RaisePropertyChanged<T, TProperty>(this T observableBase, Expression<Func<T, TProperty>> expression)
            where T : NotifyingObject
        {
            observableBase.RaisePropertyChanged(observableBase.GetPropertyName(expression));
        }

        #endregion Methods
    }
}