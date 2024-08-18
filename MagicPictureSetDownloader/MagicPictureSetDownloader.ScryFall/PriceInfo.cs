namespace MagicPictureSetDownloader.ScryFall
{
    using System;

    using MagicPictureSetDownloader.Interface;

    public class PriceInfo
    {
        public PriceValueSource PriceSource { get; set; }
        public string IdScryFall { get; set; }
        public bool Foil { get; set; }
        public int Value { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
