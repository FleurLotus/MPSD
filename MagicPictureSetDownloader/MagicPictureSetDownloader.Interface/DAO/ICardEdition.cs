namespace MagicPictureSetDownloader.Interface
{
    using System.Collections.Generic;

    public interface ICardEdition
    {
        int IdEdition { get; }
        int IdCard { get; }
        int IdRarity { get; }
        string IdScryFall { get; }

        IReadOnlyDictionary<CardIdSource, IReadOnlyList<string>> ExternalId { get; }
    }
}