namespace Common.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Threading;

    using Common.Library.Notify;

    public class NotifyPropertyChangedBase: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Lazy<LinkedProperties> _lazyLinkedProperties;
        
        protected NotifyPropertyChangedBase()
        {
            _lazyLinkedProperties = new Lazy<LinkedProperties>(() => new LinkedProperties(this), LazyThreadSafetyMode.PublicationOnly);
        }
        
        protected void OnNotifyPropertyChanged(string propertyName)
        {
            if (_lazyLinkedProperties.IsValueCreated)
            {
                foreach (string linkedPropertyName in _lazyLinkedProperties.Value.GetNotifyList(propertyName))
                {
                    RaiseNotifyPropertyChanged(linkedPropertyName);
                }
            }
            else
            {
                RaiseNotifyPropertyChanged(propertyName);
            }
        }
        protected void OnEventRaise<T>(EventHandler<EventArgs<T>> ev, T arg)
        {
            if (ev != null && arg != null)
            {
                ev(this, new EventArgs<T>(arg));
            }
        }
        protected void OnEventRaise(EventHandler ev)
        {
            if (ev != null)
            {
                ev(this, EventArgs.Empty);
            }
        }

        private void RaiseNotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler e = PropertyChanged;
            if (e != null)
            {
                e(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void AddLinkedProperty(string source, string destination)
        {
            _lazyLinkedProperties.Value.Add(source, destination);
        }
        protected void AddLinkedProperty(string[] sources, string destination)
        {
            foreach (string source in sources)
            {
                AddLinkedProperty(source, destination);
            }
        }
        protected void AddLinkedProperty(string source, string[] destinations)
        {
            foreach (string destination in destinations)
            {
                AddLinkedProperty(source, destination);
            }
        }
    }
}