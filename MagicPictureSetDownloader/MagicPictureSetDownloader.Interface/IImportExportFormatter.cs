
namespace MagicPictureSetDownloader.Interface
{
    public interface IImportExportFormatter
    {
        bool IsMatchingPattern(string line);
        ExportFormat Format { get; }
        string Extension { get; }
        IImportExportCardCount[] Parse(string input);
        string ToFile(IImportExportCardCount[] cardCount);
    }
}
