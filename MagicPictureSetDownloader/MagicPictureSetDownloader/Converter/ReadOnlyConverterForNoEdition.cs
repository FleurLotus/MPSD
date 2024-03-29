namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Common.WPF.Converter;

    using MagicPictureSetDownloader.ViewModel.Management;

    [ValueConversion(typeof(ChangeState), typeof(bool))]
    public class ReadOnlyConverterForNoEdition : NoConvertBackConverter
    {
        private readonly EnumMatchToBooleanConverter _enumMatchToBooleanConverter = new EnumMatchToBooleanConverter();

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _enumMatchToBooleanConverter.Convert(value, targetType, ChangeState.NoEdition.ToString(), culture);
        }
    }
}
