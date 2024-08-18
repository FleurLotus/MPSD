namespace MagicPictureSetDownloader.Interface
{
    public interface IImportExportCardCount 
    {
        string IdScryFall { get; }
        int Number { get; }
        int FoilNumber { get; }
        int AltArtNumber { get; }
        int FoilAltArtNumber { get; }
        int IdLanguage { get; }

        int GetCount(ICardCountKey key);
        ICardCount GetCardCount();
    }
}