namespace MagicPictureSetDownloader.Core.Deck
{
    internal class DeckCardInfo
    {
        public DeckCardInfo(int idGatherer, int number)
        {
            IdGatherer = idGatherer;
            Number = number;
            NeedToCreate = false;
        }
        public DeckCardInfo(int idEdition, int idCard, int number, int idRarity, string pictureUrl) 
        {
            NeedToCreate = true;
            IdEdition = idEdition;
            IdCard = idCard;
            Number = number;
            IdRarity = idRarity;
            PictureUrl = pictureUrl;
        }

        public bool NeedToCreate { get; }
        public int IdGatherer { get; }
        public int Number { get; }
        public int IdEdition { get; }
        public int IdCard { get; }
        public int IdRarity { get; }
        public string PictureUrl { get; }
    }
}