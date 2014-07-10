using System.Collections.Generic;

namespace MagicPictureSetDownloader.Core.CardInfo
{
    internal interface ICardInfoParserWorker
    {
        IDictionary<string, string> WorkOnElement(IAwareXmlTextReader awareXmlReader);
    }
}
