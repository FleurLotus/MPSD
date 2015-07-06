namespace Common.Library
{
    using System;

    public interface IDispatcherInvoker
    {
        void Invoke(Action action);
    }
}
