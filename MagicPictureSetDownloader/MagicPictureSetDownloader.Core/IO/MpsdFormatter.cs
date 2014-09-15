
namespace MagicPictureSetDownloader.Core.IO
{
    using System.Text.RegularExpressions;

    using MagicPictureSetDownloader.Interface;

    internal class MpsdFormatter : FormatterBase
    {
        private readonly Regex _regLine = new Regex(@"^(?<IdGatherer>\d+)#(?<Count>\d+)#(?<FoilCount>\d+)$", RegexOptions.Compiled);

        public MpsdFormatter()
            : base(ExportFormat.MPSD, ".mpsd")
        {
        }
        protected override IImportExportCardCount ParseLine(string line)
        {
            Match m =  _regLine.Match(line);
            return m.Success ? new ImportExportCardInfo(int.Parse(m.Groups["IdGatherer"].Value), int.Parse(m.Groups["Count"].Value), int.Parse(m.Groups["FoilCount"].Value)) : null;
        }
        protected override string ToLine(IImportExportCardCount cardCount)
        {
            if (cardCount == null || (cardCount.FoilNumber == 0 && cardCount.Number == 0))
                return null;

            return string.Format("{0}#{1}#{2}\n", cardCount.IdGatherer, cardCount.Number, cardCount.FoilNumber);
        }
        public override bool IsMatchingPattern(string line)
        {
            return _regLine.IsMatch(line);
        }
    }
}
