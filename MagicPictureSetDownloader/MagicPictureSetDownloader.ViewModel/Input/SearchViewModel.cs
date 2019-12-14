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
        CollectionBased,
    }

    public enum MultiSelectedAggregation
    {
        Or,
        And,
    }
    public enum ComparisonType
    {
        GreaterOrEquals = 0,
        LessThan = 1,
    }

    public class SearchViewModel : DialogViewModelBase
    {
        private readonly IMagicDatabaseReadOnly _magicDatabase;
        private MultiSelectedAggregation _colorAggregation;
        private MultiSelectedAggregation _typeAggregation;
        private MultiSelectedAggregation _subTypeAggregation;
        private PerimeterScope _perimeterScope;
        private bool _excludeFunEditions;
        private bool _excludeOnlineOnlyEditions;
        private bool _excludeSpecialCards;
        private bool _countIncludeFoil;
        private bool _countIsNameBased;
        private bool _allLanguages;
        private ComparisonType _countComparatorWanted;
        private int _countSelected;
        private string _name;
        private int _idBlockFun;
        private int _idBlockOnlineOnly;
        
        public SearchViewModel()
        {
            _magicDatabase = MagicDatabaseManager.ReadOnly;
            //Never change
            CountComparator = new[] { "≥", "<" };
            Colors = (ShardColor[])Enum.GetValues(typeof(ShardColor));
            Types = ((CardType[])Enum.GetValues(typeof(CardType))).Where(t => t != CardType.Token)
                                                                  .ToArray();
            SubTypes = ((CardSubType[])Enum.GetValues(typeof(CardSubType))).Where(t => t != CardSubType.None)
                                                                           .ToArray();

            Display.Title = "Search";
            Display.OkCommandLabel = "Search";
            Display.CancelCommandLabel = "Close";
            Display.OtherCommandLabel = "Reinit";

            EditionsSelected = new ObservableCollection<IEdition>();
            CollectionsSelected = new ObservableCollection<ICardCollection>();
            ColorsSelected = new ObservableCollection<ShardColor>();
            TypesSelected = new ObservableCollection<CardType>();
            SubTypesSelected = new ObservableCollection<CardSubType>();
            ReInit();
            Reuse();
        }
        
        public ICollection<IEdition> Editions { get; private set; }
        public ICollection<IEdition> EditionsSelected { get; }
        public ICollection<ICardCollection> Collections { get; private set; }
        public ICollection<ICardCollection> CollectionsSelected { get; }
        public ICollection<ShardColor> Colors { get; }
        public ICollection<ShardColor> ColorsSelected { get; }
        public ICollection<CardType> Types { get; }
        public ICollection<CardType> TypesSelected { get; }
        public ICollection<CardSubType> SubTypes { get; }
        public ICollection<CardSubType> SubTypesSelected { get; }
        public string[] CountComparator { get; }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnNotifyPropertyChanged(nameof(Name));
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
                    OnNotifyPropertyChanged(nameof(ExcludeFunEditions));
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
                    OnNotifyPropertyChanged(nameof(ExcludeOnlineOnlyEditions));
                }
            }
        }
        public bool ExcludeSpecialCards
        {
            get { return _excludeSpecialCards; }
            set
            {
                if (value != _excludeSpecialCards)
                {
                    _excludeSpecialCards = value;
                    OnNotifyPropertyChanged(nameof(ExcludeSpecialCards));
                }
            }
        }
        public string CountComparatorSelected
        {
            get { return CountComparator[(int)_countComparatorWanted]; }
            set
            {
                for (int i = 0; i < CountComparator.Length; i++)
                {
                    if (value == CountComparator[i])
                    {
                        if (_countComparatorWanted == (ComparisonType)i)
                        {
                            return;
                        }

                        _countComparatorWanted = (ComparisonType)i;
                        OnNotifyPropertyChanged(nameof(CountComparatorSelected));
                        break;
                    }
                }
            }
        }
        public int CountSelected
        {
            get { return _countSelected; }
            set
            {
                if (value != _countSelected)
                {
                    _countSelected = value;
                    OnNotifyPropertyChanged(nameof(CountSelected));
                }
            }
        }
        public bool CountIsNameBased
        {
            get { return _countIsNameBased; }
            set
            {
                if (value != _countIsNameBased)
                {
                    _countIsNameBased = value;
                    OnNotifyPropertyChanged(nameof(CountIsNameBased));
                }
            }
        }
        public bool CountIncludeFoil
        {
            get { return _countIncludeFoil; }
            set
            {
                if (value != _countIncludeFoil)
                {
                    _countIncludeFoil = value;
                    OnNotifyPropertyChanged(nameof(CountIncludeFoil));
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
                    OnNotifyPropertyChanged(nameof(AllLanguages));
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
                    OnNotifyPropertyChanged(nameof(PerimeterScope));
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
                    OnNotifyPropertyChanged(nameof(ColorAggregation));
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
                    OnNotifyPropertyChanged(nameof(TypeAggregation));
                }
            }
        }

        public MultiSelectedAggregation SubTypeAggregation
        {
            get { return _subTypeAggregation; }
            set
            {
                if (value != _subTypeAggregation)
                {
                    _subTypeAggregation = value;
                    OnNotifyPropertyChanged(nameof(SubTypeAggregation));
                }
            }
        }

        protected override bool OkCommandCanExecute(object o)
        {
            return ((!string.IsNullOrWhiteSpace(Name) || EditionsSelected.Count > 0 || ColorsSelected.Count > 0 || TypesSelected.Count > 0 || SubTypesSelected.Count > 0) && PerimeterScope == PerimeterScope.All) || 
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
            {
                EditionsSelected.Remove(edition);
            }

            ICardCollection[] collections = CollectionsSelected.ToArray();

            foreach (ICardCollection collection in collections.Where(collection => !Collections.Contains(collection)))
            {
                CollectionsSelected.Remove(collection);
            }
        }
        private void ReInit()
        {
            //Default values
            Name = null;
            ExcludeFunEditions = true;
            ExcludeOnlineOnlyEditions = true;
            ExcludeSpecialCards = true;
            CountIncludeFoil = false;
            CountIsNameBased = false;
            CountComparatorSelected = CountComparator[(int)ComparisonType.GreaterOrEquals];
            CountSelected = 1;
            AllLanguages = false;
            PerimeterScope = PerimeterScope.All;
            ColorAggregation = MultiSelectedAggregation.Or;
            TypeAggregation = MultiSelectedAggregation.Or;
            SubTypeAggregation = MultiSelectedAggregation.Or;
            EditionsSelected.Clear();
            CollectionsSelected.Clear();
            ColorsSelected.Clear();
            TypesSelected.Clear();
            SubTypesSelected.Clear();
        }
       
        internal IEnumerable<CardViewModel> SearchResultAsViewModel()
        {
            return _magicDatabase.GetAllInfos().Where(cai => CheckPerimeter(cai) && CheckName(cai) && CheckEdition(cai) && CheckColor(cai) && CheckType(cai) && CheckSubType(cai))
                                               .Select(cai => new CardViewModel(cai));
        }

        private bool CheckPerimeter(ICardAllDbInfo cai)
        {
            if (PerimeterScope == PerimeterScope.All)
            {
                return true;
            }

            IEnumerable<ICardInCollectionCount> cardStats = CountIsNameBased ? cai.Statistics :
                                                                     //We filter statitiscs on only current version of the card 
                                                                     cai.Statistics.Where(s => s.IdGatherer == cai.IdGatherer);

            //Filter on collection
            ICardInCollectionCount[] statistics = cardStats.Where(s => CollectionsSelected.Any(cc => cc.Id == s.IdCollection)).ToArray();

            int count = statistics.Sum(stat => stat.Number);
            if (CountIncludeFoil)
            {
                count += statistics.Sum(stat => stat.FoilNumber);
            }

            return _countComparatorWanted == ComparisonType.GreaterOrEquals ? count >= CountSelected : count < CountSelected;
        }
        private bool CheckName(ICardAllDbInfo cai)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return true;
            }

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
                {
                    return false;
                }
            }
            if (ExcludeOnlineOnlyEditions)
            {
                if (_magicDatabase.GetEdition(cai.IdGatherer).IdBlock == _idBlockOnlineOnly)
                {
                    return false;
                }
            }

            if (EditionsSelected.Count == 0)
            {
                return true;
            }

            IEdition edition = _magicDatabase.GetEdition(cai.IdGatherer);

            return EditionsSelected.Contains(edition);
        }
        private bool CheckColor(ICardAllDbInfo cai)
        {
            if (ColorsSelected.Count == 0)
            {
                return true;
            }

            ShardColor color = MagicRules.GetColor(cai.Card.CastingCost);
            if (cai.Card.IsSplitted)
            {
                color |= MagicRules.GetColor(cai.CardPart2.CastingCost);
            }

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
            if (ExcludeSpecialCards)
            {
                if (MagicRules.IsSpecial(cai.Card.Type))
                {
                    return false;
                }
            }

            if (TypesSelected.Count == 0)
            {
                return true;
            }

            CardType type = MagicRules.GetCardType(cai.Card.Type, cai.Card.CastingCost);
            if (cai.Card.IsSplitted)
            {
                type |= MagicRules.GetCardType(cai.CardPart2.Type, cai.CardPart2.CastingCost);
            }

            CardType wantedType = TypesSelected.Aggregate(CardType.Token, (current, newtype) => current | newtype);

            if (TypeAggregation == MultiSelectedAggregation.And)
            {
                return Matcher<CardType>.IncludeValue(type, wantedType);
            }
            //TypeAggregation == MultiSelectedAggregation.Or
            return Matcher<CardType>.HasValue(type, wantedType);
        }

        private bool CheckSubType(ICardAllDbInfo cai)
        {
            if (SubTypesSelected.Count == 0)
            {
                return true;
            }

            CardSubType subType = MagicRules.GetCardSubType(cai.Card.Type);
            if (cai.Card.IsSplitted)
            {
                subType |= MagicRules.GetCardSubType(cai.CardPart2.Type);
            }

            CardSubType wantedSubType = SubTypesSelected.Aggregate(CardSubType.None, (current, newsubtype) => current | newsubtype);

            if (SubTypeAggregation == MultiSelectedAggregation.And)
            {
                return Matcher<CardSubType>.IncludeValue(subType, wantedSubType);
            }
            //SubTypeAggregation == MultiSelectedAggregation.Or
            return Matcher<CardSubType>.HasValue(subType, wantedSubType);
        }
    }
}
