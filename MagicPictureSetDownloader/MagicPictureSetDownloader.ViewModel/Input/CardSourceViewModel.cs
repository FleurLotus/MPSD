namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System.Linq;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public class CardSourceViewModel : NotifyPropertyChangedBase
    {
        private IEdition _editionSelected;
        private ILanguage _languageSelected;
        private ILanguage[] _languages;

        private bool _isFoil;
        private int _maxCount;
        private int _count;
        private readonly ICardInCollectionCount[] _cardInCollectionCounts;

        private readonly IMagicDatabaseReadOnly _magicDatabase;

        public CardSourceViewModel(IMagicDatabaseReadOnly magicDatabase, ICardCollection sourceCardCollection, ICard card)
        {
            _magicDatabase = magicDatabase;

            Card = card;

            _cardInCollectionCounts = _magicDatabase.GetCollectionStatisticsForCard(sourceCardCollection, Card)
                .ToArray();

            Editions = _cardInCollectionCounts.Select(cicc => _magicDatabase.GetEditionByIdScryFall(cicc.IdScryFall))
                .Distinct()
                .Ordered()
                .ToArray();

            if (Editions.Length > 0)
            {
                EditionSelected = Editions[0];
            }
        }
        public ICard Card { get; }
        public IEdition[] Editions { get; }
        public ILanguage[] Languages
        {
            get { return _languages; }
            private set
            {
                if (value != _languages)
                {
                    _languages = value;
                    OnNotifyPropertyChanged();
                    if (_languages != null && _languages.Length > 0)
                    {
                        LanguageSelected = _languages[0];
                    }
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
                    OnNotifyPropertyChanged();
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
                    OnNotifyPropertyChanged();
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
                    OnNotifyPropertyChanged();
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
                    OnNotifyPropertyChanged();

                    if (value < Count)
                    {
                        Count = value;
                    }
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
                    OnNotifyPropertyChanged();
                }
            }
        }

        private void UpdateMaxCount()
        {
            string idScryFall = _magicDatabase.GetIdScryFall(Card, EditionSelected);
            if (LanguageSelected == null)
            {
                MaxCount = 0;
                return;
            }

            ICardInCollectionCount cardInCollectionCount = _cardInCollectionCounts.FirstOrDefault(cicc => cicc.IdScryFall == idScryFall && cicc.IdLanguage == LanguageSelected.Id);

            if (cardInCollectionCount == null)
            {
                MaxCount = 0;
                return;
            }
            MaxCount = cardInCollectionCount.GetCount(new CardCountKey(IsFoil));
        }
        private void ChangeLanguage()
        {
            string idScryFall = _magicDatabase.GetIdScryFall(Card, EditionSelected);
            Languages = _cardInCollectionCounts.Where(cicc => cicc.IdScryFall == idScryFall)
                                                     .Select(cicc => _magicDatabase.GetLanguage(cicc.IdLanguage))
                                                     .Distinct()
                                                     .OrderBy(l => l.Id)
                                                     .ToArray();
        }
    }
}