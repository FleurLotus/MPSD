using System;
using System.Windows.Input;
using Common.ViewModel;

namespace MagicPictureSetDownloader.ViewModel
{
    public class MainViewModel : NotifyPropertyChangedBase
    {
        public event EventHandler UpdateDatabaseRequested;
        public event EventHandler VersionRequested;
        public event EventHandler CloseRequested;

        private bool _showPicture;
        private AsyncObservableCollection<SetViewModel> _sets;

        //ALERT: TO BE CODED for display 
        public MainViewModel()
        {
            UpdateDatabaseCommand = new RelayCommand(UpdateDatabaseCommandExecute);
            VersionCommand = new RelayCommand(VersionCommandExecute);
            CloseCommand = new RelayCommand(CloseCommandExecute);
            Sets = new AsyncObservableCollection<SetViewModel>();
            ShowPicture = true;
        }

        

        public AsyncObservableCollection<SetViewModel> Sets
        {
            get { return _sets; }
            set
            {
                if (value != _sets)
                {
                    _sets = value;
                    OnNotifyPropertyChanged(() => Sets);
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

/*
 * 

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
