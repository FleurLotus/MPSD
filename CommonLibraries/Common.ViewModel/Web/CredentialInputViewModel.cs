namespace Common.ViewModel.Web
{
    using Common.ViewModel.Dialog;

    public class CredentialInputViewModel : DialogViewModelBase
    {
        private string _login;
        private string _password;

        public CredentialInputViewModel()
        {
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
                    OnNotifyPropertyChanged(nameof(Login));
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
                    OnNotifyPropertyChanged(nameof(Password));
                }
            }
        }
    }
}
