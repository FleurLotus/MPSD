namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Data;

    using Common.WPF;
    using Common.WPF.Converter;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Main;

    [ValueConversion(typeof(HierarchicalResultNodeViewModel), typeof(IDictionary<string, string>))]
    public class CardToLanguagesTranslationConverter : NoConvertBackConverter
    {
        private readonly IMagicDatabaseReadOnly _magicDatabase = Lib.IsInDesignMode() ? null : MagicDatabaseManager.ReadOnly;

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not HierarchicalResultNodeViewModel node)
            {
                return null;
            }

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (ILanguage language in _magicDatabase.GetAllLanguages())
            {
                string name = node.Card.ToString(language.Id);
                if (!string.IsNullOrEmpty(name))
                {
                    dictionary.Add(language.Name, name);
                }
            }
            return dictionary;
        }
    }
}
