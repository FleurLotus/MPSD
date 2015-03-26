
namespace Common.Libray.Notify
{
    using System;

    public interface IEventDispatcher
    {
        string Name { get; }
        void Enqueue(Action action);
    }
}
