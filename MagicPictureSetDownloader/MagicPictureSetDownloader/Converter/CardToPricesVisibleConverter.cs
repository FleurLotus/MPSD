namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using MagicPictureSetDownloader.ViewModel.Main;

    [ValueConversion(typeof(HierarchicalResultNodeViewModel), typeof(Visibility))]
    public class CardToPricesVisibleConverter : CardToPricesConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (base.Convert(value, targetType, parameter, culture) is not PriceViewModel[] prices || prices.Length == 0)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }
    }
}
