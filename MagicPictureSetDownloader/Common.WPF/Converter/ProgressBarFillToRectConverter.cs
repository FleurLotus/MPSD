using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Common.WPF.Converter
{
    public class ProgressBarFillToRectConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (null != values && values.Length >= 4 && 
                null != values[0] && null != values[1] && null != values[2] && null != values[3] &&
                values[0] is double && values[1] is double && values[2] is double && values[3] is double && 
                0 != (double)values[1])
            {
                double fillPercentage = (double)values[0] / (double)values[1];
                double width = (double)values[2];
                double height = (double)values[3];
                return new Rect(0, 0, fillPercentage * width, height);
                // ProgressBarFillWidth is calculated by multiplying Fill  percentage with actual width
            }
            return new Rect(0, 0, 0, 0); // Default Zero size rectangle
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
