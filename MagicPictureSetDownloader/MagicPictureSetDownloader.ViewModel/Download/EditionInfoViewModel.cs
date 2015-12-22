namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;

    using Common.ViewModel;
    using Common.Web;

    using MagicPictureSetDownloader.Core;
    using MagicPictureSetDownloader.Interface;

    public class EditionInfoViewModel: NotifyPropertyChangedBase
    {
        private bool _active;
        private readonly IEdition _edition;

        public EditionInfoViewModel(string baseEditionUrl, EditionInfoWithBlock editionInfoWithBlock)
        {
            _edition = editionInfoWithBlock.Edition;

            string seachUrl = WebAccess.ToAbsoluteUrl(baseEditionUrl, editionInfoWithBlock.BaseSearchUrl, true);
            Url = string.Format("{0}?output=checklist&action=advanced&special=true&set=[\"{1}\"]", seachUrl, editionInfoWithBlock.Edition.GathererName);
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
