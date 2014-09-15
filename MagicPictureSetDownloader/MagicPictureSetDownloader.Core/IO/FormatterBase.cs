
namespace MagicPictureSetDownloader.Core.IO
{
    using System;
    using System.Linq;
    using System.Text;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    internal abstract class FormatterBase : IImportExportFormatter
    {
        protected readonly IMagicDatabaseReadOnly MagicDatabase;

        protected FormatterBase(ExportFormat format, string extension)
        {
            Format = format;
            Extension = extension;
            MagicDatabase = MagicDatabaseManager.ReadOnly;
        }
        public ExportFormat Format { get; private set; }
        public string Extension { get; private set; }

        protected abstract IImportExportCardCount ParseLine(string line);
        protected abstract string ToLine(IImportExportCardCount cardCount);
        public abstract bool IsMatchingPattern(string line);

        public IImportExportCardCount[] Parse(string input)
        {
            return input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(ParseLine).ToArray();
        }
        public string ToFile(IImportExportCardCount[] cardCount)
        {
            StringBuilder sb = new StringBuilder();
            foreach (IImportExportCardCount card in cardCount.Where(cic => cic != null))
            {
                string line = ToLine(card);
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (!line.EndsWith("\n"))
                        line += "\n";

                    sb.Append(line);
                }
            }
            return sb.ToString();
        }
    }
}
