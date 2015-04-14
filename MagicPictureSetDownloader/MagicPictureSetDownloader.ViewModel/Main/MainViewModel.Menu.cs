namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Common.Libray.Notify;
    using Common.ViewModel;

    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Input;
    using MagicPictureSetDownloader.ViewModel.Management;

    public partial class MainViewModel
    {
        const string None = "--- None ---";

        #region EventHandler

        public event EventHandler UpdateDatabaseRequested;
        public event EventHandler UpdateImageDatabaseRequested;
        public event EventHandler VersionRequested;
        public event EventHandler CloseRequested;
        public event EventHandler ImportExportRequested;
        public event EventHandler<EventArgs<InputViewModel>> InputRequested;
        public event EventHandler<EventArgs<DialogViewModelBase>> DialogWanted;
        public event EventHandler<EventArgs<BlockDatabaseInfoModificationViewModel>> BlockModificationRequested;
        public event EventHandler<EventArgs<EditionDatabaseInfoModificationViewModel>> EditionModificationRequested;
        public event EventHandler<EventArgs<LanguageDatabaseInfoModificationViewModel>> LanguageModificationRequested;

        #endregion

        private readonly MenuViewModel _menuRoot;
        private readonly MenuViewModel _contextMenuRoot;
        private SearchViewModel _searchViewModel;

        private MenuViewModel _showPictureViewModel;
        private MenuViewModel _showStatisticsViewModel;
        private MenuViewModel _showOtherLanguagesViewModel;
        private MenuViewModel _collectionViewModel;

        public MenuViewModel MenuRoot
        {
            get { return _menuRoot; }
        }
        public MenuViewModel ContextMenuRoot
        {
            get { return _contextMenuRoot; }
        }
        public bool ShowPicture
        {
            get { return _showPictureViewModel.IsChecked; }
        }
        public bool ShowStatistics
        {
            get { return _showStatisticsViewModel.IsChecked; }
        }
        public bool ShowOtherLanguages
        {
            get { return _showOtherLanguagesViewModel.IsChecked; }
        }

        #region Events

        private void OnUpdateDatabaseRequested()
        {
            OnEventRaise(UpdateDatabaseRequested);
        }
        private void OnUpdateImageDatabaseRequested()
        {
            OnEventRaise(UpdateImageDatabaseRequested);
        }
        private void OnVersionRequested()
        {
            OnEventRaise(VersionRequested);
        }
        private void OnCloseRequested()
        {
            OnEventRaise(CloseRequested);
        }
        private void OnImportExportRequested()
        {
            OnEventRaise(ImportExportRequested);
        }
        private void OnInputRequestedRequested(InputViewModel vm)
        {
            OnEventRaise(InputRequested, vm);
        }
        private void OnDialogWanted(DialogViewModelBase vm)
        {
            OnEventRaise(DialogWanted, vm);
        }
        #endregion

        private void OnEventRaise<T>(EventHandler<EventArgs<T>> ev, T arg) where T: class
        {
            if (ev != null && arg != null)
                ev(this, new EventArgs<T>(arg));
        }
        private void OnEventRaise(EventHandler ev)
        {
            if (ev != null)
                ev(this, EventArgs.Empty);
        }

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
        private void ShowOtherLanguagesCommandExecute(object o)
        {
            OnNotifyPropertyChanged(() => ShowOtherLanguages);
            _magicDatabase.InsertNewOption(TypeOfOption.Display, "ShowOtherLanguages", ShowOtherLanguages.ToString());
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
            ICollection<string> cardCollections = _magicDatabase.GetAllCollections().Select(cc => cc.Name).ToList();
            List<string> source = new List<string>(cardCollections);
            List<string> dest = new List<string>(cardCollections);

            dest.Insert(0, None);

            InputViewModel vm = InputViewModelFactory.Instance.CreateMoveFromListToOtherViewModel("Delete Collection", "Choose collection to delete", source, "Move card to: ('None' to remove them)", dest);

            OnInputRequestedRequested(vm);
            if (vm.Result.HasValue && vm.Result.Value)
            {
                string toBeDeleted = vm.Selected;
                string toAdd = vm.Selected2;

                if (!string.IsNullOrWhiteSpace(toBeDeleted)&& !string.IsNullOrWhiteSpace(toAdd))
                {
                    Loading = true;

                    ThreadPool.QueueUserWorkItem(DeleteCollectionAsync, vm);
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
            OnImportExportRequested();
        }
        private void CardInputCommandExecute(object o)
        {
            OnDialogWanted(new CardInputViewModel(Hierarchical.Name));
            LoadCardsHierarchy();
        }
        private void ChangeCardCommandExecute(object o)
        {
            CardUpdateViewModel vm = new CardUpdateViewModel(Hierarchical.Name, (o as CardViewModel).Card);
            OnDialogWanted(vm);

            if (vm.Result.HasValue && vm.Result.Value)
            {
                _magicDatabase.ChangeCardEditionFoilLanguage(vm.SourceCardCollection, vm.Card, vm.Count, vm.SourceEditionSelected, vm.SourceIsFoil,vm.SourceLanguageSelected, vm.DestinationEditionSelected, vm.DestinationIsFoil, vm.DestinationLanguageSelected);
                LoadCardsHierarchy();
            }
        }
        private void MoveCardCommandExecute(object o)
        {
            CardMoveViewModel vm = new CardMoveViewModel(Hierarchical.Name, (o as CardViewModel).Card);
            OnDialogWanted(vm);
            if (vm.Result.HasValue && vm.Result.Value)
            {
                _magicDatabase.MoveCardToOtherCollection(vm.SourceCardCollection, vm.Card, vm.SourceEditionSelected,vm.SourceLanguageSelected, vm.Count, vm.SourceIsFoil, vm.DestinationCardCollectionSelected);
                LoadCardsHierarchy();
            }
        }
        private void SearchCommandExecute(object o)
        {
            if (_searchViewModel == null)
                _searchViewModel = new SearchViewModel();
            else
                _searchViewModel.Reuse();

            OnDialogWanted(_searchViewModel);

            if (_searchViewModel.Result.HasValue && _searchViewModel.Result.Value)
            {
                CreateSearchResult(_searchViewModel);
                LoadCardsHierarchyAsync();
            }
        }
        private void BlockModificationCommandExecute(object o)
        {
            OnEventRaise(BlockModificationRequested, new BlockDatabaseInfoModificationViewModel());
        }
        private void EditionModificationCommandExecute(object o)
        {
            OnEventRaise(EditionModificationRequested, new EditionDatabaseInfoModificationViewModel());
        }
        private void LanguageModificationCommandExecute(object o)
        {
            OnEventRaise(LanguageModificationRequested, new LanguageDatabaseInfoModificationViewModel());
        }
        #endregion

        private void CreateMenu()
        {
            //File
            MenuViewModel fileMenu = new MenuViewModel("_File");
            fileMenu.AddChild(new MenuViewModel("Update _Editions Database...", new RelayCommand(UpdateDatabaseCommandExecute)));
            fileMenu.AddChild(new MenuViewModel("Update _Images Database..", new RelayCommand(UpdateImageDatabaseCommandExecute)));
            fileMenu.AddChild(MenuViewModel.Separator());
            fileMenu.AddChild(new MenuViewModel("Search...", new RelayCommand(SearchCommandExecute)));
            fileMenu.AddChild(MenuViewModel.Separator());
            fileMenu.AddChild(new MenuViewModel("_Exit", new RelayCommand(CloseCommandExecute)));
            MenuRoot.AddChild(fileMenu);

            //View
            MenuViewModel viewMenu = new MenuViewModel("_View");

            bool isShowStatisticsChecked = true;
            IOption option = _magicDatabase.GetOption(TypeOfOption.Display, "ShowStatistics");
            if (option != null)
                isShowStatisticsChecked = bool.Parse(option.Value);
            _showStatisticsViewModel = new MenuViewModel("Show _Statistics", new RelayCommand(ShowStatisticsCommandExecute)) { IsCheckable = true, IsChecked = isShowStatisticsChecked };
            viewMenu.AddChild(_showStatisticsViewModel);

            bool isShowOtherLanguagesChecked = true;
            option = _magicDatabase.GetOption(TypeOfOption.Display, "ShowOtherLanguages");
            if (option != null)
                isShowOtherLanguagesChecked = bool.Parse(option.Value);

            _showOtherLanguagesViewModel = new MenuViewModel("Show _Other Languages", new RelayCommand(ShowOtherLanguagesCommandExecute)) { IsCheckable = true, IsChecked = isShowOtherLanguagesChecked };
            viewMenu.AddChild(_showOtherLanguagesViewModel);

            bool isShowPictureChecked = true;
            option = _magicDatabase.GetOption(TypeOfOption.Display, "ShowPicture");
            if (option != null)
                isShowPictureChecked = bool.Parse(option.Value);

            _showPictureViewModel = new MenuViewModel("Show _Picture", new RelayCommand(ShowPictureCommandExecute)) { IsCheckable = true, IsChecked = isShowPictureChecked };
            viewMenu.AddChild(_showPictureViewModel);


            MenuRoot.AddChild(viewMenu);

            //Collection
            _collectionViewModel = new MenuViewModel("_Collection");
            GenerateCollectionMenu();
            MenuRoot.AddChild(_collectionViewModel);

            //Management
            MenuViewModel dbManagementMenu = new MenuViewModel("_Database Management");
            dbManagementMenu.AddChild(new MenuViewModel("_Block", new RelayCommand(BlockModificationCommandExecute)));
            dbManagementMenu.AddChild(new MenuViewModel("_Edition", new RelayCommand(EditionModificationCommandExecute)));
            dbManagementMenu.AddChild(new MenuViewModel("_Language", new RelayCommand(LanguageModificationCommandExecute)));
            MenuRoot.AddChild(dbManagementMenu);

            //?
            MenuViewModel aboutMenu = new MenuViewModel("?");
            aboutMenu.AddChild(new MenuViewModel("_Version", new RelayCommand(VersionCommandExecute)));
            MenuRoot.AddChild(aboutMenu);

            GenerateContextMenu();
        }
        private void GenerateContextMenu()
        {
            ContextMenuRoot.RemoveAllChildren();
            if (Hierarchical == null || Hierarchical == _allhierarchical || Hierarchical == _searchHierarchical)
                return;

            ContextMenuRoot.AddChild(new MenuViewModel("Input cards", new RelayCommand(CardInputCommandExecute)));

            HierarchicalResultNodeViewModel nodeViewModel = Hierarchical.Selected as HierarchicalResultNodeViewModel;
            if (nodeViewModel != null)
            {
                ContextMenuRoot.AddChild(new MenuViewModel("Change edition/language/foil", new RelayCommand(ChangeCardCommandExecute), nodeViewModel.Card));
                ContextMenuRoot.AddChild(new MenuViewModel("Move to other collection", new RelayCommand(MoveCardCommandExecute), nodeViewModel.Card));
            }
        }
        private void GenerateCollectionMenu()
        {
            List<ICardCollection> cardCollections = new List<ICardCollection>(_magicDatabase.GetAllCollections());

            bool hasCollection = cardCollections.Count > 0;

            _collectionViewModel.RemoveAllChildren();
            _collectionViewModel.AddChild(new MenuViewModel("New collection...", new RelayCommand(CreateCollectionCommandExecute)));
            if (hasCollection)
            {
                _collectionViewModel.AddChild(new MenuViewModel("Delete collection...", new RelayCommand(DeleteCollectionCommandExecute)));
                _collectionViewModel.AddChild(new MenuViewModel("Rename collection...", new RelayCommand(RenameCollectionCommandExecute)));
                _collectionViewModel.AddChild(MenuViewModel.Separator());
                _collectionViewModel.AddChild(new MenuViewModel("Import/Export..", new RelayCommand(ImportExportCommandExecute)));
            }

            _collectionViewModel.AddChild(MenuViewModel.Separator());
            _collectionViewModel.AddChild(new MenuViewModel("All cards", new RelayCommand(ShowAllCollectionCommandExecute)));

            if (hasCollection)
            {
                _collectionViewModel.AddChild(MenuViewModel.Separator());
                cardCollections.Sort((c1, c2) => string.Compare(c1.Name, c2.Name, StringComparison.Ordinal));
                foreach (ICardCollection cardCollection in cardCollections)
                {
                    MenuViewModel menuViewModel = new MenuViewModel(cardCollection.Name, new RelayCommand(ShowCollectionCommandExecute), cardCollection.Name)
                                                      {
                                                          IsChecked = Hierarchical != null && Hierarchical.Name == cardCollection.Name
                                                      };

                    _collectionViewModel.AddChild(menuViewModel);
                }
            }
        }
        private void DeleteCollectionAsync(object obj)
        {
            InputViewModel vm = obj as InputViewModel;

            string toBeDeleted = vm.Selected;
            string toAdd = vm.Selected2;

            if (toAdd == None)
                _magicDatabase.DeleteAllCardInCollection(toBeDeleted);
            else
                _magicDatabase.MoveCollection(toBeDeleted, toAdd);

            _magicDatabase.DeleteCollection(toBeDeleted);

            Loading = false;

            _dispatcherInvoker.Invoke(() =>
                {
                    //Delete current collection -> reset display to default
                    if (Hierarchical.Name == toBeDeleted)
                        LoadCollection();
                    else
                        GenerateCollectionMenu();
                });
        }
    }
}
