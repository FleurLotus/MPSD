namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Common.WPF;
    using Common.WPF.Converter;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    [ValueConversion(typeof(string), typeof(string))]
    public class TreeToTreeImageConverter : NoConvertBackConverter
    {
        private readonly IMagicDatabaseReadOnly MagicDatabase = Lib.IsInDesignMode() ? null : MagicDatabaseManager.ReadOnly;

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string data)
            {
                return null;
            }

            return MagicDatabase.GetTreePicture(data)?.FilePath;
        }
    }
}
