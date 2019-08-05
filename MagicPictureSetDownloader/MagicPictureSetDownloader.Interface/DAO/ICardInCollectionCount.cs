namespace MagicPictureSetDownloader.Interface
{
    public interface ICardInCollectionCount : IImportExportCardCount
    {
        int IdCollection { get; }
    }
}