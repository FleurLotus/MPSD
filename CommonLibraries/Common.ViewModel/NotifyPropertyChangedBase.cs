namespace Common.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Threading;

    public class NotifyPropertyChangedBase: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Lazy<LinkedProperties> _lazyLinkedProperties;
        
        protected NotifyPropertyChangedBase()
        {
            _lazyLinkedProperties = new Lazy<LinkedProperties>(() => new LinkedProperties(this), LazyThreadSafetyMode.PublicationOnly);
        }
        
        public void OnNotifyPropertyChanged<T>(Expression<Func<T>> expression)
        {
            if (_lazyLinkedProperties.IsValueCreated)
            {
                foreach (string propertyName in _lazyLinkedProperties.Value.GetNotifyList(expression))
                {
                    OnNotifyPropertyChanged(propertyName);
                }
            }
            else
            {
                OnNotifyPropertyChanged(expression.GetMemberName());
            }
        }
        
        private void OnNotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler e = PropertyChanged;
            if (e != null)
                e(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void AddLinkedProperty<T1, T2>(Expression<Func<T1>> source, Expression<Func<T2>> destination)
        {
            _lazyLinkedProperties.Value.Add(source, destination);
        }
        protected void AddLinkedProperty<T1, T2>(Expression<Func<T1>>[] sources, Expression<Func<T2>> destination)
        {
            foreach (Expression<Func<T1>> source in sources)
                AddLinkedProperty(source, destination);
        }
        protected void AddLinkedProperty<T1, T2>(Expression<Func<T1>> source, Expression<Func<T2>>[] destinations)
        {
            foreach (Expression<Func<T2>> destination in destinations)
                AddLinkedProperty(source, destination);
        }
    }
}