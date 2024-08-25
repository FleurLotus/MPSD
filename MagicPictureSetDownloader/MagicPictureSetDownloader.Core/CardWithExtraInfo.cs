namespace MagicPictureSetDownloader.Core
{
    using System.Collections.Generic;

    using MagicPictureSetDownloader.Interface;

    internal class CardWithExtraInfo
    {
        public string IdScryFall { get; set; }
        public string Name { get; set; }
        public string Edition { get; set; }
        public string Layout { get; set; }
        public string Rarity { get; set; }
        public List<(CardIdSource, string)> ExternalId { get; } = new List<(CardIdSource, string)>();
        public IList<CardFaceWithExtraInfo> CardFaceWithExtraInfos { get;  } = new List<CardFaceWithExtraInfo>();
        public string Language { get; set; }
        public string PrinterName { get; set; }
    }
}
