namespace Common.ViewModel
{
    using System;
    using System.Linq.Expressions;

    internal static class Extension
    {
        public static string GetMemberName<T>(this Expression<Func<T>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            MemberExpression memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException(string.Format("Expression of type {0} is not supported by the GetMemberName method. " +
                                                          "Only MemberExpressions are currently supported.", expression.Body.Type));
            return memberExpression.Member.Name;
        }
    }
}
