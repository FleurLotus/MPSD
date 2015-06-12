namespace Common.WPF
{
    using System;

    using Common.ViewModel.Exception;
    using Common.WPF.UI;

    public static class Extension
    {
        public static void UserDisplay(this Exception ex)
        {
            ExceptionViewModel vm = new ExceptionViewModel(ex);
            new ExceptionDialog(vm).ShowDialog();
        }
    }
}
