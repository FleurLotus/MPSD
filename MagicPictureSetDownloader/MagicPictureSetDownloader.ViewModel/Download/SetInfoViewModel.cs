namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Interface;

    public class SetInfoViewModel: NotifyPropertyChangedBase
    {
        private bool _active;
        private readonly IEdition _edition;

        public SetInfoViewModel(string baseSetUrl, SetInfoWithBlock setInfoWithBlock)
        {
            _edition = setInfoWithBlock.Edition;

            string seachUrl = DownloadManager.ToAbsoluteUrl(baseSetUrl, setInfoWithBlock.BaseSearchUrl, true);
            Url = string.Format("{0}?output=checklist&set=[\"{1}\"]", seachUrl, setInfoWithBlock.Edition.GathererName);
            DownloadReporter = new DownloadReporterViewModel();
        }

        public string Url { get; private set; }
        public DownloadReporterViewModel DownloadReporter { get; private set; }
        public string Name
        {
            get { return _edition.Name; }
        }
        public DateTime? ReleaseDate
        {
            get { return _edition.ReleaseDate; }
        }
        public int? CardNumber
        {
            get { return _edition.CardNumber; }
        }
        public int EditionId
        {
            get { return _edition.Id; }
        }
        public string BlockName
        {
            get { return _edition.BlockName; }
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
    }
}
