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

            return classValue.ToLowerInvariant() switch
            {
                "communityratings" => new SkipWorker(),
                "row" or "row manarow" => new RowWorker(xmlReader),
                "variations" => new VariationsWorker(),
                _ => null,
            };
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

            string infoType = lowerClassValue[(lowerClassValue.LastIndexOf('_') + 1)..];
            //ctl00_ctl00_ctl00_maincontent_subcontent_subcontent_* for normalcard
            //ctl00_ctl00_ctl00_maincontent_subcontent_subcontent_ctl03_* for part A off multi part card
            //ctl00_ctl00_ctl00_maincontent_subcontent_subcontent_ctl04_* for part B off multi part card

            return infoType switch
            {
                "namerow" => new SimpleValueRowWorker(CardParserBase.NameKey),
                "manarow" => new ManaRowWorker(),
                "cmcrow" => new SimpleValueRowWorker(CardParserBase.CmcKey),
                "ptrow" => new SimpleValueRowWorker(CardParserBase.PTKey),
                "typerow" => new SimpleValueRowWorker(CardParserBase.TypeKey),
                "rarityrow" => new SimpleValueRowWorker(CardParserBase.RarityKey),
                "textrow" => new TextRowWorker(),
                //Not used
                "setrow" or "numberrow" or "artistrow" or "flavorRow" => null,
                _ => null,
            };
        }
    }
}
