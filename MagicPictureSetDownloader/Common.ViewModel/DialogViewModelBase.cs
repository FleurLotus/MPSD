namespace Common.ViewModel
{
    using System;
    using System.Windows.Input;

    public class DialogViewModelBase : NotifyPropertyChangedBase
    {
        public event EventHandler Closing;
        
        protected DialogViewModelBase()
        {
            OkCommand = new RelayCommand(OkCommandExecute, OkCommandCanExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute, CancelCommandCanExecute);
        }

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public bool? Result { get; private set; }

        private void OkCommandExecute(object o)
        {
            Result = true;
            OnClosing();
        }
        private void CancelCommandExecute(object o)
        {
            Result = false;
            OnClosing();
        }

        protected virtual bool OkCommandCanExecute(object o)
        {
            return true;
        }
        protected virtual bool CancelCommandCanExecute(object o)
        {
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
