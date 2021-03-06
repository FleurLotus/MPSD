﻿
namespace Common.Library.Notify
{
    using System;
    using System.Collections.Generic;

    using Common.Library.Exception;

    public class EventHandlers<T>
        where T : EventArgs
    {
        private readonly List<EventHandler<T>> _handlers = new List<EventHandler<T>>();
        private readonly object _synclock = new object();

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

                if (ExecuteOnAdding != null)
                {
                    ExecuteOnAdding(handler, _handlers.Count);
                }

                _handlers.Add(handler);

                if (ExecuteOnAdded != null)
                {
                    ExecuteOnAdded(handler, _handlers.Count);
                }
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
                foreach (var handler in _handlers)
                {
                    EventHandler<T> handler1 = handler;
                    Action a = () =>
                        {
                            try
                            {
                                handler1(sender, args);
                            }
                            catch (Exception e)
                            {
                                if (executeOnException != null)
                                {
                                    executeOnException(handler1, e);
                                }
                            }
                        };

                    eventDispatcher.Enqueue(a);
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

                if (ExecuteOnRemoving != null)
                {
                    ExecuteOnRemoving(handler, _handlers.Count);
                }

                _handlers.Remove(handler);

                if (ExecuteOnRemoved != null)
                {
                    ExecuteOnRemoved(handler, _handlers.Count);
                }
            }
        }
    }
}
