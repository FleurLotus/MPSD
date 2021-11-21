namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    using Common.WPF.Converter;
     using MagicPictureSetDownloader.ViewModel.Main;

    [ValueConversion(typeof(HierarchicalResultNodeViewModel), typeof(PriceViewModel[]))]
    public class CardToPricesConverter : NoConvertBackConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not HierarchicalResultNodeViewModel node)
            {
                return null;
            }

            return node.AllCard.SelectMany(c  => c.Prices)
                               .OrderByDescending(p => p.AddDate)
                               .ThenBy(p => p.Foil)
                               .ThenBy(p => p.EditionName)
                               .ThenBy(p => p.Source).ToArray();
        }
    }
}
