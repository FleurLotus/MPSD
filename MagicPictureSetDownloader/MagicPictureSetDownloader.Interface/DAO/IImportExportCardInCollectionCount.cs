namespace MagicPictureSetDownloader.Interface
{
    public interface IImportExportCardCount 
    {
        int IdGatherer { get; }
        int Number { get; }
        int FoilNumber { get; }
        int IdLanguage { get; }
    }
}