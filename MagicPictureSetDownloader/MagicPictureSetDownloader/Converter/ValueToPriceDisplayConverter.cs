namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Common.WPF.Converter;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ViewModel.Main;

    [ValueConversion(typeof(PriceViewModel), typeof(string))]
    public class ValueToPriceDisplayConverter : NoConvertBackConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not PriceViewModel data)
            {
                return null;
            }

            string currency;
            switch (data.Source)
            {
                case PriceValueSource.Cardmarket:
                    currency = "€";
                    break;
                case PriceValueSource.TCGplayer:
                    currency = "$";
                    break;
                case PriceValueSource.Unknown:
                default:
                    currency = "";
                    break;
            }

            return string.Format("{0:###,##0.00} {1}", data.Value / 100.0, currency);
        }
    }
}
