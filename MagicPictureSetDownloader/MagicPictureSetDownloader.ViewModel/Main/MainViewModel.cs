namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public partial class MainViewModel : NotifyPropertyChangedWithLinkedPropertiesBase
    {
        private const string MagicCards = "Magic Cards";

        private bool _showFilterConfig;

        private readonly HierarchicalViewModel _allhierarchical;
        private HierarchicalViewModel _hierarchical;
        private readonly IMagicDatabaseReadAndWriteFull _magicDatabase;

        //TODO: Test delete with card in collection + same with move 
        //TODO: Test import/export
        //TODO: test add/remove splitted card and statistics
        //TODO: On refresh try to keep previous card selection
        public MainViewModel()
        {
            AddLinkedProperty(() => Hierarchical, () => Title);

            _allhierarchical = new HierarchicalViewModel(MagicCards, AllCardAsViewModel);

            _magicDatabase = MagicDatabaseManager.ReadAndWriteFull;
            Analysers = new HierarchicalInfoAnalysersViewModel();
            CreateMenu();

            //ALERT: Temp for helping load file to tree picture 
            /*
            foreach (string file in System.IO.Directory.GetFiles(@"C:\Users\fbossout042214.ASI\Documents\Visual Studio 2012\Projects\MagicPictureSetDownloader\Sample\Others"))
            {
                _magicDatabaseManager.InsertNewTreePicture(System.IO.Path.GetFileNameWithoutExtension(file), System.IO.File.ReadAllBytes(file));
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
                        LoadCardsHierarchy();
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
            LoadCardsHierarchy();
        }
        private void HierarchicalPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
                GenerateContextMenu();
        }

        private void LoadCardsHierarchy()
        {
            HierarchicalInfoAnalyserViewModel[] selectedAnalysers = Analysers.All.Where(a => a.IsActive).ToArray();

            Hierarchical.MakeHierarchyAsync(selectedAnalysers.Select(hiav => hiav.Analyser).ToArray(),
                                            selectedAnalysers.Select(hiav => hiav.IsAscendingOrder).ToArray());
            GenerateCollectionMenu();
            GenerateContextMenu();
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
