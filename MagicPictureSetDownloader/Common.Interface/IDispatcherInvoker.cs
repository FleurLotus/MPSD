using System;

namespace Common.Interface
{
    public interface IDispatcherInvoker
    {
        void Invoke(Action action);
    }
}
