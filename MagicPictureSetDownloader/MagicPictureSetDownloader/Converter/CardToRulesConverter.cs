namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Common.WPF.Converter;

    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Main;

    [ValueConversion(typeof(HierarchicalResultNodeViewModel), typeof(IRuling[]))]
    public class CardToRulesConverter : NoConvertBackConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not HierarchicalResultNodeViewModel node)
            {
                return null;
            }

            return node.Card.Rulings;
        }
    }
}
