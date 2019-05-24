namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

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

            HierarchicalResultNodeViewModel node = value[0] as HierarchicalResultNodeViewModel;

            if (node == null || node.Card.OtherCardPart == null || !node.Card.OtherCardPart.Is90DegreeSide)
            {
                return 0.0;
            }

            double actualHeight = (double)value[1];
            double actualWidth = (double)value[2];
            return (actualHeight - actualWidth) / 2;
        }
    }
}
