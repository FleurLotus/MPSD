namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Common.WPF.Converter;

    using MagicPictureSetDownloader.ViewModel.Main;

    [ValueConversion(typeof(HierarchicalResultNodeViewModel), typeof(int))]
    public class CardToRotationAngleConverter : NoConvertBackConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CardViewModel card = value as CardViewModel;
            if (card == null && value is HierarchicalResultNodeViewModel node)
            {
                card = node.Card;
            }

            if (card?.OtherCardPart == null)
            {
                return 0.0;
            }

            int param = int.Parse(parameter.ToString());

            if (param == 0)
            {
                if (card.Is90DegreeSide)
                {
                    return 90.0;
                }
            }
            else if (param == 1)
            {
                if (card.OtherCardPart.IsDownSide)
                {
                    return 180.0;
                }

                if (card.OtherCardPart.Is90DegreeSide)
                {
                    return -90.0;
                }
            }

            return 0.0;
        }
    }
}
