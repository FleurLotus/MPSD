namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Core.HierarchicalAnalysing;

    public class HierarchicalInfoAnalysersViewModel : NotifyPropertyChangedBase
    {
        private int _selectedIndex;
        private List<HierarchicalInfoAnalyserViewModel> _all;
        private readonly int _allCount;

        public HierarchicalInfoAnalysersViewModel()
        {
            UpCommand = new RelayCommand(UpCommandExecute, UpCommandCanExecute);
            DownCommand = new RelayCommand(DownCommandExecute, DownCommandCanExecute);
            _all = new List<HierarchicalInfoAnalyserViewModel>(HierarchicalInfoAnalyserFactory.Instance.Names.Select(s => new HierarchicalInfoAnalyserViewModel(s)));
            _allCount = _all.Count;
            SelectedIndex = -1;
        }

        public ICollection<HierarchicalInfoAnalyserViewModel> All
        {
            get { return _all.AsReadOnly(); }
        }

        public ICommand UpCommand { get; private set; }
        public ICommand DownCommand { get; private set; }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value != _selectedIndex)
                {
                    _selectedIndex = value;
                    OnNotifyPropertyChanged(() => SelectedIndex);
                }
            }
        }

        #region Command

        private void UpCommandExecute(object o)
        {
            int index = _selectedIndex;
            HierarchicalInfoAnalyserViewModel current = _all[index];
            _all.RemoveAt(index);

            index--;
            _all.Insert(index, current);
            OnNotifyPropertyChanged(() => All);
            SelectedIndex= index;
        }
        private bool UpCommandCanExecute(object o)
        {
            return _selectedIndex >= 1 && _selectedIndex < _allCount;
        }
        private void DownCommandExecute(object o)
        {
            int index = _selectedIndex;
            HierarchicalInfoAnalyserViewModel current = _all[index];
            _all.RemoveAt(index);

            index++;
            _all.Insert(index, current);
            OnNotifyPropertyChanged(() => All);
            SelectedIndex = index;
        }
        private bool DownCommandCanExecute(object o)
        {
            return _selectedIndex >= 0 && _selectedIndex < _allCount - 1;
        }

        #endregion

    }
}
