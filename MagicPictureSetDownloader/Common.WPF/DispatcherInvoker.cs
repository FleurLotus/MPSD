using System;
using System.Windows;
using System.Windows.Threading;
using Common.Libray;

namespace Common.WPF
{
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
