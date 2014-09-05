namespace Common.ViewModel
{
    public class InputTextViewModel : DialogViewModelBase
    {
        private string _text;

        public InputTextViewModel(string title, string label)
        {
            Title = title;
            Label = label;
        }

        public string Title { get; private set; }
        public string Label { get; private set; }

        public string Text
        {
            get { return _text; }
            set
            {
                if (value != _text)
                {
                    _text = value;
                    OnNotifyPropertyChanged(() => Text);
                }
            }
        }
    }
}
