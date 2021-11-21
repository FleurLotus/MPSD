namespace Common.WPF.Converter
{
    using System;
    using System.Globalization;
    using System.Windows;

    public class ProgressBarFillToRectConverter : NoConvertBackMultiConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (null != values && values.Length >= 4 && 
                null != values[0] && null != values[1] && null != values[2] && null != values[3] &&
                values[0] is double d0 && values[1] is double d1 && values[2] is double d2 && values[3] is double d3 && 
                0 != d1)
            {
                double fillPercentage = d0 / d1;
                double width = d2;
                double height = d3;
                return new Rect(0, 0, fillPercentage * width, height);
                // ProgressBarFillWidth is calculated by multiplying Fill  percentage with actual width
            }
            return new Rect(0, 0, 0, 0); // Default Zero size rectangle
        }
    }
}
