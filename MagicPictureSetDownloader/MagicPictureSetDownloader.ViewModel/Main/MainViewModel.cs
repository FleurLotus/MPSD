namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;

    using Common.Libray;
    using Common.ViewModel;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public partial class MainViewModel : NotifyPropertyChangedWithLinkedPropertiesBase
    {
        private const string MagicCards = "Magic Cards";

        private bool _showFilterConfig;
        private bool _loading;

        private readonly HierarchicalViewModel _allhierarchical;
        private readonly IDispatcherInvoker _dispatcherInvoker;
        private readonly IMagicDatabaseReadAndWriteFull _magicDatabase;
        private HierarchicalViewModel _hierarchical;
        
        //TODO: Test import/export
        //TODO: test add/remove splitted card and statistics
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

        public HierarchicalInfoAnalysersViewModel Analysers { get; private set; }

        public bool Loading
        {
            get { return _loading; }
            set
            {
                if (value != _loading)
                {
                    _loading = value;
                    OnNotifyPropertyChanged(() => Loading);
                }
            }
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
        public HierarchicalViewModel Hierarchical
        {
            get { return _hierarchical; }
            private set
            {
                if (value != _hierarchical)
                {
                    //Need Listen to Selected modification for ContextMenu recreation
                    if (_hierarchical != null)
                        _hierarchical.PropertyChanged -= HierarchicalPropertyChanged;
                    if (value != null)
                        value.PropertyChanged += HierarchicalPropertyChanged;
                    _hierarchical = value;

                    OnNotifyPropertyChanged(() => Hierarchical);
                }
            }
        }

        public void LoadCollection(string collectionName = "")
        {
            //Save the chosen option
            _magicDatabase.InsertNewOption(TypeOfOption.SelectedCollection, "Name", collectionName);

            Hierarchical = string.IsNullOrEmpty(collectionName) ? _allhierarchical : new HierarchicalViewModel(collectionName, CardCollectionAsViewModel);
            LoadCardsHierarchyAsync();
        }
        private void HierarchicalPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
                GenerateContextMenu();
        }
        private void LoadCardsHierarchyAsync()
        {
            ThreadPool.QueueUserWorkItem(o => LoadCardsHierarchy());
        }

        private void LoadCardsHierarchy()
        {
            Loading = true;
            HierarchicalInfoAnalyserViewModel[] selectedAnalysers = Analysers.All.Where(a => a.IsActive).ToArray();

            Hierarchical.MakeHierarchy(selectedAnalysers.Select(hiav => hiav.Analyser).ToArray(),
                                       selectedAnalysers.Select(hiav => hiav.IsAscendingOrder).ToArray());

            GenerateCollectionMenu();
            GenerateContextMenu();
            Loading = false;
        }

        private IEnumerable<CardViewModel> AllCardAsViewModel(string collectionName)
        {
            return _magicDatabase.GetAllInfos().Select(cai => new CardViewModel(cai));
        }
        private IEnumerable<CardViewModel> CardCollectionAsViewModel(string collectionName)
        {
            ICardCollection cardCollection = _magicDatabase.GetCollection(collectionName);
            return _magicDatabase.GetAllInfos(cardCollection.Id).Select(cai => new CardViewModel(cai));
        }
        private void CheckCollectionNameNotAlreadyExists(string name)
        {
            if (_magicDatabase.GetCollection(name) != null)
            {
                throw new ApplicationException("Name is already used for an other collection");
            }
        }
    }
}
