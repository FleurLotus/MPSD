namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using Common.WPF.Converter;

    using MagicPictureSetDownloader.ViewModel.Main;

    [ValueConversion(typeof(HierarchicalResultViewModel), typeof(Visibility))]
    public class TreeToVisibleConverter : NoConvertBackConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is HierarchicalResultNodeViewModel ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
