namespace Common.WPF.Attach.UI.UnitTests
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class AutoCompleteWindowViewModel : INotifyPropertyChanged
    {
        private string _selected;

        public event PropertyChangedEventHandler PropertyChanged;

        public AutoCompleteWindowViewModel()
        {
            Possibles = new ObservableCollection<string>
            {
                "Apple",
                "Apricot",
                "Banana",
            };
        }
        public ObservableCollection<string> Possibles { get; }
        public string Selected
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    RaiseNotifyPropertyChanged();
                }
            }
        }

        private void RaiseNotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
