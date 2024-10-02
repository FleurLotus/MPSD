namespace MagicPictureSetDownloader.Interface
{
    public interface IPreconstructedDeck : IIdName
    {
        string Url { get; }
        int? IdEdition { get; }
    }
}