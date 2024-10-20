﻿namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System.Threading;
    using System.Windows.Input;

    using Common.Library;
    using Common.ViewModel;
    using Common.ViewModel.Menu;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.Core.Upgrade;
    using MagicPictureSetDownloader.ViewModel.Option;

    public partial class MainViewModel : NotifyPropertyChangedBase
    {
        private bool _showFilterConfig;
        private bool _loading;
        private string _statusBarInfo;

        private readonly ProgramUpgrader _programUpdater;
        private readonly IDispatcherInvoker _dispatcherInvoker;
        private readonly IMagicDatabaseReadOnly _magicDatabase;
        private readonly IMagicDatabaseReadAndWriteOption _magicDatabaseForOption;
        private readonly IMagicDatabaseReadAndWriteCollection _magicDatabaseForCollection;
        private readonly IMagicDatabaseReadAndWriteCardInCollection _magicDatabaseForCardInCollection;

        private UpgradeStatus _upgradeStatus;

        //TODO: Test add/remove splitted card and statistics
        public MainViewModel(IDispatcherInvoker dispatcherInvoker)
        {
            AddLinkedProperty(nameof(Hierarchical), nameof(Title));

            HideResultCommand = new RelayCommand(o => UpgradeStatus = UpgradeStatus.NotChecked);
            _dispatcherInvoker = dispatcherInvoker;
            _allhierarchical = new HierarchicalViewModel(MagicCards, AllCardAsViewModel);

            _magicDatabase = MagicDatabaseManager.ReadOnly;
            _magicDatabaseForOption = MagicDatabaseManager.ReadAndWriteOption;
            _magicDatabaseForCollection = MagicDatabaseManager.ReadAndWriteCollection;
            _magicDatabaseForCardInCollection = MagicDatabaseManager.ReadAndWriteCardInCollection;

            Options = new OptionsViewModel(_magicDatabaseForOption);
            _programUpdater = new ProgramUpgrader();
            UpgradeStatus = _programUpdater.Status;
           
            if (Options.AutoCheckUpgrade)
            {
                ThreadPool.QueueUserWorkItem(DoCheckNewVersion);
            }

            Analysers = new HierarchicalInfoAnalysersViewModel();
            _menuRoot = new MenuViewModel();
            _contextMenuRoot = new MenuViewModel();

            CreateMenu();

            //Reload last chosen option
            IOption option = _magicDatabase.GetOption(TypeOfOption.SelectedCollection, "Name");
            if (option != null)
            {
                LoadCollection(option.Value);
            }
            else
            {
                LoadCollection();
            }
        }

        public ICommand HideResultCommand { get; }
        public OptionsViewModel Options { get; }
        public bool ShowFilterConfig
        {
            get { return _showFilterConfig; }
            set
            {
                if (value != _showFilterConfig)
                {
                    _showFilterConfig = value;
                    OnNotifyPropertyChanged(nameof(ShowFilterConfig));
                    if (!_showFilterConfig)
                    {
                        Analysers.Save();
                        LoadCardsHierarchyAsync();
                    }
                }
            }
        }
  
        public string Title
        {
            get { return Hierarchical == null ? "MagicPictureSetDownloader" : $"MagicPictureSetDownloader - {Hierarchical.Name}"; }
        }
        public UpgradeStatus UpgradeStatus
        {
            get { return _upgradeStatus; }
            private set
            {
                if (value != _upgradeStatus)
                {
                    _upgradeStatus = value;
                    OnNotifyPropertyChanged(nameof(UpgradeStatus));
                }
            }
        }
        public string StatusBarInfo
        {
            get { return _statusBarInfo; }
            private set
            {
                if (value != _statusBarInfo)
                {
                    _statusBarInfo = value;
                    OnNotifyPropertyChanged(nameof(StatusBarInfo));
                }
            }
        }
        private void DoCheckNewVersion(object o)
        {
            try
            {
                _programUpdater.HasNewVersionAvailable();
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            {
                //Call by threadpool must not throw exception
            }
            // ReSharper restore EmptyGeneralCatchClause
            finally
            {
                UpgradeStatus = _programUpdater.Status;
            }
        }
        private void GenerateStatusBarInfo()
        {
            if (Hierarchical == null)
            {
                StatusBarInfo = null;
                return;
            }
            StatusBarInfo = Hierarchical.GetInfo(Hierarchical == _allhierarchical || Hierarchical == _searchHierarchical);
        }
    }
}
