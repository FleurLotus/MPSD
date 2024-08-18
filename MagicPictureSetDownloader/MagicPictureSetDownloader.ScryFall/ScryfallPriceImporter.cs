namespace MagicPictureSetDownloader.ScryFall
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;

    using Common.Web;
   
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ScryFall.JsonData;

    internal class ScryfallPriceImporter : IPriceImporter
    {
        //ALERT review the PriceImporter to become the file parser and replace the download from gather 
        //ALERT review the db structure to be able to store the scryfall data like the the scryfall id and use it at key instead of gatherer same for picture and icons from svg using sharp vector

        private const string ScryfallBulk = @"https://api.scryfall.com/bulk-data";
        private const string ScryfallSets = @"https://api.scryfall.com/sets";
        private const string DefaultCard = "default_cards";
        private const string AllCard = "all_cards";

        private IList<string> _errorMessages;

        private BulkDataList GetBulkData(WebAccess webAccess)
        {
            string json = webAccess.GetHtml(ScryfallBulk);
            return JsonSerializer.Deserialize<BulkDataList>(json);
        }
        public FullSet[] GetBulkSets(WebAccess webAccess)
        {
            string json = webAccess.GetHtml(ScryfallSets);
            return JsonSerializer.Deserialize<FullSet[]>(json);
        }

        private IReadOnlyList<KeyValuePair<string, object>> GetUrls(WebAccess webAccess, string type)
        {
            IList<KeyValuePair<string, object>> urls = new List<KeyValuePair<string, object>>();

            BulkDataList bulkDataList = GetBulkData(webAccess);

            BulkData bulkData = bulkDataList.Data.FirstOrDefault(d => d.Type == type);
            if (bulkData != null)
            {
                urls.Add(new KeyValuePair<string, object>(bulkData.DownloadUri, bulkData));
            }
            return urls.ToArray();
        }
        public IReadOnlyList<KeyValuePair<string, object>> GetDefaultCardUrls(WebAccess webAccess)
        {
            return GetUrls(webAccess, DefaultCard);
        }
        public IReadOnlyList<KeyValuePair<string, object>> GetAllCardUrls(WebAccess webAccess)
        {
            return GetUrls(webAccess, AllCard);
        }

        public IEnumerable<PriceInfo> Parse(WebAccess webAccess, string url, object param, out string errorMessage)
        {
            _errorMessages = new List<string>();

            BulkData bulkData = (BulkData) param;
            FullCard[] cards = GetCardsInfo(webAccess, url);

            List<PriceInfo> ret = cards.SelectMany(c => ExtractCardPrice(c, bulkData.UpdatedAt)).ToList();

            errorMessage = string.Join("\r\n", _errorMessages);
            return ret;
        }
        private static FullCard[] GetCardsInfo(WebAccess webAccess, string url)
        {
            string filePath = Path.Combine(Path.GetTempPath(), DefaultCard + ".json");
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                webAccess.DownloadFile(url, filePath);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    return JsonSerializer.Deserialize<FullCard[]>(fileStream);
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

        private IEnumerable<PriceInfo> ExtractCardPrice(FullCard scryfallCard, DateTime updatedAt)
        {
            List<int> ids = scryfallCard.MultiverseIds;

            IList<ICard> cards = new List<ICard>();
            foreach (int id in ids)
            {
                //ALERT: to put back
                ICard c = null; // _magicDatabase.GetCard(id);
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
                int p;
                if (double.TryParse(scryfallCard.Prices.Usd, out double price))
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
        private void CheckCard(FullCard scryfallCard, IList<ICard> cards)
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
        private bool CheckCardName(FullCard scryfallCard, ICard card)
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
