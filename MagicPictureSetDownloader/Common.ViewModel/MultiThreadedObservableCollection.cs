namespace Common.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    using Common.Libray;

    //From http://geekswithblogs.net/NewThingsILearned/archive/2008/01/16/have-worker-thread-update-observablecollection-that-is-bound-to-a.aspx
    public class MultiThreadedObservableCollection<T> : ObservableCollection<T>
    {
        // Override the event so this class can access it
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        private readonly IDispatcherInvoker _dispatcherInvoker;

        public MultiThreadedObservableCollection(IDispatcherInvoker dispatcherInvoker)
        {
            _dispatcherInvoker = dispatcherInvoker;
        }
        
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // Be nice - use BlockReentrancy like MSDN said
            using (BlockReentrancy())
            {
               NotifyCollectionChangedEventHandler eventHandler = CollectionChanged;
                if (eventHandler == null)
                    return;

                Delegate[] delegates = eventHandler.GetInvocationList();
                // Walk thru invocation list
                foreach (NotifyCollectionChangedEventHandler handler in delegates)
                {
                    NotifyCollectionChangedEventHandler handler1 = handler;
                    _dispatcherInvoker.Invoke(() => handler1(this, e));
                }
            }
        }
    }
}
