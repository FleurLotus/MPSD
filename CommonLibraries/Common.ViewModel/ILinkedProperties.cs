namespace Common.ViewModel
{
    using System;
    using System.Linq.Expressions;

    internal interface ILinkedProperties
    {
        void AddLinkedProperty<T1, T2>(Expression<Func<T1>> source, Expression<Func<T2>> destination);
        void AddLinkedProperty<T1, T2>(Expression<Func<T1>>[] sources, Expression<Func<T2>> destination);
        void AddLinkedProperty<T1, T2>(Expression<Func<T1>> source, Expression<Func<T2>>[] destinations);
    }
}