namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using MagicPictureSetDownloader.ViewModel.Main;

    public class CardToStatisticsConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            HierarchicalResultNodeViewModel node = value as HierarchicalResultNodeViewModel;

            if (node == null)
                return null;

            return node.Card.Statistics;
        }
    }
}
