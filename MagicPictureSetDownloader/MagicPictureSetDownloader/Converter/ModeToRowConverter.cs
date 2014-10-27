namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Common.WPF.Converter;

    using MagicPictureSetDownloader.ViewModel.Input;

    [ValueConversion(typeof(InputMode), typeof(int))]
    public class ModeToRowConverter : NoConvertBackConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == parameter.ToString() ? 0 : 1;
        }
    }
}
