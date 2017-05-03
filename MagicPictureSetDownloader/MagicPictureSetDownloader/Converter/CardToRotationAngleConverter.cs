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
            HierarchicalResultNodeViewModel node = value as HierarchicalResultNodeViewModel;

            if (node == null || node.Card.OtherCardPart == null)
                return 0;

            if (node.Card.OtherCardPart.IsDownSide)
                return 180;

            if (node.Card.OtherCardPart.Is90DegreeSide)
                return -90;

            return 0;
        }
    }
}
