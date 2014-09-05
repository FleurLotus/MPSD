namespace Common.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public class MenuViewModel : NotifyPropertyChangedBase
    {
        private bool _isCheckable;
        private bool _isChecked;
        public static MenuViewModel Separator = new MenuViewModel(null) { IsSeparator = true };

        public MenuViewModel(string text) : this (text, null)
        {
        }
        public MenuViewModel(string text, ICommand command) : this (text, command, null)
        {
        }

        public MenuViewModel(string text, ICommand command, object commandParameter)
        {
            MenuText = text;
            Command = command;
            CommandParameter = commandParameter;
            Children = new ObservableCollection<MenuViewModel>();
        }

        public ObservableCollection<MenuViewModel> Children { get; private set; }
        public ICommand Command { get; private set; }
        public string MenuText { get; private set; }
        public bool IsSeparator { get; private set; }
        public object CommandParameter { get; private set; }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (value != _isChecked)
                {
                    _isChecked = value;
                    OnNotifyPropertyChanged(() => IsChecked);
                }
            }
        }
        public bool IsCheckable
        {
            get { return _isCheckable; }
            set
            {
                if (value != _isCheckable)
                {
                    _isCheckable = value;
                    OnNotifyPropertyChanged(() => IsCheckable);
                }
            }
        }
    }
}
