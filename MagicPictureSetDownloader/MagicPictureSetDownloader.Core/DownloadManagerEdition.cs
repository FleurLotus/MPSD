namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Library.Notify;
    using Common.Web;
    using MagicPictureSetDownloader.Interface;

    public class DownloadManagerEdition
    {
        private class JobData
        {
            public string Url { get; }
            public int EditionId { get; }
            public string Html { get; private set; }

            public JobData(string url, int editionId)
            {
                Url = url;
                EditionId = editionId;
            }

            public void AddHtml(string html)
            {
                Html = html;
            }
        }

        public event EventHandler<EventArgs<string>> Error;
        public event EventHandler Finished;

        private readonly DownloadManager _downloadManager;
        private readonly IProgressReporter _globalProgressReporter;
        private volatile bool _isStopping;

        private readonly Dictionary<int, IProgressReporter> _editions = new Dictionary<int, IProgressReporter>();
        private readonly BlockingCollection<JobData> _inputs = new BlockingCollection<JobData>();
        private readonly BlockingCollection<JobData> _inputsWithHtml = new BlockingCollection<JobData>(100);

        public DownloadManagerEdition(DownloadManager downloadManager, IProgressReporter globalProgressReporter)
        {
            _downloadManager = downloadManager;
            _globalProgressReporter = globalProgressReporter;
        }

        public void AddRange(IEnumerable<string> urls, int editionId, IProgressReporter progressReporter)
        {
            bool hasUrl = false;
            foreach (string url in urls)
            {
                _inputs.Add(new JobData(url, editionId));
                hasUrl = true;
            }
            if (hasUrl)
            {
                _editions.Add(editionId, progressReporter);
            }
        }

        public void Start()
        {
            var htmlTasks = Enumerable.Range(0, 4).Select(_ => Task.Run((Action)GetHtml)).ToArray();
            var parserTasks = Enumerable.Range(0, 2).Select(_ => Task.Run((Action)Parse)).ToArray();

            _inputs.CompleteAdding();
            Task.WaitAll(htmlTasks);
            _inputsWithHtml.CompleteAdding();
            Task.WaitAll(parserTasks);

            if (!_isStopping)
            {
                foreach (var kv in _editions)
                {
                    _downloadManager.EditionCompleted(kv.Key);
                    kv.Value.Finish();
                }
            }

            _globalProgressReporter.Finish();
            OnFinished();
        }

        private void GetHtml()
        {
            foreach (var jobData in _inputs.GetConsumingEnumerable())
            {
                try
                {
                    if (_isStopping)
                    {
                        return;
                    }
                    jobData.AddHtml(_downloadManager.GetExtraInfo(jobData.Url));
                    _inputsWithHtml.Add(jobData);
                }
                catch (Exception ex)
                {
                    SendError(ex, $"{jobData.Url} ReTrying .. ");

                    if (_isStopping)
                    {
                        return;
                    }
                    Thread.Sleep(2000);
                    try
                    {
                        if (_isStopping)
                        {
                            return;
                        }
                        jobData.AddHtml(_downloadManager.GetExtraInfo(jobData.Url));
                        _inputsWithHtml.Add(jobData);
                    }
                    catch (Exception ex2)
                    {
                        SendError(ex2, $"{jobData.Url} Final Failure");
                    }
                }
            }
        }
        private void Parse()
        {
            foreach (var jobData in _inputsWithHtml.GetConsumingEnumerable())
            {
                try
                {
                    if (_isStopping)
                    {
                        return;
                    }
                    foreach (CardWithExtraInfo cardWithExtraInfo in Parser.ParseCardInfo(jobData.Html))
                    {
                        string pictureUrl = WebAccess.ToAbsoluteUrl(jobData.Url, cardWithExtraInfo.PictureUrl);
                        int idGatherer = Parser.ExtractIdGatherer(pictureUrl);
                        string baseUrl = WebAccess.ToAbsoluteUrl(jobData.Url, string.Format("Languages.aspx?multiverseid={0}", idGatherer));

                        CardWithExtraInfo info = cardWithExtraInfo;

                        _downloadManager.ManageMultiPage(baseUrl, html =>
                        {
                            foreach (CardLanguageInfo language in Parser.ParseCardLanguage(html))
                            {
                                info.Add(language);
                            }
                        });

                        _downloadManager.InsertCardInDb(cardWithExtraInfo);
                        _downloadManager.InsertCardEditionInDb(jobData.EditionId, cardWithExtraInfo, pictureUrl);

                        foreach (int otherIdGatherer in cardWithExtraInfo.OtherIdGatherer)
                        {
                            _downloadManager.InsertCardEditionVariationInDb(idGatherer, otherIdGatherer, WebAccess.ToAbsoluteUrl(jobData.Url, string.Format(Parser.AlternativePictureUrl, otherIdGatherer), true));
                        }
                    }
                    _editions[jobData.EditionId].Progress();
                    _globalProgressReporter.Progress();
                }
                catch (Exception ex)
                {
                    SendError(ex, jobData.Url);
                }
            }
        }
        public void Stop()
        {
            _isStopping = true;
        }
        private void SendError(Exception ex, string url)
        {
            OnError($"{url} -> {ex.Message}");
        }
        private void OnError(string message)
        {
            var e = Error;
            if (e != null)
            {
                e(this, new EventArgs<string>(message));
            }
        }
        private void OnFinished()
        {
            var e = Finished;
            if (e != null)
            {
                e(this, EventArgs.Empty);
            }
        }
    }
}
