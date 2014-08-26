namespace MagicPictureSetDownloader.Core.CardInfo
{
    using System.Collections.Generic;

    internal class SkipWorker : ICardInfoParserWorker
    {
        public bool WorkOnCurrentAtStart
        {
            get { return false; }
        }
        public IDictionary<string, string> WorkOnElement(IAwareXmlTextReader xmlReader)
        {
            //Do nothing only skip elements
            return new Dictionary<string, string>();
        }
    }
}