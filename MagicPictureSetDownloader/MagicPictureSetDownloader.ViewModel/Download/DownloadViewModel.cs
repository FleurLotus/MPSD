﻿namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Windows.Input;

    using Common.Libray;
    using Common.Libray.Collection;
    using Common.ViewModel;

    using MagicPictureSetDownloader.Core;

    public class DownloadViewModel : DownloadViewModelBase
    {
        private string _baseEditionUrl;
        private bool _hasJob;
        
        public DownloadViewModel(IDispatcherInvoker dispatcherInvoker, bool showConfig)
            : base(dispatcherInvoker)
        {
            BaseEditionUrl = @"http://gatherer.wizards.com/Pages/Default.aspx";

            ShowConfig = showConfig;
            Editions = new AsyncObservableCollection<EditionInfoViewModel>();
            GetEditionListCommand = new RelayCommand(GetEditionListCommandExecute, GetEditionListCommandCanExecute);
            FeedEditionsCommand = new RelayCommand(FeedEditionsCommandExecute, FeedEditionsCommandCanExecute);

            if (!ShowConfig)
                GetEditionListCommand.Execute(null);
        }

        public IList<EditionInfoViewModel> Editions { get; private set; }
        public ICommand GetEditionListCommand { get; private set; }
        public ICommand FeedEditionsCommand { get; private set; }
        public bool ShowConfig { get; private set; }
        public bool HasJob
        {
            get { return _hasJob; }
            set
            {
                if (value != _hasJob)
                {
                    _hasJob = value;
                    OnNotifyPropertyChanged(() => HasJob);
                }
            }
        }
        public string BaseEditionUrl
        {
            get { return _baseEditionUrl; }
            set
            {
                if (value != _baseEditionUrl)
                {
                    _baseEditionUrl = value;
                    OnNotifyPropertyChanged(() => BaseEditionUrl);
                }
            }
        }

        #region Command

        private bool GetEditionListCommandCanExecute(object o)
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(BaseEditionUrl);
        }
        private void GetEditionListCommandExecute(object o)
        {
            JobStarting();
            ThreadPool.QueueUserWorkItem(GetEditionListCallBack, BaseEditionUrl);
        }

        private bool FeedEditionsCommandCanExecute(object o)
        {
            return !IsBusy && HasJob;
        }
        private void FeedEditionsCommandExecute(object o)
        {
            JobStarting();
            ThreadPool.QueueUserWorkItem(FeedEditionsCallBack);
        }

        #endregion

        private void GetEditionListCallBack(object state)
        {
            try
            {
                string baseUrl = (string)state;
                HasJob = false;

                while (Editions.Count > 0)
                {
                    EditionInfoViewModel editionInfoViewModel = Editions[0];
                    editionInfoViewModel.PropertyChanged -= EditionInfoViewModelPropertyChanged;
                    Editions.Remove(editionInfoViewModel);
                }

                foreach (EditionInfoWithBlock editionInfoWithBlock in DownloadManager.GetEditionList(baseUrl).Where(s=> !s.Edition.Completed))
                {
                    EditionInfoViewModel editionInfoViewModel = new EditionInfoViewModel(BaseEditionUrl, editionInfoWithBlock);
                    editionInfoViewModel.PropertyChanged += EditionInfoViewModelPropertyChanged;
                    Editions.Add(editionInfoViewModel);
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            JobFinished();
        }
        private void FeedEditionsCallBack(object state)
        {
            DownloadReporter.Reset();

            foreach (EditionInfoViewModel editionInfoViewModel in Editions.Where(s => s.Active))
            {
                Interlocked.Increment(ref CountDown);
                string[] cardInfos = DownloadManager.GetCardUrls(editionInfoViewModel.Url);

                editionInfoViewModel.DownloadReporter.Total = cardInfos.Length;
                DownloadReporter.Total += cardInfos.Length;

                EditionInfoViewModel model = editionInfoViewModel;
                ThreadPool.QueueUserWorkItem(RetrieveEditionDataCallBack, new object[] { model.DownloadReporter, model.EditionId, cardInfos.Select(s => DownloadManager.ToAbsoluteUrl(model.Url, s)) });
            }
        }
        private void RetrieveEditionDataCallBack(object state)
        {
            object[] args = (object[])state;
            DownloadReporterViewModel editionDownloadReporter = (DownloadReporterViewModel)args[0];
            int editionId = (int)args[1];
            IEnumerable<string> urls = (IEnumerable<string>)args[2];

            try
            {
                bool hasCard = false;
                foreach (string cardUrl in urls)
                {
                    if (IsStopping)
                        break;
                    DownloadManager.GetCardInfo(cardUrl, editionId);
                    hasCard = true;
                    editionDownloadReporter.Progress();
                    DownloadReporter.Progress();
                }
                if (!IsStopping && hasCard)
                    DownloadManager.EditionCompleted(editionId);
            }
            catch (Exception ex)
            {
                Message += ex.Message;
            }

            editionDownloadReporter.Finish();
            Interlocked.Decrement(ref CountDown);
            if (CountDown == 0)
            {
                DownloadReporter.Finish();
                
                //Keep previous error
                string msg = Message;
                GetEditionListCommand.Execute(null);
                
                if (!string.IsNullOrWhiteSpace(msg))
                    Message = msg + Message;
                JobFinished();
            }
        }
        private void EditionInfoViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Active")
                HasJob = Editions.Any(sivm => sivm.Active);
        }
    }
}
