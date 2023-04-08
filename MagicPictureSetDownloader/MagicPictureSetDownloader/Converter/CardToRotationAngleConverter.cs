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
            if (value is not HierarchicalResultNodeViewModel node || node.Card.OtherCardPart == null)
            {
                return 0.0;
            }

            int param = int.Parse(parameter.ToString());

            if (param == 0)
            {
                if (node.Card.Is90DegreeSide)
                {
                    return 90.0;
                }
            }
            else if (param == 1)
            {
                if (node.Card.OtherCardPart.IsDownSide)
                {
                    return 180.0;
                }

                if (node.Card.OtherCardPart.Is90DegreeSide)
                {
                    return -90.0;
                }
            }

            return 0.0;
        }
    }
}
