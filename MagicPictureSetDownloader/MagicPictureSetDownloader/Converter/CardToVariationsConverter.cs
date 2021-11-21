namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Collections.Generic;

    using MagicPictureSetDownloader.ViewModel.Main;

    using Common.WPF.Converter;
    using System.Linq;

    [ValueConversion(typeof(CardViewModel), typeof(IList<int>))]
    public class CardToVariationsConverter : NoConvertBackConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not CardViewModel card)
            {
                return Array.Empty<int>();
            }

            HashSet<int> ret = new HashSet<int>();
            foreach (int idGatherer in card.VariationIdGatherers)
            {
                ret.Add(idGatherer);
            }

            if (card.OtherCardPart != null)
            {
                foreach (int idGatherer in card.OtherCardPart.VariationIdGatherers)
                {
                    ret.Add(idGatherer);
                }
            }

            return ret.ToArray();
        }
    }
}
