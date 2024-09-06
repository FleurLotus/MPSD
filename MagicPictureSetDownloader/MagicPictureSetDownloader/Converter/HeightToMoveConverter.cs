namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;

    using Common.WPF.Converter;

    using MagicPictureSetDownloader.ViewModel.Main;

    public class HeightToMoveConverter : NoConvertBackMultiConverter
    {
        public override object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.Length != 3)
            {
                return 0.0;
            }

            CardViewModel card = value[0] as CardViewModel;
            if (card == null && value[0] is HierarchicalResultNodeViewModel node)
            {
                card = node.Card;
            }

            if (card == null)
            {
                return 0.0;
            }


            int param = int.Parse(parameter.ToString());
            int coef = 1;

            if (param == 0)
            {
                if (!card.Is90DegreeSide)
                {
                    return 0.0;
                }
                coef = -1;
            }
            else if (param == 1)
            {
                if (card.OtherCardPart == null || !card.OtherCardPart.Is90DegreeSide)
                {
                    return 0.0;
                }
                coef = 1;
            }

            double actualHeight = (double)value[1];
            double actualWidth = (double)value[2];
            return coef * (actualHeight - actualWidth) / 2;
        }
    }
}
