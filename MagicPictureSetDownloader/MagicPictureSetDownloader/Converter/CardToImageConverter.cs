namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Main;

    [ValueConversion(typeof(HierarchicalResultNodeViewModel), typeof(BitmapImage))]
    public class CardToImageConverter : ImageConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            HierarchicalResultNodeViewModel node = value as HierarchicalResultNodeViewModel;
            int param = int.Parse(parameter.ToString());

            if (node == null)
                return null;

            int idGatherer = -1;
            if (param == 0)
            {
                idGatherer = node.Card.IdGatherer;
            }
            else if (node.Card.OtherCardPart != null)
            {
                idGatherer = node.Card.OtherCardPart.IsDownSide || node.Card.OtherCardPart.Is90DegreeSide ? node.Card.IdGatherer : node.Card.OtherCardPart.IdGatherer;
            }

            if (idGatherer == -1)
                return null;

            IPicture picture = MagicDatabase.GetPicture(idGatherer);
            if (null == picture || picture.Image == null || picture.Image.Length == 0)
                picture = MagicDatabase.GetDefaultPicture();

            if (null == picture || picture.Image == null || picture.Image.Length == 0)
                return null;

            return BytesToImage(picture.Image);
        }
    }
}
