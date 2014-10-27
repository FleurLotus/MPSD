
namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;

    using Common.Libray;
    using Common.ViewModel;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public enum InputMode
    {
        None,
        ByEdition,
        ByCard,
    }

    public class CardInputViewModel : NotifyPropertyChangedBase
    {
        public event EventHandler Closing;
        public event EventHandler<EventArgs<InputViewModel>> InputRequested;

        private InputMode _inputMode = InputMode.ByEdition;
        private bool _isFoil;
        private int _count;
        private ICard _cardSelected;
        private readonly ICard[] _cards;
        private IEdition _editionSelected;
        private string _currentCollectionDetail;
        private readonly IEdition[] _editions;
        private ICardCollection _cardCollection;
        private readonly ICardCollection[] _collections;

        private readonly IMagicDatabaseReadAndWriteCardInCollection _magicDatabase;
        private readonly ICardAllDbInfo[] _allCardInfos;

        //ALERT: language + CHECK input  foil x edition x language ?
        public CardInputViewModel(string name)
        {
            _magicDatabase = MagicDatabaseManager.ReadAndWriteCardInCollection;

            AddCommand = new RelayCommand(AddCommandExecute, AddCommandCanExecute);
            CloseCommand = new RelayCommand(CloseCommandExecute);
            ChangeCollectionCommand = new RelayCommand(ChangeCollectionCommandExecute);

            _allCardInfos = _magicDatabase.GetAllInfos().ToArray();
            _collections = _magicDatabase.GetAllCollections().ToArray();
            SelectCardCollection(name);

            _cards = _allCardInfos.Select(cadi => cadi.Card).Distinct().OrderBy(c => c.ToString()).ToArray();
            Cards = new ObservableCollection<ICard>();

            _editions = _magicDatabase.AllEditions().OrderByDescending(ed => ed.ReleaseDate).ToArray();
            Editions = new ObservableCollection<IEdition>();

            InitWindow();
        }
        public ICommand AddCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand ChangeCollectionCommand { get; private set; }
        public ObservableCollection<IEdition> Editions { get; private set; }
        public ObservableCollection<ICard> Cards { get; private set; }

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

        private void AddCommandExecute(object o)
        {
            AddNewCard();
            InitWindow();
        }
        private void CloseCommandExecute(object o)
        {
            OnClosing();
        }
        private bool AddCommandCanExecute(object o)
        {
            return Count != 0 && EditionSelected != null && CardSelected != null;
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
        private void OnClosing()
        {
            var e = Closing;
            if (e != null)
                e(this, EventArgs.Empty);
        }
        private void OnInputRequestedRequested(InputViewModel vm)
        {
            var e = InputRequested;
            if (e != null && vm != null)
                e(this, new EventArgs<InputViewModel>(vm));
        }
        private void InitWindow()
        {
            switch (InputMode)
            {
                case InputMode.ByCard:

                    Cards.Clear();

                    foreach (ICard card in _cards)
                        Cards.Add(card);
                    
                    Editions.Clear();
                    EditionSelected = null;
                    CardSelected = null;
                    break;

                case InputMode.ByEdition:

                    Cards.Clear();

                    IEdition save = EditionSelected;
                    Editions.Clear();
                    
                    foreach (IEdition editions in _editions)
                        Editions.Add(editions);
                    

                    CardSelected = null;
                    EditionSelected = save;
                    break;

                case InputMode.None:

                    Cards.Clear();
                    Cards.Clear();
                    EditionSelected = null;
                    CardSelected = null;
                    break;
            }

            CurrentCollectionDetail = null;
            Count = 1;
            IsFoil = false;
        }
        private void AddNewCard()
        {
            if (CheckDetail())
            {
                int count = IsFoil ? 0 : Count;
                int foilCount = IsFoil ? Count: 0;

                ICardAllDbInfo cardAllDbInfo = _allCardInfos.First(cadi => cadi.Edition == EditionSelected && cadi.Card == CardSelected);
                _magicDatabase.InsertOrUpdateCardInCollection(CardCollection.Id, cardAllDbInfo.IdGatherer, count, foilCount);
            }
        }
        private bool CheckDetail()
        {
            //TODO: check foil available , language available
            return true;
        }
        private void SelectCardCollection(string name)
        {
            CardCollection = _collections.First(cc => cc.Name == name);
            RefreshDisplayedData(InputMode.None);
        }
        private void RefreshDisplayedData(InputMode modifyData)
        {
            UpdateCurrentCollectionDetail();

            //Not the reference info changed, nothing to recompute
            if (InputMode != modifyData)
                return;

            switch (InputMode)
            {
                case InputMode.ByEdition:

                    IEdition editionSelected = EditionSelected;
                    Cards.Clear();
                    if (editionSelected == null)
                        return;

                    foreach (ICard card in _allCardInfos.Where(cadi => cadi.Edition == editionSelected).Select(cadi => cadi.Card).Distinct().OrderBy(c => c.ToString()))
                        Cards.Add(card);

                    break;

                case InputMode.ByCard:

                    ICard cardNameSelected = CardSelected;
                    Editions.Clear();
                    if (cardNameSelected == null)
                        return;

                    foreach (IEdition edition in _allCardInfos.Where(cadi => cadi.Card == cardNameSelected).Select(cadi => cadi.Edition).OrderByDescending(ed => ed.ReleaseDate))
                        Editions.Add(edition);

                    break;
            }

        }
        private void UpdateCurrentCollectionDetail()
        {
            if (EditionSelected == null || CardSelected == null)
            {
                CurrentCollectionDetail = null;
                return;
            }

            int totalInCollection = 0;
            int totalInEditionInCollection = 0;
            
            foreach (ICardInCollectionCount cardInCollectionCount in _magicDatabase.GetCardCollectionStatistics(CardSelected).Where(cicc => cicc.IdCollection == CardCollection.Id))
            {
                totalInCollection += cardInCollectionCount.Number + cardInCollectionCount.FoilNumber;
                if (_magicDatabase.GetEdition(cardInCollectionCount.IdGatherer) == EditionSelected)
                    totalInEditionInCollection += IsFoil ? cardInCollectionCount.FoilNumber : cardInCollectionCount.Number;
            }

            CurrentCollectionDetail = string.Format("{0} {2}{3}/ {1}", totalInEditionInCollection, totalInCollection, EditionSelected.Code, IsFoil ? "(Foil)" : string.Empty);
        }
    }
}
