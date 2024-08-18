namespace MagicPictureSetDownloader.ScryFall
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    using MagicPictureSetDownloader.ScryFall.JsonData;

    internal class SmallFileCreator
    {
        //private static readonly IList<CardEdition> CardEditionsForVariation = new List<CardEdition>();

        public async static Task CreateSmallFile()
        {
            Console.WriteLine("Create smaller files");
            string setFilePath = await CreateSetSmallFile();
            string cardFilePath = await CreateCardSmallFile();
            string allCardFilePath = await CreateAllCardSmallFile();

            /* ALERT To be reviewed
            Console.WriteLine("Load Referentials");
            Referential.LoadReferentials();

            Console.WriteLine("Edition");
            await AddEditionDb(setFilePath);

            Console.WriteLine("Card");
            await AddCardDb(cardFilePath);

            Console.WriteLine("Variations");
            AddCardDbWithVariations();

            Referential.CloseConnection();
            // How to manage the multi language ?
            */
        }
        private static async Task<string> CreateSetSmallFile()
        {
            string sourcePath = FileManager.GetSetFile();
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new Exception("No Set file");
            }

            string filePath = FileManager.ToSmallName(sourcePath);

            IDictionary<Guid, Set> dic = new SortedDictionary<Guid, Set>();
            await foreach (FullSet s in GetAndCheck<FullSet>(sourcePath))
            {
                dic.Add(s.Id, new Set(s));
            }

            using (var sr = new StreamWriter(new FileStream(filePath, FileMode.Create)))
            {
                sr.Write(JsonSerializer.Serialize(dic.Values, new JsonSerializerOptions { WriteIndented = true }));
            }
            return filePath;
        }
        private async static Task<string> CreateCardSmallFile()
        {
            string sourcePath = FileManager.GetDefaultCardFile();
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new Exception("No Card file");
            }

            string filePath = FileManager.ToSmallName(sourcePath);

            IDictionary<Guid, Card> dic = new SortedDictionary<Guid, Card>();
            await foreach (FullCard c in GetAndCheck<FullCard>(sourcePath))
            {
                dic.Add(c.Id, new Card(c));
            }

            using (var sr = new StreamWriter(new FileStream(filePath, FileMode.Create)))
            {
                sr.Write(JsonSerializer.Serialize(dic.Values, new JsonSerializerOptions { WriteIndented = true }));
            }
            return filePath;
        }
        private async static Task<string> CreateAllCardSmallFile()
        {
            string sourcePath = FileManager.GetAllCardFile();
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new Exception("No Card file");
            }

            string filePath = FileManager.ToSmallName(sourcePath);

            IDictionary<Guid, Card> dic = new SortedDictionary<Guid, Card>();
            await foreach (FullCard c in GetAndCheck<FullCard>(sourcePath))
            {
                dic.Add(c.Id, new Card(c));
            }

            using (var sr = new StreamWriter(new FileStream(filePath, FileMode.Create)))
            {
                sr.Write(JsonSerializer.Serialize(dic.Values, new JsonSerializerOptions { WriteIndented = true }));
            }
            return filePath;
        }

        /*
        private async static Task AddEditionDb(string filePath)
        {
            await foreach (Set set in GetAndCheck<Set>(filePath))
            {
                string blockName = set.Block;

                int? blockId = null;

                if (!string.IsNullOrWhiteSpace(blockName))
                {
                    Block block = Referential.GetBlock(blockName);
                    if (block == null)
                    {
                        block = new Block { Name = blockName };
                        Referential.AddBlock(block);
                        blockId = Referential.GetBlock(blockName).Id;
                    }
                }

                Edition edition = new Edition
                {
                    Name = set.Name,
                    IdBlock = blockId,
                    CardNumber = set.CardCount,
                    HasFoil = !set.NonFoilOnly,
                    ReleaseDate = set.ReleasedAt,
                    Code = set.Code?.ToUpper(),
                    Guid = set.Id.ToString(),
                };

                Referential.AddEdition(edition);
            }
        }
        private static async Task AddCardDb(string filePath)
        {
            int count = 0;
            await foreach (Card c in GetAndCheck<Card>(filePath))
            {
                CardFace face1 = c;
                CardFace face2 = null;

                if (c.CardFaces.Count > 0)
                {
                    face1 = c.CardFaces[0];
                    face2 = c.CardFaces[1];

                    if (face1.Name == face2.Name)
                    {
                        face2 = null;
                    }
                }

                string name = c.Layout == Layout.Split ? c.Name : face1.Name;

                DbCard card1 = new DbCard
                {
                    Name = name,
                    CastingCost = face1.ManaCost,
                    Type = face1.TypeLine,
                    Power = face1.Power,
                    Toughness = face1.Toughness,
                    Defense = face1.Defense,
                    Loyalty = face1.Loyalty,
                    Text = face1.OracleText,
                    OracleId = face1.OracleId?.ToString() ?? c.OracleId?.ToString(),
                    PartName = face1.Name,
                    OtherPartName = face2?.Name
                };

                Referential.AddCard(card1);

                int card1Id = Referential.GetCard(card1.Name, card1.PartName).Id;
                string faceImageUrl = face1.ImageUris?.Normal?.ToString();

                CardEdition cardEdition = new CardEdition
                {
                    IdCard = card1Id,
                    IdRarity = Referential.GetRarity(c.Rarity.ToString()).Id,
                    IdScryfall = c.Id.ToString(),
                    IdEdition = Referential.GetEdition(c.SetId).Id,
                    IdGatherer = c.MultiverseIds.Count > 0 ? c.MultiverseIds[0] : null,
                    Url = faceImageUrl ?? c.ImageUris?.Normal?.ToString(),
                    Part = face2 == null || faceImageUrl == null ? null : 1
                };
                if (IsVariation(c))
                {
                    CardEditionsForVariation.Add(cardEdition);
                }
                else
                {
                    Referential.AddCardEdition(cardEdition);
                }

                if (face2 != null)
                {
                    AddExtraFace(c, face2, face1.Name, 2);
                }

                if (c.CardFaces.Count > 2)
                {
                    for (int i = 2; i < c.CardFaces.Count; i++)
                    {
                        AddExtraFace(c, c.CardFaces[i], face1.Name, i + 1);
                    }
                }

                count++;
                if (count % 100 == 0)
                {
                    Console.WriteLine(count);
                    Referential.CloseConnection();
                }
            }
        }
        private static void AddCardDbWithVariations()
        {
            int count = 0;
            foreach (CardEdition ce in CardEditionsForVariation)
            {
                CardEdition cardEdition = Referential.GetCardEditionForVariations(ce);
                if (cardEdition == null)
                {
                    Referential.AddCardEdition(ce);
                }
                else
                {
                    if (cardEdition.IdScryfall == ce.IdScryfall)
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(ce.Url))
                    {
                        continue;
                    }

                    CardEditionVariation cardEditionVariation = new CardEditionVariation
                    {
                        IdScryfall = cardEdition.IdScryfall,
                        OtherIdScryfall = ce.IdScryfall,
                        Part = ce.Part,
                        Url = ce.Url,
                        IdGatherer = ce.IdGatherer
                    };
                    Referential.AddCardEditionVariation(cardEditionVariation);
                }

                count++;
                if (count % 100 == 0)
                {
                    Console.WriteLine(count);
                    Referential.CloseConnection();
                }

            }
        }

        private static void AddExtraFace(Card c, CardFace face, string face1Name, int part)
        {
            string name = c.Layout == Layout.Split ? c.Name : face.Name;

            DbCard card = new DbCard
            {
                Name = name,
                CastingCost = face.ManaCost,
                Type = face.TypeLine ?? "None",
                Power = face.Power,
                Toughness = face.Toughness,
                Defense = face.Defense,
                Loyalty = face.Loyalty,
                Text = face.OracleText,
                OracleId = face.OracleId?.ToString() ?? c.OracleId?.ToString(),
                PartName = face.Name,
                OtherPartName = face1Name
            };

            Referential.AddCard(card);

            int cardId = Referential.GetCard(card.Name, card.PartName).Id;

            if (face.ImageUris?.Normal?.ToString() == null)
            {
                return;
            }

            CardEdition cardEdition = new CardEdition
            {
                IdCard = cardId,
                IdRarity = Referential.GetRarity(c.Rarity.ToString()).Id,
                IdScryfall = c.Id.ToString(),
                IdEdition = Referential.GetEdition(c.SetId).Id,
                IdGatherer = c.MultiverseIds.Count >= part ? c.MultiverseIds[part - 1] : null,
                Url = face.ImageUris?.Normal?.ToString(),
                Part = part
            };
            
            if (IsVariation(c))
            {
                CardEditionsForVariation.Add(cardEdition);
            }
            else
            {
                Referential.AddCardEdition(cardEdition);
            }
        }
        */
        private static bool IsVariation(Card c)
        {
            return c.NonFoil ||
                   c.FrameEffects.Any(fe => fe == FrameEffect.FullArt || fe == FrameEffect.Textless || fe == FrameEffect.ExtendedArt || 
                                            fe == FrameEffect.ShowCase || fe == FrameEffect.ShatteredGlass || fe == FrameEffect.Inverted);
        }

        private static async IAsyncEnumerable<T> GetAndCheck<T>(string filepath)
        {
            using (FileStream fileStream = new FileStream(filepath, FileMode.Open))
            {
                IAsyncEnumerable<T> objets = JsonSerializer.DeserializeAsyncEnumerable<T>(fileStream);

                int i = 0;

                await foreach (T o in objets)
                {
                    IList<string> l = JsonMissingMapping.Check(o, $"{o.GetType().Name}[{i}]");
                    if (l != null && l.Count > 0)
                    {
                        throw new Exception(string.Join(";", l));
                    }

                    i++;

                    yield return o;
                }
            }
        }
    }
}
