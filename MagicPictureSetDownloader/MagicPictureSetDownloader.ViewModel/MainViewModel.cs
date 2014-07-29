namespace MagicPictureSetDownloader.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Common.ViewModel;
    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Core.HierarchicalAnalysing;

    public class MainViewModel : NotifyPropertyChangedBase
    {
        public event EventHandler UpdateDatabaseRequested;
        public event EventHandler VersionRequested;
        public event EventHandler CloseRequested;

        private bool _showPicture;
        private readonly MagicDatabaseManager _magicDatabaseManager;

        //TODO: TO BE CODED for display picture and text of selected node
        //TODO: manage picture 
        //TODO: manage treepicture
        public MainViewModel() : this(false)
        {
        }
        protected MainViewModel(bool inDesign)
        {
            UpdateDatabaseCommand = new RelayCommand(UpdateDatabaseCommandExecute);
            VersionCommand = new RelayCommand(VersionCommandExecute);
            CloseCommand = new RelayCommand(CloseCommandExecute);
            Hierarchical = new HierarchicalViewModel("Magic Cards");
            Analysers = HierarchicalInfoAnalyserFactory.Instance.Names.Select(s=> new HierarchicalInfoAnalyserViewModel(s)).ToList();
            if (!inDesign)
            {
                _magicDatabaseManager = new MagicDatabaseManager();
                Hierarchical.MakeHierarchyAsync(Analysers.Select(a => a.Analyser), _magicDatabaseManager.GetAllInfos().Select(cai => new CardViewModel(cai)));
            }
            ShowPicture = true;
        }

        public HierarchicalViewModel Hierarchical { get; private set; }
        public List<HierarchicalInfoAnalyserViewModel> Analysers { get; private set; }
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

/*
     FeedSetsCommand = new RelayCommand(FeedSetsCommandExecute, FeedSetsCommandCanExecute);
            DownloadReporter = new DownloadReporter();
            _downloadManager = new DownloadManager();
            _downloadManager.CredentialRequiered += OnCredentialRequiered;
        }
        */
        public ICommand UpdateDatabaseCommand { get; private set; }
        public ICommand VersionCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        


        private void OnUpdateDatabaseRequested()
        {
            EventHandler e = UpdateDatabaseRequested;
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

        #region Command
        private void UpdateDatabaseCommandExecute(object o)
        {
            OnUpdateDatabaseRequested();
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
