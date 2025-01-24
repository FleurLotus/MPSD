namespace Common.ViewModel.UnitTests
{
    using System;
    using System.ComponentModel;

    using NUnit.Framework;

    public class NotifyPropertyChangedChecker : IDisposable
    {
        private PropertyChangedEventArgs _args = null;
        private object _eventSender = null;
        private INotifyPropertyChanged _notifyPropertyChanged = null;

        public NotifyPropertyChangedChecker(INotifyPropertyChanged notifyPropertyChanged)
        {
            _notifyPropertyChanged = notifyPropertyChanged;
            _notifyPropertyChanged.PropertyChanged += NotifyPropertyChangedPropertyChanged;
        }

        public void Check(string propertyName)
        {
            Assert.That(_eventSender, Is.EqualTo(_notifyPropertyChanged));
            Assert.That(_args, Is.Not.Null);
            Assert.That(_args.PropertyName, Is.EqualTo(propertyName));
        }
        private void NotifyPropertyChangedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _eventSender = sender;
            _args = e;
        }

        public void Dispose()
        {
            _notifyPropertyChanged.PropertyChanged -= NotifyPropertyChangedPropertyChanged;
        }
    }
}
