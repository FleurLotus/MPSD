namespace Common.Libray.Notify
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public sealed class EventDispatcher : IEventDispatcher, IDisposable
    {
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        private readonly Queue<Action> _queue = new Queue<Action>();
        private readonly Thread _thread;
        private volatile bool _exit;

        public EventDispatcher(string name)
        {
            _thread = new Thread(ActionExecutorThreadLoop) { Name = name, IsBackground = true };
            _thread.Start();
            Name = name + ":" + _thread.ManagedThreadId;
        }
        public string Name { get; private set; }
        
        public void Dispose()
        {
            _exit = true;
            _autoResetEvent.Set();
            
            _thread.Abort();
            _autoResetEvent.Close();
        }

        public void Enqueue(Action action)
        {
            if (_exit)
                return;

            lock (_queue)
                _queue.Enqueue(action);

            _autoResetEvent.Set();
        }
        
        private void ActionExecutorThreadLoop()
        {
            try
            {
                while (!_exit)
                {
                    // Wait for work
                    _autoResetEvent.WaitOne();

                    // Dequeue actions
                    var todo = new List<Action>();
                    lock (_queue)
                    {
                        while (_queue.Count > 0)
                        {
                            todo.Add(_queue.Dequeue());
                        }
                    }

                    // Execute them
                    foreach (var action in todo)
                    {
                        if (_exit)
                            return;

                        try
                        {
                            action();
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine("ActionExecutorThreadLoop got exception: " + e);
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }
        }
    }
}
