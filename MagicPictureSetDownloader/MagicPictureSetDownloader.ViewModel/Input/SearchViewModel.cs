namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Common.Library.Enums;
    using Common.ViewModel.Dialog;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Main;

    public enum PerimeterScope
    {
        All,
        InSelectedCollections,
        NotInSelectedCollections,
        NotInSelectedCollectionsAnyEdition,
    }

    public enum MultiSelectedAggregation
    {
        Or,
        And,
    }

    public class SearchViewModel : DialogViewModelBase
    {
        private readonly IMagicDatabaseReadOnly _magicDatabase;
        private MultiSelectedAggregation _colorAggregation;
        private MultiSelectedAggregation _typeAggregation;
        private PerimeterScope _perimeterScope;
        private bool _excludeFunEditions;
        private bool _excludeOnlineOnlyEditions;
        private bool _allLanguages;
        private string _name;
        private int _idBlockFun;
        private int _idBlockOnlineOnly;
        
        public SearchViewModel()
        {
            _magicDatabase = MagicDatabaseManager.ReadOnly;
            //Never change
            Colors = (ShardColor[])Enum.GetValues(typeof(ShardColor));
            Types = ((CardType[])Enum.GetValues(typeof(CardType))).Where(t => t != CardType.Token)
                                                                  .ToArray();

            Display.Title = "Search";
            Display.OkCommandLabel = "Search";
            Display.CancelCommandLabel = "Close";
            Display.OtherCommandLabel = "Reinit";

            EditionsSelected = new ObservableCollection<IEdition>();
            CollectionsSelected = new ObservableCollection<ICardCollection>();
            ColorsSelected = new ObservableCollection<ShardColor>();
            TypesSelected = new ObservableCollection<CardType>();
            ReInit();
            Reuse();
        }


        public ICollection<IEdition> Editions { get; private set; }
        public ICollection<IEdition> EditionsSelected { get; private set; }
        public ICollection<ICardCollection> Collections { get; private set; }
        public ICollection<ICardCollection> CollectionsSelected { get; private set; }
        public ICollection<ShardColor> Colors { get; private set; }
        public ICollection<ShardColor> ColorsSelected { get; private set; }
        public ICollection<CardType> Types { get; private set; }
        public ICollection<CardType> TypesSelected { get; private set; }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnNotifyPropertyChanged(() => Name);
                }
            }
        }
        public bool ExcludeFunEditions
        {
            get { return _excludeFunEditions; }
            set
            {
                if (value != _excludeFunEditions)
                {
                    _excludeFunEditions = value;
                    OnNotifyPropertyChanged(() => ExcludeFunEditions);
                }
            }
        }
        public bool ExcludeOnlineOnlyEditions
        {
            get { return _excludeOnlineOnlyEditions; }
            set
            {
                if (value != _excludeOnlineOnlyEditions)
                {
                    _excludeOnlineOnlyEditions = value;
                    OnNotifyPropertyChanged(() => ExcludeOnlineOnlyEditions);
                }
            }
        }
        public bool AllLanguages
        {
            get { return _allLanguages; }
            set
            {
                if (value != _allLanguages)
                {
                    _allLanguages = value;
                    OnNotifyPropertyChanged(() => AllLanguages);
                }
            }
        }
        public PerimeterScope PerimeterScope
        {
            get { return _perimeterScope; }
            set
            {
                if (value != _perimeterScope)
                {
                    _perimeterScope = value;
                    OnNotifyPropertyChanged(() => PerimeterScope);
                }
            }
        }
        public MultiSelectedAggregation ColorAggregation
        {
            get { return _colorAggregation; }
            set
            {
                if (value != _colorAggregation)
                {
                    _colorAggregation = value;
                    OnNotifyPropertyChanged(() => ColorAggregation);
                }
            }
        }
        public MultiSelectedAggregation TypeAggregation
        {
            get { return _typeAggregation; }
            set
            {
                if (value != _typeAggregation)
                {
                    _typeAggregation = value;
                    OnNotifyPropertyChanged(() => TypeAggregation);
                }
            }
        }

        protected override bool OkCommandCanExecute(object o)
        {
            return ((!string.IsNullOrWhiteSpace(Name) || EditionsSelected.Count > 0 || ColorsSelected.Count > 0 || TypesSelected.Count > 0) && PerimeterScope == PerimeterScope.All) || 
                CollectionsSelected.Count > 0;
        }
        protected override void OtherCommandExecute(object o)
        {
            ReInit();
        }
        internal void Reuse()
        {
            //Can be updated by application so refill the lists
            Result = null;
            _idBlockFun = _magicDatabase.GetAllBlocks().Where(b => b.Name.IndexOf("Fun", StringComparison.InvariantCultureIgnoreCase) >= 0).Select(b => b.Id).First();
            _idBlockOnlineOnly = _magicDatabase.GetAllBlocks().Where(b => b.Name.IndexOf("OnlineOnly", StringComparison.InvariantCultureIgnoreCase) >= 0).Select(b => b.Id).First();
            Editions = _magicDatabase.GetAllEditionsOrdered();
            Collections = _magicDatabase.GetAllCollections();

            IEdition[] editions = EditionsSelected.ToArray();

            foreach (IEdition edition in editions.Where(edition => !Editions.Contains(edition)))
                EditionsSelected.Remove(edition);
            ICardCollection[] collections = CollectionsSelected.ToArray();

            foreach (ICardCollection collection in collections.Where(collection => !Collections.Contains(collection)))
                CollectionsSelected.Remove(collection);
        }
        private void ReInit()
        {
            //Default values
            Name = null;
            ExcludeFunEditions = true;
            ExcludeOnlineOnlyEditions = true;
            AllLanguages = false;
            PerimeterScope = PerimeterScope.All;
            ColorAggregation = MultiSelectedAggregation.Or;
            TypeAggregation = MultiSelectedAggregation.Or;
            EditionsSelected.Clear();
            CollectionsSelected.Clear();
            ColorsSelected.Clear();
            TypesSelected.Clear();
        }
       
        internal IEnumerable<CardViewModel> SearchResultAsViewModel()
        {
            return _magicDatabase.GetAllInfos().Where(cai => CheckPerimeter(cai) && CheckName(cai) && CheckEdition(cai) && CheckColor(cai) && CheckType(cai))
                .Select(cai => new CardViewModel(cai));
        }

        private bool CheckPerimeter(ICardAllDbInfo cai)
        {
            if (PerimeterScope == PerimeterScope.All)
                return true;

            ICardInCollectionCount[] statistics;
            //We filter statitiscs on only current version of the card 
            if (PerimeterScope == PerimeterScope.InSelectedCollections || PerimeterScope == PerimeterScope.NotInSelectedCollections)
                statistics = cai.Statistics.Where(s => s.IdGatherer == cai.IdGatherer).ToArray();
            else
                statistics = cai.Statistics.ToArray();


            if (PerimeterScope == PerimeterScope.InSelectedCollections)
            {
                //Find at least one collection selected with the specific car card 
                return statistics.Length != 0 && statistics.Any(cicc => CollectionsSelected.Any(cc => cc.Id == cicc.IdCollection));
            }

            //All statistics contains only out of selection collection
            return statistics.Length == 0 ||  statistics.All(cicc => CollectionsSelected.All(cc => cc.Id != cicc.IdCollection));
        }
        private bool CheckName(ICardAllDbInfo cai)
        {
            if (string.IsNullOrWhiteSpace(Name))
                return true;

            if (!AllLanguages)
            {
                return (cai.Card.Name.IndexOf(Name, StringComparison.InvariantCultureIgnoreCase) >= 0) ||
                       (cai.CardPart2 != null && cai.CardPart2.Name.IndexOf(Name, StringComparison.InvariantCultureIgnoreCase) >= 0);
            }

            return _magicDatabase.GetAllLanguages().Any(l =>
                {
                    string name = cai.Card.ToString(l.Id);
                    return !string.IsNullOrEmpty(name) && name.IndexOf(Name, StringComparison.InvariantCultureIgnoreCase) >= 0;
                });
        }
        private bool CheckEdition(ICardAllDbInfo cai)
        {
            if (ExcludeFunEditions)
            {
                if (_magicDatabase.GetEdition(cai.IdGatherer).IdBlock == _idBlockFun)
                    return false;
            }
            if (ExcludeOnlineOnlyEditions)
            {
                if (_magicDatabase.GetEdition(cai.IdGatherer).IdBlock == _idBlockOnlineOnly)
                    return false;
            }

            if (EditionsSelected.Count == 0)
                return true;

            IEdition edition = _magicDatabase.GetEdition(cai.IdGatherer);

            return EditionsSelected.Contains(edition);
        }
        private bool CheckColor(ICardAllDbInfo cai)
        {
            if (ColorsSelected.Count == 0)
                return true;

            ShardColor color = MagicRules.GetColor(cai.Card.CastingCost);
            if (cai.Card.IsSplitted)
                color |= MagicRules.GetColor(cai.CardPart2.CastingCost);

            bool wantedColorless = ColorsSelected.Contains(ShardColor.Colorless);

            ShardColor wantedColor = ColorsSelected.Aggregate(ShardColor.Colorless, (current, newColor) => current | newColor);

            if (ColorAggregation == MultiSelectedAggregation.And)
            {
                return Matcher<ShardColor>.IncludeValue(color, wantedColor);
            }
            //ColorAggregation == MultiSelectedAggregation.Or
            return Matcher<ShardColor>.HasValue(color, wantedColor) || (wantedColorless && color == ShardColor.Colorless);

        }
        private bool CheckType(ICardAllDbInfo cai)
        {
            if (TypesSelected.Count == 0)
                return true;

            CardType type = MagicRules.GetCardType(cai.Card.Type);
            if (cai.Card.IsSplitted)
                type |= MagicRules.GetCardType(cai.CardPart2.Type);
            
            CardType wantedType = TypesSelected.Aggregate(CardType.Token, (current, newColor) => current | newColor);

            if (TypeAggregation == MultiSelectedAggregation.And)
            {
                return Matcher<CardType>.IncludeValue(type, wantedType);
            }
            //TypeAggregation == MultiSelectedAggregation.Or
            return Matcher<CardType>.HasValue(type, wantedType);
        }
    }
}
