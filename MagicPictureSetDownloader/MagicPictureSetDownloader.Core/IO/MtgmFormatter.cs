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
            Match m = _regLine.Match(line);
            if (!m.Success)
            {
                return new ErrorImportExportCardInfo(line, "Can't parse line");
            }

            int count = 0;
            int foilCount = 0;
            int tmpcount;

            if (!int.TryParse(m.Groups["Count"].Value, out tmpcount) || tmpcount <= 0)
            {
                return new ErrorImportExportCardInfo(line, "Invalid value for count");
            }
            
            if (m.Groups["Foil"].Value.ToUpper() == "TRUE")
            {
                foilCount = tmpcount;
            }
            else
            {
                count = tmpcount;
            }


            ICard card = MagicDatabase.GetCard(m.Groups["Name"].Value, null);
            if (card == null)
            {
                return new ErrorImportExportCardInfo(line, string.Format("Can't find card named {0}", m.Groups["Name"].Value));
            }

            IEdition edition = MagicDatabase.GetEditionFromCode(m.Groups["Edition"].Value);
            if (edition == null)
            {
                return new ErrorImportExportCardInfo(line, string.Format("Can't find edition named {0}", m.Groups["Edition"].Value));
            }

            int idGatherer = MagicDatabase.GetIdGatherer(card, edition);
            if (idGatherer == 0)
            {
                return new ErrorImportExportCardInfo(line, string.Format("Can't find gatherer id for card {0} edition {1}", card, edition));
            }

            return new ImportExportCardInfo(idGatherer, count, foilCount, 0, 0, 0);
        }
        protected override string ToLine(IImportExportCardCount cardCount)
        {
            //Ignore AltArt
            if (cardCount == null || (cardCount.FoilNumber == 0 && cardCount.Number == 0))
            {
                return null;
            }

            ICard card = MagicDatabase.GetCard(cardCount.IdGatherer);
            IEdition edition = MagicDatabase.GetEdition(cardCount.IdGatherer);
            if (card == null || edition == null)
            {
                throw new ImportExportException("Can't find card with IdGatherer={0}", cardCount.IdGatherer);
            }

            string ret = string.Empty;
            if (cardCount.Number > 0)
            {
                ret += string.Format("{0}#{1}#{2}#False\n", card, edition.AlternativeCode(Format), cardCount.Number);
            }

            if (cardCount.FoilNumber > 0)
            {
                ret += string.Format("{0}#{1}#{2}#True\n", card, edition.AlternativeCode(Format), cardCount.FoilNumber);
            }

            return ret;
        }
        public override bool IsMatchingPattern(string line)
        {
            return _regLine.IsMatch(line);
        }
    }
}
