namespace Common.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Threading;

    public class NotifyPropertyChangedBase: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Lazy<LinkedProperties> _lazyLinkedProperties;
        
        protected NotifyPropertyChangedBase()
        {
            _lazyLinkedProperties = new Lazy<LinkedProperties>(() => new LinkedProperties(this), LazyThreadSafetyMode.PublicationOnly);
        }
        
        public void OnNotifyPropertyChanged(string propertyName)
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