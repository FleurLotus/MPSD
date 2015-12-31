namespace MagicPictureSetDownloader.ViewModel.Main
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

        private readonly ProgramUpgrader _programUpdater;
        private readonly IDispatcherInvoker _dispatcherInvoker;
        private readonly IMagicDatabaseReadOnly _magicDatabase;
        private readonly IMagicDatabaseReadAndWriteOption _magicDatabaseForOption;
        private readonly IMagicDatabaseReadAndWriteCollection _magicDatabaseForCollection;
        private readonly IMagicDatabaseReadAndWriteCardInCollection _magicDatabaseForCardInCollection;

        private UpgradeStatus _status;

        //TODO: Test add/remove splitted card and statistics
        //TODO: (Maybe) Import / save historical price 

        //TODO: manage ocT for tap symbol
        //TODO: think about adding complete prebuilt deck

        //TODO: add extra infos in status bar
        public MainViewModel(IDispatcherInvoker dispatcherInvoker)
        {
            AddLinkedProperty(() => Hierarchical, () => Title);

            HideResultCommand = new RelayCommand(o => Status = UpgradeStatus.NotChecked);
            _dispatcherInvoker = dispatcherInvoker;
            _allhierarchical = new HierarchicalViewModel(MagicCards, AllCardAsViewModel);

            _magicDatabase = MagicDatabaseManager.ReadOnly;
            _magicDatabaseForOption = MagicDatabaseManager.ReadAndWriteOption;
            _magicDatabaseForCollection = MagicDatabaseManager.ReadAndWriteCollection;
            _magicDatabaseForCardInCollection = MagicDatabaseManager.ReadAndWriteCardInCollection;

            Options = new OptionsViewModel(_magicDatabaseForOption);
            _programUpdater = new ProgramUpgrader();
            Status = _programUpdater.Status;
           
            if (Options.AutoCheckUpgrade)
            {
                ThreadPool.QueueUserWorkItem(DoCheckNewVersion);
            }

            Analysers = new HierarchicalInfoAnalysersViewModel();
            _menuRoot = new MenuViewModel();
            _contextMenuRoot = new MenuViewModel();

            CreateMenu();

            //ALERT: Temp for helping load file to tree picture
            /*
            foreach (string file in System.IO.Directory.GetFiles(@"C:\Users\fbossout042214\Documents\Visual Studio 2013\Projects\MagicPictureSetDownloader\Sample"))
            {
                _magicDatabase.InsertNewTreePicture(System.IO.Path.GetFileNameWithoutExtension(file), System.IO.File.ReadAllBytes(file));
            }
            */

            //Reload last chosen option
            IOption option = _magicDatabase.GetOption(TypeOfOption.SelectedCollection, "Name");
            if (option != null)
                LoadCollection(option.Value);
            else
                LoadCollection();
        }

        public ICommand HideResultCommand { get; private set; }
        public OptionsViewModel Options { get; private set; }
        public bool ShowFilterConfig
        {
            get { return _showFilterConfig; }
            set
            {
                if (value != _showFilterConfig)
                {
                    _showFilterConfig = value;
                    OnNotifyPropertyChanged(() => ShowFilterConfig);
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
            get { return "MagicPictureSetDownloader - " + Hierarchical.Name; }
        }
        public UpgradeStatus Status
        {
            get { return _status; }
            private set
            {
                if (value != _status)
                {
                    _status = value;
                    OnNotifyPropertyChanged(() => Status);
                }
            }
        }

        private void DoCheckNewVersion(object o)
        {
            try
            {
                _programUpdater.HasNewVersionAvailable();
            }
            catch
            {
                //Call by threadpool must not throw exception
            }
            finally
            {
                Status = _programUpdater.Status;
            }
        }
    }
}
