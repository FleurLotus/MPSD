namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;

    using MagicPictureSetDownloader.Interface;

    public class PriceViewModel
    {
        public PriceViewModel(IPrice price, IEdition edition)
        {
            AddDate = price.AddDate;
            Source = price.Source;
            Foil = price.Foil;
            Value = price.Value;
            EditionName = edition.Name;
        }

        public DateTime AddDate { get; }
        public string Source { get;  }
        public bool Foil { get; }
        public int Value { get; }
        public string EditionName { get; }
    }
}