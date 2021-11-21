namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using MagicPictureSetDownloader.ViewModel.Main;

    [ValueConversion(typeof(HierarchicalResultNodeViewModel), typeof(Visibility))]
    public class CardToLanguagesTranslationVisibleConverter : CardToLanguagesTranslationConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (base.Convert(value, targetType, parameter, culture) is not IDictionary<string, string> dic)
            {
                return Visibility.Collapsed;
            }

            return dic.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
