namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Common.Libray;
    using Common.ViewModel;

    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Input;

    public partial class MainViewModel
    {
        public event EventHandler UpdateDatabaseRequested;
        public event EventHandler UpdateImageDatabaseRequested;
        public event EventHandler VersionRequested;
        public event EventHandler CloseRequested;
        public event EventHandler ImportExportWanted;
        public event EventHandler<EventArgs<CardInputViewModel>>  AddCardWanted;
        public event EventHandler<EventArgs<InputViewModel>> InputRequested;
        public event EventHandler<EventArgs<CardUpdateViewModel>> UpdateCardWanted;

        private MenuViewModel _showPictureViewModel;
        private MenuViewModel _showStatisticsViewModel;
        private MenuViewModel _collectionViewModel;

        public ObservableCollection<MenuViewModel> Menus { get; private set; }
        public ObservableCollection<MenuViewModel> ContextMenus { get; private set; }
        public bool ShowPicture
        {
            get { return _showPictureViewModel.IsChecked; }
        }
        public bool ShowStatistics
        {
            get { return _showStatisticsViewModel.IsChecked; }
        }
        
        #region Events

        private void OnUpdateDatabaseRequested()
        {
            EventHandler e = UpdateDatabaseRequested;
            if (e != null)
                e(this, EventArgs.Empty);
        }
        private void OnUpdateImageDatabaseRequested()
        {
            EventHandler e = UpdateImageDatabaseRequested;
            if (e != null)
                e(this, EventArgs.Empty);
        }
        private void OnVersionRequested()
        {
            EventHandler e = VersionRequested;
            if (e != null)
                e(this, EventArgs.Empty);
        }
        private void OnCloseRequested()
        {
            EventHandler e = CloseRequested;
            if (e != null)
                e(this, EventArgs.Empty);
        }
        private void OnImportExportWanted()
        {
            EventHandler e = ImportExportWanted;
            if (e != null)
                e(this, EventArgs.Empty);
        }
        private void OnAddCardWanted(string name)
        {
            var e = AddCardWanted;
            if (e != null)
                e(this, new EventArgs<CardInputViewModel>(new CardInputViewModel(name)));
        }
        private void OnInputRequestedRequested(InputViewModel vm)
        {
            var e = InputRequested;
            if (e != null && vm != null)
                e(this, new EventArgs<InputViewModel>(vm));
        }
        private void OnUpdateCardWanted(CardUpdateViewModel vm)
        {
            var e = UpdateCardWanted;
            if (e != null && vm != null)
                e(this, new EventArgs<CardUpdateViewModel>(vm));
        }
        #endregion

        #region Command

        private void UpdateDatabaseCommandExecute(object o)
        {
            OnUpdateDatabaseRequested();
            LoadCardsHierarchy();
        }
        private void UpdateImageDatabaseCommandExecute(object o)
        {
            OnUpdateImageDatabaseRequested();
        }
        private void VersionCommandExecute(object o)
        {
            OnVersionRequested();
        }
        private void CloseCommandExecute(object o)
        {
            OnCloseRequested();
        }
        private void ShowPictureCommandExecute(object o)
        {
            OnNotifyPropertyChanged(() => ShowPicture);
            _magicDatabase.InsertNewOption(TypeOfOption.Display, "ShowPicture", ShowPicture.ToString());
        }
        private void ShowStatisticsCommandExecute(object o)
        {
            OnNotifyPropertyChanged(() => ShowStatistics);
            _magicDatabase.InsertNewOption(TypeOfOption.Display, "ShowStatistics", ShowStatistics.ToString());
        }
        private void ShowAllCollectionCommandExecute(object o)
        {
            LoadCollection();
        }
        private void ShowCollectionCommandExecute(object o)
        {
            LoadCollection(o as string);
        }
        private void CreateCollectionCommandExecute(object o)
        {
            InputViewModel vm = InputViewModelFactory.Instance.CreateTextViewModel("New Collection", "Input new collection name");
            OnInputRequestedRequested(vm);
            if (vm.Result.HasValue && vm.Result.Value)
            {
                string newName = vm.Text;
                
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    CheckCollectionNameNotAlreadyExists(newName);

                    _magicDatabase.InsertNewCollection(newName);
                    GenerateCollectionMenu();
                }
            }
        }
        private void DeleteCollectionCommandExecute(object o)
        {
            const string none = "--- None ---";
            ICollection<string> cardCollections = _magicDatabase.GetAllCollections().Select(cc=>cc.Name).ToList();
            List<string> source = new List<string>(cardCollections);
            List<string> dest = new List<string>(cardCollections);
            
            dest.Insert(0, none);

            InputViewModel vm = InputViewModelFactory.Instance.CreateMoveFromListToOtherViewModel("Delete Collection", "Choose collection to delete", source, "Move card to: ('None' to remove them)", dest);
            
            OnInputRequestedRequested(vm);
            if (vm.Result.HasValue && vm.Result.Value)
            {
                string toBeDeleted = vm.Selected;
                string toAdd = vm.Selected2;

                if (!string.IsNullOrWhiteSpace(toBeDeleted)&& !string.IsNullOrWhiteSpace(toAdd))
                {
                    if (toAdd == none)
                        _magicDatabase.DeleteAllCardInCollection(toBeDeleted);
                    else
                        _magicDatabase.MoveCollection(toBeDeleted, toAdd);

                    _magicDatabase.DeleteCollection(toBeDeleted);
                    
                    //Delete current collection -> reset display to default
                    if (Hierarchical.Name == toBeDeleted)
                        LoadCollection();
                    GenerateCollectionMenu();
                }
            }
        }
        private void RenameCollectionCommandExecute(object o)
        {
            ICollection<string> cardCollections = _magicDatabase.GetAllCollections().Select(cc => cc.Name).ToList();
            List<string> source = new List<string>(cardCollections);

            InputViewModel vm = InputViewModelFactory.Instance.CreateChooseInListAndTextViewModel("Rename Collection", "Choose collection to rename and input new name", source);

            OnInputRequestedRequested(vm);
            if (vm.Result.HasValue && vm.Result.Value)
            {
                string toBeRenamed = vm.Selected;
                string newName = vm.Text;

                if (!string.IsNullOrWhiteSpace(toBeRenamed) && !string.IsNullOrWhiteSpace(newName))
                {
                    CheckCollectionNameNotAlreadyExists(newName);
                    _magicDatabase.UpdateCollectionName(_magicDatabase.GetCollection(toBeRenamed), newName);
                    GenerateCollectionMenu();
                }
            }
        }
        private void ImportExportCommandExecute(object o)
        {
            OnImportExportWanted();
        }
        private void CardInputCommandExecute(object o)
        {
            OnAddCardWanted(Hierarchical.Name);
            LoadCardsHierarchy();
        }
        private void ChangeCardCommandExecute(object o)
        {
            CardUpdateViewModel vm = new CardUpdateViewModel(Hierarchical.Name, (o as CardViewModel).Card, false);
            OnUpdateCardWanted(vm);

            if (vm.Result.HasValue && vm.Result.Value)
            {
                _magicDatabase.ChangeCardEditionFoilLanguage(vm.SourceCardCollection, vm.Card, vm.Count, vm.SourceEditionSelected, vm.SourceIsFoil,vm.SourceLanguageSelected, vm.DestinationEditionSelected, vm.DestinationIsFoil, vm.DestinationLanguageSelected);
                LoadCardsHierarchy();
            }
        }
        private void MoveCardCommandExecute(object o)
        {
            CardUpdateViewModel vm = new CardUpdateViewModel(Hierarchical.Name, (o as CardViewModel).Card, true);
            OnUpdateCardWanted(vm);
            if (vm.Result.HasValue && vm.Result.Value)
            {
                _magicDatabase.MoveCardToOtherCollection(vm.SourceCardCollection, vm.Card, vm.SourceEditionSelected,vm.SourceLanguageSelected, vm.Count, vm.SourceIsFoil, vm.DestinationCardCollectionSelected);
                LoadCardsHierarchy();
            }
        }

        #endregion

        private void CreateMenu()
        {
            Menus = new ObservableCollection<MenuViewModel>();
            ContextMenus = new ObservableCollection<MenuViewModel>();

            MenuViewModel fileMenu = new MenuViewModel("_File");
            fileMenu.Children.Add(new MenuViewModel("Update _Set Database...", new RelayCommand(UpdateDatabaseCommandExecute)));
            fileMenu.Children.Add(new MenuViewModel("Update _Image Database..",new RelayCommand(UpdateImageDatabaseCommandExecute)));
            fileMenu.Children.Add(MenuViewModel.Separator);
            fileMenu.Children.Add(new MenuViewModel("_Exit", new RelayCommand(CloseCommandExecute)));
            Menus.Add(fileMenu);

            MenuViewModel viewMenu = new MenuViewModel("_View");

            bool isShowStatisticsChecked = true;
            IOption option = _magicDatabase.GetOption(TypeOfOption.Display, "ShowStatistics");
            if (option != null)
                isShowStatisticsChecked = bool.Parse(option.Value);
            _showStatisticsViewModel = new MenuViewModel("Show _Statistics", new RelayCommand(ShowStatisticsCommandExecute)) { IsCheckable = true, IsChecked = isShowStatisticsChecked };
            viewMenu.Children.Add(_showStatisticsViewModel);

            bool isShowPictureChecked = true;
            option = _magicDatabase.GetOption(TypeOfOption.Display, "ShowPicture");
            if (option != null)
                isShowPictureChecked = bool.Parse(option.Value);

            _showPictureViewModel = new MenuViewModel("Show _Picture", new RelayCommand(ShowPictureCommandExecute)) { IsCheckable = true, IsChecked = isShowPictureChecked };
            viewMenu.Children.Add(_showPictureViewModel);
            Menus.Add(viewMenu);

            _collectionViewModel = new MenuViewModel("_Collection");
            GenerateCollectionMenu();
            Menus.Add(_collectionViewModel);

            MenuViewModel aboutMenu = new MenuViewModel("?");
            aboutMenu.Children.Add(new MenuViewModel("_Version", new RelayCommand(VersionCommandExecute)));
            Menus.Add(aboutMenu);

            GenerateContextMenu();
        }
        private void GenerateContextMenu()
        {
            ContextMenus.Clear();
            if (Hierarchical == null || Hierarchical == _allhierarchical)
                return;

            ContextMenus.Add(new MenuViewModel("Input cards", new RelayCommand(CardInputCommandExecute)));

            HierarchicalResultNodeViewModel nodeViewModel = Hierarchical.Selected as HierarchicalResultNodeViewModel;
            if (nodeViewModel != null)
            {
                ContextMenus.Add(new MenuViewModel("Change edition/language/foil", new RelayCommand(ChangeCardCommandExecute), nodeViewModel.Card));
                ContextMenus.Add(new MenuViewModel("Move to other collection", new RelayCommand(MoveCardCommandExecute), nodeViewModel.Card));
            }
        }
        private void GenerateCollectionMenu()
        {
            List<ICardCollection> cardCollections = new List<ICardCollection>(_magicDatabase.GetAllCollections());

            bool hasCollection = cardCollections.Count > 0;

            _collectionViewModel.Children.Clear();
            _collectionViewModel.Children.Add(new MenuViewModel("New collection...", new RelayCommand(CreateCollectionCommandExecute)));
            if (hasCollection)
            {
                _collectionViewModel.Children.Add(new MenuViewModel("Delete collection...", new RelayCommand(DeleteCollectionCommandExecute)));
                _collectionViewModel.Children.Add(new MenuViewModel("Rename collection...", new RelayCommand(RenameCollectionCommandExecute)));
                _collectionViewModel.Children.Add(MenuViewModel.Separator);
                _collectionViewModel.Children.Add(new MenuViewModel("Import/Export..", new RelayCommand(ImportExportCommandExecute)));
            }

            _collectionViewModel.Children.Add(MenuViewModel.Separator);
            _collectionViewModel.Children.Add(new MenuViewModel("All cards", new RelayCommand(ShowAllCollectionCommandExecute)));

            if (hasCollection)
            {
                _collectionViewModel.Children.Add(MenuViewModel.Separator);
                cardCollections.Sort((c1, c2) => string.Compare(c1.Name, c2.Name, StringComparison.Ordinal));
                foreach (ICardCollection cardCollection in cardCollections)
                {
                    MenuViewModel menuViewModel = new MenuViewModel(cardCollection.Name, new RelayCommand(ShowCollectionCommandExecute), cardCollection.Name)
                                                      {
                                                          IsChecked = Hierarchical != null && Hierarchical.Name == cardCollection.Name
                                                      };

                    _collectionViewModel.Children.Add(menuViewModel);
                }
            }
        }
    }
}
