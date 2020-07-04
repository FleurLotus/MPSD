namespace MagicPictureSetDownloader.Interface
{
    public interface ICardEditionVariation
    {
        int IdGatherer { get; }
        int OtherIdGatherer { get; }
        string Url { get; }
    }
}