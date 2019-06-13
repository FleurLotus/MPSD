namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Main;

    [ValueConversion(typeof(CardViewModel), typeof(BitmapImage))]
    public class CardToImageConverter : ImageConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CardViewModel card = value as CardViewModel;
            int param = int.Parse(parameter.ToString());

            if (card == null)
            {
                return null;
            }

            int idGatherer = 0;
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

            IPicture picture = MagicDatabase.GetPicture(idGatherer);
            if (null == picture || picture.Image == null || picture.Image.Length == 0)
            {
                picture = MagicDatabase.GetDefaultPicture();
            }

            if (null == picture || picture.Image == null || picture.Image.Length == 0)
            {
                return null;
            }

            return BytesToImage(picture.Image);
        }
    }
}
