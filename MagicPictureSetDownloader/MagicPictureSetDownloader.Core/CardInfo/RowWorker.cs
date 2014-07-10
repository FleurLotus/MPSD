using System.Collections.Generic;

namespace MagicPictureSetDownloader.Core.CardInfo
{
    internal class RowWorker : ICardInfoParserWorker
    {
        private readonly ICardInfoParserWorker _innerWorker;

        public RowWorker(IAwareXmlTextReader xmlReader)
        {
            _innerWorker = CardInfoParserWorkerFactory.CreateParserRowSubWorker(xmlReader);
        }
        public IDictionary<string, string> WorkOnElement(IAwareXmlTextReader xmlReader)
        {
            if (_innerWorker == null) 
                return new Dictionary<string, string>();

            return _innerWorker.WorkOnElement(new AwareXmlTextReader(xmlReader));
        }
    }
}