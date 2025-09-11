namespace MagicPictureSetDownloader.Core.Deck
{
    using System.Collections.Generic;
    using System.Linq;

    internal class DeckInfo
    {
        public DeckInfo(int? idEdition, string name, IEnumerable<DeckCardInfo> deckCards)
        {
            IdEdition = idEdition;
            Name = name;
            Cards = new List<DeckCardInfo>(deckCards).AsReadOnly();
        }

        public string Name { get; }
        public int? IdEdition { get; }
        public IList<DeckCardInfo> Cards { get; }
        public int Count
        {
            get
            {
                return Cards.Sum(c => c.Number);
            }
        }
    }
}