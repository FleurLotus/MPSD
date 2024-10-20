﻿namespace MagicPictureSetDownloader.ViewModel.Deck
{
    using System.Collections.Generic;
    using System.Linq;

    using Common.ViewModel.Dialog;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Main;

    public class PreconstructedDecksViewModel: DialogViewModelBase
    {
        private PreconstructedDeckViewModel _preconstructedDeckSelected;
        private ICardCollection _cardCollectionSelected;
        private ILanguage _languageSelected;
        private readonly IMagicDatabaseReadOnly _magicDatabase;

        public PreconstructedDecksViewModel()
        {
            _magicDatabase = MagicDatabaseManager.ReadOnly;
            Decks = LoadReferentialData();
            Collections = new List<ICardCollection>(_magicDatabase.GetAllCollections()).AsReadOnly();
            Languages = new List<ILanguage>(_magicDatabase.GetAllLanguages()).AsReadOnly();
        }
        public IList<PreconstructedDeckViewModel> Decks { get; }
        public IList<ICardCollection> Collections { get; }
        public IList<ILanguage> Languages { get; }

        public PreconstructedDeckViewModel PreconstructedDeckSelected
        {
            get
            {
                return _preconstructedDeckSelected;
            }
            set
            {
                if (_preconstructedDeckSelected != value)
                {
                    _preconstructedDeckSelected = value;
                    OnNotifyPropertyChanged(nameof(PreconstructedDeckSelected));
                }
            }
        }
        public ICardCollection CardCollectionSelected
        {
            get
            {
                return _cardCollectionSelected;
            }
            set
            {
                if (_cardCollectionSelected != value)
                {
                    _cardCollectionSelected = value;
                    OnNotifyPropertyChanged(nameof(CardCollectionSelected));
                }
            }
        }
        public ILanguage LanguageSelected
        {
            get
            {
                return _languageSelected;
            }
            set
            {
                if (_languageSelected != value)
                {
                    _languageSelected = value;
                    OnNotifyPropertyChanged(nameof(LanguageSelected));
                }
            }
        }

        private IList<PreconstructedDeckViewModel> LoadReferentialData()
        {
            List<PreconstructedDeckViewModel> ret = new List<PreconstructedDeckViewModel>();
            IDictionary<string, CardViewModel> allCardsViewModel = _magicDatabase.GetAllInfos().ToDictionary(cai => cai.IdScryFall, cai => new CardViewModel(cai));

            foreach (IPreconstructedDeck preconstructedDeck in _magicDatabase.GetAllPreconstructedDecks())
            {
                ICollection<IPreconstructedDeckCardEdition> deckComposition = _magicDatabase.GetPreconstructedDeckCards(preconstructedDeck);
                IEdition edition = preconstructedDeck.IdEdition.HasValue ? _magicDatabase.GetEditionById(preconstructedDeck.IdEdition.Value) : null;

                ret.Add(new PreconstructedDeckViewModel(preconstructedDeck, edition,
                            deckComposition.Select(pdce => new KeyValuePair<CardViewModel, int>(allCardsViewModel[pdce.IdScryFall], pdce.Number))));
            }

            ret.Sort((x, y) =>
            {
                //Most recent set first
                if (!x.EditionDate.HasValue)
                {
                    return y.EditionDate.HasValue ? 1 : 0;
                }
                if (!y.EditionDate.HasValue)
                {
                    return -1;
                }
                int comp = y.EditionDate.Value.CompareTo(x.EditionDate.Value);
                if (comp == 0)
                {
                    //Alphabet order
                    comp = x.Name.CompareTo(y.Name);
                }


                return comp;
            });

            return ret.AsReadOnly();
        }

        protected override bool OkCommandCanExecute(object o)
        {
            return CardCollectionSelected != null && PreconstructedDeckSelected != null && LanguageSelected != null;
        }
    }
}
