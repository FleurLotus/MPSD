namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Main;

    [ValueConversion(typeof(HierarchicalResultNodeViewModel), typeof(Visibility))]
    public class CardToRulesVisibleConverter : CardToRulesConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IRuling[] rules = base.Convert(value, targetType, parameter, culture) as IRuling[];

            if (rules == null || rules.Length == 0)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }
    }
}
