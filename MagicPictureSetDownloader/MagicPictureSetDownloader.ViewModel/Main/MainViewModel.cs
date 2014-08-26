namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Linq;
    using System.Windows.Input;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Core;

    public class MainViewModel : NotifyPropertyChangedBase
    {
        public event EventHandler UpdateDatabaseRequested;
        public event EventHandler UpdateImageDatabaseRequested;
        public event EventHandler VersionRequested;
        public event EventHandler CloseRequested;

        private bool _showPicture;
        private bool _showFilterConfig;
        private readonly MagicDatabaseManager _magicDatabaseManager;

        //TODO: manage collection
        //TODO: Import/export of collection
        public MainViewModel()
        {
            UpdateDatabaseCommand = new RelayCommand(UpdateDatabaseCommandExecute);
            UpdateImageDatabaseCommand = new RelayCommand(UpdateImageDatabaseCommandExecute);
            VersionCommand = new RelayCommand(VersionCommandExecute);
            CloseCommand = new RelayCommand(CloseCommandExecute);
            Hierarchical = new HierarchicalViewModel("Magic Cards");
            _magicDatabaseManager = new MagicDatabaseManager();
            Analysers = new HierarchicalInfoAnalysersViewModel(_magicDatabaseManager);
            

            //ALERT: Temp for helping load file to tree picture 
            /*
            foreach (string file in System.IO.Directory.GetFiles(@"C:\Users\fbossout042214.ASI\Documents\Visual Studio 2012\Projects\MagicPictureSetDownloader\Sample\Others"))
            {
                _magicDatabaseManager.InsertNewTreePicture(System.IO.Path.GetFileNameWithoutExtension(file), System.IO.File.ReadAllBytes(file));
            }
            */
            LoadCardsHierarchy();

            ShowPicture = true;
        }
        
        public HierarchicalViewModel Hierarchical { get; private set; }
        public HierarchicalInfoAnalysersViewModel Analysers { get; private set; }

        public ICommand UpdateDatabaseCommand { get; private set; }
        public ICommand UpdateImageDatabaseCommand { get; private set; }
        public ICommand VersionCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        public bool ShowFilterConfig
        {
            get { return _showFilterConfig; }
            set
            {
                if (value != _showFilterConfig)
                {
                    _showFilterConfig = value;
                    OnNotifyPropertyChanged(() => ShowFilterConfig);
                    if (!_showFilterConfig)
                    {
                        Analysers.Save();
                        LoadCardsHierarchy();
                    }
                }
            }
        }
        public bool ShowPicture
        {
            get { return _showPicture; }
            set
            {
                if (value != _showPicture)
                {
                    _showPicture = value;
                    OnNotifyPropertyChanged(() => ShowPicture);
                }
            }
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

        #endregion

        private void LoadCardsHierarchy()
        {
            HierarchicalInfoAnalyserViewModel[] selectedAnalysers = Analysers.All.Where(a => a.IsActive).ToArray();

            Hierarchical.MakeHierarchyAsync(selectedAnalysers.Select(hiav => hiav.Analyser).ToArray(),
                                            selectedAnalysers.Select(hiav => hiav.IsAscendingOrder).ToArray(),
                                            _magicDatabaseManager.GetAllInfos().Select(cai => new CardViewModel(cai)));
        }

    }
}
