namespace MagicPictureSetDownloader.ViewModel.Management
{
    using System;
    using System.Windows.Input;

    using Common.Library.Collection;
    using Common.ViewModel;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public enum ChangeState
    {
        NoEdition,
        New,
        Updating,
    }

    public abstract class DatabaseInfoModificationViewModelBase<T> : NotifyPropertyChangedBase where T: class
    {
        public event EventHandler Closing;

        private string _name;
        private T _selected;
        private ChangeState _state;
        protected readonly IMagicDatabaseReadAndWriteFull MagicDatabase;

        protected DatabaseInfoModificationViewModelBase()
        {
            All = new RangeObservableCollection<T>();
            MagicDatabase = MagicDatabaseManager.ReadAndWriteFull;
            State = ChangeState.NoEdition;

            NewCommand = new RelayCommand(NewCommandExecute, NewCommandCanExecute);
            UpdateCommand = new RelayCommand(UpdateCommandExecute, UpdateCommandCanExecute);

            ValidateCommand = new RelayCommand(ValidateCommandExecute, ValidateCommandCanExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute, CancelCommandCanExecute);
            CloseCommand = new RelayCommand(CloseCommandExecute);
        }

        public RangeObservableCollection<T> All { get; private set; }
        public string Title { get; protected set; }
        public ICommand NewCommand { get; private set; }
        public ICommand ValidateCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand UpdateCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnNotifyPropertyChanged(() => Name);
                }
            }
        }
        public ChangeState State
        {
            get { return _state; }
            set
            {
                if (value != _state)
                {
                    _state = value;
                    OnNotifyPropertyChanged(() => State);
                }
            }
        }
        public T Selected
        {
            get { return _selected; }
            set
            {
                if (value != _selected)
                {
                    _selected = value;
                    OnNotifyPropertyChanged(() => Selected);
                    DisplayCurrent();
                }
            }
        }
        public virtual bool AllowNew
        {
            get { return true; }
        }

        protected virtual bool ValidateCurrent()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }
        protected virtual bool CouldBeUpdated()
        {
            return true;
        }
        protected abstract void DisplayCurrent();
        protected abstract bool ApplyEditionToDatabase();

        private void NewCommandExecute(object o)
        {
            if (CreateNew())
                State = ChangeState.New;

        }
        private void UpdateCommandExecute(object o)
        {
            State = ChangeState.Updating;
        }
        private void ValidateCommandExecute(object o)
        {
            if (ApplyEditionToDatabase())
                State = ChangeState.NoEdition;

            DisplayCurrent();
        }
        private void CloseCommandExecute(object o)
        {
            OnClosing();
        }
        private void CancelCommandExecute(object o)
        {
            if (CancelEdition())
                State = ChangeState.NoEdition;
        }

        private bool NewCommandCanExecute(object o)
        {
            return State == ChangeState.NoEdition;
        }
        private bool CancelCommandCanExecute(object o)
        {
            return State != ChangeState.NoEdition;
        }
        private bool UpdateCommandCanExecute(object o)
        {
            return Selected != null && State == ChangeState.NoEdition && CouldBeUpdated();
        }
        private bool ValidateCommandCanExecute(object o)
        {
            return State != ChangeState.NoEdition && ValidateCurrent();
        }
        private bool CancelEdition()
        {
            DisplayCurrent();
            return true;
        }
        private bool CreateNew()
        {
            Selected = null;
            DisplayCurrent();
            return true;
        }
        private void OnClosing()
        {
            var e = Closing;
            if (e != null)
                e(this, EventArgs.Empty);
        }
    }
}
