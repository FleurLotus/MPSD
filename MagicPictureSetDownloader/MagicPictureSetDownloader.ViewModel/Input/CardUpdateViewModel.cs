
namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System.Linq;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;


    public class CardUpdateViewModel : DialogViewModelBase
    {

        private ICardCollection _destinationCardCollectionSelected;
        private readonly ICardCollection[] _collections;
        private IEdition _sourceEditionSelected;
        private IEdition _destinationEditionSelected;
        private ILanguage _sourceLanguageSelected;
        private ILanguage _destinationLanguageSelected;
        private ILanguage[] _sourceLanguages;
        private ILanguage[] _destinationLanguages;

        private bool _sourceIsFoil;
        private bool _destinationIsFoil;
        private int _count;
        private int _maxCount;
        private readonly IEdition[] _sourceEditions;
        private readonly IEdition[] _destinationEditions;
        private readonly ICardInCollectionCount[] _cardInCollectionCounts;
        private readonly IMagicDatabaseReadOnly _magicDatabase;


        public CardUpdateViewModel(string collectionName, ICard card, bool forMoving)
        {
            Card = card;

            ForMoving = forMoving;

            _magicDatabase = MagicDatabaseManager.ReadOnly;

            SourceCardCollection = _magicDatabase.GetAllCollections().First(cc => cc.Name == collectionName);
            _collections = _magicDatabase.GetAllCollections().Where(c => c != SourceCardCollection)
                                                             .ToArray();

            _cardInCollectionCounts = _magicDatabase.GetCollectionStatisticsForCard(SourceCardCollection, Card)
                                                    .ToArray();

            _sourceEditions = _cardInCollectionCounts.Select(cicc => _magicDatabase.GetEdition(cicc.IdGatherer))
                                                     .Ordered()
                                                     .ToArray();

            _destinationEditions = _magicDatabase.GetAllEditionIncludingCardOrdered(card)
                                                 .ToArray();

            if (_sourceEditions.Length > 0)
            {
                SourceEditionSelected = _sourceEditions[0];
                if (!ForMoving)
                    DestinationEditionSelected = SourceEditionSelected;
            }

            if (ForMoving)
            {
                if (_collections.Length > 0)
                    DestinationCardCollectionSelected = _collections[0];
            }
        }
        public bool ForMoving { get; private set; }
        public ICard Card { get; private set; }
        public IEdition[] SourceEditions
        {
            get { return _sourceEditions; }
        }
        public IEdition[] DestinationEditions
        {
            get { return _destinationEditions; }
        }

        public ICardCollection[] Collections
        {
            get { return _collections; }
        }
        public ICardCollection SourceCardCollection { get; private set; }

        public ILanguage[] DestinationLanguages
        {
            get { return _destinationLanguages; }
            private set
            {
                if (value != _destinationLanguages)
                {
                    _destinationLanguages = value;
                    OnNotifyPropertyChanged(() => DestinationLanguages);
                    if (_destinationLanguages != null && _destinationLanguages.Length > 0)
                        DestinationLanguageSelected = _destinationLanguages[0];
                }
            }
        }
        public ILanguage[] SourceLanguages
        {
            get { return _sourceLanguages; }
            private set
            {
                if (value != _sourceLanguages)
                {
                    _sourceLanguages = value;
                    OnNotifyPropertyChanged(() => SourceLanguages);
                    if (_sourceLanguages != null && _sourceLanguages.Length > 0)
                        SourceLanguageSelected = _sourceLanguages[0];
                }
            }
        }

        public bool SourceIsFoil
        {
            get { return _sourceIsFoil; }
            set
            {
                if (value != _sourceIsFoil)
                {
                    _sourceIsFoil = value;
                    OnNotifyPropertyChanged(() => SourceIsFoil);
                    UpdateMaxCount();
                }
            }
        }
        public IEdition SourceEditionSelected
        {
            get { return _sourceEditionSelected; }
            set
            {
                if (value != _sourceEditionSelected)
                {
                    _sourceEditionSelected = value;
                    OnNotifyPropertyChanged(() => SourceEditionSelected);
                    ChangeSourceLanguage();
                    UpdateMaxCount();
                }
            }
        }
        public ILanguage SourceLanguageSelected
        {
            get { return _sourceLanguageSelected; }
            set
            {
                if (value != _sourceLanguageSelected)
                {
                    _sourceLanguageSelected = value;
                    OnNotifyPropertyChanged(() => SourceLanguageSelected);
                    UpdateMaxCount();
                }
            }
        }
        public bool DestinationIsFoil
        {
            get { return _destinationIsFoil; }
            set
            {
                if (value != _destinationIsFoil)
                {
                    _destinationIsFoil = value;
                    OnNotifyPropertyChanged(() => DestinationIsFoil);
                }
            }
        }
        public IEdition DestinationEditionSelected
        {
            get { return _destinationEditionSelected; }
            set
            {
                if (value != _destinationEditionSelected)
                {
                    _destinationEditionSelected = value;
                    OnNotifyPropertyChanged(() => DestinationEditionSelected);
                    ChangeDestinationLanguage();
                    if (_destinationEditionSelected != null && !_destinationEditionSelected.HasFoil)
                        DestinationIsFoil = false;
                }
            }
        }
        public ILanguage DestinationLanguageSelected
        {
            get { return _destinationLanguageSelected; }
            set
            {
                if (value != _destinationLanguageSelected)
                {
                    _destinationLanguageSelected = value;
                    OnNotifyPropertyChanged(() => DestinationLanguageSelected);
                }
            }
        }
        public ICardCollection DestinationCardCollectionSelected
        {
            get { return _destinationCardCollectionSelected; }
            set
            {
                if (value != _destinationCardCollectionSelected)
                {
                    _destinationCardCollectionSelected = value;
                    OnNotifyPropertyChanged(() => DestinationCardCollectionSelected);
                }
            }
        }
        public int Count
        {
            get { return _count; }
            set
            {
                if (value != _count)
                {
                    _count = value;
                    OnNotifyPropertyChanged(() => Count);
                }
            }
        }
        public int MaxCount
        {
            get { return _maxCount; }
            set
            {
                if (value != _maxCount)
                {
                    _maxCount = value;
                    OnNotifyPropertyChanged(() => MaxCount);

                    if (value < Count)
                        Count = value;
                }
            }
        }

        private void UpdateMaxCount()
        {
            int idGatherer = _magicDatabase.GetIdGatherer(Card, SourceEditionSelected);
            ICardInCollectionCount cardInCollectionCount = _cardInCollectionCounts.FirstOrDefault(cicc => cicc.IdGatherer == idGatherer && cicc.IdLanguage == SourceLanguageSelected.Id);
          
            if (cardInCollectionCount == null)
            {
                MaxCount = 0;
                return;
            }

            MaxCount = SourceIsFoil? cardInCollectionCount.FoilNumber: cardInCollectionCount.Number;
        }

        private void ChangeSourceLanguage()
        {
            int idGatherer = _magicDatabase.GetIdGatherer(Card, SourceEditionSelected);
            SourceLanguages = _magicDatabase.GetLanguages(idGatherer).ToArray();
        }
        private void ChangeDestinationLanguage()
        {
            int idGatherer = _magicDatabase.GetIdGatherer(Card, DestinationEditionSelected);
            DestinationLanguages = _magicDatabase.GetLanguages(idGatherer).ToArray();
        }
        protected override bool OkCommandCanExecute(object o)
        {
            if (Count <= 0 || Count > MaxCount || SourceEditionSelected == null)
                return false;

            if (ForMoving)
                return DestinationCardCollectionSelected != null && DestinationCardCollectionSelected != SourceCardCollection;

            return DestinationEditionSelected != null && DestinationLanguageSelected!= null && 
                   (DestinationLanguageSelected != SourceLanguageSelected || DestinationEditionSelected != SourceEditionSelected || SourceIsFoil != DestinationIsFoil) && 
                   (DestinationEditionSelected.HasFoil || !DestinationIsFoil);
        }
    }
}
