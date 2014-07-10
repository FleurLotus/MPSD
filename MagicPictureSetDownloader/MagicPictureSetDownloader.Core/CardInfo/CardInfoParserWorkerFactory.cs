namespace MagicPictureSetDownloader.Core.CardInfo
{
    internal static class CardInfoParserWorkerFactory
    {
        public static ICardInfoParserWorker CreateParserWorker(IAwareXmlTextReader xmlReader)
        {
            string classValue = xmlReader.GetAttribute("class");
            if (classValue == null)
                return null;

            switch (classValue.ToLowerInvariant())
            {
                case "cardimage":
                    return new ImageWorker();

                case "communityratings":
                case "variations":
                    return new SkipWorker();

                case "row":
                    return new RowWorker(xmlReader);

                default:
                    return null;
            }
        }
        public static ICardInfoParserWorker CreateParserRowSubWorker(IAwareXmlTextReader xmlReader)
        {
            string classValue = xmlReader.GetAttribute("id");
            if (classValue == null)
                return null;

            switch (classValue.ToLowerInvariant())
            {
                case "ctl00_ctl00_ctl00_maincontent_subcontent_subcontent_namerow":
                    return new SimpleValueRowWorker(CardParser.NameKey);
                case "ctl00_ctl00_ctl00_maincontent_subcontent_subcontent_manarow":
                    return new ManaRowWorker();
                case "ctl00_ctl00_ctl00_maincontent_subcontent_subcontent_cmcrow":
                    return new SimpleValueRowWorker(CardParser.CmcKey);
                case "ctl00_ctl00_ctl00_maincontent_subcontent_subcontent_ptrow":
                    return new SimpleValueRowWorker(CardParser.PTKey);
                case "ctl00_ctl00_ctl00_maincontent_subcontent_subcontent_typerow":
                    return new SimpleValueRowWorker(CardParser.TypeKey);
                case "ctl00_ctl00_ctl00_maincontent_subcontent_subcontent_rarityrow":
                    return new SimpleValueRowWorker(CardParser.RarityKey);
                case "ctl00_ctl00_ctl00_maincontent_subcontent_subcontent_textrow":
                    return new TextRowWorker();

                //Not used
                case "ctl00_ctl00_ctl00_maincontent_subcontent_subcontent_setrow":
                case "ctl00_ctl00_ctl00_maincontent_subcontent_subcontent_numberrow":
                case "ctl00_ctl00_ctl00_maincontent_subcontent_subcontent_artistrow":
                    return null;
                default:
                    return null;
            }
        }
    }
}
