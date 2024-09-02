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
                idScryFall = card.IdScryFall;
            }
            else if (card.OtherCardPart != null) 
            {
                idScryFall = card.IdScryFall + MagicDatabase.GetVersoExtension();
            }

            if (string.IsNullOrEmpty(idScryFall))
            {
                return null;
            }

            return base.Convert(idScryFall, targetType, parameter, culture); 
        }
    }
}
