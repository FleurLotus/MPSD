namespace MagicPictureSetDownloader.Core
{
    using MagicPictureSetDownloader.Interface;
    using System.Collections.Generic;

    internal class CardWithExtraInfo
    {
        public string IdScryFall { get; set; }
        public string Name { get; set; }
        public string Edition { get; set; }
        public string Layout { get; set; }
        public string Rarity { get; set; }
        public List<(CardIdSource, string)> ExternalId { get; } = new List<(CardIdSource, string)>();
        public IList<CardFaceWithExtraInfo> CardFaceWithExtraInfos { get;  } = new List<CardFaceWithExtraInfo>();
    }
}
