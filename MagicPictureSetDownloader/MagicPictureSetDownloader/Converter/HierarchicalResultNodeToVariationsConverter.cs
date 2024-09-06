namespace MagicPictureSetDownloader.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Collections.Generic;
    using System.Linq;

    using MagicPictureSetDownloader.ViewModel.Main;

    using Common.WPF.Converter;


    [ValueConversion(typeof(HierarchicalResultNodeViewModel), typeof(IList<CardViewModel>))]
    public class HierarchicalResultNodeToVariationsConverter : NoConvertBackConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not HierarchicalResultNodeViewModel node)
            {
                return Array.Empty<string>();
            }

            HashSet<CardViewModel> ret = new HashSet<CardViewModel>();
            

            foreach (CardViewModel card in node.AllCard)
            {
                ret.Add(card);
            }

            return ret.ToArray();
        }
    }
}
