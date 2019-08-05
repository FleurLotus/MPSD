namespace MagicPictureSetDownloader.Interface
{
    public interface IPreconstructedDeckCardEdition
    {
        int IdPreconstructedDeck { get; }
        int IdGatherer { get; }
        int Number { get; }
    }
}