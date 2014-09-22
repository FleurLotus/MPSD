
namespace MagicPictureSetDownloader.Interface
{
    using System.Collections.Generic;

    public interface IImportExportFormatter
    {
        bool IsMatchingPattern(string line);
        ExportFormat Format { get; }
        string Extension { get; }
        IImportExportCardCount[] Parse(string input);
        string ToFile(IEnumerable<IImportExportCardCount> cardCount);
    }
}
