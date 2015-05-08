﻿namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;

    using Common.Libray.Notify;
    using Common.Libray.Threading;
    using Common.ViewModel;
    using Common.WPF;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Input;
    using MagicPictureSetDownloader.ViewModel.IO;
    using MagicPictureSetDownloader.ViewModel.Management;

    public partial class MainViewModel
    {
        const string None = "--- None ---";

        #region EventHandler

        public event EventHandler UpdateDatabaseRequested;
        public event EventHandler UpdateImageDatabaseRequested;
        public event EventHandler VersionRequested;
        public event EventHandler CloseRequested;
        public event EventHandler<EventArgs<ImportExportViewModel>> ImportExportRequested;
        public event EventHandler<EventArgs<InputViewModel>> InputRequested;
        public event EventHandler<EventArgs<DialogViewModelBase>> DialogWanted;
        public event EventHandler<EventArgs<INotifyPropertyChanged>> DatabaseModificationRequested;

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
        private void OnImportExportRequested(ImportExportViewModel vm)
        {
            OnEventRaise(ImportExportRequested, vm);
        }
        private void OnInputRequested(InputViewModel vm)
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
            OnInputRequested(vm);
            if (vm.Result == true)
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

            OnInputRequested(vm);
            if (vm.Result == true)
            {
                string toBeDeleted = vm.Selected;
                string toAdd = vm.Selected2;

                if (!string.IsNullOrWhiteSpace(toBeDeleted)&& !string.IsNullOrWhiteSpace(toAdd))
                {
                    Loading = true;
                    ThreadPool.QueueUserWorkItem(AsyncCalling, new ThreadPoolArgs(DeleteCollectionAsync, vm));
                }
            }
        }
        private void RenameCollectionCommandExecute(object o)
        {
            ICollection<string> cardCollections = _magicDatabase.GetAllCollections().Select(cc => cc.Name).ToList();
            List<string> source = new List<string>(cardCollections);

            InputViewModel vm = InputViewModelFactory.Instance.CreateChooseInListAndTextViewModel("Rename Collection", "Choose collection to rename and input new name", source);

            OnInputRequested(vm);
            if (vm.Result == true)
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
            ImportExportViewModel vm = new ImportExportViewModel();
            OnImportExportRequested(vm);

            if (vm.Result == true)
            {
                Loading = true;
                ThreadPool.QueueUserWorkItem(AsyncCalling, new ThreadPoolArgs(ImportExportAsync, vm));
            }
        }
        private void CardInputCommandExecute(object o)
        {
            OnDialogWanted(new CardInputViewModel(Hierarchical.Name));
            LoadCardsHierarchy();
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
            OnEventRaise(DatabaseModificationRequested, new BlockDatabaseInfoModificationViewModel());
        }
        private void EditionModificationCommandExecute(object o)
        {
            OnEventRaise(DatabaseModificationRequested, new EditionDatabaseInfoModificationViewModel());
        }
        private void LanguageModificationCommandExecute(object o)
        {
            OnEventRaise(DatabaseModificationRequested, new LanguageDatabaseInfoModificationViewModel());
        }
        private void AuditCommandExecute(object o)
        {
            OnDialogWanted(new AuditViewModel());
        }
        private void ChangeCardCommandExecute(object o)
        {
            HierarchicalResultNodeViewModel nodeViewModel = Hierarchical.Selected as HierarchicalResultNodeViewModel;
            if (nodeViewModel == null)
                return;
            
            CardUpdateViewModel vm = new CardUpdateViewModel(Hierarchical.Name, nodeViewModel.Card.Card);
            OnDialogWanted(vm);

            if (vm.Result == true)
            {
                _magicDatabase.ChangeCardEditionFoilLanguage(vm.SourceCollection, vm.Source.Card, vm.Source.Count, vm.Source.EditionSelected, vm.Source.IsFoil, vm.Source.LanguageSelected, vm.EditionSelected, vm.IsFoil, vm.LanguageSelected);
                LoadCardsHierarchy();
            }
        }

        private void RemoveCardCommandExecute(object o)
        {
            HierarchicalResultNodeViewModel nodeViewModel = Hierarchical.Selected as HierarchicalResultNodeViewModel;
            if (nodeViewModel == null)
                return;
            
            var vm = new CardRemoveViewModel(Hierarchical.Name, nodeViewModel.Card.Card);
            OnDialogWanted(vm);

            if (vm.Result == true)
            {
                _magicDatabase.InsertOrUpdateCardInCollection(vm.SourceCollection.Id, _magicDatabase.GetIdGatherer(vm.Source.Card, vm.Source.EditionSelected), vm.Source.LanguageSelected.Id, vm.Source.IsFoil ? 0 : -vm.Source.Count, vm.Source.IsFoil ? -vm.Source.Count : 0);
                LoadCardsHierarchy();
            }
        }
        private void CopyCardCommandExecute(object o)
        {
            HierarchicalResultNodeViewModel nodeViewModel = Hierarchical.Selected as HierarchicalResultNodeViewModel;
            if (nodeViewModel == null)
                return;
            
            MoveOrCopyCard(nodeViewModel.Card.Card, true);
        }
        private void MoveCardCommandExecute(object o)
        {
            HierarchicalResultNodeViewModel nodeViewModel = Hierarchical.Selected as HierarchicalResultNodeViewModel;
            if (nodeViewModel == null)
                return;

            MoveOrCopyCard(nodeViewModel.Card.Card, false);
        }
        private void RemoveCommandExecute(object o)
        {
            HierarchicalResultViewModel vm = Hierarchical.Selected;
            if (vm == null)
                return;
            
            ICardCollection sourceCollection = _magicDatabase.GetAllCollections().First(cc => cc.Name == Hierarchical.Name);

            InputViewModel questionViewModel = InputViewModelFactory.Instance.CreateQuestionViewModel("Remove", string.Format("Remove selected from {0}?", sourceCollection.Name));
            OnInputRequested(questionViewModel);

            if (questionViewModel.Result == true)
            {
                foreach (ICardInCollectionCount cicc in GetCardInCollectionInSelected(vm, sourceCollection))
                {
                    _magicDatabase.InsertOrUpdateCardInCollection(sourceCollection.Id, cicc.IdGatherer, cicc.IdLanguage, -cicc.Number, -cicc.FoilNumber);
                }

                LoadCardsHierarchy();
            }

        }
        private void CopyCommandExecute(object o)
        {
            MoveOrCopy(o as ICardCollection, Hierarchical.Selected, true);
        }
        private void MoveCommandExecute(object o)
        {
            MoveOrCopy(o as ICardCollection, Hierarchical.Selected, false);
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

            //Database Management
            MenuViewModel dbManagementMenu = new MenuViewModel("_Database Management");
            dbManagementMenu.AddChild(new MenuViewModel("_Audit", new RelayCommand(AuditCommandExecute)));
            dbManagementMenu.AddChild(MenuViewModel.Separator());
            MenuViewModel updateTableMenu = new MenuViewModel("_Update table");
            dbManagementMenu.AddChild(updateTableMenu);
            MenuRoot.AddChild(dbManagementMenu);

            //Database Management/Update table
            updateTableMenu.AddChild(new MenuViewModel("_Block", new RelayCommand(BlockModificationCommandExecute)));
            updateTableMenu.AddChild(new MenuViewModel("_Edition", new RelayCommand(EditionModificationCommandExecute)));
            updateTableMenu.AddChild(new MenuViewModel("_Language", new RelayCommand(LanguageModificationCommandExecute)));
            
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
            ContextMenuRoot.AddChild(MenuViewModel.Separator());

            HierarchicalResultNodeViewModel nodeViewModel = Hierarchical.Selected as HierarchicalResultNodeViewModel;
            if (nodeViewModel == null)
            {
                MenuViewModel copyMenu = new MenuViewModel("Copy to ..");
                ContextMenuRoot.AddChild(copyMenu);
                ContextMenuRoot.AddChild(new MenuViewModel("Remove from collection", new RelayCommand(RemoveCommandExecute)));
                MenuViewModel moveMenu = new MenuViewModel("Move to ..");

                List<ICardCollection> cardCollections = new List<ICardCollection>(_magicDatabase.GetAllCollections());
                if (cardCollections.Count > 1)
                    ContextMenuRoot.AddChild(moveMenu);

                foreach (ICardCollection collection in cardCollections)
                {
                    string name = collection.Name;
                    copyMenu.AddChild(new MenuViewModel(name, new RelayCommand(CopyCommandExecute), collection));
                    if (Hierarchical.Name != name)
                        moveMenu.AddChild(new MenuViewModel(name, new RelayCommand(MoveCommandExecute), collection));
                }
            }
            else
            {
                ContextMenuRoot.AddChild(new MenuViewModel("Copy card to other collection", new RelayCommand(CopyCardCommandExecute)));
                ContextMenuRoot.AddChild(new MenuViewModel("Remove card from collection", new RelayCommand(RemoveCardCommandExecute)));
                ContextMenuRoot.AddChild(new MenuViewModel("Move card to other collection", new RelayCommand(MoveCardCommandExecute)));
                ContextMenuRoot.AddChild(MenuViewModel.Separator());
                ContextMenuRoot.AddChild(new MenuViewModel("Change edition/language/foil", new RelayCommand(ChangeCardCommandExecute)));
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
            }
            _collectionViewModel.AddChild(MenuViewModel.Separator());
            _collectionViewModel.AddChild(new MenuViewModel("Import/Export..", new RelayCommand(ImportExportCommandExecute)));

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

        private void MoveOrCopyCard(ICard card, bool copy)
        {
            CardMoveOrCopyViewModel vm = new CardMoveOrCopyViewModel(Hierarchical.Name, card, copy);
            OnDialogWanted(vm);

            if (vm.Result == true)
            {
                if (vm.Copy)
                    _magicDatabase.InsertOrUpdateCardInCollection(vm.CardCollectionSelected.Id, _magicDatabase.GetIdGatherer(vm.Source.Card, vm.Source.EditionSelected), vm.Source.LanguageSelected.Id, vm.Source.IsFoil ? 0 : vm.Source.Count, vm.Source.IsFoil ? vm.Source.Count : 0);
                else
                    _magicDatabase.MoveCardToOtherCollection(vm.SourceCollection, vm.Source.Card, vm.Source.EditionSelected, vm.Source.LanguageSelected, vm.Source.Count, vm.Source.IsFoil, vm.CardCollectionSelected);

                LoadCardsHierarchy();
            }
        }
        private void MoveOrCopy(ICardCollection destCollection, HierarchicalResultViewModel vm, bool copy)
        {
            if (destCollection == null || vm == null)
                return; 
            
            ICardCollection sourceCollection = _magicDatabase.GetAllCollections().First(cc => cc.Name == Hierarchical.Name);
            
            string title = copy ? "Copy" : "Move";
            InputViewModel questionViewModel = InputViewModelFactory.Instance.CreateQuestionViewModel(title, string.Format("{0} selected from {1} to {2}?", title, sourceCollection.Name, destCollection.Name));
            OnInputRequested(questionViewModel);

            if (questionViewModel.Result == true)
            {
                foreach (ICardInCollectionCount cicc in GetCardInCollectionInSelected(vm, sourceCollection))
                {
                    if (copy)
                    {
                        _magicDatabase.InsertOrUpdateCardInCollection(destCollection.Id, cicc.IdGatherer, cicc.IdLanguage, cicc.Number, cicc.FoilNumber);
                    }
                    else
                    {
                        _magicDatabase.MoveCardToOtherCollection(sourceCollection, cicc.IdGatherer, cicc.IdLanguage, cicc.Number, false, destCollection);
                        _magicDatabase.MoveCardToOtherCollection(sourceCollection, cicc.IdGatherer, cicc.IdLanguage, cicc.FoilNumber, true, destCollection);
                    }
                }
                LoadCardsHierarchy();
            }
        }
        private IEnumerable<ICardInCollectionCount> GetCardInCollectionInSelected(HierarchicalResultViewModel vm, ICardCollection collection)
        {
            if (vm == null)
                yield break;

            HierarchicalResultNodeViewModel node = vm as HierarchicalResultNodeViewModel;
            if (node == null)
            {
                foreach (ICardInCollectionCount cicc in vm.Children.SelectMany(c => GetCardInCollectionInSelected(c, collection)))
                    yield return cicc;
            }
            else
            {
                foreach (CardViewModel cardViewModel in node.AllCard)
                    foreach (ICardInCollectionCount cicc in _magicDatabase.GetCollectionStatisticsForCard(collection, cardViewModel.Card))
                        if (cicc.IdGatherer == cardViewModel.IdGatherer)
                            yield return cicc;
            }
        }
        
        #region Async

        private void AsyncCalling(object state)
        {
            try
            {
                ThreadPoolArgs args = state as ThreadPoolArgs;
                if (args != null)
                    args.Invoke();
            }
            catch (Exception ex)
            {
                _dispatcherInvoker.Invoke(() => ex.UserDisplay());
            }
            finally
            {
                Loading = false;
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
        private void ImportExportAsync(object obj)
        {
            ImportExportViewModel vm = obj as ImportExportViewModel;
            vm.ImportExport();
            LoadCardsHierarchy();
        }

        #endregion
    }
}
