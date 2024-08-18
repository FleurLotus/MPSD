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

            string idScryFall = null;
            if (param == 0)
            {
                idGatherer = card.IdGatherer;
            }
            else if (card.OtherCardPart != null)
            {
                idGatherer = card.OtherCardPart.IsDownSide || card.OtherCardPart.Is90DegreeSide ? card.IdGatherer : card.OtherCardPart.IdGatherer;
            }

            if (idGatherer == 0)
            {
                return null;
            }

            return base.Convert(idGatherer, targetType, parameter, culture); 
        }
    }
}
