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
            if (value == null || value.Length != 3 || value[0] is not HierarchicalResultNodeViewModel node)
            {
                return 0.0;
            }

            int param = int.Parse(parameter.ToString());
            int coef = 1;

            if (param == 0)
            {
                if (!node.Card.Is90DegreeSide)
                {
                    return 0.0;
                }
                coef = -1;
            }
            else if (param == 1)
            {
                if (node.Card.OtherCardPart == null || !node.Card.OtherCardPart.Is90DegreeSide)
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
