namespace MagicPictureSetDownloader.Core.CardInfo
{
    using System.Collections.Generic;

    internal interface ICardInfoParserWorker
    {
        IDictionary<string, string> WorkOnElement(IAwareXmlTextReader awareXmlReader);
    }
}
