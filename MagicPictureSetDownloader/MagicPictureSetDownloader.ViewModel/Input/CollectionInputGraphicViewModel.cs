namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Input;

    using Common.Library.Collection;
    using Common.ViewModel;
    using Common.ViewModel.Dialog;
    using Common.ViewModel.Input;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Main;

    public class CollectionInputGraphicViewModel : DialogViewModelBase
    {
        private IEdition _editionSelected;
        private ILanguage _inputLanguage;
        private string _filter;
        private bool _foil;
        private bool _hasChange;
        private int _size;
        private IDictionary<ICard, string> _allCardTranslation;
        private readonly RangeObservableCollection<CardCollectionInputGraphicViewModel> _cards;

        private readonly IMagicDatabaseReadAndWriteCardInCollection _magicDatabase;
        private readonly IMagicDatabaseReadAndWriteOption _magicDatabaseForOption;
        private readonly ICardAllDbInfo[] _allCardInfos;
        private readonly ILanguage[] _allLanguages;

        public CollectionInputGraphicViewModel(string name)
        {
            _magicDatabase = MagicDatabaseManager.ReadAndWriteCardInCollection;
            _magicDatabaseForOption = MagicDatabaseManager.ReadAndWriteOption;

            IOption option = _magicDatabaseForOption.GetOption(TypeOfOption.Input, "Language");
            if (option != null)
            {
                int id;
                if (int.TryParse(option.Value, out id))
                {
                    _inputLanguage = _magicDatabase.GetLanguage(id);
                }
            }

            Display.Title = "Input cards";
            Display.OkCommandLabel = "Add";
            Display.CancelCommandLabel = "Close";

            _allCardInfos = _magicDatabase.GetAllInfos().ToArray();
            _allLanguages = _magicDatabase.GetAllLanguages().ToArray();

            Editions = _magicDatabase.GetAllEditionsOrdered();
            _cards = new RangeObservableCollection<CardCollectionInputGraphicViewModel>();
            Cards = CollectionViewSource.GetDefaultView(_cards);
            Cards.Filter = ToDisplay;
            ChangeInputLanguageCommand = new RelayCommand(ChangeInputLanguageCommandExecute);
            ResetCommand = new RelayCommand(ResetCommandExecute);
            CardCollection = _magicDatabase.GetAllCollections().First(cc => cc.Name == name);
            Size = 126;
            AddLinkedProperty(nameof(InputLanguage), nameof(InputLanguageName));

            RebuildOrder();
        }

        public ICommand ChangeInputLanguageCommand { get; }
        public ICommand ResetCommand { get; }
        public IEdition[] Editions { get; }
        public ICardCollection CardCollection { get; }
        public ICollectionView Cards { get; private set; }

        public string InputLanguageName
        {
            get { return _inputLanguage == null ? "Default" : _inputLanguage.Name; }
        }
        public bool Foil
        {
            get { return _foil; }
            set
            {
                if (value != _foil)
                {
                    _foil = value;
                    OnNotifyPropertyChanged(nameof(Foil));
                    RefreshDisplayedData(false);
                }
            }
        }
        public bool HasChange
        {
            get { return _hasChange; }
            set
            {
                if (value != _hasChange)
                {
                    _hasChange = value;
                    OnNotifyPropertyChanged(nameof(HasChange));
                }
            }
        }
        public int Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    OnNotifyPropertyChanged(nameof(Size));
                }
            }
        }
        public ILanguage InputLanguage
        {
            get { return _inputLanguage; }
            set
            {
                if (value != _inputLanguage)
                {
                    _inputLanguage = value;
                    OnNotifyPropertyChanged(nameof(InputLanguage));
                    RebuildOrder();
                    RefreshDisplayedData(false);
                }
            }
        }
        public string Filter
        {
            get { return _filter; }
            set
            {
                if (value != _filter)
                {
                    _filter = value;
                    OnNotifyPropertyChanged(nameof(Filter));
                    Cards.Refresh();
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
                    OnNotifyPropertyChanged(nameof(EditionSelected));
                    RefreshDisplayedData(true);
                }
            }
        }

        private void ResetCommandExecute(object obj)
        {
            foreach (CardCollectionInputGraphicViewModel card in _cards)
            {
                card.Reset();
            }
        }
        private void ChangeInputLanguageCommandExecute(object obj)
        {
            InputViewModel vm = InputViewModelFactory.Instance.CreateChooseInListViewModel("Input language", "Choose input language", _allLanguages.Select(c => c.Name).ToList());
            OnInputRequestedRequested(vm);
            if (vm.Result == true)
            {
                string languageName = vm.Selected;

                if (!string.IsNullOrWhiteSpace(languageName))
                {
                    SelectInputLanguage(languageName);
                }
            }
        }
        private void SelectInputLanguage(string name)
        {
            InputLanguage = _allLanguages.First(l => l.Name == name);
            if (InputLanguage == _magicDatabase.GetDefaultLanguage())
            {
                InputLanguage = null;
                _magicDatabaseForOption.DeleteOption(TypeOfOption.Input, "Language");
            }
            else
            {
                _magicDatabaseForOption.InsertNewOption(TypeOfOption.Input, "Language", InputLanguage.Id.ToString(CultureInfo.InvariantCulture));
            }
        }
        protected override void OkCommandExecute(object o)
        {
            foreach (CardCollectionInputGraphicViewModel card in _cards.Where(c => c.ChangedCount != 0))
            {
                CardCount cardCount = new CardCount();
                cardCount.Add(new CardCountKey(Foil, false), card.ChangedCount);

                _magicDatabase.InsertOrUpdateCardInCollection(CardCollection.Id, card.Card.IdGatherer, InputLanguage.Id, cardCount);
            }

            RefreshDisplayedData(true);
            HasChange = false;
        }
        protected override bool OkCommandCanExecute(object o)
        {
            return HasChange;
        }
        private void RebuildOrder()
        {
            _allCardTranslation = _allCardInfos.GetAllCardWithTranslation(_inputLanguage).ToDictionary(kv => kv.Value, kv => kv.Key);
        }
        private bool ToDisplay(object o)
        {
            if (string.IsNullOrEmpty(Filter))
            {
                return true;
            }

            return ((CardCollectionInputGraphicViewModel)o).NameInLanguage.Contains(Filter);
        }
        private void ItemChanged(object sender, PropertyChangedEventArgs e)
        {
            HasChange = _cards.Any(c => c.ChangedCount != 0);
        }
        private void RefreshDisplayedData(bool full)
        {
            IEdition editionSelected = EditionSelected;
            if (full)
            {
                foreach (CardCollectionInputGraphicViewModel c in _cards)
                {
                    c.PropertyChanged -= ItemChanged;
                }
                _cards.Clear();
            }

            if (editionSelected == null)
            {
                return;
            }

            List<CardCollectionInputGraphicViewModel> toSort = new List<CardCollectionInputGraphicViewModel>();
            if (full)
            {
                foreach (ICardAllDbInfo cardInfo in _allCardInfos.Where(cadi => cadi.Edition == editionSelected))
                {
                    CardCollectionInputGraphicViewModel newCard = new CardCollectionInputGraphicViewModel(new CardViewModel(cardInfo));
                    newCard.PropertyChanged += ItemChanged;
                    toSort.Add(newCard);
                }
            }
            else
            {
                toSort.AddRange(_cards);
                _cards.Clear();
            }

            foreach (CardCollectionInputGraphicViewModel ccigvm in toSort)
            {
                CardViewModel card = ccigvm.Card;
                _allCardTranslation.TryGetValue(card.Card, out string name);
                int count = 0;

                foreach (ICardInCollectionCount cardInCollectionCount in _magicDatabase.GetCollectionStatisticsForCard(CardCollection, card.Card)
                                .Where(cicc =>  cicc.IdLanguage == InputLanguage.Id && _magicDatabase.GetEdition(cicc.IdGatherer).Id == editionSelected.Id))
                {
                    count += Foil ? cardInCollectionCount.FoilNumber : cardInCollectionCount.Number;

                }
                ccigvm.SetInfo(name, count);

            }

            toSort.Sort();
            _cards.AddRange(toSort);
        }
    }
}
