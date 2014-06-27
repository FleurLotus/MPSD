using Common.ViewModel;
using MagicPictureSetDownloader.Core;

namespace MagicPictureSetDownloader.ViewModel
{
    public class SetInfoViewModel: NotifyPropertyChangedBase
    {
        private string _name;
        private bool _active;

        public DownloadReporter DownloadReporter { get; private set; }
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
            Name = setInfo.Name;

            string seachUrl = DownloadManager.ToAbsoluteUrl(DownloadManager.ExtractBaseUrl(baseSetUrl), setInfo.BaseSearchUrl);
            Url = string.Format("{0}?output=checklist&set=[\"{1}\"]", seachUrl, setInfo.Name);
            DownloadReporter = new DownloadReporter();
        }
    }
}
