
namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System.Linq;
    using System.Windows.Input;

    using Common.Libray.Collection;
    using Common.ViewModel;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public enum InputMode
    {
        None,
        ByEdition,
        ByCard,
    }

    public class CardInputViewModel : DialogViewModelBase
    {

        private InputMode _inputMode = InputMode.ByEdition;
        private bool _isFoil;
        private int _count;
        private ICard _cardSelected;
        private readonly ICard[] _allCards;
        private IEdition _editionSelected;
        private ILanguage _languageSelected;
        private string _currentCollectionDetail;
        private readonly IEdition[] _allEditions;
        private ICardCollection _cardCollection;
        private readonly ICardCollection[] _collections;

        private readonly IMagicDatabaseReadAndWriteCardInCollection _magicDatabase;
        private readonly ICardAllDbInfo[] _allCardInfos;

        public CardInputViewModel(string name)
        {
            _magicDatabase = MagicDatabaseManager.ReadAndWriteCardInCollection;

            Display.Title = "Input cards";
            Display.OkCommandLabel = "Add";
            Display.CancelCommandLabel = "Close";
            ChangeCollectionCommand = new RelayCommand(ChangeCollectionCommandExecute);

            _allCardInfos = _magicDatabase.GetAllInfos().ToArray();
            _collections = _magicDatabase.GetAllCollections().ToArray();
            SelectCardCollection(name);

            _allCards = _allCardInfos.GetAllCardOrdered().ToArray();
            Cards = new RangeObservableCollection<ICard>();

            _allEditions = _magicDatabase.GetAllEditionsOrdered();
            Editions = new RangeObservableCollection<IEdition>();

            Languages = new RangeObservableCollection<ILanguage>();

            InitWindow();
        }
        public ICommand ChangeCollectionCommand { get; private set; }
        public RangeObservableCollection<IEdition> Editions { get; private set; }
        public RangeObservableCollection<ILanguage> Languages { get; private set; }
        public RangeObservableCollection<ICard> Cards { get; private set; }

        public ILanguage LanguageSelected
        {
            get { return _languageSelected; }
            set
            {
                if (value != _languageSelected)
                {
                    _languageSelected = value;
                    OnNotifyPropertyChanged(() => LanguageSelected);
                    RefreshDisplayedData(InputMode.None);
                }
            }
        }

        public ICardCollection CardCollection
        {
            get { return _cardCollection; }
            set
            {
                if (value != _cardCollection)
                {
                    _cardCollection = value;
                    OnNotifyPropertyChanged(() => CardCollection);
                }
            }
        }
        public string CurrentCollectionDetail
        {
            get { return _currentCollectionDetail; }
            private set
            {
                if (value != _currentCollectionDetail)
                {
                    _currentCollectionDetail = value;
                    OnNotifyPropertyChanged(() => CurrentCollectionDetail);
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
                    RefreshDisplayedData(InputMode.None);
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
                    RefreshDisplayedData(InputMode.ByEdition);
                    if (_editionSelected != null && !_editionSelected.HasFoil)
                        IsFoil = false;

                }
            }
        }
        public ICard CardSelected
        {
            get { return _cardSelected; }
            set
            {
                if (value != _cardSelected)
                {
                    _cardSelected = value;
                    OnNotifyPropertyChanged(() => CardSelected);
                    RefreshDisplayedData(InputMode.ByCard);
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
        public InputMode InputMode
        {
            get { return _inputMode; }
            set
            {
                if (value != _inputMode)
                {
                    _inputMode = value;
                    OnNotifyPropertyChanged(() => InputMode);
                    InitWindow();
                }
            }
        }

        protected override void OkCommandExecute(object o)
        {
            AddNewCard();
            InitWindow();
        }
        protected override bool OkCommandCanExecute(object o)
        {
            return Count != 0 && EditionSelected != null && CardSelected != null && _languageSelected != null && (EditionSelected.HasFoil || !IsFoil);
        }
        private void ChangeCollectionCommandExecute(object obj)
        {
            InputViewModel vm = InputViewModelFactory.Instance.CreateChooseInListViewModel("Other collection", "Choose other collection to input", _collections.Select(c => c.Name).ToList());
            OnInputRequestedRequested(vm);
            if (vm.Result.HasValue && vm.Result.Value)
            {
                string collection = vm.Selected;

                if (!string.IsNullOrWhiteSpace(collection))
                {
                    SelectCardCollection(collection);
                }
            }
        }
        private void InitWindow()
        {
            switch (InputMode)
            {
                case InputMode.ByCard:
                    {
                        Cards.Clear();
                        Cards.AddRange(_allCards);

                        Editions.Clear();
                        EditionSelected = null;
                        CardSelected = null;

                        break;
                    }

                case InputMode.ByEdition:
                    {
                        Cards.Clear();

                        IEdition save = EditionSelected;
                        Editions.Clear();
                        Editions.AddRange(_allEditions);

                        CardSelected = null;
                        EditionSelected = save;
                        break;
                    }

                case InputMode.None:

                    Cards.Clear();
                    Editions.Clear();
                    EditionSelected = null;
                    CardSelected = null;
                    break;
            }

            Languages.Clear();
            CurrentCollectionDetail = null;
            LanguageSelected = null;
            Count = 1;
            IsFoil = false;
        }
        private void AddNewCard()
        {
            int count = IsFoil ? 0 : Count;
            int foilCount = IsFoil ? Count: 0;

            ICardAllDbInfo cardAllDbInfo = _allCardInfos.First(cadi => cadi.Edition == EditionSelected && cadi.Card == CardSelected);
            _magicDatabase.InsertOrUpdateCardInCollection(CardCollection.Id, cardAllDbInfo.IdGatherer, LanguageSelected.Id, count, foilCount);
        }
        private void SelectCardCollection(string name)
        {
            CardCollection = _collections.First(cc => cc.Name == name);
            RefreshDisplayedData(InputMode.None);
        }
        private void RefreshDisplayedData(InputMode modifyData)
        {
            UpdateCurrentCollectionDetail();

            //None one the key changed, nothing to recompute
            if (modifyData == InputMode.None)
                return;

            //Change one of the key but no the reference one
            if (InputMode != modifyData)
            {
                Languages.Clear();
                IEdition editionSelected = EditionSelected;
                ICard cardNameSelected = CardSelected;
                if (editionSelected == null || cardNameSelected == null)
                    return;

                ICardAllDbInfo cardAllDbInfo = _allCardInfos.First(cadi => cadi.Edition == editionSelected && cadi.Card == cardNameSelected);
                if (cardAllDbInfo == null)
                    return;

                foreach (ILanguage language in _magicDatabase.GetLanguages(cardAllDbInfo.IdGatherer))
                    Languages.Add(language);

                if (Languages.Count > 0)
                    LanguageSelected = Languages[0];
            }
            else
            {
                //Change the reference
                switch (InputMode)
                {
                    case InputMode.ByEdition:

                        IEdition editionSelected = EditionSelected;
                        Cards.Clear();
                        Languages.Clear();
                        if (editionSelected == null)
                            return;

                        foreach (ICard card in _allCardInfos.GetAllCardOrdered(editionSelected))
                            Cards.Add(card);

                        break;

                    case InputMode.ByCard:

                        ICard cardNameSelected = CardSelected;
                        Editions.Clear();
                        Languages.Clear();
                        if (cardNameSelected == null)
                            return;

                        foreach (IEdition edition in _allCardInfos.GetAllEditionIncludingCardOrdered(cardNameSelected))
                            Editions.Add(edition);

                        break;
                }
            }
        }
        private void UpdateCurrentCollectionDetail()
        {
            if (EditionSelected == null || CardSelected == null || LanguageSelected == null)
            {
                CurrentCollectionDetail = null;
                return;
            }

            int totalInCollection = 0;
            int totalInEditionInCollection = 0;
            int totalInEditionAndLanguageInCollectionNotFoil = 0;
            int totalInEditionAndLanguageInCollectionFoil = 0;

            foreach (ICardInCollectionCount cardInCollectionCount in _magicDatabase.GetCollectionStatisticsForCard(CardCollection, CardSelected))
            {
                int inCollection = cardInCollectionCount.Number + cardInCollectionCount.FoilNumber;
                totalInCollection += inCollection;
                if (_magicDatabase.GetEdition(cardInCollectionCount.IdGatherer) == EditionSelected)
                {
                    totalInEditionInCollection += inCollection;
                    if (cardInCollectionCount.IdLanguage == LanguageSelected.Id)
                    {
                        totalInEditionAndLanguageInCollectionNotFoil += cardInCollectionCount.Number;
                        totalInEditionAndLanguageInCollectionFoil += cardInCollectionCount.FoilNumber;
                    }
                }
            }

            CurrentCollectionDetail = string.Format("{2}+{3}(Foil) {0} {1}\n{4} {0}\n{5} All Edition", EditionSelected.Code, LanguageSelected.Name, totalInEditionAndLanguageInCollectionNotFoil,
                                                                                                       totalInEditionAndLanguageInCollectionFoil, totalInEditionInCollection, totalInCollection);
        }
    }
}
