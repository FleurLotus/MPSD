namespace Common.ViewModel.Dialog
{
    using System;
    using System.Windows.Input;

    using Common.Library.Notify;
    using Common.ViewModel.Input;

    public class DialogViewModelBase : NotifyPropertyChangedBase
    {
        public event EventHandler Closing;
        public event EventHandler<EventArgs<DialogViewModelBase>> DialogWanted;
        public event EventHandler<EventArgs<InputViewModel>> InputRequested;
        
        protected DialogViewModelBase()
        {
            OkCommand = new RelayCommand(OkCommandExecute, OkCommandCanExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute, CancelCommandCanExecute);

            OtherCommand = new RelayCommand(OtherCommandExecute, OtherCommandCanExecute);
            Other2Command = new RelayCommand(Other2CommandExecute, Other2CommandCanExecute);
            Display = new DisplayInfo();
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand OtherCommand { get; }
        public ICommand Other2Command { get; }
        public DisplayInfo Display { get; }
        public bool? Result { get; protected set; }

        protected virtual void OkCommandExecute(object o)
        {
            Result = true;
            OnClosing();
        }
        protected virtual void CancelCommandExecute(object o)
        {
            Result = false;
            OnClosing();
        }

        protected virtual void OtherCommandExecute(object o)
        {
        }
        protected virtual void Other2CommandExecute(object o)
        {
        }

        protected virtual bool OkCommandCanExecute(object o)
        {
            return true;
        }
        protected virtual bool CancelCommandCanExecute(object o)
        {
            return true;
        }
        protected virtual bool OtherCommandCanExecute(object o)
        {
            return true;
        }
        protected virtual bool Other2CommandCanExecute(object o)
        {
            return true;
        }

        protected void OnDialogWanted(DialogViewModelBase arg)
        {
            var ev = DialogWanted;
            if (ev != null && arg != null)
            {
                ev(this, new EventArgs<DialogViewModelBase>(arg));
            }
        }
        protected void OnInputRequestedRequested(InputViewModel vm)
        {
            var e = InputRequested;
            if (e != null && vm != null)
            {
                e(this, new EventArgs<InputViewModel>(vm));
            }
        }
        protected void OnClosing()
        {
            var e = Closing;
            if (e != null)
            {
                e(this, EventArgs.Empty);
            }
        }
    }
}
