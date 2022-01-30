namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    using Common.Web;
   
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.Core.JsonData;


    internal class ScryfallPriceImporter : IPriceImporter
    {
        private const string Scryfall = @"https://api.scryfall.com/bulk-data";
        private const string WantedBulkType = "default_cards";

        private readonly IMagicDatabaseReadOnly _magicDatabase = MagicDatabaseManager.ReadOnly;
        private IList<string> _errorMessages;

        public IReadOnlyList<KeyValuePair<string, object>> GetUrls(WebAccess webAccess)
        {
            IList<KeyValuePair<string, object>> urls = new List<KeyValuePair<string, object>>();

            string json = webAccess.GetHtml(Scryfall);
            BulkDataList bulkDataList = JsonConvert.DeserializeObject<BulkDataList>(json);

            BulkData bulkData = bulkDataList.Data.FirstOrDefault(d => d.Type == WantedBulkType);
            if (bulkData != null)
            {
                urls.Add(new KeyValuePair<string, object>(bulkData.DownloadUri, bulkData));
            }
            return urls.ToArray();
        }
        public IEnumerable<PriceInfo> Parse(WebAccess webAccess, string url, object param, out string errorMessage)
        {
            _errorMessages = new List<string>();

            BulkData bulkData = (BulkData) param;
            Card[] cards = GetCardsInfo(webAccess, url);

            List<PriceInfo> ret = cards.SelectMany(c => ExtractCardPrice(c, bulkData.UpdatedAt)).ToList();

            errorMessage = string.Join("\r\n", _errorMessages);
            return ret;
        }
        private static Card[] GetCardsInfo(WebAccess webAccess, string url)
        {
            string filePath = Path.Combine(Path.GetTempPath(), WantedBulkType + ".json");
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                webAccess.DownloadFile(url, filePath);
                using (StreamReader re = new StreamReader(filePath))
                {
                    using (JsonTextReader reader = new JsonTextReader(re))
                    {
                        JsonSerializer se = new JsonSerializer();
                        return se.Deserialize<Card[]>(reader);
                    }
                }
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        private IEnumerable<PriceInfo> ExtractCardPrice(Card scryfallCard, DateTime updatedAt)
        {
            List<int> ids = scryfallCard.MultiverseIds;

            //Temporary fix
            if (scryfallCard.Id == "71ccc444-54c8-4f7c-a425-82bc3eea1eb0")
            {
                ids = new List<int> { 534954, 534953 };
            }

            IList<ICard> cards = new List<ICard>();
            foreach (int id in ids)
            {
                ICard c = _magicDatabase.GetCard(id);
                if (c != null)
                {
                    cards.Add(c);
                }
            }

            if (cards.Count == 0)
            {
                yield break;
            }

            CheckCard(scryfallCard, cards);

            if (scryfallCard.Prices == null)
            {
                yield break;
            }
            foreach (int id in ids)
            {
                double price;
                int p;
                if (double.TryParse(scryfallCard.Prices.Usd, out price))
                {
                    p = (int) (price * 100);
                    yield return new PriceInfo { UpdateDate = updatedAt, IdGatherer = id, PriceSource = PriceValueSource.TCGplayer, Foil = false, Value = p };
                }
                if (double.TryParse(scryfallCard.Prices.UsdFoil, out price))
                {
                    p = (int)(price * 100);
                    yield return new PriceInfo { UpdateDate = updatedAt, IdGatherer = id, PriceSource = PriceValueSource.TCGplayer, Foil = true, Value = p };
                }
                if (double.TryParse(scryfallCard.Prices.Eur, out price))
                {
                    p = (int)(price * 100);
                    yield return new PriceInfo { UpdateDate = updatedAt, IdGatherer = id, PriceSource = PriceValueSource.Cardmarket, Foil = false, Value = p };
                }
                if (double.TryParse(scryfallCard.Prices.EurFoil, out price))
                {
                    p = (int)(price * 100);
                    yield return new PriceInfo { UpdateDate = updatedAt, IdGatherer = id, PriceSource = PriceValueSource.Cardmarket, Foil = true, Value = p };
                }
            }
        }

        private void CheckCard(Card scryfallCard, IList<ICard> cards)
        {
            ICard card = cards[0];

            if (!CheckCardName(scryfallCard, card))
            {
                _errorMessages.Add($"Missmatching card name for {scryfallCard.Id} : {scryfallCard.Name} vs {card.Name}");
            }
                      

            if (cards.Count > 1)
            {
                if (card.OtherPartName == null)
                {
                    _errorMessages.Add($"Card {scryfallCard.Id} : {scryfallCard.Name} is multipart in scryfall but not in MPSD");

                }
                else
                {
                    for (int i = 1; i < cards.Count; i++)
                    {
                        ICard c = cards[i];
                        if (c.Name == card.Name || (card.OtherPartName == c.PartName && card.PartName == c.OtherPartName))
                        {
                            continue;
                        }

                        _errorMessages.Add($"Card {scryfallCard.Id} : {scryfallCard.Name} is referencing {c.Name} in scryfall");
                    }
                }
            }
        }

        private bool CheckCardName(Card scryfallCard, ICard card)
        {
            string cardName = card.Name;
            string scryfallCardName = scryfallCard.Name;

            if (scryfallCardName == cardName)
            {
                return true;
            }
            
            if (scryfallCardName.Contains("//"))
            {
                //Split card 
                if (scryfallCardName.Replace(" // ", "//") == cardName)
                {
                    return true;
                }

                //double face card
                if (scryfallCardName == string.Format("{0} // {1}", card.PartName, card.OtherPartName) ||
                    scryfallCardName == string.Format("{0} // {1}", card.OtherPartName, card.PartName))
                {
                    return true;
                }
                //Up-down
                string name = scryfallCardName[..scryfallCardName.IndexOf("/")].TrimEnd();
                if (name == cardName)
                {
                    return true;
                }
                //Who/What/When/Where/Why
                if (cardName.Contains("/") && !cardName.Contains("//"))
                {
                    if (scryfallCardName.Replace(" // ", "//") == cardName.Replace("/", "//"))
                    {
                        return true;
                    }
                }
            }
            //Special card with multiple version
            if (cardName.Contains("("))
            {
                string name = cardName[..cardName.IndexOf("(")].TrimEnd();
                if (scryfallCardName == name)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
