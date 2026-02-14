namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System;
    using System.Linq;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public class CardSourceViewModel : NotifyPropertyChangedBase
    {
        private IEdition _editionSelected;
        private string[] _idScryfalls;
        private string _idScryfallSelected;
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
        public string[] IdScryfalls
        {
            get { return _idScryfalls; }
            private set
            {
                if (value != _idScryfalls)
                {
                    _idScryfalls = value;
                    OnNotifyPropertyChanged();
                    if (_idScryfalls != null && _idScryfalls.Length > 0)
                    {
                        IdScryfallSelected = _idScryfalls[0];
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
                    ChangeIdScryfalls();
                    ChangeLanguage();
                    UpdateMaxCount();
                }
            }
        }
        public string IdScryfallSelected
        {
            get { return _idScryfallSelected; }
            set
            {
                if (value != _idScryfallSelected)
                {
                    _idScryfallSelected = value;
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
            if (LanguageSelected == null || IdScryfallSelected == null)
            {
                MaxCount = 0;
                return;
            }

            ICardInCollectionCount cardInCollectionCount = _cardInCollectionCounts.FirstOrDefault(cicc => cicc.IdScryFall == IdScryfallSelected && cicc.IdLanguage == LanguageSelected.Id);

            if (cardInCollectionCount == null)
            {
                MaxCount = 0;
                return;
            }
            MaxCount = cardInCollectionCount.GetCount(new CardCountKey(IsFoil));
        }
        private void ChangeIdScryfalls()
        {
            string[] idScryFalls = _magicDatabase.GetAllIdScryFall(Card, EditionSelected);

            IdScryfalls = idScryFalls.Where(id => _cardInCollectionCounts.Any(cicc => cicc.IdScryFall == id)).ToArray();
        }
        private void ChangeLanguage()
        {
            if (IdScryfallSelected == null)
            {
                Languages = Array.Empty<ILanguage>();
                return;
            }
            Languages = _cardInCollectionCounts.Where(cicc => cicc.IdScryFall == IdScryfallSelected)
                                               .Select(cicc => _magicDatabase.GetLanguage(cicc.IdLanguage))
                                               .Distinct()
                                               .OrderBy(l => l.Id)
                                               .ToArray();
        }
    }
}