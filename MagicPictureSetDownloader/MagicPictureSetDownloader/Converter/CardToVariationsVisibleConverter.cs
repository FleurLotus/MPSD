namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Collections.Generic;
    using System.Windows;

    using MagicPictureSetDownloader.ViewModel.Main;

    [ValueConversion(typeof(CardViewModel), typeof(Visibility))]
    public class CardToVariationsVisibleConverter : CardToVariationsConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IList<int> ids = base.Convert(value, targetType, parameter, culture) as IList<int>;
            if (ids == null || ids.Count == 0)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }
    }
}
