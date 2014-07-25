namespace Common.Libray
{
    using System;

    public interface IDispatcherInvoker
    {
        void Invoke(Action action);
    }
}
