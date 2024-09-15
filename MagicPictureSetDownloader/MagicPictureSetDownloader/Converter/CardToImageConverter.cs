namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;
    using MagicPictureSetDownloader.ViewModel.Main;

    [ValueConversion(typeof(CardViewModel), typeof(BitmapImage))]
    public class CardToImageConverter : ScryFallIdToImageConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int param = int.Parse(parameter.ToString());

            if (value is not CardViewModel card)
            {
                return null;
            }

            if (param == 0)
            {
                return base.Convert(card.IdScryFall, targetType, parameter, culture);
            }

            if (card.OtherCardPart == null)
            {
                return null;
            }

            object o = base.Convert(card.IdScryFall + MagicDatabase.GetVersoExtension(), targetType, parameter, culture);

            if (o != null)
            {
                return o;
            }

            if (card.OtherCardPart.IsDownSide || card.OtherCardPart.Is90DegreeSide)
            {
                return base.Convert(card.IdScryFall, targetType, parameter, culture);
            }

            return null;
        }
    }
}
