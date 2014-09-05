namespace Common.ViewModel
{
    using System;

    public class ExceptionViewModel : DialogViewModelBase
    {
        public ExceptionViewModel(Exception exception)
        {
            ExceptionText = exception.Message;
        }

        public string ExceptionText { get; private set; }
    }
}
