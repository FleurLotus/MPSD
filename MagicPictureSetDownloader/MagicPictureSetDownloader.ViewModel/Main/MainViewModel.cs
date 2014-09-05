namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System.Collections.Generic;
    using System.Linq;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Interface;

    public partial class MainViewModel : NotifyPropertyChangedWithLinkedPropertiesBase
    {
        private const string MagicCards = "Magic Cards";

        private bool _showFilterConfig;

        private readonly HierarchicalViewModel _allhierarchical;
        private HierarchicalViewModel _hierarchical;
        private readonly MagicDatabaseManager _magicDatabaseManager;
        

        //TODO: manage collection
        //          - Change Name
        //          - Delete Collection
        //          - Import/export of collection
        //          - Add remove/card in collection
        //          - Move card
        public MainViewModel()
        {
            AddLinkedProperty(() => Hierarchical, () => Title);

            _allhierarchical = new HierarchicalViewModel(MagicCards, AllCardAsViewModel);

            _magicDatabaseManager = new MagicDatabaseManager();
            Analysers = new HierarchicalInfoAnalysersViewModel(_magicDatabaseManager);
            CreateMenu();

            //ALERT: Temp for helping load file to tree picture 
            /*
            foreach (string file in System.IO.Directory.GetFiles(@"C:\Users\fbossout042214.ASI\Documents\Visual Studio 2012\Projects\MagicPictureSetDownloader\Sample\Others"))
            {
                _magicDatabaseManager.InsertNewTreePicture(System.IO.Path.GetFileNameWithoutExtension(file), System.IO.File.ReadAllBytes(file));
            }
            */
            
            LoadAllCards();
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
                    _hierarchical = value;
                    OnNotifyPropertyChanged(() => Hierarchical);
                }
            }
        }

        public void LoadCollection(string collectionName)
        {
            Hierarchical = new HierarchicalViewModel(collectionName, CardCollectionAsViewModel);
            LoadCardsHierarchy();
        }
        public void LoadAllCards()
        {
            Hierarchical = _allhierarchical;
            LoadCardsHierarchy();
        }
        
        private void LoadCardsHierarchy()
        {
            HierarchicalInfoAnalyserViewModel[] selectedAnalysers = Analysers.All.Where(a => a.IsActive).ToArray();

            Hierarchical.MakeHierarchyAsync(selectedAnalysers.Select(hiav => hiav.Analyser).ToArray(),
                                            selectedAnalysers.Select(hiav => hiav.IsAscendingOrder).ToArray());
        }
        private IEnumerable<CardViewModel> AllCardAsViewModel(string collectionName)
        {
            return _magicDatabaseManager.GetAllInfos(false, -1).Select(cai => new CardViewModel(cai));
        }
        private IEnumerable<CardViewModel> CardCollectionAsViewModel(string collectionName)
        {
            ICardCollection cardCollection = _magicDatabaseManager.GetCollection(collectionName);
            return _magicDatabaseManager.GetAllInfos(true, cardCollection.Id).Select(cai => new CardViewModel(cai));
        }
    }
}
