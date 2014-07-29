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

            IPicture picture = MagicDatabaseManager.GetPicture(node.IdGatherer);
            if (null == picture || picture.Image.Length == 0)
                picture = MagicDatabaseManager.GetDefaultPicture();

            return BytesToImage(picture.Image);
        }
    }
}
