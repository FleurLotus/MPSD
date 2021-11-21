namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows;
 
    using MagicPictureSetDownloader.ViewModel.Main;

    public class CardToStatisticsVisibleConverter : CardToStatisticsConverter
    {
       public override object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (base.Convert(value, targetType, parameter, culture) is not StatisticViewModel[] stats || stats.Length == 0)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }
    }
}
