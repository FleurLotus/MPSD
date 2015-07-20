namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
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
        private readonly IMagicDatabaseReadAndWriteFull _magicDatabase;
        private UpgradeStatus _status;

        //TODO: Test add/remove splitted card and statistics
        //TODO: (Maybe) Import / save historical price 

        //TODO: think about upgrade set using reference script
        public MainViewModel(IDispatcherInvoker dispatcherInvoker)
        {
            AddLinkedProperty(() => Hierarchical, () => Title);

            HideResultCommand = new RelayCommand(o => Status = UpgradeStatus.NotChecked);
            _dispatcherInvoker = dispatcherInvoker;
            _allhierarchical = new HierarchicalViewModel(MagicCards, AllCardAsViewModel);

            _magicDatabase = MagicDatabaseManager.ReadAndWriteFull;
            Options = new OptionsViewModel(_magicDatabase);
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
