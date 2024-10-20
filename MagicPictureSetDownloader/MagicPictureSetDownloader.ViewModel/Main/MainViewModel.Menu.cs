﻿namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;

    using Common.Library.Notify;
    using Common.Library.Threading;
    using Common.ViewModel;
    using Common.ViewModel.Menu;
    using Common.ViewModel.Input;
    using Common.ViewModel.Dialog;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Deck;
    using MagicPictureSetDownloader.ViewModel.Download;
    using MagicPictureSetDownloader.ViewModel.Download.Auto;
    using MagicPictureSetDownloader.ViewModel.Input;
    using MagicPictureSetDownloader.ViewModel.IO;
    using MagicPictureSetDownloader.ViewModel.Management;
    using MagicPictureSetDownloader.ViewModel.Option;

    public partial class MainViewModel
    {
        const string None = "--- None ---";

        #region EventHandler

        public event EventHandler<EventArgs<DownloadViewModelBase>> UpdateDatabaseRequested;
        public event EventHandler<EventArgs<DownloadViewModelBase>> AutoUpdateDatabaseRequested;
        public event EventHandler VersionRequested;
        public event EventHandler CloseRequested;
        public event EventHandler<EventArgs<ImportExportViewModel>> ImportExportRequested;
        public event EventHandler<EventArgs<InputViewModel>> InputRequested;
        public event EventHandler<EventArgs<DialogViewModelBase>> DialogWanted;
        public event EventHandler<EventArgs<INotifyPropertyChanged>> DatabaseModificationRequested;
        public event EventHandler<EventArgs<PreconstructedDecksViewModel>> PreconstructedDecksRequested;
        public event EventHandler<EventArgs<CollectionInputGraphicViewModel>> CollectionInputGraphicRequested;
        public event EventHandler<EventArgs<Exception>> ExceptionOccured;

        #endregion
        private readonly MenuViewModel _menuRoot;
        private readonly MenuViewModel _contextMenuRoot;
        private SearchViewModel _searchViewModel;
        
        private MenuViewModel _collectionViewModel;

        public MenuViewModel MenuRoot
        {
            get { return _menuRoot; }
        }
        public MenuViewModel ContextMenuRoot
        {
            get { return _contextMenuRoot; }
        }
     
        #region Events

        private void OnUpdateDatabaseRequested(DownloadViewModelBase vm)
        {
            OnEventRaise(UpdateDatabaseRequested, vm);
        }
        private void OnAutoUpdateDatabaseRequested(DownloadViewModelBase vm)
        {
            OnEventRaise(AutoUpdateDatabaseRequested, vm);
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
        private void OnExceptionOccured(Exception ex)
        {
            OnEventRaise(ExceptionOccured, ex);
        }

        #endregion

        #region Command

        private void UpdateDatabaseCommandExecute(object o)
        {
            OnUpdateDatabaseRequested(new DownloadViewModel());
            LoadCardsHierarchy();
        }
        private void UpdateLanguageDatabaseCommandExecute(object o)
        {
            OnUpdateDatabaseRequested(new DownloadLanguageViewModel());
            LoadCardsHierarchy();
        }
        private void UpdateImageDatabaseCommandExecute(object o)
        {
            OnAutoUpdateDatabaseRequested(new AutoDownloadImageViewModel());
        }
        private void UpdatePriceDatabaseCommandExecute(object o)
        {
            OnAutoUpdateDatabaseRequested(new AutoDownloadPriceViewModel((PriceSource)o));
            LoadCardsHierarchy();
        }
        private void UpdatePreconstructedDeckCommandExecute(object o)
        {
            OnAutoUpdateDatabaseRequested(new AutoDownloadPreconstructedDeckViewModel());
        }
        private void VersionCommandExecute(object o)
        {
            OnVersionRequested();
        }
        private void CloseCommandExecute(object o)
        {
            OnCloseRequested();
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

                    _magicDatabaseForCollection.InsertNewCollection(newName);
                    GenerateCollectionMenu();
                }
            }
        }
        private void ShowPreconstructedDecksCommandExecute(object o)
        {
            PreconstructedDecksViewModel vm = new PreconstructedDecksViewModel();
            OnEventRaise(PreconstructedDecksRequested, vm);

            if (vm.Result == true)
            {
                Loading = true;
                ThreadPool.QueueUserWorkItem(AsyncCalling, new ThreadPoolArgs(AddPreconstructedDeckToCollectionAsync, vm));
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
                    _magicDatabaseForCollection.UpdateCollectionName(_magicDatabase.GetCollection(toBeRenamed), newName);
                    GenerateCollectionMenu();
                }
            }
        }
        private void ImportExportCommandExecute(object o)
        {
            ImportExportViewModel vm = new ImportExportViewModel(_dispatcherInvoker);
            OnImportExportRequested(vm);

            if (vm.Result == true)
            {
                Loading = true;
                ThreadPool.QueueUserWorkItem(AsyncCalling, new ThreadPoolArgs(ImportExportAsync, vm));
            }
        }
        private void CardInputCommandExecute(object o)
        {
            OnDialogWanted(new CardInputViewModel(Hierarchical.Name, (int)o));
            LoadCardsHierarchy();
        }
        private void CollectionInputGraphicCommandExecute(object o)
        {
            OnEventRaise(CollectionInputGraphicRequested, new CollectionInputGraphicViewModel(Hierarchical.Name));
            LoadCardsHierarchy();
        }
        private void SearchCommandExecute(object o)
        {
            if (_searchViewModel == null)
            {
                _searchViewModel = new SearchViewModel();
            }
            else
            {
                _searchViewModel.Reuse();
            }

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
        private void TranslateModificationCommandExecute(object o)
        {
            OnEventRaise(DatabaseModificationRequested, new TranslateDatabaseInfoModificationViewModel());
        }
        private void AuditCommandExecute(object o)
        {
            OnDialogWanted(new AuditViewModel());
        }
        private void OptionCommandExecute(object o)
        {
            OptionsChangeViewModel vm = new OptionsChangeViewModel(Options);
            OnDialogWanted(vm);

            if (vm.Result == true)
            {
                Options.Save();
            }
            else
            {
                //reset data
                Options.GetDbOptions();
            }
        }
        private void CheckNewVersionCommandExecute(object o)
        {
            if (_programUpdater.HasNewVersionAvailable())
            {
                InputViewModel vm = InputViewModelFactory.Instance.CreateQuestionViewModel("New version available", "Do you want to upgrade?");
                OnInputRequested(vm);
                if (vm.Result == true)
                {
                    Loading = true;
                    _programUpdater.Upgrade();
                    OnCloseRequested();
                }
            }
            else
            {
                InputViewModel vm = InputViewModelFactory.Instance.CreateInfoViewModel("No new version", "You have the lastest version");
                OnInputRequested(vm);
            }
        }
        private void ChangeCardCommandExecute(object o)
        {
            if (Hierarchical.Selected is not HierarchicalResultNodeViewModel nodeViewModel)
            {
                return;
            }

            CardUpdateViewModel vm = new CardUpdateViewModel(Hierarchical.Name, nodeViewModel.Card.Card);
            OnDialogWanted(vm);

            if (vm.Result == true)
            {
                _magicDatabaseForCardInCollection.ChangeCardEditionFoilAltArtLanguage(vm.SourceCollection, vm.Source.Card, vm.Source.Count, vm.Source.EditionSelected, new CardCountKey(vm.Source.IsFoil, vm.Source.IsAltArt), vm.Source.LanguageSelected, vm.EditionSelected, new CardCountKey(vm.IsFoil, vm.IsAltArt), vm.LanguageSelected);
                LoadCardsHierarchy();
            }
        }

        private void RemoveCardCommandExecute(object o)
        {
            if (Hierarchical.Selected is not HierarchicalResultNodeViewModel nodeViewModel)
            {
                return;
            }

            var vm = new CardRemoveViewModel(Hierarchical.Name, nodeViewModel.Card.Card);
            OnDialogWanted(vm);

            if (vm.Result == true)
            {
                CardCount cardCount = new CardCount
                {
                    { new CardCountKey(vm.Source.IsFoil, vm.Source.IsAltArt), -vm.Source.Count }
                };

                _magicDatabaseForCardInCollection.InsertOrUpdateCardInCollection(vm.SourceCollection.Id, _magicDatabase.GetIdScryFall(vm.Source.Card, vm.Source.EditionSelected), vm.Source.LanguageSelected.Id, cardCount);
                LoadCardsHierarchy();
            }
        }
        private void CopyCardCommandExecute(object o)
        {
            if (Hierarchical.Selected is not HierarchicalResultNodeViewModel nodeViewModel)
            {
                return;
            }

            MoveOrCopyCard(nodeViewModel.Card.Card, true);
        }
        private void MoveCardCommandExecute(object o)
        {
            if (Hierarchical.Selected is not HierarchicalResultNodeViewModel nodeViewModel)
            {
                return;
            }

            MoveOrCopyCard(nodeViewModel.Card.Card, false);
        }
        private void RemoveCommandExecute(object o)
        {
            HierarchicalResultViewModel vm = Hierarchical.Selected;
            if (vm == null)
            {
                return;
            }

            ICardCollection sourceCollection = _magicDatabase.GetAllCollections().First(cc => cc.Name == Hierarchical.Name);

            InputViewModel questionViewModel = InputViewModelFactory.Instance.CreateQuestionViewModel("Remove", string.Format("Remove selected from {0}?", sourceCollection.Name));
            OnInputRequested(questionViewModel);

            if (questionViewModel.Result == true)
            {
                using (_magicDatabaseForCardInCollection.BatchMode())
                {
                    foreach (ICardInCollectionCount cicc in GetCardInCollectionInSelected(vm, sourceCollection))
                    {
                        ICardCount cardCount = new CardCount();
                        foreach (KeyValuePair<ICardCountKey, int> kv in cicc.GetCardCount())
                        {
                            cardCount.Add(kv.Key, -kv.Value);
                        }

                        _magicDatabaseForCardInCollection.InsertOrUpdateCardInCollection(sourceCollection.Id, cicc.IdScryFall, cicc.IdLanguage, cardCount);
                    }
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
            fileMenu.AddChild(new MenuViewModel("Update _Editions/Cards Database...", new RelayCommand(UpdateDatabaseCommandExecute)));
            fileMenu.AddChild(new MenuViewModel("Update _Cards Language Database...", new RelayCommand(UpdateLanguageDatabaseCommandExecute)));
            fileMenu.AddChild(new MenuViewModel("Update _Images Database..", new RelayCommand(UpdateImageDatabaseCommandExecute)));
            //Not Allowed in release version, the update is done by copy of referential
#if DEBUG
            fileMenu.AddChild(new MenuViewModel("Update _Preconstructed Decks Database..", new RelayCommand(UpdatePreconstructedDeckCommandExecute)));
#endif
            fileMenu.AddChild(MenuViewModel.Separator());
            //Price
            MenuViewModel priceMenu = new MenuViewModel("Update _Prices Database");
            foreach (PriceSource pricesource in (PriceSource[])Enum.GetValues(typeof(PriceSource)))
            {
                priceMenu.AddChild(new MenuViewModel(pricesource.ToString("g"), new RelayCommand(UpdatePriceDatabaseCommandExecute), pricesource));
            }

            fileMenu.AddChild(priceMenu);
            fileMenu.AddChild(MenuViewModel.Separator());
            fileMenu.AddChild(new MenuViewModel("Search...", new RelayCommand(SearchCommandExecute)));
            fileMenu.AddChild(MenuViewModel.Separator());
            fileMenu.AddChild(new MenuViewModel("_Exit", new RelayCommand(CloseCommandExecute)));
            MenuRoot.AddChild(fileMenu);

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
            updateTableMenu.AddChild(new MenuViewModel("_Translate", new RelayCommand(TranslateModificationCommandExecute)));

            //Tools
            MenuViewModel toolsMenu = new MenuViewModel("_Tools");
            toolsMenu.AddChild(new MenuViewModel("_Options", new RelayCommand(OptionCommandExecute)));
            toolsMenu.AddChild(MenuViewModel.Separator());
            toolsMenu.AddChild(new MenuViewModel("_Check for new version", new RelayCommand(CheckNewVersionCommandExecute)));
            MenuRoot.AddChild(toolsMenu);

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
            {
                return;
            }

            ContextMenuRoot.AddChild(new MenuViewModel("Add cards", new RelayCommand(CardInputCommandExecute), 1));
            ContextMenuRoot.AddChild(new MenuViewModel("Remove cards", new RelayCommand(CardInputCommandExecute), -1));
            ContextMenuRoot.AddChild(MenuViewModel.Separator());
            ContextMenuRoot.AddChild(new MenuViewModel("Add cards (Graphic)", new RelayCommand(CollectionInputGraphicCommandExecute)));
            ContextMenuRoot.AddChild(MenuViewModel.Separator());

            if (Hierarchical.Selected is not HierarchicalResultNodeViewModel nodeViewModel)
            {
                MenuViewModel copyMenu = new MenuViewModel("Copy to ..");
                ContextMenuRoot.AddChild(copyMenu);
                ContextMenuRoot.AddChild(new MenuViewModel("Remove from collection", new RelayCommand(RemoveCommandExecute)));
                MenuViewModel moveMenu = new MenuViewModel("Move to ..");

                List<ICardCollection> cardCollections = new List<ICardCollection>(_magicDatabase.GetAllCollections());
                if (cardCollections.Count > 1)
                {
                    ContextMenuRoot.AddChild(moveMenu);
                }

                foreach (ICardCollection collection in cardCollections)
                {
                    string name = collection.Name;
                    copyMenu.AddChild(new MenuViewModel(name, new RelayCommand(CopyCommandExecute), collection));
                    if (Hierarchical.Name != name)
                    {
                        moveMenu.AddChild(new MenuViewModel(name, new RelayCommand(MoveCommandExecute), collection));
                    }
                }
            }
            else
            {
                ContextMenuRoot.AddChild(new MenuViewModel("Copy card to other collection", new RelayCommand(CopyCardCommandExecute)));
                ContextMenuRoot.AddChild(new MenuViewModel("Remove card from collection", new RelayCommand(RemoveCardCommandExecute)));
                ContextMenuRoot.AddChild(new MenuViewModel("Move card to other collection", new RelayCommand(MoveCardCommandExecute)));
                ContextMenuRoot.AddChild(MenuViewModel.Separator());
                ContextMenuRoot.AddChild(new MenuViewModel("Change edition/language/foil/alt art", new RelayCommand(ChangeCardCommandExecute)));
            }
        }
        private void GenerateCollectionMenu()
        {
            List<ICardCollection> cardCollections = new List<ICardCollection>(_magicDatabase.GetAllCollections());

            bool hasCollection = cardCollections.Count > 0;

            _collectionViewModel.RemoveAllChildren();
            _collectionViewModel.AddChild(new MenuViewModel("Preconstructed deck...", new RelayCommand(ShowPreconstructedDecksCommandExecute)));
            _collectionViewModel.AddChild(MenuViewModel.Separator());
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
                CardCount cardCount = new CardCount
                {
                    { new CardCountKey(vm.Source.IsFoil, vm.Source.IsAltArt), vm.Source.Count }
                };

                if (vm.Copy)
                {
                    _magicDatabaseForCardInCollection.InsertOrUpdateCardInCollection(vm.CardCollectionSelected.Id, _magicDatabase.GetIdScryFall(vm.Source.Card, vm.Source.EditionSelected), vm.Source.LanguageSelected.Id, cardCount);
                }
                else
                {
                    _magicDatabaseForCardInCollection.MoveCardToOtherCollection(vm.SourceCollection, vm.Source.Card, vm.Source.EditionSelected, vm.Source.LanguageSelected, cardCount, vm.CardCollectionSelected);
                }

                LoadCardsHierarchy();
            }
        }
        private void MoveOrCopy(ICardCollection destCollection, HierarchicalResultViewModel vm, bool copy)
        {
            if (destCollection == null || vm == null)
            {
                return;
            }

            ICardCollection sourceCollection = _magicDatabase.GetAllCollections().First(cc => cc.Name == Hierarchical.Name);
            
            string title = copy ? "Copy" : "Move";
            InputViewModel questionViewModel = InputViewModelFactory.Instance.CreateQuestionViewModel(title, string.Format("{0} selected from {1} to {2}?", title, sourceCollection.Name, destCollection.Name));
            OnInputRequested(questionViewModel);

            if (questionViewModel.Result == true)
            {
                using (_magicDatabaseForCardInCollection.BatchMode())
                {
                    foreach (ICardInCollectionCount cicc in GetCardInCollectionInSelected(vm, sourceCollection))
                    {
                        if (copy)
                        {
                            _magicDatabaseForCardInCollection.InsertOrUpdateCardInCollection(destCollection.Id, cicc.IdScryFall, cicc.IdLanguage, cicc.GetCardCount());
                        }
                        else
                        {
                            _magicDatabaseForCardInCollection.MoveCardToOtherCollection(sourceCollection, cicc.IdScryFall, cicc.IdLanguage, cicc.GetCardCount(), destCollection);
                        }
                    }
                }
                LoadCardsHierarchy();
            }
        }
        private IEnumerable<ICardInCollectionCount> GetCardInCollectionInSelected(HierarchicalResultViewModel vm, ICardCollection collection)
        {
            if (vm == null)
            {
                yield break;
            }

            if (vm is not HierarchicalResultNodeViewModel node)
            {
                foreach (ICardInCollectionCount cicc in vm.Children.SelectMany(c => GetCardInCollectionInSelected(c, collection)))
                {
                    yield return cicc;
                }
            }
            else
            {
                foreach (CardViewModel cardViewModel in node.AllCard)
                {
                    foreach (ICardInCollectionCount cicc in _magicDatabase.GetCollectionStatisticsForCard(collection, cardViewModel.Card))
                    {
                        if (cicc.IdScryFall == cardViewModel.IdScryFall)
                        {
                            yield return cicc;
                        }
                    }
                }
            }
        }
        
        #region Async

        private void AsyncCalling(object state)
        {
            try
            {
                if (state is ThreadPoolArgs args)
                {
                    args.Invoke();
                }
            }
            catch (Exception ex)
            {
                _dispatcherInvoker.Invoke(() => OnExceptionOccured(ex));
            }
            finally
            {
                Loading = false;
            }
        }
        private void DeleteCollectionAsync(object obj)
        {
            InputViewModel vm = obj as InputViewModel;

            // ReSharper disable PossibleNullReferenceException
            string toBeDeleted = vm.Selected;
            // ReSharper restore PossibleNullReferenceException
            string toAdd = vm.Selected2;

            if (toAdd == None)
            {
                _magicDatabaseForCollection.DeleteAllCardInCollection(toBeDeleted);
            }
            else
            {
                _magicDatabaseForCollection.MoveCollection(toBeDeleted, toAdd);
            }

            _magicDatabaseForCollection.DeleteCollection(toBeDeleted);

            Loading = false;

            _dispatcherInvoker.Invoke(() =>
                {
                    //Delete current collection -> reset display to default
                    if (Hierarchical.Name == toBeDeleted)
                    {
                        LoadCollection();
                    }
                    else
                    {
                        GenerateCollectionMenu();
                    }
                });
        }
        private void ImportExportAsync(object obj)
        {
            ImportExportViewModel vm = obj as ImportExportViewModel;
            // ReSharper disable PossibleNullReferenceException
            vm.ImportExport();
            // ReSharper restore PossibleNullReferenceException
            LoadCardsHierarchy();
        }
        private void AddPreconstructedDeckToCollectionAsync(object obj)
        {
            PreconstructedDecksViewModel vm = obj as PreconstructedDecksViewModel;
            // ReSharper disable PossibleNullReferenceException
            _magicDatabaseForCollection.PreconstructedDeckToCollection(vm.PreconstructedDeckSelected.PreconstructedDeck, vm.CardCollectionSelected, vm.LanguageSelected);
            // ReSharper restore PossibleNullReferenceException
            LoadCardsHierarchy();
        }
        #endregion
    }
}
