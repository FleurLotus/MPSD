namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using Common.WPF.Converter;

    [ValueConversion(typeof(int), typeof(Size))]
    public class ToSize : NoConvertBackConverter
    {
        //This classs is used to avoid the virtualizingWrapPanel to recompute the size of the child every time
        // it is not an exact formula but it is doing the job
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            double imageWidth = System.Convert.ToDouble(value);
            double imageHeight = imageWidth * 88 / 63;
            // add border size
            double width = imageWidth + 12;
            // add label size and border size 
            double height = imageHeight + 23 + 12;

            return new Size(width, height);
        }
    }
}
