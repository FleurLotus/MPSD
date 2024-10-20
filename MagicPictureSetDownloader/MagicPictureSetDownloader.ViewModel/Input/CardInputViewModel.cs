﻿namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Input;

    using Common.Library.Extension;
    using Common.Library.Collection;
    using Common.ViewModel;
    using Common.ViewModel.Dialog;
    using Common.ViewModel.Input;

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
        private bool _isAltArt;
        private bool _isFocused;
        private int _count;
        private ICard _cardSelected;
        private string _cardSelectedName;
        private IDictionary<string, ICard> _allCardSorted;
        private IEdition _editionSelected;
        private ILanguage _languageSelected;
        private ILanguage _inputLanguage;
        private string _currentCollectionDetail;
        private string _translate;
        private readonly IEdition[] _allEditions;
        private ICardCollection _cardCollection;
        private readonly ICardCollection[] _collections;

        private readonly IMagicDatabaseReadAndWriteCardInCollection _magicDatabase;
        private readonly IMagicDatabaseReadAndWriteOption _magicDatabaseForOption;
        private readonly ICardAllDbInfo[] _allCardInfos;
        private readonly ILanguage[] _allLanguages;
        private readonly int _defaultQuantity;
        private const int MaxCount = 99;
        private const int MinCount = -99;

        public CardInputViewModel(string name, int defaultQuantity)
        {
            _magicDatabase = MagicDatabaseManager.ReadAndWriteCardInCollection;
            _magicDatabaseForOption = MagicDatabaseManager.ReadAndWriteOption;
            _defaultQuantity = defaultQuantity;

            //Set directly field and not property to avoid save the retrieve value
            IOption option = _magicDatabaseForOption.GetOption(TypeOfOption.Input, "Mode");
            if (option != null)
            {
                if (Enum.TryParse(option.Value, out InputMode mode))
                {
                    _inputMode = mode;
                }
            }

            option = _magicDatabaseForOption.GetOption(TypeOfOption.Input, "Language");
            if (option != null)
            {
                if (int.TryParse(option.Value, out int id))
                {
                    _inputLanguage = _magicDatabase.GetLanguage(id);
                }
            }

            Display.Title = "Input cards";
            Display.OkCommandLabel = "Add";
            Display.CancelCommandLabel = "Close";
            ChangeCollectionCommand = new RelayCommand(ChangeCollectionCommandExecute);
            ChangeInputLanguageCommand = new RelayCommand(ChangeInputLanguageCommandExecute);
            IncreaseQuantityCommand = new RelayCommand(IncreaseQuantityCommandExecute);

            _allCardInfos = _magicDatabase.GetAllInfos().ToArray();
            _collections = _magicDatabase.GetAllCollections().ToArray();
            SelectCardCollection(name);

            _allLanguages = _magicDatabase.GetAllLanguages().ToArray();

            Cards = new RangeObservableCollection<string>();

            _allEditions = _magicDatabase.GetAllEditionsOrdered();
            Editions = new RangeObservableCollection<IEdition>();

            Languages = new RangeObservableCollection<ILanguage>();

            RebuildOrder();
            InitWindow();
        }
        public ICommand ChangeCollectionCommand { get; }
        public ICommand ChangeInputLanguageCommand { get; }
        public ICommand IncreaseQuantityCommand { get; }
        public RangeObservableCollection<IEdition> Editions { get; }
        public RangeObservableCollection<ILanguage> Languages { get; }
        public RangeObservableCollection<string> Cards { get; }

        public ILanguage LanguageSelected
        {
            get { return _languageSelected; }
            set
            {
                if (value != _languageSelected)
                {
                    _languageSelected = value;
                    OnNotifyPropertyChanged(nameof(LanguageSelected));
                    RefreshDisplayedData(InputMode.None);
                }
            }
        }
        public string InputLanguageName
        {
            get { return _inputLanguage == null ? "Default" : _inputLanguage.Name; }
        }

        public ICardCollection CardCollection
        {
            get { return _cardCollection; }
            set
            {
                if (value != _cardCollection)
                {
                    _cardCollection = value;
                    OnNotifyPropertyChanged(nameof(CardCollection));
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
                    OnNotifyPropertyChanged(nameof(CurrentCollectionDetail));
                }
            }
        }
        public bool IsFocused
        {
            get { return _isFocused; }
            set
            {
                if (value != _isFocused)
                {
                    _isFocused = value;
                    OnNotifyPropertyChanged(nameof(IsFocused));
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
                    OnNotifyPropertyChanged(nameof(IsFoil));
                    RefreshDisplayedData(InputMode.None);
                }
            }
        }
        public bool IsAltArt
        {
            get { return _isAltArt; }
            set
            {
                if (value != _isAltArt)
                {
                    _isAltArt = value;
                    OnNotifyPropertyChanged(nameof(IsAltArt));
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
                    OnNotifyPropertyChanged(nameof(EditionSelected));
                    RefreshDisplayedData(InputMode.ByEdition);
                    if (_editionSelected != null && !_editionSelected.HasFoil)
                    {
                        IsFoil = false;
                    }
                }
            }
        }
        public string CardSelectedName
        {
            get { return _cardSelectedName; }
            set
            {
                if (value != _cardSelectedName)
                {
                    _cardSelectedName = value;
                    _cardSelected = _cardSelectedName == null ? null : _allCardSorted.GetOrDefault(_cardSelectedName);

                    OnNotifyPropertyChanged(nameof(CardSelectedName));
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
                    if (value > MaxCount)
                    {
                        _count = MaxCount;
                    }
                    else if (value < MinCount)
                    {
                        _count = MinCount;
                    }
                    else
                    {
                        _count = value;
                    }
                    OnNotifyPropertyChanged(nameof(Count));
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
                    _magicDatabaseForOption.InsertNewOption(TypeOfOption.Input, "Mode", _inputMode.ToString());
                    OnNotifyPropertyChanged(nameof(InputMode));
                    InitWindow();
                }
            }
        }
        public string Translate
        {
            get { return _translate; }
            private set
            {
                if (value != _translate)
                {
                    _translate = value;
                    OnNotifyPropertyChanged(nameof(Translate));
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
            return Count != 0 && EditionSelected != null && _cardSelected != null && _languageSelected != null && (EditionSelected.HasFoil || !IsFoil);
        }
        private void ChangeCollectionCommandExecute(object obj)
        {
            InputViewModel vm = InputViewModelFactory.Instance.CreateChooseInListViewModel("Other collection", "Choose other collection to input", _collections.Select(c => c.Name).ToList());
            OnInputRequestedRequested(vm);
            if (vm.Result == true)
            {
                string collection = vm.Selected;

                if (!string.IsNullOrWhiteSpace(collection))
                {
                    SelectCardCollection(collection);
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
        private void IncreaseQuantityCommandExecute(object obj)
        {
            int inc = int.Parse(obj.ToString());
            Count += inc;
        }
        private void InitWindow()
        {
            switch (InputMode)
            {
                case InputMode.ByCard:
                    {
                        Cards.Clear();
                        Cards.AddRange(_allCardSorted.Keys);

                        Editions.Clear();
                        EditionSelected = null;
                        CardSelectedName = null;
                        break;
                    }

                case InputMode.ByEdition:
                    {
                        Cards.Clear();

                        IEdition save = EditionSelected;
                        Editions.Clear();
                        Editions.AddRange(_allEditions);

                        CardSelectedName = null;
                        EditionSelected = save;
                        break;
                    }

                case InputMode.None:

                    Cards.Clear();
                    Editions.Clear();
                    EditionSelected = null;
                    CardSelectedName = null;
                    break;
            }

            Languages.Clear();
            CurrentCollectionDetail = null;
            LanguageSelected = null;
            Count = _defaultQuantity;
            IsFoil = false;
            IsAltArt = false;
            ResetFocus();
        }
        private void AddNewCard()
        {
            CardCount cardCount = new CardCount
            {
                { new CardCountKey(IsFoil, IsAltArt), Count }
            };

            ICardAllDbInfo cardAllDbInfo = _allCardInfos.First(cadi => cadi.Edition == EditionSelected && cadi.Card == _cardSelected);
            _magicDatabase.InsertOrUpdateCardInCollection(CardCollection.Id, cardAllDbInfo.IdScryFall, LanguageSelected.Id, cardCount);
        }
        private void SelectCardCollection(string name)
        {
            CardCollection = _collections.First(cc => cc.Name == name);
            RefreshDisplayedData(InputMode.None);
        }
        private void SelectInputLanguage(string name)
        {
            _inputLanguage = _allLanguages.First(l => l.Name == name);
            if (_inputLanguage == _magicDatabase.GetDefaultLanguage())
            {
                _inputLanguage = null;
                _magicDatabaseForOption.DeleteOption(TypeOfOption.Input, "Language");
            }
            else
            {
                _magicDatabaseForOption.InsertNewOption(TypeOfOption.Input, "Language", _inputLanguage.Id.ToString(CultureInfo.InvariantCulture));
            }
            
            OnNotifyPropertyChanged(nameof(InputLanguageName));
            RebuildOrder();
            InitWindow();
        }
        private void RebuildOrder()
        {
            _allCardSorted = _allCardInfos.GetAllCardsOrderByTranslation(_inputLanguage);
        }
        private void RefreshDisplayedData(InputMode modifyData)
        {
            UpdateCurrentCollectionDetailAndTranslate();

            //None one the key changed, nothing to recompute
            if (modifyData == InputMode.None)
            {
                return;
            }

            //Change one of the key but no the reference one
            if (InputMode != modifyData)
            {
                Languages.Clear();
                IEdition editionSelected = EditionSelected;
                ICard cardNameSelected = _cardSelected;
                if (editionSelected == null || cardNameSelected == null)
                {
                    return;
                }

                ICardAllDbInfo cardAllDbInfo = _allCardInfos.First(cadi => cadi.Edition == editionSelected && cadi.Card == cardNameSelected);
                if (cardAllDbInfo == null)
                {
                    return;
                }

                foreach (ILanguage language in _magicDatabase.GetLanguages(cardAllDbInfo.IdScryFall))
                {
                    Languages.Add(language);
                }

                if (Languages.Count > 0)
                {
                    if (_inputLanguage != null && Languages.Contains(_inputLanguage))
                    {
                        LanguageSelected = _inputLanguage;
                    }
                    else
                    {
                        LanguageSelected = Languages[0];
                    }
                }
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
                        {
                            return;
                        }

                        List<string> sorted = new List<string>();
                        //Could not call directly GetAllCardsOrderByTranslation because the key must be the same as in all card even if there is no duplicate traduction in the subset
                        foreach (KeyValuePair<string, ICard> kv in _allCardInfos.Where(cadi => cadi.Edition == editionSelected).GetAllCardWithTranslation(_inputLanguage))
                        {
                            //Normal case
                            if (_allCardSorted.ContainsKey(kv.Key))
                            {
                                sorted.Add(kv.Key);
                            }
                            else
                            {
                                //Key was changed because of duplicate traduction, we need to look for the card
                                sorted.Add(_allCardSorted.First(acsKv => kv.Value == acsKv.Value).Key);
                            }
                        }
                        sorted.Sort();
                        Cards.AddRange(sorted);
                        break;

                    case InputMode.ByCard:

                        ICard cardNameSelected = _cardSelected;
                        Editions.Clear();
                        Languages.Clear();
                        if (cardNameSelected == null)
                        {
                            return;
                        }

                        foreach (IEdition edition in _allCardInfos.GetAllEditionIncludingCardOrdered(cardNameSelected))
                        {
                            Editions.Add(edition);
                        }

                        if (Editions.Count > 0)
                        {
                            EditionSelected = Editions[0];
                        }

                        break;
                }
            }
        }
        private void UpdateCurrentCollectionDetailAndTranslate()
        {
            if (EditionSelected == null || _cardSelected == null || LanguageSelected == null)
            {
                CurrentCollectionDetail = null;
                Translate = null;
                return;
            }

            Translate = _cardSelected.ToString(LanguageSelected.Id);

            int totalInCollection = 0;
            int totalInEditionInCollection = 0;
            int totalInEditionAndLanguageInCollectionNotFoilNotAltArt = 0;
            int totalInEditionAndLanguageInCollectionFoilNotAltArt = 0;
            int totalInEditionAndLanguageInCollectionNotFoilAltArt = 0;
            int totalInEditionAndLanguageInCollectionFoilAltArt = 0;

            foreach (ICardInCollectionCount cardInCollectionCount in _magicDatabase.GetCollectionStatisticsForCard(CardCollection, _cardSelected))
            {
                int inCollection = cardInCollectionCount.Number + cardInCollectionCount.FoilNumber + cardInCollectionCount.AltArtNumber + cardInCollectionCount.FoilAltArtNumber;
                totalInCollection += inCollection;
                if (_magicDatabase.GetEdition(cardInCollectionCount.IdScryFall) == EditionSelected)
                {
                    totalInEditionInCollection += inCollection;
                    if (cardInCollectionCount.IdLanguage == LanguageSelected.Id)
                    {
                        totalInEditionAndLanguageInCollectionNotFoilNotAltArt += cardInCollectionCount.Number;
                        totalInEditionAndLanguageInCollectionFoilNotAltArt += cardInCollectionCount.FoilNumber;
                        totalInEditionAndLanguageInCollectionNotFoilAltArt += cardInCollectionCount.AltArtNumber;
                        totalInEditionAndLanguageInCollectionFoilAltArt += cardInCollectionCount.FoilAltArtNumber;
                    }
                }
            }

            CurrentCollectionDetail = string.Format("{2}{3}{4}{5} {0} {1}\n{6} {0}\n{7} All Edition", EditionSelected.Code, LanguageSelected.Name, 
                                                                                                       totalInEditionAndLanguageInCollectionNotFoilNotAltArt,
                                                                                                       totalInEditionAndLanguageInCollectionFoilNotAltArt > 0 ? "+" + totalInEditionAndLanguageInCollectionFoilNotAltArt + "(Foil)" : string.Empty,
                                                                                                       totalInEditionAndLanguageInCollectionNotFoilAltArt > 0 ? "+" + totalInEditionAndLanguageInCollectionNotFoilAltArt + "(AltArt)" : string.Empty,
                                                                                                       totalInEditionAndLanguageInCollectionFoilAltArt > 0 ? "+" + totalInEditionAndLanguageInCollectionFoilAltArt + "(FoilAltArt)" : string.Empty,
                                                                                                       totalInEditionInCollection, totalInCollection);
        }
        private void ResetFocus()
        {
            if (EditionSelected != null || InputMode == InputMode.ByCard)
            {
                IsFocused = true;
            }
            IsFocused = false;
        }
    }
}
