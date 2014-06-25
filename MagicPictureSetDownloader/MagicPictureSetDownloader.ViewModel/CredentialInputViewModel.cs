using System;
using System.Windows.Input;
using Common.ViewModel;

namespace MagicPictureSetDownloader.ViewModel
{
    public class CredentialInputViewModel : NotifyPropertyChangedBase
    {
        public event EventHandler Closing;

        private string _login;
        private string _password;

        public CredentialInputViewModel()
        {
            OkCommand = new RelayCommand(OkCommandExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute);
            
            if (Environment.UserName == "fbossout042214")
            {
                Login = "fabien.bossoutrot";
                Password = "Jolmdp!01";
            }

        }
        
        public string Login
        {
            get { return _login; }
            set
            {
                if (_login != value)
                {
                    _login = value;
                    OnNotifyPropertyChanged(() => Login);
                }
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnNotifyPropertyChanged(() => Password);
                }
            }
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

        private void OnClosing()
        {
            var e = Closing;
            if (e != null)
                e(this, EventArgs.Empty);
        }
    }
}
