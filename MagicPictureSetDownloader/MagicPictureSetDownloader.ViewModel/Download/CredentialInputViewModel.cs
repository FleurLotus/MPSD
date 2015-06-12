namespace MagicPictureSetDownloader.ViewModel.Download
{
    using System;

    using Common.ViewModel.Dialog;

    public class CredentialInputViewModel : DialogViewModelBase
    {
        private string _login;
        private string _password;

        public CredentialInputViewModel()
        {
            if (Environment.UserName == "fbossout042214")
            {
                Login = "fabien.bossoutrot";
                Password = "Jolmdp!01";
            }
            Display.Title = "Proxy Credential";
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
    }
}
