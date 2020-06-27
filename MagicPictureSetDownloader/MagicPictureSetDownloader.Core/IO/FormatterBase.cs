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
        public ExportFormat Format { get; }
        public string Extension { get; }

        protected abstract IImportExportCardCount ParseLine(string line);
        protected abstract string ToLine(IImportExportCardCount cardCount);
        public abstract bool IsMatchingPattern(string line);

        public IEnumerable<IImportExportCardCount> Parse(string input)
        {
            IDictionary<string, ImportExportCardInfo> ret = new Dictionary<string, ImportExportCardInfo>();
            IEnumerable<IImportExportCardCount> enumerable = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                                                  .Select(ParseLine);

            //Merge if multiple lines from file like in mtgm format
            foreach (IImportExportCardCount importExportCardCount in enumerable)
            {
                ImportExportCardInfo cardInfo;
                string key = string.Format("{0}¤{1}", importExportCardCount.IdGatherer, importExportCardCount.IdLanguage);
                if (ret.TryGetValue(key, out cardInfo))
                {
                    if (importExportCardCount.Number > 0)
                    {
                        cardInfo.Add(importExportCardCount.Number);
                    }
                    if (importExportCardCount.FoilNumber > 0)
                    {
                        cardInfo.AddFoil(importExportCardCount.FoilNumber);
                    }
                    if (importExportCardCount.AltArtNumber > 0)
                    {
                        cardInfo.AddAltArt(importExportCardCount.AltArtNumber);
                    }
                    if (importExportCardCount.FoilAltArtNumber > 0)
                    {
                        cardInfo.AddFoilAltArt(importExportCardCount.FoilAltArtNumber);
                    }
                }
                else
                {
                    cardInfo = importExportCardCount as ImportExportCardInfo;
                    if (null == cardInfo)
                    {
                        //First the error because no id to put in dictionary
                        yield return importExportCardCount;
                        continue;
                    }

                    ret.Add(key, cardInfo);
                }
            }

            //them the merged values
            foreach (var value in ret.Values)
            {
                yield return value;
            }
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
                    {
                        line += "\n";
                    }

                    sb.Append(line);
                }
            }
            return sb.ToString();
        }
    }
}
