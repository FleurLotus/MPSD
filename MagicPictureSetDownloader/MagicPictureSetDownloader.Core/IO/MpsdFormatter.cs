 namespace MagicPictureSetDownloader.Core.IO
{
    using System.Linq;
    using System.Text.RegularExpressions;

    using MagicPictureSetDownloader.Interface;

    internal class MpsdFormatter : FormatterBase
    {
        private readonly Regex _regLine = new Regex(@"^(?<IdGatherer>\d+)#(?<Count>\d+)#(?<FoilCount>\d+)#(?<Language>\d+)?$", RegexOptions.Compiled);

        public MpsdFormatter()
            : base(ExportFormat.MPSD, ".mpsd")
        {
        }

        protected override IImportExportCardCount ParseLine(string line)
        {
            Match m = _regLine.Match(line);
            if (!m.Success)
            {
                return new ErrorImportExportCardInfo(line, "Can't parse line");
            }
            int idGatherer;
            if (!int.TryParse(m.Groups["IdGatherer"].Value, out idGatherer) || MagicDatabase.GetCard(idGatherer) == null)
            {
                return new ErrorImportExportCardInfo(line, "Invalid IdGatherer");
            }
            int count;
            if (!int.TryParse(m.Groups["Count"].Value, out count) || count < 0)
            {
                return new ErrorImportExportCardInfo(line, "Invalid Count");
            }
            int foilCount;
            if (!int.TryParse(m.Groups["FoilCount"].Value, out foilCount) || foilCount < 0)
            {
                return new ErrorImportExportCardInfo(line, "Invalid FoilCount");
            }
            int idLanguage;
            if (!int.TryParse(m.Groups["Language"].Value, out idLanguage) || MagicDatabase.GetLanguages(idGatherer).All(l => l.Id != idLanguage))
            {
                return new ErrorImportExportCardInfo(line, "Invalid idLanguage");
            }

            return new ImportExportCardInfo(idGatherer, count, foilCount, idLanguage);
        }
        protected override string ToLine(IImportExportCardCount cardCount)
        {
            if (cardCount == null || (cardCount.FoilNumber == 0 && cardCount.Number == 0))
                return null;

            return string.Format("{0}#{1}#{2}#{3}\n", cardCount.IdGatherer, cardCount.Number, cardCount.FoilNumber, cardCount.IdLanguage);
        }
        public override bool IsMatchingPattern(string line)
        {
            return _regLine.IsMatch(line);
        }
    }
}
