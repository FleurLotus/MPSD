using MagicPictureSetDownloader.Core.Db;

namespace MagicPictureSetDownloader.Core
{
    public class SetInfoWithBlock
    {
        private readonly SetInfo _setInfo;
        private readonly Edition _edition;

        internal SetInfoWithBlock(SetInfo setInfo, Edition edition)
        {
            _setInfo = setInfo;
            _edition = edition;
        }

        public string BaseSearchUrl
        {
            get { return _setInfo.BaseSearchUrl; }
        }
        public string BlockName
        {
            get { return _edition.BlockName; }
        }
        public string Name
        {
            get { return _edition.Name; }
        }

    }
}
