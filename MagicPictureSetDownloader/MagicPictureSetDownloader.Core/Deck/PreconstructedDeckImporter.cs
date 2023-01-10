namespace MagicPictureSetDownloader.Core.Deck
{
    using System;
    using System.Net;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;

    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.Db;

    public class PreconstructedDeckImporter
    {
        private const string BaseUrl = @"http://mtg.wtf";
        private readonly Regex _decksRegex = new Regex(@"<a href=""(?<url>/deck/\w+/[^>]+)"">", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _deckNameRegex = new Regex(@"<h4>(?<name>[^<]+)</h4>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _deckEditionRegex = new Regex(@"<a download=""true"" href=""/deck/(?<edition>\w+)/[^>]*"">", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _cardInfoRegex = new Regex(@"<a href=""(?<url>/card/(?<edition>\w+)/[^>]*)"">(?<name>.+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _cardImageRegex = new Regex(@"<img alt=.* src='(?<url>/cards[^>]*)'>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _cardRarityRegex = new Regex(@"Rarity: (?<rarity>\w+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private const string CardSplitter = @"<div class='card_entry'>";
        // Magic Online + Coldsnap + doublon
        private readonly Regex _excludedRegex = new Regex(@"/deck/(?:td0|me2|wth|vis|mir|csp|rqs|wc\d{2})/", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Func<string, string> _getExtraInfo;

        private readonly IMagicDatabaseReadOnly MagicDatabase = MagicDatabaseManager.ReadOnly;

        public PreconstructedDeckImporter(Func<string, string> getExtraInfo)
        {
            _getExtraInfo = getExtraInfo;
        }

        public string GetRootUrl()
        {
            return string.Format("{0}/deck", BaseUrl);
        }

        internal string[] GetDeckUrls(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return Array.Empty<string>();
            }
            
            string htmltext = WebUtility.HtmlDecode(html);
            MatchCollection matches = _decksRegex.Matches(htmltext);
            ICollection<IPreconstructedDeck> decks = MagicDatabase.GetAllPreconstructedDecks();
            return  matches.OfType<Match>()
                           .Select(m => BaseUrl + m.Groups["url"].Value)
                           .Where(u => decks.All(d => d.Url != u) && !_excludedRegex.IsMatch(u))
                           .ToArray();
        }

        internal DeckInfo ParseDeckPage(string html)
        {
            string htmltext = WebUtility.HtmlDecode(html);
            Match m = _deckNameRegex.Match(htmltext);
            if (!m.Success)
            {
                throw new ParserException("Could not find Title");
            }

            string deckName = m.Groups["name"].Value;
            m = _deckEditionRegex.Match(htmltext);
            if(!m.Success)
            {
                throw new ParserException("Could not find edition");
            }

            if (string.IsNullOrWhiteSpace(deckName))
            {
                return null;
            }

            IEdition deckEdition = GetEdition(deckName, m);
            if (deckEdition == null)
            {
                throw new ParserException("Could not find edition with name " + deckName);
            }
            if (MagicDatabase.GetPreconstructedDeck(deckEdition.Id, deckName) != null)
            {
                return null;
            }

            //Split in block
            IList<DeckCardInfo> cards = new List<DeckCardInfo>();
            string[] tokens = htmltext.Split(new string[] { CardSplitter }, StringSplitOptions.RemoveEmptyEntries);

            //Start at 1 because cards start by CardSplitter
            for (int i = 1; i < tokens.Length; i++)
            {
                string[] lines = tokens[i].Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                if (!int.TryParse(lines[0], out int number))
                {
                    throw new ParserException("Could not find Number");
                }

                foreach (string line in lines)
                {
                    m = _cardInfoRegex.Match(line);
                    if (m.Success)
                    {
                        ICard card = GetCard(m);
                        IEdition edition = GetEdition(deckName, m);

                        if (edition == null)
                        {
                            throw new ParserException("Could not find edition for card in " + deckName);
                        }

                        int idGatherer = MagicDatabase.GetIdGatherer(card, edition);
                        if (idGatherer == 0)
                        {
                            //It is not a gatherer edition, we will add the card to it
                            if (edition.IsNoneGatherer() && _getExtraInfo != null)
                            {
                                Tuple<string, IRarity> t = ExtractExtraInfo(m.Groups["url"].Value);
                                cards.Add(new DeckCardInfo(edition.Id, card.Id, number, t.Item2.Id, t.Item1));
                                break;
                            }

                            throw new ParserException(string.Format("Could not find card with idCard {0} and idEdition {1}", card.Id, edition.Id));
                        }
                        else
                        {
                            cards.Add(new DeckCardInfo(idGatherer, number));
                            break;
                        }
                    }
                }
            }

            if (cards.Count == 0)
            {
                return null;
            }
            DeckInfo deckInfo =  new DeckInfo(deckEdition.Id, deckName, cards);
            int cardCount = deckInfo.Count;
            //  60 Usual
            //  75 Usual with side board
            //  62 Deckmasters
            //  61 Beatdown
            // 100 Commander
            //  15 or 22 or 26 or 30 or 35 or 40 or 41 Welcome Pack / Portal
            //  80 Archenemy
            //  70 Planechase
            //  20 Jumpstart
            //  19 Jumpstart (+1 semi-random)
            if (cardCount != 60 && cardCount != 75 && 
                cardCount != 62 &&
                cardCount != 61 &&
                cardCount != 100 &&
                cardCount != 15 && cardCount != 22 && cardCount != 26 && cardCount != 30 && cardCount != 35 && cardCount != 40 && cardCount != 41 && 
                cardCount != 80 &&
                cardCount != 70 &&
                cardCount != 20 && cardCount != 19
                )
            {
                throw new ParserException(string.Format("Deck {0} contains {1} cards", deckName, cardCount));
            }

            return deckInfo;
        }

        private ICard GetCard(Match m)
        {
            string cardName = m.Groups["name"].Value.TrimEnd();
            string cardName2 = null;
  
            if (cardName.Contains("/"))
            {
                string[] c = cardName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                // Special case for Who / What / When / Where / Why that is considered as 1 card with no second card 
                if (c.Length == 2)
                {
                    cardName = c[0].Trim();
                    cardName2 = c[1].Trim();
                }
            }

            ICard card = MagicDatabase.GetCard(cardName, cardName2);
            if (card == null)
            {
                card = MagicDatabase.GetCard(cardName, null);
                if (card == null)
                {
                    throw new ParserException("Could not find Card with name " + cardName);
                }
            }

            return card;
        }
        private IEdition GetEdition(string deckName, Match m)
        {
            string cardEdition = m.Groups["edition"].Value.TrimEnd();

            IEdition edition = MagicDatabase.GetEditionFromCode(cardEdition);
            if (edition == null)
            {
                //Special case for guild pack
                string cardEdition2 = cardEdition + "_" + deckName;
                if (cardEdition2.Length > 10)
                {
                    cardEdition2 = cardEdition2[..10];
                }
                edition = MagicDatabase.GetEditionFromCode(cardEdition2);
             }

            return edition;
        }
        private Tuple<string, IRarity> ExtractExtraInfo(string url)
        {
            string extraHtml = WebUtility.HtmlDecode(_getExtraInfo(BaseUrl + url));

            Match m = _cardImageRegex.Match(extraHtml);
            if (!m.Success)
            {
                throw new ParserException("Could not find Card image in with " + url);
            }
            string pictureUrl = BaseUrl + m.Groups["url"].Value;

            m = _cardRarityRegex.Match(extraHtml);
            if (!m.Success)
            {
                throw new ParserException("Could not find Card rarity in with " + url);
            }
            string rarityString = m.Groups["rarity"].Value.Trim();
            if (rarityString.ToLower() == "mythic")
            {
                rarityString = "mythic rare";
            }
            else if (rarityString.ToLower() == "basic")
            {
                rarityString = "basic land";
            }
            IRarity rarity = MagicDatabase.GetRarity(rarityString);
            if (rarity == null)
            {
                throw new ParserException("Unknown rarity " + rarityString);

            }
            return new Tuple<string, IRarity>(pictureUrl, rarity);
        }
    }
}
