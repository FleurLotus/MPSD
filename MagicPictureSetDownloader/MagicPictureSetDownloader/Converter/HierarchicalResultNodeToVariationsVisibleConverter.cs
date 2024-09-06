namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Collections.Generic;
    using System.Windows;

    using MagicPictureSetDownloader.ViewModel.Main;

    [ValueConversion(typeof(HierarchicalResultNodeViewModel), typeof(Visibility))]
    public class HierarchicalResultNodeToVariationsVisibleConverter : HierarchicalResultNodeToVariationsConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (base.Convert(value, targetType, parameter, culture) is not IList<CardViewModel> ids || ids.Count == 0)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }
    }
}