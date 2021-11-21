namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Linq;
    using System.Globalization;

    using Common.WPF.Converter;

    using MagicPictureSetDownloader.ViewModel.Main;

    public class CardToStatisticsConverter : NoConvertBackMultiConverter
    {
        public override object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.Length != 3)
            {
                return null;
            }

            string name = value[2] as string;
            if (value[0] is not HierarchicalResultNodeViewModel node || value[1] is not bool || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            StatisticViewModel[] statistics = node.Card.Statistics;
            if (!((bool)value[1]))
            {
                return statistics;
            }
            return statistics.Where(s => s.Collection == name).ToArray();
        }
    }
}
