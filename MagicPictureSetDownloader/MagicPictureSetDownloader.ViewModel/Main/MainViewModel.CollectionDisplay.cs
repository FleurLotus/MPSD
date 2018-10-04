namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;

    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Input;

    public partial class MainViewModel
    {
        private const string MagicCards = "Magic Cards";
        private readonly HierarchicalViewModel _allhierarchical;
        private HierarchicalViewModel _searchHierarchical;
        private HierarchicalViewModel _hierarchical;

        public HierarchicalInfoAnalysersViewModel Analysers { get; }

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
        public HierarchicalViewModel Hierarchical
        {
            get { return _hierarchical; }
            private set
            {
                if (value != _hierarchical)
                {
                    //Need Listen to Selected modification for ContextMenu recreation
                    if (_hierarchical != null)
                    {
                        _hierarchical.PropertyChanged -= HierarchicalPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += HierarchicalPropertyChanged;
                    }

                    _hierarchical = value;

                    OnNotifyPropertyChanged(() => Hierarchical);
                }
            }
        }

        public void LoadCollection(string collectionName = "")
        {
            //Save the chosen option
            _magicDatabaseForOption.InsertNewOption(TypeOfOption.SelectedCollection, "Name", collectionName);
            //Force to null to avoid block on refreshing
            Hierarchical = null;
            Hierarchical = string.IsNullOrEmpty(collectionName) ? _allhierarchical : new HierarchicalViewModel(collectionName, CardCollectionAsViewModel);
            LoadCardsHierarchyAsync();
        }
        private void CreateSearchResult(SearchViewModel vm)
        {
            _searchHierarchical = new HierarchicalViewModel("Search result", s => vm.SearchResultAsViewModel());
            Hierarchical = _searchHierarchical;
        }
        private void HierarchicalPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                GenerateContextMenu();
            }
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
            GenerateStatusBarInfo();
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
