namespace Common.ViewModel.UnitTests
{
    using Common.ViewModel.Dialog;
    using Common.ViewModel.Input;

    public class DialogViewModelBaseTesting : DialogViewModelBase
    {
        public void ForceClosing()
        {
            OnClosing();
        }
        public void ForceDialogWanted(DialogViewModelBase arg)
        {
            OnDialogWanted(arg);
        }
        public void ForceInputRequested(InputViewModel arg)
        {
            OnInputRequestedRequested(arg);
        }
    }
}
