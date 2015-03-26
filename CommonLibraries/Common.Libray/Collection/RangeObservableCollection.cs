namespace Common.Libray.Collection
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    using Common.Libray.Threading;

    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
       private const string SuppressNotification = "SuppressNotification";

        public RangeObservableCollection()
        {
        }

        public RangeObservableCollection(IEnumerable<T> list)
            : base(list)
        {
        }

        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            
            using (IDisposable flag = this.SetFlag(SuppressNotification))
            {
                foreach (T value in list)
                    Add(value);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!this.IsFlagSet(SuppressNotification))
                base.OnCollectionChanged(e);
        }
    }
}
