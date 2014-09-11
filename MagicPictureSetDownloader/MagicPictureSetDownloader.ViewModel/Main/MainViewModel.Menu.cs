namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Common.Libray;
    using Common.ViewModel;

    using MagicPictureSetDownloader.Interface;

    public partial class MainViewModel
    {
        public event EventHandler UpdateDatabaseRequested;
        public event EventHandler UpdateImageDatabaseRequested;
        public event EventHandler VersionRequested;
        public event EventHandler CloseRequested;
        public event EventHandler<EventArgs<InputViewModel>> InputRequested;

        private MenuViewModel _showPictureViewModel;
        private MenuViewModel _collectionViewModel;

        public ObservableCollection<MenuViewModel> Menus { get; private set; }
        public bool ShowPicture
        {
            get { return _showPictureViewModel.IsChecked; }
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
        private void OnInputRequestedRequested(InputViewModel vm)
        {
            var e = InputRequested;
            if (e != null && vm != null)
                e(this, new EventArgs<InputViewModel>(vm));
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
        }
        private void ShowAllCollectionCommandExecute(object o)
        {
            LoadAllCards();
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
                    if (_magicDatabaseManager.GetCollection(newName) != null)
                        throw new ApplicationException("Name is already used for a other collection");

                    _magicDatabaseManager.InsertNewCollection(newName);
                    GenerateCollectionMenu();
                }
            }
        }
        private void DeleteCollectionCommandExecute(object o)
        {
            const string none = "--- None ---";
            ICollection<string> cardCollections = _magicDatabaseManager.GetAllCollections().Select(cc=>cc.Name).ToList();
            List<string> source = new List<string>(cardCollections);
            List<string> dest = new List<string>(cardCollections);
            
            dest.Insert(0, none);

            InputViewModel vm = InputViewModelFactory.Instance.CreateMoveFromListToOtherViewModel("Delete Collection", "Choose collection to delete", source, "Move card to: ('None' to remove them)", dest);
            
            OnInputRequestedRequested(vm);
            if (vm.Result.HasValue && vm.Result.Value)
            {
                string toBeDeleted = vm.Selected;
                string toMove = vm.Selected2;

                if (!string.IsNullOrWhiteSpace(toBeDeleted)&& !string.IsNullOrWhiteSpace(toMove))
                {
                    //TODO: delete collection with move card if needed
                    GenerateCollectionMenu();
                }
            }
        }

        #endregion

        private void CreateMenu()
        {
            Menus = new ObservableCollection<MenuViewModel>();

            MenuViewModel fileMenu = new MenuViewModel("_File");
            fileMenu.Children.Add(new MenuViewModel("Update _Set Database...", new RelayCommand(UpdateDatabaseCommandExecute)));
            fileMenu.Children.Add(new MenuViewModel("Update _Image Database..",new RelayCommand(UpdateImageDatabaseCommandExecute)));
            fileMenu.Children.Add(MenuViewModel.Separator);
            fileMenu.Children.Add(new MenuViewModel("_Exit", new RelayCommand(CloseCommandExecute)));
            Menus.Add(fileMenu);

            MenuViewModel viewMenu = new MenuViewModel("_View");
            _showPictureViewModel = new MenuViewModel("_Show Picture", new RelayCommand(ShowPictureCommandExecute)) { IsCheckable = true, IsChecked = true };
            viewMenu.Children.Add(_showPictureViewModel);
            Menus.Add(viewMenu);

            _collectionViewModel = new MenuViewModel("_Collection");
            GenerateCollectionMenu();
            Menus.Add(_collectionViewModel);

            MenuViewModel aboutMenu = new MenuViewModel("?");
            aboutMenu.Children.Add(new MenuViewModel("_Version", new RelayCommand(VersionCommandExecute)));
            Menus.Add(aboutMenu);
        }
        
        private void GenerateCollectionMenu()
        {
            ICollection<ICardCollection> cardCollections = _magicDatabaseManager.GetAllCollections();
            bool hasCollection = cardCollections != null && cardCollections.Count > 0;

            _collectionViewModel.Children.Clear();
            _collectionViewModel.Children.Add(new MenuViewModel("New collection...", new RelayCommand(CreateCollectionCommandExecute)));
            if (hasCollection)
            {
                _collectionViewModel.Children.Add(new MenuViewModel("Delete collection...", new RelayCommand(DeleteCollectionCommandExecute)));
            }

            _collectionViewModel.Children.Add(MenuViewModel.Separator);
            _collectionViewModel.Children.Add(new MenuViewModel("All cards", new RelayCommand(ShowAllCollectionCommandExecute)));

            if (hasCollection)
            {
                _collectionViewModel.Children.Add(MenuViewModel.Separator);
                foreach (ICardCollection cardCollection in cardCollections)
                {
                    _collectionViewModel.Children.Add(new MenuViewModel(cardCollection.Name, new RelayCommand(ShowCollectionCommandExecute), cardCollection.Name));
                }
                
            }
        }
    }
}
