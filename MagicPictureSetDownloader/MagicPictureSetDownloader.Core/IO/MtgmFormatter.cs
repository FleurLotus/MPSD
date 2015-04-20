namespace MagicPictureSetDownloader.Core.IO
{
    using System.Text.RegularExpressions;

    using MagicPictureSetDownloader.Interface;

    internal class MtgmFormatter : FormatterBase
    {
        private readonly Regex _regLine = new Regex(@"^(?<Name>.+)#(?<Edition>[A-Z0-9]{2,3})#(?<Count>\d+)#(?<Foil>(?i)true|false(?-i))(?<Reserve>#(?:(?i)true|false(?-i)))?$", RegexOptions.Compiled);
        
        public MtgmFormatter()
            : base(ExportFormat.MTGM, ".dk2")
        {
        }
        protected override IImportExportCardCount ParseLine(string line)
        {
            Match m =  _regLine.Match(line);
            if (!m.Success)
                return null;

            int count = 0;
            int foilCount = 0;
            if (m.Groups["Foil"].Value.ToUpper() == "TRUE")
            {
                foilCount = int.Parse(m.Groups["Count"].Value);
            }
            else
            {
                count = int.Parse(m.Groups["Count"].Value);
            }

            ICard card = MagicDatabase.GetCard(m.Groups["Name"].Value, null);
            if (card == null)
                return null;

            IEdition edition = MagicDatabase.GetEditionFromCode(m.Groups["Edition"].Value);
            if (edition == null)
                return null;

            int idGatherer = MagicDatabase.GetIdGatherer(card, edition);
            if (idGatherer <= 0)
                return null;

            return new ImportExportCardInfo(idGatherer, count, foilCount);
        }
        protected override string ToLine(IImportExportCardCount cardCount)
        {
            if (cardCount == null || (cardCount.FoilNumber == 0 && cardCount.Number == 0))
                return null;


            ICard card = MagicDatabase.GetCard(cardCount.IdGatherer);
            IEdition edition = MagicDatabase.GetEdition(cardCount.IdGatherer);
            if (card == null || edition == null)
                return null;

            string ret = string.Empty;
            if (cardCount.Number > 0)
                ret += string.Format("{0}#{1}#{2}#False\n", card, edition.AlternativeCode(Format), cardCount.Number);
            if (cardCount.FoilNumber > 0)
                ret += string.Format("{0}#{1}#{2}#True\n", card, edition.AlternativeCode(Format), cardCount.FoilNumber);

            return ret;
        }
        public override bool IsMatchingPattern(string line)
        {
            return _regLine.IsMatch(line);
        }
    }
}
