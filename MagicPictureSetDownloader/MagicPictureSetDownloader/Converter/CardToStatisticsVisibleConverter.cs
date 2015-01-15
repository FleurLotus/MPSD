namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using Common.WPF.Converter;

    using MagicPictureSetDownloader.ViewModel.Main;

    [ValueConversion(typeof(HierarchicalResultNodeViewModel), typeof(Visibility))]
    public class CardToStatisticsVisibleConverter : NoConvertBackConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            HierarchicalResultNodeViewModel node = value as HierarchicalResultNodeViewModel;

            if (node == null)
                return Visibility.Collapsed;

            return node.Card.Statistics.Length > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
