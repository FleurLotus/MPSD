
namespace MagicPictureSetDownloader.Core.IO
{
    using System;
    using System.Collections.Generic;
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
            IDictionary<int, ImportExportCardInfo> ret = new Dictionary<int, ImportExportCardInfo>();
            IEnumerable<IImportExportCardCount> enumerable = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(ParseLine);

            //Merge if multiple lines from file like in mtgm format
            foreach (IImportExportCardCount importExportCardCount in enumerable)
            {
                ImportExportCardInfo cardInfo;
                if (ret.TryGetValue(importExportCardCount.IdGatherer, out cardInfo))
                {
                    if (importExportCardCount.FoilNumber > 0)
                        cardInfo.AddFoil(importExportCardCount.FoilNumber);

                    if (importExportCardCount.Number > 0)
                        cardInfo.Add(importExportCardCount.Number);
                }
                else
                {
                    cardInfo = importExportCardCount as ImportExportCardInfo;
                    if (null == cardInfo)
                        continue;

                    ret.Add(cardInfo.IdGatherer, cardInfo);
                }
            }

            return ret.Values.Cast<IImportExportCardCount>().ToArray();

        }
        public string ToFile(IEnumerable<IImportExportCardCount> cardCount)
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
