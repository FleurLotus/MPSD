namespace MagicPictureSetDownloader.Core.CardInfo
{
    using System.Collections.Generic;

    internal interface ICardInfoParserWorker
    {
        bool WorkOnCurrentAtStart { get; }
        IDictionary<string, string> WorkOnElement(IAwareXmlTextReader awareXmlReader);
    }
}
