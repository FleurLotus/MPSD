namespace Common.UIBaseTests
{
    using System.ComponentModel;
    using System.Globalization;

    using Common.ViewModel;

    [TypeConverter(typeof(SampleWindowViewModelTypeConverter))]
    public class SampleWindowViewModel : NotifyPropertyChangedBase
    {
        private string _sampleText;

        public SampleWindowViewModel()
        {
            SampleText = "Sample";
        }

        public string SampleText
        {
            get { return _sampleText; }
            set
            {
                if (value != _sampleText)
                {
                    _sampleText = value;
                    OnNotifyPropertyChanged();
                }
            }
        }

        public override string ToString()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(GetType());

            return converter.CanConvertTo(typeof(string)) ? converter.ConvertToString(null, CultureInfo.InvariantCulture, this) : base.ToString();
        }
    }
}