namespace MagicPictureSetDownloader.Core
{
    using MagicPictureSetDownloader.Interface;

    public class SetInfoWithBlock
    {
        internal SetInfoWithBlock(SetInfo setInfo, IEdition edition)
        {
            BaseSearchUrl = setInfo.BaseSearchUrl;
            Edition = edition;
        }

        public IEdition Edition { get; private set; }
        public string BaseSearchUrl { get; private set; }
    }
}
