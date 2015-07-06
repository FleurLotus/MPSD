﻿namespace MagicPictureSetDownloader.ViewModel.Main
{
    using Common.Library;
    using Common.ViewModel;
    using Common.ViewModel.Menu;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public partial class MainViewModel : NotifyPropertyChangedBase
    {
        private bool _showFilterConfig;
        private bool _loading;

        private readonly IDispatcherInvoker _dispatcherInvoker;
        private readonly IMagicDatabaseReadAndWriteFull _magicDatabase;

        //TODO: Test add/remove splitted card and statistics
        //TODO: (Maybe) Import / save historical price 
        public MainViewModel(IDispatcherInvoker dispatcherInvoker)
        {
            AddLinkedProperty(() => Hierarchical, () => Title);

            _dispatcherInvoker = dispatcherInvoker;
            _allhierarchical = new HierarchicalViewModel(MagicCards, AllCardAsViewModel);

            _magicDatabase = MagicDatabaseManager.ReadAndWriteFull;
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
    }
}
