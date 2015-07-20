namespace MagicPictureSetDownloader.ViewModel.Option
{
    using Common.ViewModel.Dialog;

    public class OptionsChangeViewModel : DialogViewModelBase
    {
        public OptionsChangeViewModel(OptionsViewModel optionsViewModel)
        {
            Options = optionsViewModel;

            Display.Title = "Options";
        }
        public OptionsViewModel Options { get; private set; }
    }
}
