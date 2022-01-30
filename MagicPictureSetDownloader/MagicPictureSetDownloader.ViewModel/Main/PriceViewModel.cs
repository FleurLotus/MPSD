namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;

    using MagicPictureSetDownloader.Interface;

    public class PriceViewModel
    {
        public PriceViewModel(IPrice price, IEdition edition)
        {
            AddDate = price.AddDate;
            if (Enum.TryParse(price.Source, out PriceValueSource source))
            {
                Source = source;
            }
            else
            {
                Source = PriceValueSource.Unknown;
            }
            Foil = price.Foil;
            Value = price.Value;
            EditionName = edition.Name;
        }

        public DateTime AddDate { get; }
        public PriceValueSource Source { get;  }
        public bool Foil { get; }
        public int Value { get; }
        public string EditionName { get; }
    }
}