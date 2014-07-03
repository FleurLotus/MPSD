using Common.ViewModel;
using MagicPictureSetDownloader.Core;

namespace MagicPictureSetDownloader.ViewModel
{
    public class SetInfoViewModel: NotifyPropertyChangedBase
    {
        private bool _active;

        public SetInfoViewModel(string baseSetUrl, SetInfoWithBlock setInfoWithBlock)
        {
            Name = setInfoWithBlock.Name;
            BlockName = setInfoWithBlock.BlockName;

            string seachUrl = DownloadManager.ToAbsoluteUrl(DownloadManager.ExtractBaseUrl(baseSetUrl), setInfoWithBlock.BaseSearchUrl);
            Url = string.Format("{0}?output=checklist&set=[\"{1}\"]", seachUrl, setInfoWithBlock.Name);
            DownloadReporter = new DownloadReporter();
        }

        public DownloadReporter DownloadReporter { get; private set; }
        public string Name { get; private set; }
        public string BlockName { get; private set; }
        public string Url { get; private set; }
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
    }
}
