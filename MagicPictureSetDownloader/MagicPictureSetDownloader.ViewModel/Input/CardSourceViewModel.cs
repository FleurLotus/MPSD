namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System.Linq;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Interface;

    public class CardSourceViewModel : NotifyPropertyChangedBase
    {
        private IEdition _editionSelected;
        private ILanguage _languageSelected;
        private ILanguage[] _languages;

        private bool _isFoil;
        private int _maxCount;
        private int _count;
        private readonly IEdition[] _editions;
        private readonly ICardInCollectionCount[] _cardInCollectionCounts;

        private readonly IMagicDatabaseReadOnly _magicDatabase;

        public CardSourceViewModel(IMagicDatabaseReadOnly magicDatabase, ICardCollection sourceCardCollection, ICard card)
        {
            _magicDatabase = magicDatabase;

            Card = card;

            _cardInCollectionCounts = _magicDatabase.GetCollectionStatisticsForCard(sourceCardCollection, Card)
                .ToArray();

            _editions = _cardInCollectionCounts.Select(cicc => _magicDatabase.GetEdition(cicc.IdGatherer))
                .Distinct()
                .Ordered()
                .ToArray();

            if (_editions.Length > 0)
            {
                EditionSelected = _editions[0];
            }
        }
        public ICard Card { get; private set; }
        public IEdition[] Editions
        {
            get { return _editions; }
        }
        public ILanguage[] Languages
        {
            get { return _languages; }
            private set
            {
                if (value != _languages)
                {
                    _languages = value;
                    OnNotifyPropertyChanged(() => Languages);
                    if (_languages != null && _languages.Length > 0)
                        LanguageSelected = _languages[0];
                }
            }
        }
        public bool IsFoil
        {
            get { return _isFoil; }
            set
            {
                if (value != _isFoil)
                {
                    _isFoil = value;
                    OnNotifyPropertyChanged(() => IsFoil);
                    UpdateMaxCount();
                }
            }
        }
        public IEdition EditionSelected
        {
            get { return _editionSelected; }
            set
            {
                if (value != _editionSelected)
                {
                    _editionSelected = value;
                    OnNotifyPropertyChanged(() => EditionSelected);
                    ChangeLanguage();
                    UpdateMaxCount();
                }
            }
        }
        public ILanguage LanguageSelected
        {
            get { return _languageSelected; }
            set
            {
                if (value != _languageSelected)
                {
                    _languageSelected = value;
                    OnNotifyPropertyChanged(() => LanguageSelected);
                    UpdateMaxCount();
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

        private void UpdateMaxCount()
        {
            int idGatherer = _magicDatabase.GetIdGatherer(Card, EditionSelected);
            if (LanguageSelected == null)
            {
                MaxCount = 0;
                return;
            }

            ICardInCollectionCount cardInCollectionCount = _cardInCollectionCounts.FirstOrDefault(cicc => cicc.IdGatherer == idGatherer && cicc.IdLanguage == LanguageSelected.Id);
          
            if (cardInCollectionCount == null)
            {
                MaxCount = 0;
                return;
            }
            MaxCount = IsFoil ? cardInCollectionCount.FoilNumber : cardInCollectionCount.Number;
        }
        private void ChangeLanguage()
        {
            int idGatherer = _magicDatabase.GetIdGatherer(Card, EditionSelected);
            Languages = _cardInCollectionCounts.Where(cicc => cicc.IdGatherer == idGatherer)
                                                     .Select(cicc => _magicDatabase.GetLanguage(cicc.IdLanguage))
                                                     .Distinct()
                                                     .OrderBy(l => l.Id)
                                                     .ToArray();
        }
    }
}
