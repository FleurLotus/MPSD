namespace Common.WPF.UI.UnitTests
{
    using System.Windows.Input;

    using Common.ViewModel;
    using Common.ViewModel.Command;

    public class ProgressBarWindowViewModel : NotifyPropertyChangedBase
    {
        private double _value;

        public ProgressBarWindowViewModel()
        {
            Value = 5;
            AddCommand = new RelayCommand(o => Value++);
        }

        public ICommand AddCommand { get; }

        public double Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnNotifyPropertyChanged();
                }
            }
        }
    }
}