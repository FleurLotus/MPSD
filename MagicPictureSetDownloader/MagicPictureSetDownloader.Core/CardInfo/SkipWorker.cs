namespace MagicPictureSetDownloader.Core.CardInfo
{
    using System.Collections.Generic;

    internal class SkipWorker : ICardInfoParserWorker
    {
        public IDictionary<string, string> WorkOnElement(IAwareXmlTextReader xmlReader)
        {
            //Do nothing only skip elements
            return new Dictionary<string, string>();
        }
    }
}