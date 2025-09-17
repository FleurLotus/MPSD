namespace Common.WPF.Attach.UI.UnitTests
{
    using System.Collections.ObjectModel;

    using Common.ViewModel;

    public class AutoCompleteWindowViewModel : NotifyPropertyChangedBase
    {
        private string _selected;
        private string _selected2;

        public AutoCompleteWindowViewModel()
        {
            Possibles = new ObservableCollection<string>
            {
                "Apple",
                "aPPle",
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
                    OnNotifyPropertyChanged();
                }
            }
        }
        public string Selected2
        {
            get { return _selected2; }
            set
            {
                if (_selected2 != value)
                {
                    _selected2 = value;
                    OnNotifyPropertyChanged();
                }
            }
        }
    }
}