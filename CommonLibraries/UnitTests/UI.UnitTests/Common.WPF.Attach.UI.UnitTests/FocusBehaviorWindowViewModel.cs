namespace Common.WPF.Attach.UI.UnitTests
{
    using System.Windows.Input;

    using Common.ViewModel;
    using Common.ViewModel.Command;

    public class FocusBehaviorWindowViewModel : NotifyPropertyChangedBase
    {
        private bool _isTextBoxFocused;
        private bool _isComboBoxFocused;

        public FocusBehaviorWindowViewModel()
        {
            _isTextBoxFocused = false;
            _isComboBoxFocused = false;
            ChangeFocusCommand = new RelayCommand(o =>
            {
                if (IsTextBoxFocused)
                {
                    IsComboBoxFocused = true;
                }
                else
                {
                    IsTextBoxFocused = true;
                }
            });
        }
        public ICommand ChangeFocusCommand { get; }
        public bool IsTextBoxFocused
        {
            get { return _isTextBoxFocused; }

            set
            {
                if (value != _isTextBoxFocused)
                {
                    _isTextBoxFocused = value;
                    if (value)
                    {
                        IsComboBoxFocused = false;
                    }
                    OnNotifyPropertyChanged();
                }
            }
        }
        public bool IsComboBoxFocused
        {
            get { return _isComboBoxFocused; }
            set
            {
                if (value != _isComboBoxFocused)
                {
                    _isComboBoxFocused = value;
                    if (value)
                    {
                        IsTextBoxFocused = false;
                    }
                    OnNotifyPropertyChanged();
                }
            }
        }
    }
}