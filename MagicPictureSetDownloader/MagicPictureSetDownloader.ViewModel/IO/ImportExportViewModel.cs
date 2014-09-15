namespace MagicPictureSetDownloader.ViewModel.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;

    using Common.ViewModel;

    using MagicPictureSetDownloader.Core.IO;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public class ImportExportViewModel : NotifyPropertyChangedBase
    {
        public event EventHandler Closing;
        private int _selectedIndex;
        private string _path;
        private ExportFormat _exportFormatSelected;
        private ImportOption _importType;
        private string _selectedCollection;
        private string _newCollectionName;
        private string _importFilePath;

        public ImportExportViewModel()
        {
            ExportFormats = (ExportFormat[])Enum.GetValues(typeof(ExportFormat));
            IMagicDatabaseReadOnly magicDatabase = MagicDatabaseManager.ReadOnly;
            ExportCollections = magicDatabase.GetAllCollections().Select(cc => new ExportedCollectionViewModel(cc.Name)).ToList();
            ImportCollections = magicDatabase.GetAllCollections().Select(cc => cc.Name).ToList();
            OkCommand = new RelayCommand(OkCommandExecute, OkCommandCanExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute);
        }

        public ExportFormat[] ExportFormats { get; private set; }
        public IList<ExportedCollectionViewModel> ExportCollections { get; private set; }
        public IList<string> ImportCollections { get; private set; }
        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
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
                }
            }
        }

        private bool OkCommandCanExecute(object o)
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

        private void OkCommandExecute(object o)
        {
            ImportExportWorker importExportWorker = new ImportExportWorker();
            if (SelectedIndex == 0)
            {
                importExportWorker.Export(ExportCollections.Where(c => c.IsSelected).Select(c => c.Name).ToArray(), Path, ExportFormatSelected);
            }
            if (SelectedIndex == 1)
            {
                switch (ImportType)
                {
                    case ImportOption.NewCollection:
                        importExportWorker.ImportToNewColletion(ImportFilePath, NewCollectionName);
                        break;
                    case ImportOption.AddToCollection:
                        importExportWorker.ImportToExistingColletion(ImportFilePath, SelectedCollection);
                        break;
                }
            }

            OnClosing();
        }
        private void CancelCommandExecute(object o)
        {
            OnClosing();
        }

        private void OnClosing()
        {
            var e = Closing;
            if (e != null)
                e(this, EventArgs.Empty);
        }
    }
}
