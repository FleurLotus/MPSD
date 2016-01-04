namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows;

    using Common.WPF.Converter;

    using MagicPictureSetDownloader.ViewModel.Main;

    public class CardToStatisticsVisibleConverter : NoConvertBackMultiConverter
    {
        private readonly CardToStatisticsConverter _innerConverter = new CardToStatisticsConverter();

        public override object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            StatisticViewModel[] stats = _innerConverter.Convert(value, targetType, parameter, culture) as StatisticViewModel[];

            if (stats == null || stats.Length == 0)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }
    }
}
