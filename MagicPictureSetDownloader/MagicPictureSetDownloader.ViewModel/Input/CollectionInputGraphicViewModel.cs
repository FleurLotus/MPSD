namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
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
        private IDictionary<string, ICard> _allCardSorted;

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
            Cards = new RangeObservableCollection<CardCollectionInputGraphicViewModel>();
            ChangeInputLanguageCommand = new RelayCommand(ChangeInputLanguageCommandExecute);
            CardCollection = _magicDatabase.GetAllCollections().First(cc => cc.Name == name);

            AddLinkedProperty(nameof(InputLanguage), nameof(InputLanguageName));

            RebuildOrder();
        }

        public ICommand ChangeInputLanguageCommand { get; }
        public IEdition[] Editions { get; }
        public ICardCollection CardCollection { get; }
        public RangeObservableCollection<CardCollectionInputGraphicViewModel> Cards { get; private set; }

        public string InputLanguageName
        {
            get { return _inputLanguage == null ? "Default" : _inputLanguage.Name; }
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
                    RefreshDisplayedData();
                }
            }
        }
        //ALERT-FBO TO BE CODED
        public string Filter
        {
            get { return _filter; }
            set
            {
                if (value != _filter)
                {
                    _filter = value;
                    OnNotifyPropertyChanged(nameof(Filter));
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
                    RefreshDisplayedData();
                }
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
            //ALERT-FBO TO BE CODED
        }
        protected override bool OkCommandCanExecute(object o)
        {
            //ALERT-FBO TO BE CODED
            return false;
        }
        private void RebuildOrder()
        {
            _allCardSorted = _allCardInfos.GetAllCardsOrderByTranslation(_inputLanguage);
        }
        private void RefreshDisplayedData()
        {
            IEdition editionSelected = EditionSelected;
            Cards.Clear();
            if (editionSelected == null)
            {
                return;
            }

            List<CardCollectionInputGraphicViewModel> sorted = new List<CardCollectionInputGraphicViewModel>();
            foreach (ICardAllDbInfo cardInfo in _allCardInfos.Where(cadi => cadi.Edition == editionSelected))
            {
                string name = _allCardSorted.First(acsKv => cardInfo.Card == acsKv.Value).Key;
                //ALERT-FBO mettre la bonne quantité
                sorted.Add(new CardCollectionInputGraphicViewModel(new CardViewModel(cardInfo), InputLanguage, name, 0));
  
            }
            sorted.Sort();
            Cards.AddRange(sorted);
        }
    }
}
