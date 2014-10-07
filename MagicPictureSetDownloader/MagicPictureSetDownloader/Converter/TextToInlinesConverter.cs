namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Media.Imaging;

    using MagicPictureSetDownloader.Core.CardInfo;

    public class TextToInlinesConverter : IValueConverter
    {
        private static readonly StringToImageConverter _conv = new StringToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            List<Inline> newList = new List<Inline>();

            string text = value.ToString();
            if (string.IsNullOrEmpty(text))
            {
                newList.Add(new Run(text));
                return newList;
            }

            int pos = text.IndexOf(SymbolParser.Prefix, StringComparison.InvariantCulture);
            if (pos < 0)
            {
                newList.Add(new Run(text));
                return newList;
            }

            bool previousIsPicture = false;

            while (pos >= 0)
            {
                if (pos > 0)
                {

                    newList.Add(new Run((previousIsPicture ? " " : string.Empty) + text.Substring(0, pos)));
                    previousIsPicture = false;
                }

                int end = text.IndexOf(' ', pos);
                if (end < 0)
                    end = text.Length;

                string symbol = text.Substring(pos, end - pos);
                BitmapImage source = (BitmapImage)_conv.Convert(symbol, typeof(BitmapImage), null, CultureInfo.InvariantCulture);
                if (source == null)
                {
                    text = text.Substring(pos + SymbolParser.Prefix.Length);
                }
                else
                {
                    
                    Image image = new Image { Source = source, Width = 12, Height = 12, Visibility = Visibility.Visible };
                    newList.Add(new InlineUIContainer(image));
                    if (text.Length > end)
                        text = text.Substring(end + 1);
                    else
                        text = string.Empty;
                    previousIsPicture = true;
                }

                pos = text.IndexOf(SymbolParser.Prefix, StringComparison.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(text))
                newList.Add(new Run(" " + text));
            
            return newList;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
