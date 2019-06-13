namespace MagicPictureSetDownloader.ViewModel.Deck
{
    using System.Collections.Generic;
    using System.Linq;

    using Common.ViewModel.Dialog;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Main;

    public class PreconstructedDecksViewModel: DialogViewModelBase
    {
        private PreconstructedDeckViewModel _selectedPreconstructedDeck;
        private readonly IMagicDatabaseReadOnly _magicDatabase;
        
        public PreconstructedDecksViewModel()
        {
            _magicDatabase = MagicDatabaseManager.ReadOnly;
            Decks = LoadReferentialData();
        }
        public IList<PreconstructedDeckViewModel> Decks { get; }

        public PreconstructedDeckViewModel SelectedPreconstructedDeck
        {
            get
            {
                return _selectedPreconstructedDeck;
            }
            set
            {
                if (_selectedPreconstructedDeck != value)
                {
                    _selectedPreconstructedDeck = value;
                    OnNotifyPropertyChanged(nameof(SelectedPreconstructedDeck));
                }
            }
        }

        private IList<PreconstructedDeckViewModel> LoadReferentialData()
        {
            List<PreconstructedDeckViewModel> ret = new List<PreconstructedDeckViewModel>();
            IDictionary<int, CardViewModel> allCardsViewModel = _magicDatabase.GetAllInfos().ToDictionary(cai => cai.IdGatherer, cai => new CardViewModel(cai));

            foreach (IPreconstructedDeck preconstructedDeck in _magicDatabase.GetAllPreconstructedDecks())
            {
                ICollection<IPreconstructedDeckCardEdition> deckComposition = _magicDatabase.GetPreconstructedDeckCards(preconstructedDeck);
                string edition = _magicDatabase.GetEditionById(preconstructedDeck.IdEdition).Name;

                ret.Add(new PreconstructedDeckViewModel(edition, preconstructedDeck.Name, 
                            deckComposition.Select(pdce => new KeyValuePair<CardViewModel, int>(allCardsViewModel[pdce.IdGatherer], pdce.Number))));
            }

            return ret.AsReadOnly();
        }


    }
}
