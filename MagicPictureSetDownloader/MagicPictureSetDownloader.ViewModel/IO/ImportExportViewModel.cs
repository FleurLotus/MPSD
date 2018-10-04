namespace MagicPictureSetDownloader.ViewModel.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Input;

    using Common.Library;
    using Common.Library.Notify;
    using Common.ViewModel;
    using Common.ViewModel.Dialog;
    using Common.ViewModel.Input;

    using MagicPictureSetDownloader.Core.IO;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public class ImportExportViewModel : DialogViewModelBase
    {
        public event EventHandler<EventArgs<InputViewModel>> OpenFileDialog;
        public event EventHandler<EventArgs<string>> DisplayResult;

        private readonly IDispatcherInvoker _dispatcherInvoker;
        private int _selectedIndex;
        private string _path;
        private ExportFormat _exportFormatSelected;
        private ImportOption _importType;
        private string _selectedCollection;
        private string _newCollectionName;
        private string _importFilePath;

        public ImportExportViewModel(IDispatcherInvoker dispatcherInvoker)
        {
            _dispatcherInvoker = dispatcherInvoker;
            ExportFormats = (ExportFormat[])Enum.GetValues(typeof(ExportFormat));
            ExportFormatSelected = ExportFormat.MPSD;
            IMagicDatabaseReadOnly magicDatabase = MagicDatabaseManager.ReadOnly;
            ExportCollections = magicDatabase.GetAllCollections().Select(cc => new ExportedCollectionViewModel(cc.Name)).ToList();
            ImportCollections = magicDatabase.GetAllCollections().Select(cc => cc.Name).ToList();

            HasCollection = ExportCollections.Count > 0;
            SelectedIndex = HasCollection ? 0 : 1;
            Path = ".";

            OpenCommand = new RelayCommand(OpenCommandExecute);
        }

        public ExportFormat[] ExportFormats { get; }
        public IList<ExportedCollectionViewModel> ExportCollections { get; }
        public IList<string> ImportCollections { get; }
        public ICommand OpenCommand { get; }
        public bool HasCollection { get; }
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
        public string ImportFilePath
        {
            get { return _importFilePath; }
            set
            {
                if (value != _importFilePath)
                {
                    _importFilePath = value;
                    OnNotifyPropertyChanged(() => ImportFilePath);
                }
            }
        }
        public string NewCollectionName
        {
            get { return _newCollectionName; }
            set
            {
                if (value != _newCollectionName)
                {
                    _newCollectionName = value;
                    OnNotifyPropertyChanged(() => NewCollectionName);
                }
            }
        }
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value != _selectedIndex)
                {
                    _selectedIndex = value;
                    OnNotifyPropertyChanged(() => SelectedIndex);
                }
            }
        }
        public string SelectedCollection
        {
            get { return _selectedCollection; }
            set
            {
                if (value != _selectedCollection)
                {
                    _selectedCollection = value;
                    OnNotifyPropertyChanged(() => SelectedCollection);
                }
            }
        }
        public ExportFormat ExportFormatSelected
        {
            get { return _exportFormatSelected; }
            set
            {
                if (value != _exportFormatSelected)
                {
                    _exportFormatSelected = value;
                    OnNotifyPropertyChanged(() => ExportFormatSelected);
                }
            }
        }
        public ImportOption ImportType
        {
            get { return _importType; }
            set
            {
                if (value != _importType)
                {
                    _importType = value;
                    OnNotifyPropertyChanged(() => ImportType);
                    if (_importType == ImportOption.AddToCollection && HasCollection && SelectedCollection == null)
                    {
                        SelectedCollection = ImportCollections[0];
                    }
                }
            }
        }

        protected override bool OkCommandCanExecute(object o)
        {
            if (SelectedIndex == 0)
            {
                return ExportCollections.Any(c => c.IsSelected) && Directory.Exists(Path);
            }
            if (SelectedIndex == 1)
            {
                switch (ImportType)
                {
                    case ImportOption.NewCollection:
                        return File.Exists(ImportFilePath) && !string.IsNullOrWhiteSpace(NewCollectionName) && !ImportCollections.Contains(NewCollectionName);
                    case ImportOption.AddToCollection:
                        return File.Exists(ImportFilePath) && SelectedCollection != null;
                }
            }

            return false;
        }
        private void OpenCommandExecute(object o)
        {
            InputViewModel vm = InputViewModelFactory.Instance.CreateTextViewModel(null, null);
            OnOpenFileDialog(vm);
            if (!string.IsNullOrWhiteSpace(vm.Text))
            {
                ImportFilePath = vm.Text;
            }
        }
        private void OnOpenFileDialog(InputViewModel vm)
        {
            var e = OpenFileDialog;
            if (e != null)
            {
                e(this, new EventArgs<InputViewModel>(vm));
            }
        }
        private void OnDisplayResult(string message)
        {
            var e = DisplayResult;
            if (e != null)
            {
                _dispatcherInvoker.Invoke(() => e(this, new EventArgs<string>(message)));
            }
        }

        public void ImportExport()
        {
            if (Result == true)
            {
                ImportExportWorker importExportWorker = new ImportExportWorker();
                if (SelectedIndex == 0)
                {
                    importExportWorker.Export(ExportCollections.Where(c => c.IsSelected).Select(c => c.Name).ToArray(), Path, ExportFormatSelected);
                }
                if (SelectedIndex == 1)
                {
                    ImportStatus status = null;
                    switch (ImportType)
                    {
                        case ImportOption.NewCollection:
                            status = importExportWorker.ImportToNewCollection(ImportFilePath, NewCollectionName);
                            break;
                        case ImportOption.AddToCollection:
                            status = importExportWorker.ImportToExistingCollection(ImportFilePath, SelectedCollection);
                            break;
                    }

                    DoPostImport(status, ImportFilePath);
                }
            }
        }

        private void DoPostImport(ImportStatus status, string importFilePath)
        {
            if (status == null)
            {
                return;
            }

            StringBuilder message = new StringBuilder();
            string fileName = System.IO.Path.GetFileName(importFilePath);
            message.AppendFormat("We manage to import {0} card(s) from {1}", status.TotalCard, fileName);

            if (status.TotalKoLine > 0)
            {
                string directoryName = System.IO.Path.GetDirectoryName(importFilePath);
                string timestamp = DateTime.Now.ToString("yyyMMddHHmmss");
                string errorFileName = "Error" + timestamp + ".txt";

                string rebuiltFileName = "Error" + fileName;
                using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(directoryName, errorFileName)))
                {
                    sw.Write(status.ErrorMessage);
                }
                using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(directoryName, rebuiltFileName)))
                {
                    sw.Write(status.RebuiltErrorFile);
                }
                message.AppendFormat(@"
We were unable to import {0} line(s).
You could find the errors in {1}
A file with only error was generated {2}",
                    status.TotalKoLine, errorFileName, rebuiltFileName);
            }

            OnDisplayResult(message.ToString());
        }
    }
}
