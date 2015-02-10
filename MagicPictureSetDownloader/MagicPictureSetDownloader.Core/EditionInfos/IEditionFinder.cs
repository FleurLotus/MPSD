
namespace MagicPictureSetDownloader.Core.EditionInfos
{
    internal interface IEditionFinder
    {
        EditionIconInfo Find(string url, string wantedEdition);
    }
}
