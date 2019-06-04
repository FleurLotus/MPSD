namespace MagicPictureSetDownloader.Core.Deck
{
    internal class DeckCardInfo
    {
        public DeckCardInfo(int idGatherer, int number)
        {
            IdGatherer = idGatherer;
            Number = number;
        }

        public int IdGatherer { get; }
        public int Number { get; }
    }
}