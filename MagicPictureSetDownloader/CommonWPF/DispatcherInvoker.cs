using System;
using System.Windows.Threading;
using CommonInterface;
using System.Windows;

namespace CommonWPF
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
