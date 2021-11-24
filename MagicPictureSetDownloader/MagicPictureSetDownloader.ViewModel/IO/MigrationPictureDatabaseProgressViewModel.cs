namespace MagicPictureSetDownloader.ViewModel.IO
{
    using System;
    using System.Threading;
    using MagicPictureSetDownloader.Core.IO;

    using MagicPictureSetDownloader.ViewModel.Download;

    public class MigrationPictureDatabaseProgressViewModel : DownloadViewModelBase
    {
        private readonly ExportImagesWorker _exportImagesWorker;
        private Tuple<bool, object>[] _pictures;

        public MigrationPictureDatabaseProgressViewModel()
           : base("Export images")
        {
            _exportImagesWorker = new ExportImagesWorker();
        }

        protected override bool StartImpl()
        {
            _pictures = _exportImagesWorker.GetAllPicture();

            if (_pictures.Length == 0)
            {
                SetMessage("Not any data to export");
                return false;
            }
            DownloadReporter.Total = _pictures.Length;

            ThreadPool.QueueUserWorkItem(Downloader);

            return true;
        }
        private void Downloader(object state)
        {

            foreach (Tuple<bool, object> t in _pictures)
            {
               try
                {
                    _exportImagesWorker.Export(t.Item1, t.Item2);
                }
                catch (Exception ex)
                {
                    string errormessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        errormessage = ex.InnerException.Message;
                    }

                    AppendMessage(string.Format("{0} -> {1}", t.Item2, errormessage), false);
                }
                DownloadReporter.Progress();
            }

            DownloadReporter.Finish();
            JobFinished();
        }
    }
}