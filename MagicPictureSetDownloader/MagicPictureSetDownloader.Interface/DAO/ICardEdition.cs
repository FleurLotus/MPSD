namespace MagicPictureSetDownloader.Interface
{
    using System.Collections.Generic;

    public interface ICardEdition
    {
        int IdEdition { get; }
        int IdCard { get; }
        int IdRarity { get; }
        string IdScryFall { get; }
        string Url { get; }
        string Url2 { get; }
        string FlavorName { get; }
        string FrameEffect { get; }

        IReadOnlyDictionary<CardIdSource, IReadOnlyList<string>> ExternalId { get; }
    }
}