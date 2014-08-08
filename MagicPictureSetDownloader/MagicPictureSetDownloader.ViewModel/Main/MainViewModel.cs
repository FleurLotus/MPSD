namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Core.HierarchicalAnalysing;

    public class MainViewModel : NotifyPropertyChangedBase
    {
        public event EventHandler UpdateDatabaseRequested;
        public event EventHandler UpdateImageDatabaseRequested;
        public event EventHandler VersionRequested;
        public event EventHandler CloseRequested;

        private bool _showPicture;
        private readonly MagicDatabaseManager _magicDatabaseManager;

        //TODO: display Card info of selected node
        //TODO: manage filter display, order and activation
        //TODO: manage picture size display
        //TODO: manage treepicture + feed (still some picture to find)
        //TODO: manage collection
        //TODO: Import/export of collection
        //TODO: manage Options with persistance 
        //TODO: Manage Download image in a other pool
        public MainViewModel()
        {
            UpdateDatabaseCommand = new RelayCommand(UpdateDatabaseCommandExecute);
            UpdateImageDatabaseCommand = new RelayCommand(UpdateImageDatabaseCommandExecute);
            VersionCommand = new RelayCommand(VersionCommandExecute);
            CloseCommand = new RelayCommand(CloseCommandExecute);
            Hierarchical = new HierarchicalViewModel("Magic Cards");
            Analysers = new ObservableCollection<HierarchicalInfoAnalyserViewModel>(HierarchicalInfoAnalyserFactory.Instance.Names.Select(s => new HierarchicalInfoAnalyserViewModel(s)));

            _magicDatabaseManager = new MagicDatabaseManager();
            //ALERT: Load file to tree picture 
            /*
                foreach (string file in Directory.GetFiles(@"C:\Users\fbossout042214.ASI\Documents\Visual Studio 2012\Projects\MagicPictureSetDownloader\Sample\Rarity"))
                {
                    _magicDatabaseManager.InsertNewTreePicture(Path.GetFileNameWithoutExtension(file), File.ReadAllBytes(file));
                }
                */
            LoadCardsHierarchy();

            ShowPicture = true;
        }
        private void LoadCardsHierarchy()
        {
            HierarchicalInfoAnalyserViewModel[] selectedAnalysers = Analysers.Where(a => a.IsActive).ToArray();

            Hierarchical.MakeHierarchyAsync(selectedAnalysers.Select(hiav => hiav.Analyser).ToArray(), 
                                            selectedAnalysers.Select(hiav => hiav.IsAscendingOrder).ToArray(),
                                            _magicDatabaseManager.GetAllInfos().Select(cai => new CardViewModel(cai)));
        }

        public HierarchicalViewModel Hierarchical { get; private set; }
        public ObservableCollection<HierarchicalInfoAnalyserViewModel> Analysers { get; private set; }
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

        public ICommand UpdateDatabaseCommand { get; private set; }
        public ICommand UpdateImageDatabaseCommand { get; private set; }
        public ICommand VersionCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

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

    }
}
