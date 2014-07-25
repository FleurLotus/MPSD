namespace Common.WPF
{
    using System;
    using System.Windows;
    using System.Windows.Threading;
    using Common.Libray;

    public class DispatcherInvoker : IDispatcherInvoker
    {
        public void Invoke(Action action)
        {
            Dispatcher dispatchObject = Application.Current.Dispatcher;
            if (dispatchObject == null || dispatchObject.CheckAccess())
            {
                action();
            }
            else
            {
                dispatchObject.Invoke(action);
            }
        }
    }
}
