namespace ScryfallTest.Db
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Common.Database;

    internal static class Referential
    {
        private static readonly IList<Edition> Editions = new List<Edition>();
        private static readonly IDictionary<int, Block> Blocks = new Dictionary<int, Block>();
        private static readonly IDictionary<string, DbCard> Cards = new Dictionary<string, DbCard>();
        private static readonly IDictionary<string, CardEdition> CardEditions = new Dictionary<string, CardEdition>();
        private static readonly IDictionary<string, IList<CardEditionVariation>> CardEditionVariations = new Dictionary<string, IList<CardEditionVariation>>();
        private static readonly IDictionary<string, Rarity> Rarities = new Dictionary<string, Rarity>(StringComparer.InvariantCultureIgnoreCase);

        public static void LoadReferentials()
        {
            using (IDbConnection cnx = new DbConnection().GetConnection())
            {
                Rarities.Clear();
                Blocks.Clear();
                Editions.Clear();
                Cards.Clear();
                CardEditions.Clear();
                CardEditionVariations.Clear();

                foreach (Rarity rarity in Mapper<Rarity>.LoadAll(cnx))
                {
                    InsertInReferential(rarity);
                }

                foreach (Block block in Mapper<Block>.LoadAll(cnx))
                {
                    InsertInReferential(block);
                }

                foreach (Edition edition in Mapper<Edition>.LoadAll(cnx))
                {
                    if (edition.IdBlock.HasValue)
                    {
                        edition.Block = Blocks.GetOrDefault(edition.IdBlock.Value);
                    }

                    InsertInReferential(edition);
                }
                foreach (DbCard card in Mapper<DbCard>.LoadAll(cnx))
                {
                    InsertInReferential(card);
                }
                foreach (CardEdition cardEdition in Mapper<CardEdition>.LoadAll(cnx))
                {
                    InsertInReferential(cardEdition);
                }
                foreach (CardEditionVariation cardEditionVariation in Mapper<CardEditionVariation>.LoadAll(cnx))
                {
                    InsertInReferential(cardEditionVariation);
                }
            }
        }

        private static IDbTransaction Transaction;
        private static IDbConnection Connection;
        public static void OpenConnection()
        {
            if (Connection == null)
            {
                Connection = new DbConnection().GetConnection();
                Transaction = Connection.BeginTransaction();
            }
        }
        public static void CloseConnection()
        {
            Transaction?.Commit();
            Connection?.Dispose();
            Connection = null;
        }

        private static void InsertInReferential(Rarity rarity)
        {
            Rarities.Add(rarity.Name, rarity);
        }
        private static void InsertInReferential(Block block)
        {
            Blocks.Add(block.Id, block);
        }
        private static void InsertInReferential(Edition edition)
        {
            Editions.Add(edition);
        }
        private static void InsertInReferential(DbCard card)
        {
            string key;
            if (card.PartName == null || card.Name == card.PartName)
            {
                key = card.Name;
            }
            else
            {
                key = card.Name + card.PartName;
            }


            Cards.Add(key, card);
        }
        private static void InsertInReferential(CardEdition cardEdition)
        {
            CardEditions.Add(cardEdition.IdScryfall + cardEdition.Part?.ToString(), cardEdition);
        }
        private static void InsertInReferential(CardEditionVariation cardEditionVariation)
        {
            if (GetCardEdition(cardEditionVariation.IdScryfall, cardEditionVariation.Part) == null)
            {
                throw new ApplicationDbException("Can't find CardEdition with id " + cardEditionVariation.IdScryfall);
            }

            if (!CardEditionVariations.TryGetValue(cardEditionVariation.IdScryfall, out IList<CardEditionVariation> variations))
            {
                variations = new List<CardEditionVariation>();
                CardEditionVariations.Add(cardEditionVariation.IdScryfall, variations);
            }
            variations.Add(cardEditionVariation);
        }

        internal static CardEdition GetCardEdition(string idScryfall, int? part)
        {
            if (CardEditions.TryGetValue(idScryfall + part?.ToString(), out CardEdition cardEdition))
            {
                return cardEdition;
            }
            return null;
        }
        
        internal static CardEdition GetCardEditionForVariations(CardEdition cardEdition)
        {
            return CardEditions.Values.FirstOrDefault(ce => ce.IdCard == cardEdition.IdCard && ce.IdEdition == cardEdition.IdEdition && ce.Part == cardEdition.Part);
        }
        internal static DbCard GetCard(string name, string partName)
        {
            string key;
            if (partName == null || partName == name)
            {
                key = name;
            }
            else
            {
                key = name + partName;
            }

            return Cards.GetOrDefault(key);
        }
        internal static Block GetBlock(string blockName)
        {
            return Blocks.Values.FirstOrDefault(b => string.Compare(b.Name, blockName, StringComparison.InvariantCultureIgnoreCase) == 0);
        }
        internal static Edition GetEdition(string guid)
        {
            return Editions.FirstOrDefault(e => e.Guid == guid) ;
        }
        internal static Rarity GetRarity(string rarity)
        {
            return Rarities.GetOrDefault(rarity);
        }
        internal static CardEditionVariation GetCardEditionVariation(string idScryfall, string otherIdScryfall, int? part)
        {
            if (GetCardEdition(idScryfall, part) == null)
            {
                return null;
            }

            if (CardEditionVariations.TryGetValue(idScryfall, out IList<CardEditionVariation> variations))
            {
                return variations.FirstOrDefault(cev => cev.OtherIdScryfall == otherIdScryfall && cev.Part == part);
            }
            return null;
        }

        internal static void AddBlock(Block block)
        {
            if (GetBlock(block.Name) == null)
            {
                AddToDbAndUpdateReferential(block, InsertInReferential);
            }
        }
        internal static void AddEdition(Edition edition)
        {
            if (GetEdition(edition.Guid) == null)
            {
                AddToDbAndUpdateReferential(edition, InsertInReferential);
            }
        }
        internal static void AddCard(DbCard card)
        {
            if (GetCard(card.Name, card.PartName) == null)
            {
                AddToDbAndUpdateReferential(card, InsertInReferential);
            }
        }
        internal static void AddCardEdition(CardEdition cardEdition)
        {
            if (GetCardEdition(cardEdition.IdScryfall, cardEdition.Part) == null)
            {
                AddToDbAndUpdateReferential(cardEdition, InsertInReferential);
            }
        }
        internal static void AddCardEditionVariation(CardEditionVariation cardEditionVariation)
        {
            if (GetCardEditionVariation(cardEditionVariation.IdScryfall, cardEditionVariation.OtherIdScryfall, cardEditionVariation.Part) == null)
            {
                AddToDbAndUpdateReferential(cardEditionVariation, InsertInReferential);
            }
        }

        private static void AddToDbAndUpdateReferential<T>(T value, Action<T> addToReferential) where T : class, new()
        {
            if (value == null)
            {
                return;
            }

            OpenConnection();
            Mapper<T>.InsertOne(Connection, value);

            addToReferential(value);
        }
    }
}
