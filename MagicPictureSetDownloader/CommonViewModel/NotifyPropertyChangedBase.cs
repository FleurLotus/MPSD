using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace CommonViewModel
{
    public class NotifyPropertyChangedBase: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected NotifyPropertyChangedBase()
        {
        }
        
        public void OnNotifyPropertyChanged<T>(Expression<Func<T>> expression)
        {
            OnNotifyPropertyChanged(expression.GetMemberName());
        }
        
        private void OnNotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler e = PropertyChanged;
            if (e != null)
                e(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}