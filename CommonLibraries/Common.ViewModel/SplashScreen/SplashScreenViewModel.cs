namespace Common.ViewModel.SplashScreen
{
    using System;

    internal class SplashScreenViewModel : NotifyPropertyChangedBase
    {
        private string _info;
        private int _currentValue;
        private bool _showProgess;
        private Uri _sourceUri;
        
        public Uri SourceUri
        {
            get { return _sourceUri; }
            set
            {
                if (value != _sourceUri)
                {
                    _sourceUri = value;
                    OnNotifyPropertyChanged(() => SourceUri);
                }
            }
        }
        public bool ShowProgess
        {
            get { return _showProgess; }
            set
            {
                if (value != _showProgess)
                {
                    _showProgess = value;
                    OnNotifyPropertyChanged(() => ShowProgess);
                }
            }
        }
        public int CurrentValue
        {
            get { return _currentValue; }
            set
            {
                if (value != _currentValue)
                {
                    _currentValue = value;
                    OnNotifyPropertyChanged(() => CurrentValue);
                }
            }
        }
        public string Info
        {
            get { return _info; }
            set
            {
                if (value != _info)
                {
                    _info = value;
                    OnNotifyPropertyChanged(() => Info);
                }
            }
        }
    }
}
