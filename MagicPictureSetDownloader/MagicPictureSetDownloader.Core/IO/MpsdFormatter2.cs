 namespace MagicPictureSetDownloader.Core.IO
{
    using System.Linq;
    using System.Text.RegularExpressions;

    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.Db;

    internal class MpsdFormatter2 : FormatterBase
    {
        private readonly Regex _regLine = new Regex(@"^(?<IdScryFall>[0-9a-f\-]+)#(?<Count>\d+)#(?<FoilCount>\d+)(#(?<AltArtCount>\d+)#(?<FoilAltArtCount>\d+))?#(?<Language>\d+)?$", RegexOptions.Compiled);

        public MpsdFormatter2()
            : base(ExportFormat.MPSD2, ".mpsd2")
        {
        }

        protected override IImportExportCardCount ParseLine(string line)
        {
            Match m = _regLine.Match(line);
            if (!m.Success)
            {
                return new ErrorImportExportCardInfo(line, "Can't parse line");
            }
            string idScryFall = m.Groups["IdScryFall"].Value;
            if (MagicDatabase.GetCardByIdScryFall(idScryFall) == null)
            {
                return new ErrorImportExportCardInfo(line, "Invalid IdScryFall");
            }
            if (!int.TryParse(m.Groups["Count"].Value, out int count) || count < 0)
            {
                return new ErrorImportExportCardInfo(line, "Invalid Count");
            }
            if (!int.TryParse(m.Groups["FoilCount"].Value, out int foilCount) || foilCount < 0)
            {
                return new ErrorImportExportCardInfo(line, "Invalid FoilCount");
            }
            int altArtCount = 0;
            string sAltArtCount = m.Groups["AltArtCount"].Value;
            if (!string.IsNullOrWhiteSpace(sAltArtCount))
            {
                if (!int.TryParse(m.Groups["AltArtCount"].Value, out altArtCount) || altArtCount < 0)
                {
                    return new ErrorImportExportCardInfo(line, "Invalid AltArtCount");
                }
            }
            int foilAltArtCount = 0;
            string sFoilAltArtCount = m.Groups["FoilAltArtCount"].Value;
            if (!string.IsNullOrWhiteSpace(sFoilAltArtCount))
            {
                if (!int.TryParse(m.Groups["FoilAltArtCount"].Value, out foilAltArtCount) || foilAltArtCount < 0)
                {
                    return new ErrorImportExportCardInfo(line, "Invalid FoilAltArtCount");
                }
            }
            if (!int.TryParse(m.Groups["Language"].Value, out int idLanguage) || MagicDatabase.GetLanguages(idScryFall).All(l => l.Id != idLanguage))
            {
                return new ErrorImportExportCardInfo(line, "Invalid idLanguage");
            }

            CardCount cardCount = new CardCount
            {
                { CardCountKeys.Standard, count },
                { CardCountKeys.Foil, foilCount },
                { CardCountKeys.AltArt, altArtCount },
                { CardCountKeys.FoilAltArt, foilAltArtCount }
            };

            return new ImportExportCardInfo(idScryFall, cardCount, idLanguage);
        }
        protected override string ToLine(IImportExportCardCount cardCount)
        {
            if (cardCount == null || (cardCount.FoilNumber == 0 && cardCount.Number == 0 && cardCount.AltArtNumber == 0 && cardCount.FoilAltArtNumber == 0))
            {
                return null;
            }

            return string.Format("{0}#{1}#{2}#{3}#{4}#{5}\n", cardCount.IdScryFall, cardCount.Number, cardCount.FoilNumber, cardCount.AltArtNumber, cardCount.FoilAltArtNumber, cardCount.IdLanguage);
        }
        public override bool IsMatchingPattern(string line)
        {
            return _regLine.IsMatch(line);
        }
    }
}
