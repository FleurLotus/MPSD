using System;

namespace CommonInterface
{
    public interface IDispatcherInvoker
    {
        void Invoke(Action action);
    }
}
