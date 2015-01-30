namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
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
            HierarchicalResultNodeViewModel node = value as HierarchicalResultNodeViewModel;

            if (node == null)
                return null;

            return _magicDatabase.GetTranslates(node.Card.Card).ToDictionary(t => _magicDatabase.GetLanguage(t.IdLanguage).Name, t => t.Name);
        }
    }
}
