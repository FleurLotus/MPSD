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
            IDictionary<string, string> dic = base.Convert(value, targetType, parameter, culture) as IDictionary<string, string>;

            if (dic == null)
            {
                return Visibility.Collapsed;
            }

            return dic.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
