using System;

namespace Common.Libray
{
    public interface IDispatcherInvoker
    {
        void Invoke(Action action);
    }
}
