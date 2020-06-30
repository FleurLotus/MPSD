namespace MagicPictureSetDownloader.Interface
{
    public interface IImportExportCardCount 
    {
        int IdGatherer { get; }
        int Number { get; }
        int FoilNumber { get; }
        int AltArtNumber { get; }
        int FoilAltArtNumber { get; }
        int IdLanguage { get; }

        int GetCount(ICardCountKey key);
        ICardCount GetCardCount();
    }
}