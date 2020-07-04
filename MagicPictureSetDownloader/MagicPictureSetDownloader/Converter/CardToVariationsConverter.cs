namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Collections.Generic;
    using System.Linq;

    using MagicPictureSetDownloader.ViewModel.Main;

    using Common.WPF.Converter;

    [ValueConversion(typeof(CardViewModel), typeof(IList<int>))]
    public class CardToVariationsConverter : NoConvertBackConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IList<int> ret = new List<int>();

            CardViewModel card = value as CardViewModel;

            if (card == null)
            {
                return ret;
            }

            return card.VariationIdGatherers.ToArray();
        }
    }
}
