namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;
    using MagicPictureSetDownloader.ViewModel.Main;

    [ValueConversion(typeof(CardViewModel), typeof(BitmapImage))]
    public class CardToImageConverter : ImageConverterBase
    {
        protected override string GetCachePrefix()
        {
            return "CardImage";
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int param = int.Parse(parameter.ToString());


            CardViewModel card = value as CardViewModel;
            if (card == null && value is HierarchicalResultNodeViewModel node)
            {
                card = node.Card;
            }

            if (card == null)
            {
                return null;
            }

            if (param == 0)
            {
                return Convert(card.IdScryFall, param);
            }

            if (card.OtherCardPart == null)
            {
                return null;
            }

            object o = Convert(card.IdScryFall + MagicDatabase.GetVersoExtension(), param);

            if (o != null)
            {
                return o;
            }

            if (card.OtherCardPart.IsDownSide || card.OtherCardPart.Is90DegreeSide)
            {
                return Convert(card.IdScryFall, param);
            }

            return null;
        }
        private BitmapImage Convert(string idScryFall, int param)
        {
            if (string.IsNullOrEmpty(idScryFall))
            {
                return null;
            }

            BitmapImage image = GetImage(idScryFall);
            if (image != null)
            {
                return image;
            }

            byte[] bytes = MagicDatabase.GetPicture(idScryFall)?.Image;
            if (null != bytes && bytes.Length != 0)
            {
                return BytesToImage(bytes, idScryFall);
            }

            if (param != 0)
            {
                return null;
            }

            return GetDefaultCardImage();
        }
    }
}
