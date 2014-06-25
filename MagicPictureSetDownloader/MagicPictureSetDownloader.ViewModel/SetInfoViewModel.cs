using Common.ViewModel;
using MagicPictureSetDownloader.Core;

namespace MagicPictureSetDownloader.ViewModel
{
    public class SetInfoViewModel: NotifyPropertyChangedBase
    {
        private string _name;
        private bool _active;

        public string Alias { get; private set; }
        public DownloadReporter DownloadReporter { get; private set; }
        public string PictureUrl { get; private set; }
        public string Url { get; private set; }
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnNotifyPropertyChanged(() => Name);
                }
                if (!string.IsNullOrWhiteSpace(_name))
                    Active = true;
            }
        }
        public bool Active
        {
            get { return _active; }
            set
            {
                if (value != _active)
                {
                    _active = value;
                    OnNotifyPropertyChanged(() => Active);
                }
            }
        }
        

        public SetInfoViewModel(string baseSetUrl, SetInfo setInfo)
        {
            Active = false;
            Alias = setInfo.Alias;
            PictureUrl = DownloadManager.ToAbsoluteUrl(baseSetUrl, setInfo.PictureUrl);
            Url = DownloadManager.ToAbsoluteUrl(baseSetUrl, setInfo.Url);
            DownloadReporter = new DownloadReporter();
        }
    }
}
