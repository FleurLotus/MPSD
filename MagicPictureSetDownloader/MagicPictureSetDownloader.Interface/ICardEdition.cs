namespace MagicPictureSetDownloader.Interface
{
    public interface ICardEdition
    {
        int IdEdition { get; }
        int IdCard { get; }
        int IdRarity { get; }
        int IdGatherer { get; }
    }
}