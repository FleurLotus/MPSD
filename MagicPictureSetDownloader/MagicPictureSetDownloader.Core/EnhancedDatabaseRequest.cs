
namespace MagicPictureSetDownloader.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using MagicPictureSetDownloader.Interface;

    public static class EnhancedDatabaseRequest
    {
        public static IEdition[] GetAllEditionsOrdered(this IMagicDatabaseReadOnly magicDatabase)
        {
            return magicDatabase.AllEditions().Ordered()
                                              .ToArray();
        }
        public static IEnumerable<ICard> GetAllCardOrdered(this IEnumerable<ICardAllDbInfo> allCardInfos, IEdition edition = null)
        {
            return allCardInfos.Where(cadi => edition == null || cadi.Edition == edition)
                               .Select(cadi => cadi.Card)
                               .Distinct()
                               .Ordered();
        }
        public static IEnumerable<IEdition> GetAllEditionIncludingCardOrdered(this IMagicDatabaseReadOnly magicDatabase, ICard card)
        {
            return magicDatabase.GetAllInfos().GetAllEditionIncludingCardOrdered(card);
        }
        public static IEnumerable<IEdition> GetAllEditionIncludingCardOrdered(this IEnumerable<ICardAllDbInfo> allCardInfos, ICard card)
        {
            return allCardInfos.Where(cadi => cadi.Card == card)
                               .Select(cadi => cadi.Edition)
                               .Ordered();
        }
        public static IEnumerable<ICardInCollectionCount> GetCollectionStatisticsForCard(this IMagicDatabaseReadOnly magicDatabase, ICardCollection collection, ICard card)
        {
            return magicDatabase.GetCardCollectionStatistics(card).Where(cicc => cicc.IdCollection == collection.Id);
        }

        public static IEnumerable<IEdition> Ordered(this IEnumerable<IEdition> editions)
        {
            return editions.OrderByDescending(ed => ed.ReleaseDate);
        }
        public static IEnumerable<ICard> Ordered(this IEnumerable<ICard> cards)
        {
            return cards.OrderBy(c => c.ToString());
        }
        public static IEnumerable<IBlock> Ordered(this IEnumerable<IBlock> blocks)
        {
            return blocks.OrderByDescending(c => c.Id);
        }

    }
}
