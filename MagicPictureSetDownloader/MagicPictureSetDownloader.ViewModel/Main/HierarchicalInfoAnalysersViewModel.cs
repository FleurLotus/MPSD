﻿namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Core.HierarchicalAnalysing;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public class HierarchicalInfoAnalysersViewModel : NotifyPropertyChangedBase
    {
        private readonly IMagicDatabaseReadAndWriteOption _magicDatabase;
        private int _selectedIndex;
        private List<HierarchicalInfoAnalyserViewModel> _all;
        private readonly int _allCount;

        public HierarchicalInfoAnalysersViewModel()
        {
            _magicDatabase = MagicDatabaseManager.ReadAndWriteOption;
            UpCommand = new RelayCommand(UpCommandExecute, UpCommandCanExecute);
            DownCommand = new RelayCommand(DownCommandExecute, DownCommandCanExecute);

            CreateHierarchy(_magicDatabase.GetOptions(TypeOfOption.Hierarchy));
            
            _allCount = _all.Count;
            SelectedIndex = -1;
        }

        public IList<HierarchicalInfoAnalyserViewModel> All
        {
            get { return _all.AsReadOnly(); }
        }

        public ICommand UpCommand { get; }
        public ICommand DownCommand { get; }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value != _selectedIndex)
                {
                    _selectedIndex = value;
                    OnNotifyPropertyChanged(nameof(SelectedIndex));
                }
            }
        }

        public void Save()
        {
            IList<HierarchicalInfoAnalyserViewModel> all = All;
            for (int i = 0; i < all.Count; i++)
            {
                HierarchicalInfoAnalyserViewModel vm = all[i];
                _magicDatabase.InsertNewOption(TypeOfOption.Hierarchy, vm.Name, vm.SaveInfo(i));
            }
        }

        private void CreateHierarchy(ICollection<IOption> options)
        {
            if (options == null || options.Count == 0)
            {
                _all = new List<HierarchicalInfoAnalyserViewModel>(HierarchicalInfoAnalyserFactory.Instance.Names.Select(s => new HierarchicalInfoAnalyserViewModel(s)));
                return;
            }

            _all = new List<HierarchicalInfoAnalyserViewModel>();

            SortedDictionary<int, HierarchicalInfoAnalyserViewModel> dic = new SortedDictionary<int, HierarchicalInfoAnalyserViewModel>();
            foreach (IOption option in options)
            {
                HierarchicalInfoAnalyserViewModel vm = new HierarchicalInfoAnalyserViewModel(option.Key);
                int pos = vm.LoadInfo(option.Value);
                dic.Add(pos, vm);
            }

            _all.AddRange(dic.Values);
        }
        
        #region Command

        private void UpCommandExecute(object o)
        {
            int index = _selectedIndex;
            HierarchicalInfoAnalyserViewModel current = _all[index];
            _all.RemoveAt(index);

            index--;
            _all.Insert(index, current);
            OnNotifyPropertyChanged(nameof(All));
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
            OnNotifyPropertyChanged(nameof(All));
            SelectedIndex = index;
        }
        private bool DownCommandCanExecute(object o)
        {
            return _selectedIndex >= 0 && _selectedIndex < _allCount - 1;
        }

        #endregion

    }
}
