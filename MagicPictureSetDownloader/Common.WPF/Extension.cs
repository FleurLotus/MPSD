namespace Common.WPF
{
    using System;

    using Common.ViewModel;
    using Common.WPF.UI;

    public static class Extension
    {
        public static void UserDisplay(this Exception ex)
        {
            //TODO: manage -> display of exception from not UI thread with not work
            ExceptionViewModel vm = new ExceptionViewModel(ex);
            new ExceptionDialog(vm).ShowDialog();
        }
    }
}
