namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;

    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Main;

    public class CardToImageConverter : ImageConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            HierarchicalResultNodeViewModel node = value as HierarchicalResultNodeViewModel;

            if (node == null)
                return null;

            IPicture picture = MagicDatabaseManager.GetPicture(node.Card.IdGatherer);
            if (null == picture || picture.Image == null || picture.Image.Length == 0)
                picture = MagicDatabaseManager.GetDefaultPicture();

            if (null == picture || picture.Image == null || picture.Image.Length == 0)
                return null;

            return BytesToImage(picture.Image);
        }
    }
}
