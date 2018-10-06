namespace MagicPictureSetDownloader.ViewModel.IO
{
    using System;
    using System.IO;
    using System.Linq;
    using Common.Library;
    using Common.Library.Notify;
    using Common.ViewModel.Dialog;

    using MagicPictureSetDownloader.Interface;

    public class ExportImagesViewModel : DialogViewModelBase
    {
        public event EventHandler<EventArgs<string>> DisplayResult;

        private readonly IDispatcherInvoker _dispatcherInvoker;
        private readonly char[] _invalidChars;

        private string _path;
        private string _suffix;
        private ExportImagesOption _exportOptionSelected;

        public ExportImagesViewModel(IDispatcherInvoker dispatcherInvoker)
        {
            _dispatcherInvoker = dispatcherInvoker;
            _invalidChars = System.IO.Path.GetInvalidFileNameChars();

            Display.Title = "Export Images";

            Path = ".";
            Suffix = ".full";
            ExportOptions = (ExportImagesOption[])Enum.GetValues(typeof(ExportImagesOption));
            ExportOptionSelected = ExportImagesOption.OneByGathererId;
        }

        public ExportImagesOption[] ExportOptions { get; }
        public string Path
        {
            get { return _path; }
            set
            {
                if (value != _path)
                {
                    _path = value;
                    OnNotifyPropertyChanged(() => Path);
                }
            }
        }
        public string Suffix
        {
            get { return _suffix; }
            set
            {
                if (value != _suffix)
                {
                    _suffix = value;
                    OnNotifyPropertyChanged(() => Suffix);
                }
            }
        }
        public ExportImagesOption ExportOptionSelected
        {
            get { return _exportOptionSelected; }
            set
            {
                if (value != _exportOptionSelected)
                {
                    _exportOptionSelected = value;
                    OnNotifyPropertyChanged(() => ExportOptionSelected);
                }
            }
        }

        protected override bool OkCommandCanExecute(object o)
        {
            return Directory.Exists(Path) && (string.IsNullOrEmpty(Suffix) || !Suffix.Any(c => _invalidChars.Contains(c)));
        }

        private void OnDisplayResult(string message)
        {
            var e = DisplayResult;
            if (e != null)
            {
                _dispatcherInvoker.Invoke(() => e(this, new EventArgs<string>(message)));
            }
        }
    }
}
