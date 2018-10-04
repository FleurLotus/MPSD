namespace MagicPictureSetDownloader.Core
{
    using MagicPictureSetDownloader.Interface;

    public class EditionInfoWithBlock
    {
        internal EditionInfoWithBlock(EditionInfo setInfo, IEdition edition)
        {
            BaseSearchUrl = setInfo.BaseSearchUrl;
            Edition = edition;
        }

        public IEdition Edition { get; }
        public string BaseSearchUrl { get; }
    }
}
