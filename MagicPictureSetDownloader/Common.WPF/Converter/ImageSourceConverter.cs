namespace Common.WPF.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class ImageSourceConverter : NoConvertBackConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(ImageSource))
            {
                string str = value as string;
                if (str != null)
                    return new BitmapImage(new Uri(str, UriKind.RelativeOrAbsolute));

                Uri uri = value as Uri;
                if (uri != null)
                    return new BitmapImage(uri);
            }
            return value;
        }
    }
}
