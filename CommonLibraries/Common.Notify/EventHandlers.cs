namespace Common.Notify
{
    using System;
    using System.Collections.Generic;

    public class EventHandlers<T>
        where T : EventArgs
    {
        private readonly List<EventHandler<T>> _handlers = new List<EventHandler<T>>();
#if NET9_0_OR_GREATER
        private readonly System.Threading.Lock _synclock = new System.Threading.Lock();
#else
        private readonly object _synclock = new object();
#endif

        public EventHandlers()
        {
        }
        public EventHandlers(Action<EventHandler<T>, int> executeOnAdding, Action<EventHandler<T>, int> executeOnAdded,
                             Action<EventHandler<T>, int> executeOnRemoving, Action<EventHandler<T>, int> executeOnRemoved)
        {
            ExecuteOnAdding = executeOnAdding;
            ExecuteOnAdded = executeOnAdded;
            ExecuteOnRemoving = executeOnRemoving;
            ExecuteOnRemoved = executeOnRemoved;
        }
        public int Count
        {
            get
            {
                lock (_synclock)
                {
                    return _handlers.Count;
                }
            }
        }

        public Action<EventHandler<T>, int> ExecuteOnAdded { get; }
        public Action<EventHandler<T>, int> ExecuteOnAdding { get; }
        public Action<EventHandler<T>, int> ExecuteOnRemoved { get; }
        public Action<EventHandler<T>, int> ExecuteOnRemoving { get; }

        public void Add(EventHandler<T> handler)
        {
            lock (_synclock)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler), "Cannot be null");
                }
                if (_handlers.Contains(handler))
                {
                    throw new HandlerAlreadyKnownException();
                }

                ExecuteOnAdding?.Invoke(handler, _handlers.Count);

                _handlers.Add(handler);

                ExecuteOnAdded?.Invoke(handler, _handlers.Count);
            }
        }
        public void Clear()
        {
            lock (_synclock)
            {
                _handlers.Clear();
            }
        }

        public void Notify(IEventDispatcher eventDispatcher, object sender, T args, Action<EventHandler<T>, Exception> executeOnException = null)
        {
            lock (_synclock)
            {
                foreach (EventHandler<T> handler in _handlers)
                {
                    EventHandler<T> handler1 = handler;

                    eventDispatcher.Enqueue(() =>
                        {
                            try
                            {
                                handler1(sender, args);
                            }
                            catch (Exception e)
                            {
                                executeOnException?.Invoke(handler1, e);
                            }
                        });
                }
            }
        }
        public void Remove(EventHandler<T> handler)
        {
            lock (_synclock)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler), "Cannot be null");
                }
                if (!_handlers.Contains(handler))
                {
                    throw new HandlerNotKnownException();
                }

                ExecuteOnRemoving?.Invoke(handler, _handlers.Count);

                _handlers.Remove(handler);

                ExecuteOnRemoved?.Invoke(handler, _handlers.Count);
            }
        }
    }
}