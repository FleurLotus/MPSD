using System.Collections.Generic;

namespace MagicPictureSetDownloader.Core.CardInfo
{
    internal class SkipWorker : ICardInfoParserWorker
    {
        public IDictionary<string, string> WorkOnElement(IAwareXmlTextReader xmlReader)
        {
            //Do nothing only skip elements
            return new Dictionary<string, string>();
        }
    }
}