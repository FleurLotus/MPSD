namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using MagicPictureSetDownloader.ViewModel.Main;

    public class CardToStatisticsVisibleConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            HierarchicalResultNodeViewModel node = value as HierarchicalResultNodeViewModel;

            if (node == null)
                return Visibility.Collapsed;

            return node.Card.Statistics.Length>0? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
