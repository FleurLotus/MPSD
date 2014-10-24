
namespace MagicPictureSetDownloader.Core.IO
{
    using System;
    using System.Text.RegularExpressions;

    using MagicPictureSetDownloader.Interface;

    internal class MtgmFormatter : FormatterBase
    {
        private readonly Regex _regLine = new Regex(@"^(?<Name>.+)#(?<Edition>[A-Z]{2,3})#(?<Count>\d+)#(?<Foil>(?i)true|false(?-i))$", RegexOptions.Compiled);
        
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

            Tuple<ICard, IEdition> cardxedition = MagicDatabase.GetCardxEdition(cardCount.IdGatherer);
            if (cardxedition == null || cardxedition.Item1 == null || cardxedition.Item2 == null)
                return null;
            
            string ret = string.Empty;
            if (cardCount.Number>0)
                ret += string.Format("{0}#{1}#{2}#False\n", cardxedition.Item1.Name,cardxedition.Item2.AlternativeCode(Format), cardCount.Number);
            if (cardCount.FoilNumber > 0)
                ret += string.Format("{0}#{1}#{2}#True\n", cardxedition.Item1.Name, cardxedition.Item2.AlternativeCode(Format), cardCount.FoilNumber);

            return ret;
        }
        public override bool IsMatchingPattern(string line)
        {
            return _regLine.IsMatch(line);
        }
    }
}
