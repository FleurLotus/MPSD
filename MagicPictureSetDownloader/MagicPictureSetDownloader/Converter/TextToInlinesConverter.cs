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

    using Common.WPF.Converter;

    using MagicPictureSetDownloader.Core;

    [ValueConversion(typeof(string), typeof(List<Inline>))]
    public class TextToInlinesConverter : NoConvertBackConverter
    {
        private static readonly StringToCastingCostImageConverter _conv = new StringToCastingCostImageConverter();

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            List<Inline> newList = new List<Inline>();

            string text = value.ToString();
            if (string.IsNullOrEmpty(text))
            {
                newList.Add(new Run(text));
                return newList;
            }

            int pos = text.IndexOf(Shard.Prefix, StringComparison.InvariantCulture);
            if (pos < 0)
            {
                newList.Add(new Run(text));
                return newList;
            }

            bool previousIsPicture = false;
            bool endOfString = false;

            while (pos >= 0)
            {
                if (pos > 0)
                {

                    newList.Add(new Run((previousIsPicture ? " " : string.Empty) + text[..pos]));
                    previousIsPicture = false;
                }

                int end = text.IndexOf(Shard.Suffix, pos);
                if (end < 0)
                {
                    endOfString = true;
                    end = text.Length;
                }

                int endPosition = endOfString ? end : end - 1;

                string symbol = text[(pos + Shard.Prefix.Length)..end];
                BitmapImage source = (BitmapImage)_conv.Convert(Shard.DisplayPrefix + symbol.Replace(Shard.Separator,string.Empty), typeof(BitmapImage), null, CultureInfo.InvariantCulture);
                if (source == null)
                {
                    int lastCharPos = endOfString ? end : end + Shard.Suffix.Length;

                    newList.Add(new Run((previousIsPicture ? " " : string.Empty) + text[pos..lastCharPos]));
                    text = text[lastCharPos..];
                }
                else
                {

                    Image image = new Image { Source = source, Width = 15.0 * source.PixelWidth / source.PixelWidth, Height = 15, Visibility = Visibility.Visible };
                    newList.Add(new InlineUIContainer(image));
                    if (text.Length > end)
                    {
                        text = text[(end + 1)..];
                    }
                    else
                    {
                        text = string.Empty;
                    }

                    previousIsPicture = true;
                }

                pos = text.IndexOf(Shard.Prefix, StringComparison.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(text))
            {
                newList.Add(new Run(" " + text));
            }

            return newList;
        }
    }
}
