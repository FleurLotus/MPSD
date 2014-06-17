using CommonViewModel;

namespace MagicPictureSetDownloader.ViewModel
{
    public class DownloadReporter: NotifyPropertyChangedBase
    {
        private int _current;
        private int _total;

        public DownloadReporter()
        {
            Total = 1;
        }
        
        public int Total
        {
            get { return _total; }
            set
            {
                if (value != _total)
                {
                    _total = value;
                    OnNotifyPropertyChanged(() => Total);
                }
            }
        }
        
        public int Current
        {
            get { return _current; }
            set
            {
                if (value != _current)
                {
                    _current = value;
                    OnNotifyPropertyChanged(() => Current);
                }
            }
        }

    }
}
