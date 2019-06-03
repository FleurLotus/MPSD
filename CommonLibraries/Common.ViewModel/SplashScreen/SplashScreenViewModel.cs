namespace Common.ViewModel.SplashScreen
{
    using System;

    public class SplashScreenViewModel : NotifyPropertyChangedBase
    {
        private string _info;
        private int _currentValue;
        private bool _showProgress;
        private Uri _sourceUri;

        internal SplashScreenViewModel()
        {
        }

        public int MaxValue
        {
            get { return 100; }
        }
        public Uri SourceUri
        {
            get { return _sourceUri; }
            set
            {
                if (value != _sourceUri)
                {
                    _sourceUri = value;
                    OnNotifyPropertyChanged(nameof(SourceUri));
                }
            }
        }
        public bool ShowProgress
        {
            get { return _showProgress; }
            set
            {
                if (value != _showProgress)
                {
                    _showProgress = value;
                    OnNotifyPropertyChanged(nameof(ShowProgress));
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
                    if (value >= 0 && value <= MaxValue)
                    {
                        _currentValue = value;
                    }
                    OnNotifyPropertyChanged(nameof(CurrentValue));
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
                    OnNotifyPropertyChanged(nameof(Info));
                }
            }
        }
    }
}
