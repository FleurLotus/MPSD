namespace MagicPictureSetDownloader.Interface
{
    public interface ICardInCollectionCount 
    {
        int IdCollection { get; }
        int IdGatherer { get; }
        int Number { get; }
        int FoilNumber { get; }
    }
}