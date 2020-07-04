namespace MagicPictureSetDownloader.Core.CardInfo
{
    using System;

    internal class CardInfoParserWorkerFactory
    {
        private static readonly Lazy<CardInfoParserWorkerFactory> _lazy = new Lazy<CardInfoParserWorkerFactory>(() => new CardInfoParserWorkerFactory());

        private CardInfoParserWorkerFactory()
        {
        }

        public static CardInfoParserWorkerFactory Instance
        {
            get { return _lazy.Value; }
        }

        public ICardInfoParserWorker CreateParserWorker(IAwareXmlTextReader xmlReader)
        {
            string classValue = xmlReader.GetAttribute("class");
            if (classValue == null || classValue.ToLowerInvariant() == "planeimage" || classValue.ToLowerInvariant() == "fadedcard")
            {
                return ImageWorker.IsWorkingInfo(xmlReader.GetAttribute("id")) ? new ImageWorker() : null;
            }

            switch (classValue.ToLowerInvariant())
            {
                case "communityratings":
                    return new SkipWorker();

                case "row":
                case "row manarow":
                    return new RowWorker(xmlReader);

                case "variations":
                    return new VariationsWorker(xmlReader);

                default:
                    return null;
            }
        }
        public ICardInfoParserWorker CreateParserRowSubWorker(IAwareXmlTextReader xmlReader)
        {
            string classValue = xmlReader.GetAttribute("id");
            if (classValue == null)
            {
                return null;
            }

            string lowerClassValue = classValue.ToLowerInvariant();
            if (!lowerClassValue.StartsWith("ctl00_ctl00_ctl00_maincontent_subcontent_subcontent"))
            {
                return null;
            }

            string infoType = lowerClassValue.Substring(lowerClassValue.LastIndexOf('_') + 1);
            //ctl00_ctl00_ctl00_maincontent_subcontent_subcontent_* for normalcard
            //ctl00_ctl00_ctl00_maincontent_subcontent_subcontent_ctl03_* for part A off multi part card
            //ctl00_ctl00_ctl00_maincontent_subcontent_subcontent_ctl04_* for part B off multi part card

            switch (infoType)
            {
                case "namerow":
                    return new SimpleValueRowWorker(CardParserBase.NameKey);
                case "manarow":
                    return new ManaRowWorker();
                case "cmcrow":
                    return new SimpleValueRowWorker(CardParserBase.CmcKey);
                case "ptrow":
                    return new SimpleValueRowWorker(CardParserBase.PTKey);
                case "typerow":
                    return new SimpleValueRowWorker(CardParserBase.TypeKey);
                case "rarityrow":
                    return new SimpleValueRowWorker(CardParserBase.RarityKey);
                case "textrow":
                    return new TextRowWorker();

                //Not used
                case "setrow":
                case "numberrow":
                case "artistrow":
                case "flavorRow":
                    return null;
                default:
                    return null;
            }
        }
    }
}
