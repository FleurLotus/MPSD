namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Common.Libray;
    using Common.ViewModel;

    using MagicPictureSetDownloader.Interface;

    public partial class MainViewModel
    {
        public event EventHandler UpdateDatabaseRequested;
        public event EventHandler UpdateImageDatabaseRequested;
        public event EventHandler VersionRequested;
        public event EventHandler CloseRequested;
        public event EventHandler<EventArgs<InputTextViewModel>> NameRequested;

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
        private string OnNameRequested(string title, string label)
        {
            var e = NameRequested;
            if (e != null)
            {
                InputTextViewModel vm = new InputTextViewModel(title, label);
                e(this, new EventArgs<InputTextViewModel>(vm));
                if (vm.Result.HasValue && vm.Result.Value)
                {
                    return vm.Text;
                }
            }
            return null;
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
            string newName = OnNameRequested("New Collection", "Input new collection name");
            if (!string.IsNullOrWhiteSpace(newName))
            {
                if (_magicDatabaseManager.GetCollection(newName) != null)
                    throw new ApplicationException("Name is already used for a other collection");

                _magicDatabaseManager.InsertNewCollection(newName);
                GenerateCollectionMenu();
            }
        }

        #endregion

        private void CreateMenu()
        {
            Menus = new ObservableCollection<MenuViewModel>();

            MenuViewModel fileMenu = new MenuViewModel("_File");
            fileMenu.Children.Add(new MenuViewModel("Update _Set Database", new RelayCommand(UpdateDatabaseCommandExecute)));
            fileMenu.Children.Add(new MenuViewModel("Update _Image Database",new RelayCommand(UpdateImageDatabaseCommandExecute)));
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
            _collectionViewModel.Children.Clear();
            _collectionViewModel.Children.Add(new MenuViewModel("New Collection", new RelayCommand(CreateCollectionCommandExecute)));
            _collectionViewModel.Children.Add(MenuViewModel.Separator);
            _collectionViewModel.Children.Add(new MenuViewModel("All Cards", new RelayCommand(ShowAllCollectionCommandExecute)));
            ICollection<ICardCollection> cardCollections = _magicDatabaseManager.GetAllCollections();
            if (cardCollections != null && cardCollections.Count > 0)
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
