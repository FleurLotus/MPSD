namespace MagicPictureSetDownloader.ViewModel.IO
{
    using System;
    using System.Threading;
    using MagicPictureSetDownloader.Core.IO;

    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Download;

    public class ExportImagesProgressViewMode : DownloadViewModelBase
    {
        private readonly string _path;
        private readonly string _suffix;
        private readonly ExportImagesOption _exportOption;
        private readonly ExportImagesWorker _exportImagesWorker;
        private ICardAllDbInfo[] _cards;

        public ExportImagesProgressViewMode(string path, string suffix, ExportImagesOption exportOption)
           : base("Export images")
        {
            _path = path;
            _suffix = suffix;
            _exportOption = exportOption;
            _exportImagesWorker = new ExportImagesWorker();
        }

        protected override bool StartImpl()
        {
            _cards = _exportImagesWorker.GetAllCardWithPicture();

            if (_cards.Length == 0)
            {
                SetMessage("Not any data to export");
                return false;
            }
            DownloadReporter.Total = _cards.Length;

            ThreadPool.QueueUserWorkItem(Downloader);

            return true;
        }
        private void Downloader(object state)
        {

            foreach (ICardAllDbInfo cardInfo in _cards)
            {
               try
                {
                    _exportImagesWorker.Export(cardInfo, _path, _suffix, _exportOption);
                }
                catch (Exception ex)
                {
                    string errormessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                    }

                    AppendMessage(string.Format("{0} -> {1}", cardInfo.IdGatherer, errormessage), false);
                }
                DownloadReporter.Progress();
            }

            DownloadReporter.Finish();
            JobFinished();
        }
    }
}