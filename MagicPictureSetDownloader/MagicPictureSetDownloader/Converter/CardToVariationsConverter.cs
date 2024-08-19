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
                return Array.Empty<string>();
            }

            HashSet<string> ret = new HashSet<string>();
            foreach (string idScryFall in card.VariationIdScryFalls)
            {
                ret.Add(idScryFall);
            }

            if (card.OtherCardPart != null)
            {
                foreach (string idScryFall in card.OtherCardPart.VariationIdScryFalls)
                {
                    ret.Add(idScryFall);
                }
            }

            return ret.ToArray();
        }
    }
}
